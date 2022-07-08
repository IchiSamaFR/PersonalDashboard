﻿using MailKit;
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
        
        private MailboxAddress fromEmail;
        private string _htmlBody;
        private MessageFlags? flags;
        private string _textBody;

        private bool _isFocused;
        public bool IsFocused
        {
            get
            {
                return _isFocused;
            }
            set
            {
                _isFocused = value;
                NotifyPropertyChanged();
            }
        }

        public uint Uid { get; set; }
        public MailboxAddress FromEmail
        {
            get
            {
                return fromEmail;
            }
            set
            {
                fromEmail = value;
                NotifyPropertyChanged();
            }
        }
        public List<InternetAddress> ReplyTo { get; set; }
        public List<InternetAddress> ToEmail { get; set; }
        public List<InternetAddress> CcEmail { get; set; }
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

        public MailItem(MailVM vm)
        {
            Init(vm);
        }
        public void Init(MailVM vm)
        {
            mailVM = vm;
        }
        public void Fill(MimeMessage mimeMessage)
        {
            FromEmail = mimeMessage.From.Mailboxes.FirstOrDefault();
            ReplyTo = mimeMessage.ReplyTo.Select(email => email).ToList();
            ToEmail = mimeMessage.To.Select(email => email).ToList();
            CcEmail = mimeMessage.Cc.Select(email => email).ToList();
            Subject = mimeMessage.Subject;
            Date = mimeMessage.Date.DateTime;
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

        public void SaveAsEml(string path)
        {
            MimeMessage msg = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = HtmlBody;
            bodyBuilder.TextBody = TextBody;
            Attachments.ForEach(att => bodyBuilder.Attachments.Add(att));

            msg.From.Add(FromEmail);
            ReplyTo?.ForEach(email => msg.ReplyTo.Add(email));
            ToEmail?.ForEach(email => msg.To.Add(email));
            CcEmail?.ForEach(email => msg.Cc.Add(email));

            msg.Subject = Subject;
            msg.Body = bodyBuilder.ToMessageBody();
            msg.Date = Date;
            msg.ResentDate = Date;

            msg.WriteTo(path);
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
