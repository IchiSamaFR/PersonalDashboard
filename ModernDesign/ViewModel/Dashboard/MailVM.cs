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
        
        private Task GetMailsTask;

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

        public override void OnFocus()
        {
            if (Focused)
            {
                GetMailsTask = Task.Run(() => GetMails());
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

        public async Task GetMails()
        {
            await App.Current.Dispatcher.Invoke(async () =>  
            {
                using (ImapClient client = new ImapClient())
                {
                    client.Connect("outlook.office365.com", 993, true);
                    client.Authenticate(new NetworkCredential(Config.Instance.MailAdress, Config.Instance.MailPass));

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    int mailItemsCount = MailItems?.Count ?? 0;
                    if (inbox.Count > mailItemsCount)
                    {
                        var lastMessages = Enumerable.Range(inbox.Count - 20 - mailItemsCount, 20).ToList();
                        var messages = await inbox.FetchAsync(lastMessages, MailKit.MessageSummaryItems.UniqueId);

                        List<MailItem> tempList = new List<MailItem>();
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
                                TextBody = mimeMessage.TextBody,
                            };
                            tempList.Insert(0, tempEmail);
                        }
                        await MailItems.AddRangeAsync(tempList);
                    }
                }
                NotifyPropertyChanged(nameof(MailItems));
            });
        }
    }
}
