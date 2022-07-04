using MailKit;
using MimeKit;
using PersonalDashboard.View.Dashboard.Mail;
using PersonalDashboard.ViewModel;
using PersonalDashboard.ViewModel.Dashboard;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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

        public uint Uid { get; set; }
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
        public List<string> ReplyTo { get; set; }
        public List<string> ToEmail { get; set; }
        public List<string> CcEmail { get; set; }
        public string Subject { get; set; }
        public List<MimeEntity> Attachments { get; set; } = new List<MimeEntity>();
        public DateTime Date { get; set; }
        public bool HasAttachment
        {
            get
            {
                return Attachments.Count() > 0;
            }
        }
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


        public MailItem(MailVM vm) : base()
        {
            Init(vm);
        }
        public void Init(MailVM vm)
        {
            mailVM = vm;
        }
        public void Fill(MimeMessage mimeMessage)
        {
            FromDisplayName = mimeMessage.From.FirstOrDefault().Name;
            FromEmail = mimeMessage.From.Mailboxes.Select(o => o.Address).FirstOrDefault();
            ToEmail = mimeMessage.To.Mailboxes.Select(item => item.Address).ToList();
            Subject = mimeMessage.Subject;
            Date = mimeMessage.Date.DateTime;
            Attachments = mimeMessage.Attachments.ToList();
            HtmlBody = mimeMessage.HtmlBody;
            TextBody = mimeMessage.TextBody;
        }
        public void Fill(UniqueId id)
        {
            Uid = uint.Parse(id.ToString());
        }
        public void Fill(MessageFlags? flags)
        {
            Flags = flags;
        }

        public string ToEml(string path)
        {
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(FromEmail);
            ReplyTo.ForEach(email => msg.ReplyToList.Add(email));
            ToEmail.ForEach(email => msg.To.Add(email));
            CcEmail.ForEach(email => msg.CC.Add(email));
            Attachments.ForEach(att => msg.Attachments.Add(new Attachment(att.ContentBase.AbsolutePath)));
            msg.Subject = Subject;
            msg.Body = HtmlBody;

            return msg.ToEml();
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
