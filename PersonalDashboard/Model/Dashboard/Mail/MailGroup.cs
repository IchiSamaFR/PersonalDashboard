using PersonalDashboard.View.Dashboard.Mail;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.Model.Dashboard.Mail
{
    public class MailGroup : ObservableObject
    {
        private ObservableCollection<MailItem> _mailItems;
        private bool _isOpen = true;

        public DateTime Date { get; set; }
        public string TimeDisplay
        {
            get
            {
                return Date.ToString("ddd dd/MM/yy");
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
                NotifyPropertyChanged();
            }
        }
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsToday
        {
            get
            {
                return Date == DateTime.Now.Date;
            }
        }

        public MailGroup(DateTime dateTime)
        {
            Date = dateTime.Date;
            MailItems = new ObservableCollection<MailItem>();
        }
        public MailGroup AddMail(MailItem mail)
        {
            MailItems.InsertWhere(m => m.Uid < mail.Uid, mail);
            return this;
        }
    }
}
