using PersonalDashboard.Model.Dashboard.Mail;
using PersonalDashboard.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MailKit.Net.Imap;
using System.Net;
using MailKit;
using MimeKit;
using System.Collections.ObjectModel;
using PersonalDashboard.Model;
using PersonalDashboard.ViewModel.Tools;
using System.Windows.Input;
using System.Windows;
using MailKit.Search;
using System.IO;
using System.Reflection;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using PersonalDashboard.ViewModel.Dashboard.Mail;
using System.Threading;

namespace PersonalDashboard.ViewModel.Dashboard
{
    public class MailVM : AbstractVM
    {
        private readonly DashboardVM _dashboardVM;
        public override UserControl UserControl { get; } = new MailView();

        #region Private
        private MailViewerVM _mailViewerVM;
        private ImapClient imapClient;
        private MailBox _selectedMailBox;
        private List<MailBox> _mailBoxes = new List<MailBox>();
        CancellationTokenSource GetMailsTaskToken;
        private Task _getMailsTask;
        private MailItem _mailSelected;
        #endregion

        #region public
        public MailViewerVM MailViewerVM
        {
            get
            {
                return _mailViewerVM;
            }
        }
        public MailBox SelectedMailBox
        {
            get
            {
                return _selectedMailBox;
            }
            set
            {
                if(SelectedMailBox != value)
                {
                    if (SelectedMail != null)
                    {
                        SelectedMail.IsFocused = false;
                        SelectedMail = null;
                    }
                    if(SelectedMailBox != null)
                    {
                        SelectedMailBox.OnCountChanged -= StartOnCountChanged;
                    }
                    _selectedMailBox = value;
                    if (SelectedMailBox != null)
                    {
                        SelectedMailBox.OnCountChanged += StartOnCountChanged;
                    }
                    NotifyPropertyChanged();
                    InitMailBox();
                }
            }
        }
        public List<MailBox> MailBoxes
        {
            get
            {
                return _mailBoxes;
            }
            set
            {
                _mailBoxes = value;
                NotifyPropertyChanged();
            }
        }
        public MailItem SelectedMail
        {
            get
            {
                return _mailSelected;
            }
            set
            {
                _mailSelected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsMailViewerVisible));

