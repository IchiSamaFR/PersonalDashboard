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
                NotifyPropertyChanged();
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

        private ImapClient imapClient;
        private IMailFolder inbox;
        private Task GetMailsTask;

        public MailVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Mail";
            Icon = ModernDesign.Properties.Resources.envelope;

            Task.Run(() => InitMails());
        }

        public async void LoadMails()
        {
            if(GetMailsTask == null || GetMailsTask.IsCanceled || GetMailsTask.IsCompleted)
            {
                GetMailsTask = Task.Run(() => GetMails());
            }
        }

        public override void OnFocus()
        {
            if (Focused)
            {
                if(MailItems.Count <= 0)
                {
                    LoadMails();
                }
            }
            else
            {
                if (GetMailsTask != null && GetMailsTask.IsCompleted)
                {
                    GetMailsTask.Dispose();
                }
                GetMailsTask = null;
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
            imapClient = new ImapClient();

            await imapClient.ConnectAsync("outlook.office365.com", 993, true);
            await imapClient.AuthenticateAsync(new NetworkCredential(Config.Instance.MailAdress, Config.Instance.MailPass));
            inbox = imapClient.Inbox;
        }

        public async Task GetMails()
        {
            await IsSet();

            await App.Current.Dispatcher.Invoke(async () =>
            {
                inbox.Open(FolderAccess.ReadOnly);

                int mailItemsCount = MailItems?.Count ?? 0;
                if (inbox.Count > mailItemsCount)
                {
                    var lastMessages = Enumerable.Range(inbox.Count - 20 - mailItemsCount, 20).ToList();
                    var messages = await inbox.FetchAsync(lastMessages, MailKit.MessageSummaryItems.UniqueId);

                    foreach (var message in messages.Reverse())
                    {
                        MimeMessage mimeMessage = inbox.GetMessage(message.UniqueId);
                        MailItem tempEmail = new MailItem()
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
                        MailItems.Add(tempEmail);
                        NotifyPropertyChanged(nameof(MailItems));
                        await Task.Delay(1);
                    }
                }
            });
        }
    }
}
