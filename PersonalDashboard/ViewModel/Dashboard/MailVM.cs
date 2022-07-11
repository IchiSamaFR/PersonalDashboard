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
        private Task GetMailsTask;
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
                        SelectedMailBox.OnCountChanged -= StartLoadNewMails;
                    }
                    _selectedMailBox = value;
                    if (SelectedMailBox != null)
                    {
                        SelectedMailBox.OnCountChanged += StartLoadNewMails;
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
        public ICommand LoadNewMailsCmd { get; }
        #endregion


        public MailVM(DashboardVM dashboard)
        {
            _mailViewerVM = new MailViewerVM();
            _dashboardVM = dashboard;
            Name = "Mail";
            Icon = Properties.Resources.mail;

            SelectMailCmd = new RelayCommand(o => { StartSelectMail((MailItem)o); });
            LoadAncientMailsCmd = new RelayCommand(o => { StartLoadOldMails(); });
            LoadNewMailsCmd = new RelayCommand(o => { StartLoadNewMails(); });

            Task.Run(() => ConnectMailBoxAsync(ConfigItem.Instance.MailAdress, ConfigItem.Instance.MailPass));
        }

        public async Task<bool> GetMailsTaskAvailable()
        {
            while (GetMailsTask != null)
            {
                await Task.Delay(100);
            }
            return true;
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
            if (GetMailsTask != null)
            {
                GetMailsTaskToken.Cancel();
            }
            await GetMailsTaskAvailable();
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
                SelectedMailBox = MailBoxes.FirstOrDefault(box => box.Name.ToLower() == "inbox") ?? MailBoxes.FirstOrDefault();
            }
            catch (Exception e)
            {
                NotificationsVM.instance.AddNotification(Name, $"Could not connect to the mailbox. {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public void StartLoadOldMails(int amount = 10)
        {
            if (GetMailsTask == null)
            {
                GetMailsTaskToken = new CancellationTokenSource();
                GetMailsTask = new Task(() => LoadMailsAsync(amount, id => SelectedMailBox.LowerId > id, LoadOldMailsAction));
                GetMailsTask.Start();
            }
        }
        public void StartLoadNewMails()
        {
            if (GetMailsTask == null)
            {
                GetMailsTaskToken = new CancellationTokenSource();
                GetMailsTask = new Task(() => LoadMailsAsync(int.MaxValue, id => id > SelectedMailBox.HigherId, LoadNewMailsAction));
                GetMailsTask.Start();
            }
        }
        public async void StartOnCountChanged()
        {
            await GetMailsTaskAvailable();

            if (GetMailsTask == null)
            {
                GetMailsTaskToken = new CancellationTokenSource();
                GetMailsTask = new Task(() => LoadMailsAsync(int.MaxValue, id => id > SelectedMailBox.LowerId, LoadNewMailsAction));
                GetMailsTask.Start();
            }
        }

        private async void LoadMailsAsync(int amount, Func<UniqueId, bool> whereFunc, Action<MailBox, IList<IMessageSummary>> action)
        {
            MailBox mailBox = SelectedMailBox;
            try
            {
                await mailBox.MailFolder.OpenAsync(FolderAccess.ReadWrite);
                IList<IMessageSummary> messagesList = GetMailsMessages(amount, mailBox, whereFunc).Result;
                if (GetMailsTaskToken.Token.IsCancellationRequested)
                {
                    await mailBox.MailFolder.CloseAsync();
                    GetMailsTaskToken.Token.ThrowIfCancellationRequested();
                }
                action.Invoke(mailBox, messagesList);
                await mailBox.MailFolder.CloseAsync();
            }
            catch { }
            finally
            {
                GetMailsTask = null;
            }
        }
        private async void LoadNewMailsAction(MailBox mailBox, IList<IMessageSummary> messagesList)
        {
            messagesList = messagesList.OrderBy(item => item.UniqueId).ToList();

            for (int i = 0; i < messagesList.Count; i++)
            {
                UniqueId mailId = messagesList[i].UniqueId;

                var message = messagesList[i];
                MailItem mail = new MailItem(this);
                mail.Fill(message.UniqueId);
                mail.Fill(message.Flags);
                MimeMessage mimeMessage = JsonTool.GetMimeMessageCache(mail.Uid);
                if (mimeMessage == null)
                {
                    mimeMessage = await mailBox.MailFolder.GetMessageAsync(message.UniqueId);
                }
                mail.Fill(mimeMessage);

                await App.Current.Dispatcher.Invoke(async () =>
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
        private async void LoadOldMailsAction(MailBox mailBox, IList<IMessageSummary> messagesList)
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
                    mimeMessage = await mailBox.MailFolder.GetMessageAsync(message.UniqueId);
                }
                mail.Fill(mimeMessage);

                await App.Current.Dispatcher.Invoke(async () =>
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
        public async Task<IList<IMessageSummary>> GetMailsMessages(int amount, MailBox mailBox, Func<UniqueId, bool> whereFunc)
        {
            IList<IMessageSummary> messages = null;
            int mailItemsCount = mailBox.MailItems?.Count ?? 0;
            if (mailBox.MailFolder.Count > mailItemsCount)
            {
                var lstMsg = await mailBox.MailFolder.SearchAsync(SearchQuery.All);
                lstMsg = lstMsg.Where(whereFunc).OrderByDescending(msg => msg).Take(amount).ToList();
                messages = await mailBox.MailFolder.FetchAsync(lstMsg, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Flags);
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
            await IsMailBoxOpen(mailBox);
            await SelectedMailBox.MailFolder.OpenAsync(FolderAccess.ReadWrite);
            await SelectedMailBox.MailFolder.AddFlagsAsync(new UniqueId(mail.Uid), MessageFlags.Seen, true);
            await SelectedMailBox.MailFolder.CloseAsync();
        }
        public async void DeleteMail(MailItem mail)
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                await SelectedMailBox.MailFolder.OpenAsync(FolderAccess.ReadWrite);
                await SelectedMailBox.MailFolder.AddFlagsAsync(new UniqueId(mail.Uid), MessageFlags.Deleted, true);
                await SelectedMailBox.MailFolder.CloseAsync();
            });
        }
    }
}
