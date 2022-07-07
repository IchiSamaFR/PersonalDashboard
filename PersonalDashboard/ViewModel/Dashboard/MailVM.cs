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

namespace PersonalDashboard.ViewModel.Dashboard
{
    public class MailVM : AbstractVM
    {
        private readonly DashboardVM _dashboardVM;
        public override UserControl UserControl { get; } = new MailView();

        #region Private
        private ImapClient imapClient;
        private MailBox _selectedMailBox;
        private List<MailBox> mailBoxes = new List<MailBox>();
        private Task GetMailsTask;

        private MailItem _mailSelected;
        #endregion

        #region public
        public MailBox SelectedMailBox
        {
            get
            {
                return _selectedMailBox;
            }
            set
            {
                _selectedMailBox = value;
                NotifyPropertyChanged();
                ReloadMailGroups();
            }
        }
        public List<MailBox> MailBoxes
        {
            get
            {
                return mailBoxes;
            }
            set
            {
                mailBoxes = value;
                NotifyPropertyChanged();
            }
        }
        public MailItem MailSelected
        {
            get
            {
                return _mailSelected;
            }
            set
            {
                if (value != null && (!SelectedMailBox.MailFolder.IsOpen || value.Flags == MessageFlags.Seen))
                {
                    if (!SelectedMailBox.MailFolder.IsOpen)
                    {
                        SeenMail(_mailSelected);
                    }

                    _mailSelected = value;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MailViewer.NavigateToString(MailSelected.HtmlBody);
                    });
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(MailViewerVisible));
                }
            }
        }
        public Visibility MailViewerVisible
        {
            get
            {
                return MailSelected == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        private WebBrowser MailViewer { get { return ((MailView)UserControl).webBrowser; } }

        #region Commands
        public ICommand LoadNewMailsCmd { get; }
        #endregion


        public MailVM(DashboardVM dashboard)
        {
            _dashboardVM = dashboard;
            Name = "Mail";
            Icon = Properties.Resources.mail;

            LoadNewMailsCmd = new RelayCommand(o => { StartLoadMails(); });
            Task.Run(() => InitMails());
        }
        private void Init()
        {

        }
        public override void OnFocus()
        {
            base.OnFocus();
        }

        public async Task<ImapClient> IsSet()
        {
            while (SelectedMailBox.MailFolder == null)
            {
                await Task.Delay(100);
            }
            return imapClient;
        }
        public async Task InitMails()
        {
            if (await ConnectMailBox(ConfigItem.Instance.MailAdress, ConfigItem.Instance.MailPass))
            {
                LoadMailsCache();
                if(SelectedMailBox.MailItems.Count < 20)
                {
                    StartLoadMails(20);
                }
            }
        }
        public async Task<bool> ConnectMailBox(string mail, string pass)
        {
            try
            {
                imapClient = new ImapClient();

                await imapClient.ConnectAsync("outlook.office365.com", 993, true);
                await imapClient.AuthenticateAsync(new NetworkCredential(mail, pass));

                MailBoxes = imapClient.GetFolder(imapClient.PersonalNamespaces[0]).GetSubfolders().Select(fold => new MailBox(fold)).ToList();
                SelectedMailBox = MailBoxes.First(box => box.Name.ToLower() == "inbox");
                return true;
            }
            catch (Exception e)
            {
                NotificationsVM.instance.AddNotification(Name, $"Could not connect to the mailbox. {MethodBase.GetCurrentMethod().Name}");
                return false;
            }
        }
        public void StartLoadMails(int amount = 10)
        {
            if (GetMailsTask == null || GetMailsTask.IsCanceled || GetMailsTask.IsCompleted || GetMailsTask.IsFaulted)
            {
                GetMailsTask = Task.Run(() => LoadMailsAsync(amount));
            }
        }
        public async Task LoadMailsAsync(int amount)
        {
            await IsSet();

            await App.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await SelectedMailBox.MailFolder.OpenAsync(FolderAccess.ReadWrite);
                }
                catch (Exception e)
                {

                }

                int mailItemsCount = SelectedMailBox.MailItems?.Count ?? 0;
                if (SelectedMailBox.MailFolder.Count > mailItemsCount)
                {
                    var lstMsg = await SelectedMailBox.MailFolder.SearchAsync(SearchQuery.All);
                    lstMsg = lstMsg.Where(id => id < SelectedMailBox.LowerId || id > SelectedMailBox.HigherId).OrderByDescending(msg => msg).Take(amount).ToList();
                    var messages = await SelectedMailBox.MailFolder.FetchAsync(lstMsg, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Flags);
                    messages = messages.OrderByDescending(item => item.UniqueId).ToList();
                    for (int i = 0; i < messages.Count; i++)
                    {
                        UniqueId mailId = messages[i].UniqueId;
                        SelectedMailBox.HigherId = mailId > SelectedMailBox.HigherId ? mailId : SelectedMailBox.HigherId;
                        SelectedMailBox.LowerId = mailId < SelectedMailBox.LowerId ? mailId : SelectedMailBox.LowerId;

                        var message = messages[i];
                        MimeMessage mimeMessage = await SelectedMailBox.MailFolder.GetMessageAsync(message.UniqueId);
                        MailItem mail = new MailItem(this);
                        mail.Fill(mimeMessage);
                        mail.Fill(message.UniqueId);
                        mail.Fill(message.Flags);

                        SelectedMailBox.MailItems.Add(mail);
                        AddMailToGroup(mail);
                    }
                    SaveMailsCache();
                }
                await SelectedMailBox.MailFolder.CloseAsync();
            });
            GetMailsTask.Dispose();
        }

        public void LoadMailsCache()
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                foreach (var mail in JsonTool.LoadMails())
                {
                    if(mail.Uid > SelectedMailBox.HigherId.Id)
                    {
                        SelectedMailBox.HigherId = new UniqueId(mail.Uid);
                    }
                    if(mail.Uid < SelectedMailBox.LowerId.Id)
                    {
                        SelectedMailBox.LowerId = new UniqueId(mail.Uid);
                    }
                    mail.Init(this);
                    SelectedMailBox.MailItems.Add(mail);
                    AddMailToGroup(mail);
                }
            });
        }
        public void SaveMailsCache()
        {
            JsonTool.SaveMails(SelectedMailBox.MailItems.ToList());
        }

        public void AddMailToGroup(MailItem mail)
        {
            if (!SelectedMailBox.MailGroups.Any(group => group.Date.Date == mail.Date.Date))
            {
                SelectedMailBox.MailGroups.InsertWhere(grp => grp.Date.Date < mail.Date.Date, new MailGroup(mail.Date));
            }
            SelectedMailBox.MailGroups.FirstOrDefault(group => group.Date.Date == mail.Date.Date).AddMail(mail);
        }
        public void ReloadMailGroups()
        {
            SelectedMailBox.MailGroups.Clear();
            foreach (var mail in SelectedMailBox.MailItems.OrderByDescending(mail => mail.Date))
            {
                AddMailToGroup(mail);
            }
        }
        public bool MailIsCache(UniqueId id)
        {
            return true;
        }
        public bool GetMailBody(UniqueId id)
        {
            return true;
        }
        public bool GetMailAttachments(UniqueId id)
        {
            return true;
        }
        
        public async void SeenMail(MailItem mail)
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                await SelectedMailBox.MailFolder.OpenAsync(FolderAccess.ReadWrite);
                await SelectedMailBox.MailFolder.AddFlagsAsync(new UniqueId(mail.Uid), MessageFlags.Seen, true);
                await SelectedMailBox.MailFolder.CloseAsync();
            });
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
