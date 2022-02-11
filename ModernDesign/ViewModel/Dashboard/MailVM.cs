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

        public MailVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Mail";
            Icon = ModernDesign.Properties.Resources.envelope;

            LoadMails();
        }
        public void LoadMails()
        {

        }

        private Task GetMailsTask;

        public override void OnFocus()
        {
            if (Focused)
            {
                GetMails();
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

        public void GetMails()
        {
            MailItems = new ObservableCollection<MailItem>();

            try
            {
                using (ImapClient client = new ImapClient())
                {
                    client.Connect("outlook.office365.com", 993, true);
                    client.Authenticate(new NetworkCredential("***REMOVED***", "***REMOVED***"));

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);


                    if (inbox.Count > 0)
                    {
                        int pageStartIndex = inbox.Count - 10;
                        int pageEndIndex = inbox.Count - 1;
                        
                        var messages = inbox.Fetch(pageStartIndex, pageEndIndex, MessageSummaryItems.UniqueId);
                        
                        foreach (var message in messages)
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
                            };
                            MailItems.Add(tempEmail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
