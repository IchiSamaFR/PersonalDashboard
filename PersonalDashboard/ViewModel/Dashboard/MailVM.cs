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

namespace PersonalDashboard.ViewModel.Dashboard
{
    public class MailVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new MailView();

        private ImapClient imapClient;
        private IMailFolder inbox;
        private Task GetMailsTask;
        private UserControl _controlSelected;
        private MailItem _mailSelected;
        private ObservableCollection<MailItem> _mailItems = new ObservableCollection<MailItem>();
        private ObservableCollection<DateItem> _mailDateItems = new ObservableCollection<DateItem>();
        private ObservableCollection<UserControl> _mailControls = new ObservableCollection<UserControl>();

        public UserControl ControlSelected
        {
            get
            {
                return _controlSelected;
            }
            set
            {
                _controlSelected = value;
                MailSelected = MailItems.FirstOrDefault(item => item.UserControl == value);
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
        public ObservableCollection<DateItem> MailDateItems
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
        public ObservableCollection<UserControl> MailControls
        {
            get
            {
                return _mailControls;
            }
            set
            {
                _mailControls = value;
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

        private UserControl UsercontrolSelected
        {
            get
            {
                return MailSelected.UserControl;
            }
            set
            {
                MailItem selected = MailItems.FirstOrDefault(item => item.UserControl == value);
                if(selected != null)
                {
                    MailSelected = MailItems.FirstOrDefault(item => item.UserControl == value);
                }
            }
        }

        public MailVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Mail";
            Icon = PersonalDashboard.Properties.Resources.envelope;

            LoadNewMailsCmd = new RelayCommand(o => { LoadMails(); });
        }
        public override void OnFocus()
        {
            base.OnFocus();
            if (Focused && imapClient == null)
            {
                Task.Run(() => InitMails());
            }
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
                LoadMails(20);
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
                    lstMsg.OrderByDescending(msg => msg.Id);
                    var messages = await inbox.FetchAsync(lstMsg, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Flags);
                    messages = messages.OrderByDescending(item => item.UniqueId).ToList();
                    for (int i = 0; i < messages.Count; i++)
                    {
                        var message = messages[i];
                        MimeMessage mimeMessage = await inbox.GetMessageAsync(message.UniqueId);

                        MailItem tempMail = new MailItem(this)
                        {
                            Uid = message.UniqueId,
                            FromDisplayName = mimeMessage.From.FirstOrDefault().Name,
                            FromEmail = mimeMessage.From.Mailboxes.Select(o => o.Address).FirstOrDefault(),
                            ToDisplayName = mimeMessage.To.Select(item => item.Name).ToList(),
                            ToEmail = mimeMessage.To.Mailboxes.Select(item => item.Address).ToList(),
                            Subject = mimeMessage.Subject,
                            TimeReceived = mimeMessage.Date.DateTime,
                            HasAttachment = mimeMessage.Attachments.Count() > 0 ? true : false,
                            Attachments = mimeMessage.Attachments.ToList(),
                            HtmlBody = mimeMessage.HtmlBody,
                            TextBody = mimeMessage.TextBody,
                            Flags = messages[i].Flags,
                        };
                        AddMail(tempMail);
                    }
                }
                await inbox.CloseAsync();
            });
            GetMailsTask.Dispose();
        }

        public void AddMail(MailItem mail)
        {
            if (MailItems.Count > 0 && MailItems.LastOrDefault().TimeReceived.Date != mail.TimeReceived.Date)
            {
                MailControls.Add(new DateItem(mail.TimeReceived).UserControl);
            }
            MailControls.Add(mail.UserControl);
            MailItems.Add(mail);
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
