using MailKit;
using MimeKit;
using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDesign.Model.Dashboard.Mail
{
    public class MailItem : ObservableObject
    {
        public bool IsOpened { get; set; }

        public UniqueId Uid { get; set; }
        private string fromDisplayName;
        public string FromDisplayName
        {
            get
            {
                return fromDisplayName;
            }
            set
            {
                fromDisplayName = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FromDisplay));
            }
        }

        private string fromEmail;
        public string FromEmail
        {
            get
            {
                return fromEmail;
            }
            set
            {
                fromEmail = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FromDisplay));
            }
        }

        public string FromDisplay
        {
            get
            {
                return string.IsNullOrEmpty(FromDisplayName) ? FromEmail : FromDisplayName;
            }
        }

        public List<string> ToDisplayName { get; set; }
        public List<string> ToEmail { get; set; }
        public string Subject { get; set; }
        public string SubjectSub
        {
            get
            {
                return Subject?.Length > 33 ? $"{Subject?.Substring(0, 30)}..." : Subject;
            }
        }

        public DateTime TimeReceived { get; set; }
        public string TimeDisplay
        {
            get
            {
                return TimeReceived.ToString("ddd dd/MM/yyyy");
            }
        }
        public bool HasAttachment { get; set; }
        public List<MimeEntity> Attachments { get; set; }

        public string HtmlBody { get; set; }
    }
}
