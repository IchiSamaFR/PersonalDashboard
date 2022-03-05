using MailKit;
using MimeKit;
using PersonalDashboard.View.Dashboard.Mail;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.Model.Dashboard.Mail
{
    public class MailItem : ObservableObject
    {
        public UserControl UserControl = new MailItemView();

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
                return TimeReceived.ToString("ddd dd/MM/yy");
            }
        }

        public bool HasAttachment { get; set; }
        public List<MimeEntity> Attachments { get; set; }

        public string htmlBody { get; set; }
        public string HtmlBody
        {
            get
            {
                return htmlBody;
            }
            set
            {
                htmlBody = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HtmlDisplay));
            }
        }
        public string HtmlDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(HtmlBody))
                {
                    return htmlBody;
                }
                else if (!string.IsNullOrEmpty(TextBody))
                {
                    return TextBody.Replace("\r", "<br/>");
                }
                else
                {
                    return "";
                }
            }
        }

        public string textBody { get; set; }
        public string TextBody
        {
            get
            {
                return textBody;
            }
            set
            {
                textBody = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TextDisplay));
            }
        }
        
        public string TextDisplay
        {
            get
            {
                if (TextBody == null)
                {
                    return "";
                }
                else
                {
                    return Regex.Replace(TextBody?.Replace("\r", " ").Replace("\n", " ").Trim(), @"\s+", " ");
                }
            }
        }

        public MailItem()
        {
            UserControl.DataContext = this;
        }
    }
}