                if (_mailSelected != null)
                {
                    _mailViewerVM.LoadMail(_mailSelected);
                    if (SelectedMail.Flags != MessageFlags.Seen)
                    {
                        SeenMail(SelectedMailBox, _mailSelected);
                    }
                }
            }
        }
        public bool IsMailViewerVisible
        {
            get
            {
                return SelectedMailBox?.MailItems.Where(mail => mail.IsFocused).Count() == 1;
            }
        }
        #endregion

        #region Commands
        public ICommand SelectMailCmd { get; }
        public ICommand LoadAncientMailsCmd { get; }
        public ICommand ReloadMailsCommand { get; }
        #endregion


        public MailVM(DashboardVM dashboard)
        {
            _mailViewerVM = new MailViewerVM();
            _dashboardVM = dashboard;
            Name = "Mail";
            Icon = Properties.Resources.mail;

            SelectMailCmd = new RelayCommand(o => { StartSelectMail((MailItem)o); });
            LoadAncientMailsCmd = new RelayCommand(o => { StartLoadOldMails(); });
            ReloadMailsCommand = new RelayCommand(o => { StartOnCountChanged(); });

            Task.Run(() => ConnectMailBoxAsync(ConfigItem.Instance.MailAdress, ConfigItem.Instance.MailPass));
        }

        public override void OnFocus()
        {
            SelectFirstMailBox();
        }
        private async void SelectFirstMailBox()
        {
            await AsyncTool.AwaitUntil(() => imapClient.IsAuthenticated && MailBoxes.Count > 0);
            if (SelectedMailBox == null)
            {
                SelectedMailBox = MailBoxes.FirstOrDefault(box => box.Name.ToLower() == "inbox") ?? MailBoxes.FirstOrDefault();
            }
        }

        public async Task<bool> IsMailBoxOpen(MailBox mailBox)
        {
            while (mailBox.MailFolder.IsOpen)
            {
                await Task.Delay(100);
            }
            return true;
        }
        public async Task<bool> IsSet()
        {
            while (SelectedMailBox == null)
            {
                await Task.Delay(100);
            }
            return true;
        }
        public async void InitMailBox()
        {
            await IsSet();
            if (_getMailsTask != null)
            {
                GetMailsTaskToken.Cancel();
            }
            await AsyncTool.AwaitUntil(() => _getMailsTask == null);
            if (SelectedMailBox.MailItems.Count == 0)
            {
                if (SelectedMailBox.MailItems.Count == 0)
                {
                    StartLoadOldMails(50);
                }
                else
                {
                    StartLoadNewMails();
                }
            }
        }
        public async Task ConnectMailBoxAsync(string mail, string pass)
        {
            try
            {
                imapClient = new ImapClient();

                await imapClient.ConnectAsync("outlook.office365.com", 993, true);
                await imapClient.AuthenticateAsync(new NetworkCredential(mail, pass));

                MailBoxes = imapClient.GetFolder(imapClient.PersonalNamespaces[0]).GetSubfolders().Select(fold => new MailBox(fold)).ToList();
            }
            catch (Exception e)
            {
                NotificationsVM.instance.AddNotification(Name, $"Could not connect to the mailbox. {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public void StartLoadOldMails(int amount = 10)
        {
            if (_getMailsTask == null)
            {
                GetMailsTaskToken = new CancellationTokenSource();
                _getMailsTask = new Task(() => LoadMailsAsync(amount, id => SelectedMailBox.LowerId > id, LoadOldMailsAction));
                _getMailsTask.Start();
            }
        }
        public void StartLoadNewMails()
        {
            if (_getMailsTask == null)
            {
                GetMailsTaskToken = new CancellationTokenSource();
                _getMailsTask = new Task(() => LoadMailsAsync(int.MaxValue, id => id > SelectedMailBox.HigherId, LoadNewMailsAction));
                _getMailsTask.Start();
            }
        }
        public async void StartOnCountChanged()
        {
            await AsyncTool.AwaitUntil(() => _getMailsTask == null);

            if (_getMailsTask == null)
            {
                GetMailsTaskToken = new CancellationTokenSource();
                _getMailsTask = new Task(() => LoadMailsAsync(int.MaxValue, id => id > SelectedMailBox.LowerId, ReloadMailsAction));
                _getMailsTask.Start();
            }
        }

        private async void LoadMailsAsync(int amount, Func<UniqueId, bool> whereFunc, Action<MailBox, IList<IMessageSummary>> action)
        {
            MailBox mailBox = SelectedMailBox;
            try
            {
                mailBox.MailFolder.Open(FolderAccess.ReadOnly);
                IList<IMessageSummary> messagesList = await Task.Run(() => GetMailsMessages(amount, mailBox, whereFunc));
                if (GetMailsTaskToken.Token.IsCancellationRequested)
                {
                    await mailBox.MailFolder.CloseAsync();
                    GetMailsTaskToken.Token.ThrowIfCancellationRequested();
                }
                if (messagesList != null)
                {
                    await Task.Run(() => action(mailBox, messagesList));
                }
                mailBox.MailFolder.Close();
            }
            catch { }
            finally
            {
                _getMailsTask = null;
            }
        }
        private void LoadNewMailsAction(MailBox mailBox, IList<IMessageSummary> messagesList)
        {
            messagesList = messagesList.OrderBy(item => item.UniqueId).ToList();
            
            foreach (var message in messagesList)
            {
                UniqueId mailId = message.UniqueId;
                MailItem mail = new MailItem(this);
                mail.Fill(message.UniqueId);
                mail.Fill(message.Flags);
                MimeMessage mimeMessage = JsonTool.GetMimeMessageCache(mail.Uid);
                if (mimeMessage == null)
                {
                    mimeMessage = mailBox.MailFolder.GetMessage(message.UniqueId);
                }
                mail.Fill(mimeMessage);

                App.Current.Dispatcher.Invoke(() =>
                {
                    mailBox.AddMail(mail);
                    SaveMailsCache();
                });

                if (GetMailsTaskToken.Token.IsCancellationRequested)
                {
                    mailBox.MailFolder.Close();
                    GetMailsTaskToken.Token.ThrowIfCancellationRequested();
                }
            }
        }
        private void LoadOldMailsAction(MailBox mailBox, IList<IMessageSummary> messagesList)
        {
            messagesList = messagesList.OrderByDescending(item => item.UniqueId).ToList();

            foreach (var message in messagesList)
            {
                UniqueId mailId = message.UniqueId;
                MailItem mail = new MailItem(this);
                mail.Fill(message.UniqueId);
                mail.Fill(message.Flags);
                MimeMessage mimeMessage = JsonTool.GetMimeMessageCache(mail.Uid);
                if (mimeMessage == null)
                {
                    mimeMessage = mailBox.MailFolder.GetMessage(message.UniqueId);
                }
                mail.Fill(mimeMessage);

                App.Current.Dispatcher.Invoke(() =>
                {
                    mailBox.AddMail(mail);
                    SaveMailsCache();
                });

                if (GetMailsTaskToken.Token.IsCancellationRequested)
                {
                    mailBox.MailFolder.Close();
                    GetMailsTaskToken.Token.ThrowIfCancellationRequested();
                }
            }
        }
        private void ReloadMailsAction(MailBox mailBox, IList<IMessageSummary> messagesList)
        {
            messagesList = messagesList.OrderByDescending(item => item.UniqueId).ToList();

            foreach (var message in messagesList)
            {
                MailItem mail = null;
                if (mailBox.MailItems.Any(m => m.Uid == uint.Parse(message.UniqueId.ToString())))
                {
                    mail = mailBox.MailItems.First(m => m.Uid == uint.Parse(message.UniqueId.ToString()));
                    mail.Fill(message.Flags);
                }
                else
                {
                    UniqueId mailId = message.UniqueId;
                    mail = new MailItem(this);
                    mail.Fill(message.UniqueId);
                    mail.Fill(message.Flags);
                    MimeMessage mimeMessage = JsonTool.GetMimeMessageCache(mail.Uid);
                    if (mimeMessage == null)
                    {
                        mimeMessage = mailBox.MailFolder.GetMessage(message.UniqueId);
                    }
                    mail.Fill(mimeMessage);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        mailBox.AddMail(mail);
                        SaveMailsCache();
                    });
                }

                if (GetMailsTaskToken.Token.IsCancellationRequested)
                {
                    mailBox.MailFolder.Close();
                    GetMailsTaskToken.Token.ThrowIfCancellationRequested();
                }
            }
        }
        public IList<IMessageSummary> GetMailsMessages(int amount, MailBox mailBox, Func<UniqueId, bool> whereFunc)
        {
            IList<IMessageSummary> messages = null;
            int mailItemsCount = mailBox.MailItems?.Count ?? 0;
            if (mailBox.MailFolder.Count > mailItemsCount)
            {
                var lstMsg = mailBox.MailFolder.Search(SearchQuery.All);
                lstMsg = lstMsg.Where(whereFunc).OrderByDescending(msg => msg).Take(amount).ToList();
                messages = mailBox.MailFolder.Fetch(lstMsg, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Flags);
            }
            return messages;
        }

        private void UnselectAllMails()
        {
            foreach (var mail in SelectedMailBox.MailItems.Where(mailItem => mailItem.IsFocused))
            {
                mail.IsFocused = false;
            }
        }
        public void StartSelectMail(MailItem mail)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                mail.IsFocused = true;
                if (SelectedMail != null)
                {
                    SelectedMail = null;
                }
                else
                {
                    SelectedMail = mail;
                }
            }
            else
            {
                UnselectAllMails();
                mail.IsFocused = true;
                SelectedMail = mail;
            }
        }

        public void LoadMailsCache()
        {
            foreach (var mail in JsonTool.LoadMails(SelectedMailBox.Name))
            {
                mail.Init(this);
                SelectedMailBox.AddMail(mail);
            }
        }
        public void SaveMailsCache()
        {
            JsonTool.SaveMails(SelectedMailBox.MailItems.ToList(), SelectedMailBox.Name);
        }

        public async void SeenMail(MailBox mailBox, MailItem mail)
        {
            await AsyncTool.AwaitUntil(() => !mailBox.MailFolder.IsOpen);
            SelectedMailBox.MailFolder.Open(FolderAccess.ReadWrite);
            SelectedMailBox.MailFolder.AddFlags(new UniqueId(mail.Uid), MessageFlags.Seen, false);
            SelectedMailBox.MailFolder.Close();
            mail.Flags = MessageFlags.Seen;
        }
        public async void DeleteMail(MailBox mailBox, MailItem mail)
        {
            await AsyncTool.AwaitUntil(() => !mailBox.MailFolder.IsOpen);
            SelectedMailBox.MailFolder.Open(FolderAccess.ReadWrite);
            SelectedMailBox.MailFolder.AddFlags(new UniqueId(mail.Uid), MessageFlags.Deleted, true);
            SelectedMailBox.MailFolder.Close();
        }
    }
}
