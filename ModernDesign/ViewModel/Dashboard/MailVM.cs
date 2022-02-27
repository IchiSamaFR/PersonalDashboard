using ModernDesign.Model.Dashboard.Mail;
using ModernDesign.View.Dashboard;
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
using ModernDesign.Model;
using ModernDesign.ViewModel.Tools;
using System.Windows.Input;

namespace ModernDesign.ViewModel.Dashboard
{
    public class MailVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new MailView();

        private ObservableCollection<MailItem> _mailItems = new ObservableCollection<MailItem>();
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

        private ObservableCollection<DateItem> _mailDateItems = new ObservableCollection<DateItem>();
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

        private ObservableCollection<UserControl> _mailControls = new ObservableCollection<UserControl>();
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
        
        private ICommand _loadNewMailsCmd;
        public ICommand LoadNewMailsCmd
        {
            get
            {
                if (_loadNewMailsCmd == null)
                {
                    _loadNewMailsCmd = new RelayCommand(o => { LoadMails(); });
                }
                return _loadNewMailsCmd;
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

        private MailItem _mailSelected;
        public MailItem MailSelected
        {
            get
            {
                return _mailSelected;
            }
            set
            {
                if (value != null && _mailSelected != value)
                {
                    _mailSelected = value;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        string background = "#" + App.Current.Resources["col_Background"].ToString().Substring(3);
                        string foreground = "#" + App.Current.Resources["col_LightForeground"].ToString().Substring(3);
                        MailViewer.NavigateToString($"<html style=\"background-color:#DDD;\"/>" + MailSelected.HtmlDisplay);
                    });
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(MailViewer));
                }
            }
        }
        private UserControl _controlSelected;
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

        private ImapClient imapClient;
        private IMailFolder inbox;
        private Task GetMailsTask;

        public MailVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Mail";
            Icon = ModernDesign.Properties.Resources.envelope;
        }

        public void LoadMails(int amount = 10)
        {
            if(GetMailsTask == null || GetMailsTask.IsCanceled || GetMailsTask.IsCompleted || GetMailsTask.IsFaulted)
            {
                GetMailsTask = Task.Run(() => GetMails(amount));
            }
        }

        public override void OnFocus()
        {
            base.OnFocus();
            if (Focused && imapClient == null)
            {
                Task.Run(() => InitMails());
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
            try
            {
                imapClient = new ImapClient();

                await imapClient.ConnectAsync("outlook.office365.com", 993, true);
                await imapClient.AuthenticateAsync(new NetworkCredential(Config.Instance.MailAdress, Config.Instance.MailPass));
                inbox = imapClient.Inbox;
                LoadMails(20);
            }
            catch
            {
                NotificationsVM.instance.AddNotification(this, "Could not connect to the mailbox.");
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
                    var lastMessages = Enumerable.Range(inbox.Count - amount - mailItemsCount, amount).ToList();
                    //This fetch some times wait for nothing
                    var messages = await inbox.FetchAsync(lastMessages, MailKit.MessageSummaryItems.UniqueId);
                    foreach (var message in messages.Reverse())
                    {
                        // Item 38811 bug car trop grand, trop de data
                        MimeMessage mimeMessage = await inbox.GetMessageAsync(message.UniqueId);
                        MailItem tempMail = new MailItem()
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
                        };
                        AddMail(tempMail);
                    }
                }
            });
            GetMailsTask.Dispose();
        }

        public void AddMail(MailItem mail)
        {
            if (MailItems.Count > 0 && MailItems[MailItems.Count - 1].TimeReceived.Date != mail.TimeReceived.Date)
            {
                MailControls.Add(new DateItem(mail.TimeReceived).UserControl);
            }
            MailControls.Add(mail.UserControl);
            MailItems.Add(mail);
        }
    }
}
