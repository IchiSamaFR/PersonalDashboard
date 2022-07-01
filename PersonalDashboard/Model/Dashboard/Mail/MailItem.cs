using MailKit;
using MimeKit;
using PersonalDashboard.View.Dashboard.Mail;
using PersonalDashboard.ViewModel;
using PersonalDashboard.ViewModel.Dashboard;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalDashboard.Model.Dashboard.Mail
{
    public class MailItem : ObservableObject
    {
        private MailVM mailVM;
        public UserControl UserControl { get; } = new MailItemView();

        private string fromDisplayName;
        private string fromEmail;
        private string _htmlBody;
        private MessageFlags? flags;
        private string _textBody;
        #region Commands

        private ICommand _deleteMailCmd;
        public ICommand DeleteMailCmd
        {
            get
            {
                if (_deleteMailCmd == null)
                {
                    _deleteMailCmd = new RelayCommand(o => { DeleteMail(); });
                }
                return _deleteMailCmd;
            }
        }
        #endregion

        public bool IsOpened { get; set; }

        public UniqueId Uid { get; set; }
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
        public List<string> ToName { get; set; }
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

        public string HtmlBody
        {
            get
            {
                return _htmlBody;
            }
            set
            {
                _htmlBody = value;
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
                    return _htmlBody;
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
        public string TextBody
        {
            get
            {
                return _textBody;
            }
            set
            {
                _textBody = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TextDisplay));
            }
        }
        public MessageFlags? Flags
        {
            get
            {
                return flags;
            }
            set
            {
                flags = value;
                NotifyPropertyChanged();
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

        public MailItem(MailVM vm)
        {
            UserControl.DataContext = this;
            mailVM = vm;
        }

        public void DeleteMail()
        {
            mailVM.DeleteMail(this);
        }
        public override string ToString()
        {

            return "";
        }
    }
}
