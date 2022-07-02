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
        private readonly DashboardVM dashboardVM;
        private const string MAILSFOLDER = @"mails";
        private const string MAILSCONFIG = @"config.txt";
        private const string MAILSCACHE = @"cache";
        public override UserControl UserControl { get; } = new MailView();

        private UniqueId lowerId = UniqueId.MaxValue;
        private UniqueId higherId = UniqueId.MinValue;
        private ImapClient imapClient;
        private IMailFolder inbox;
        private Task GetMailsTask;
        private MailItem _mailSelected;
        private ObservableCollection<MailItem> _mailItems = new ObservableCollection<MailItem>();
        private ObservableCollection<MailGroup> _mailDateItems = new ObservableCollection<MailGroup>();

        public MailItem MailSelected
        {
            get
            {
                return _mailSelected;
            }
            set
            {
                if (value != null && (!inbox.IsOpen || value.Flags == MessageFlags.Seen))
                {
                    if (!inbox.IsOpen)
                    {
                        SeenMail(_mailSelected);
                    }

                    _mailSelected = value;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MailViewer.NavigateToString(MailSelected.HtmlDisplay);
                    });
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(MailViewerVisible));
                }
            }
        }
        public ObservableCollection<MailItem> MailItems
        {
            get
            {
                return _mailItems;
            }
            set
            {
                _mailItems = value;
            }
        }
        public ObservableCollection<MailGroup> MailGroups
        {
            get
            {
                return _mailDateItems;
            }
            set
            {
                _mailDateItems = value;
                NotifyPropertyChanged();
            }
        }
        public string GetConfigFilePath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), MAILSFOLDER, MAILSCONFIG);
            }
        }
        public string GetTempFolderPath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), MAILSFOLDER, MAILSCACHE);
            }
        }

        public ICommand LoadNewMailsCmd { get; }
        public Visibility MailViewerVisible
        {
            get
            {
                return MailSelected == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private WebBrowser MailViewer { get { return ((MailView)UserControl).webBrowser; } }


        public MailVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Mail";
            Icon = PersonalDashboard.Properties.Resources.envelope;

            LoadNewMailsCmd = new RelayCommand(o => { LoadMails(); });
            Task.Run(() => InitMails());
        }
        public override void OnFocus()
        {
            base.OnFocus();
        }
        
        public void LoadMails(int amount = 10)
        {
            if(GetMailsTask == null || GetMailsTask.IsCanceled || GetMailsTask.IsCompleted || GetMailsTask.IsFaulted)
            {
                GetMailsTask = Task.Run(() => GetMails(amount));
            }
        }

        public async Task<ImapClient> IsSet()
        {
            while (inbox == null)
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
                if(MailItems.Count < 20)
                {
                    LoadMails(20);
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
                inbox = imapClient.Inbox;
                return true;
            }
            catch
            {
                NotificationsVM.instance.AddNotification(this, "Could not connect to the mailbox.");
                return false;
            }
        }
        public async Task GetMails(int amount)
        {
            await IsSet();

            await App.Current.Dispatcher.Invoke(async () =>
            {
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                int mailItemsCount = MailItems?.Count ?? 0;
                if (inbox.Count > mailItemsCount)
                {
                    var lstMsg = await inbox.SearchAsync(SearchQuery.All);
                    lstMsg = lstMsg.Where(id => id < lowerId || id > higherId).OrderByDescending(msg => msg).Take(amount).ToList();
                    var messages = await inbox.FetchAsync(lstMsg, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Flags);
                    messages = messages.OrderByDescending(item => item.UniqueId).ToList();
                    for (int i = 0; i < messages.Count; i++)
                    {
                        UniqueId mailId = messages[i].UniqueId;
                        higherId = mailId > higherId ? mailId : higherId;
                        lowerId = mailId < lowerId ? mailId : lowerId;

                        var message = messages[i];
                        MimeMessage mimeMessage = await inbox.GetMessageAsync(message.UniqueId);
                        MailItem tempMail = new MailItem(this);
                        tempMail.Fill(mimeMessage);
                        tempMail.Fill(message.UniqueId);
                        tempMail.Fill(message.Flags);
                        
                        AddMail(tempMail);
                    }
                    SaveMailsCache();
                }
                await inbox.CloseAsync();
            });
            GetMailsTask.Dispose();
        }

        public void AddMail(MailItem mail)
        {
            if (!MailGroups.Any(group => group.TimeReceived.Date == mail.TimeReceived.Date))
            {
                MailGroups.Add(new MailGroup(mail.TimeReceived));
            }
            MailItems.Add(mail);
            MailGroups.FirstOrDefault(group => group.TimeReceived.Date == mail.TimeReceived.Date).AddMail(mail);
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
        
        public bool LoadMailsCache()
        {
            foreach (var item in JsonTool.LoadMails())
            {
                AddMail(item);
            }
            return false;
        }
        public void SaveMailsCache()
        {
            JsonTool.SaveMails(MailItems.ToList());
        }
        public async void SeenMail(MailItem mail)
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                await inbox.OpenAsync(FolderAccess.ReadWrite);
                await inbox.AddFlagsAsync(mail.Uid, MessageFlags.Seen, true);
                await inbox.CloseAsync();
            });
        }
        public async void DeleteMail(MailItem mail)
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                await inbox.OpenAsync(FolderAccess.ReadWrite);
                await inbox.AddFlagsAsync(mail.Uid, MessageFlags.Deleted, true);
                await inbox.CloseAsync();
            });
        }
    }
}
