using MailKit;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDashboard.Model.Dashboard.Mail
{
    public class MailBox : ObservableObject
    {
        public UniqueId LowerId = UniqueId.MaxValue;
        public UniqueId HigherId = UniqueId.MinValue;

        private ObservableCollection<MailItem> _mailItems = new ObservableCollection<MailItem>();
        private ObservableCollection<MailGroup> _mailGroups = new ObservableCollection<MailGroup>();

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
        public ObservableCollection<MailGroup> MailGroups
        {
            get
            {
                return _mailGroups;
            }
            set
            {
                _mailGroups = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return MailFolder.Name;
            }
        }
        public string FullName
        {
            get
            {
                return MailFolder.FullName;
            }
        }
        public IMailFolder MailFolder { get; }


        public MailBox(IMailFolder mailFolder)
        {
            MailFolder = mailFolder;
        }
    }
}
