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
        public Action OnCountChanged;
        public bool CountChanged = false;

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
            MailFolder.MessageExpunged += MailFolder_MessageExpunged;
        }

        private void MailFolder_MessageExpunged(object sender, MessageEventArgs e)
        {
            RemoveMail(e.UniqueId);
        }


        public void AddMail(MailItem mail)
        {
            if (mail.Uid > HigherId.Id)
            {
                HigherId = new UniqueId(mail.Uid);
            }
            if (mail.Uid < LowerId.Id)
            {
                LowerId = new UniqueId(mail.Uid);
            }
            MailItems.Add(mail);
            AddMailToGroup(mail);
        }
        public void RemoveMail(UniqueId? uniqueId)
        {
            if(uniqueId == null)
            {
                return;
            }
            uint id = uint.Parse(uniqueId.ToString());

            MailItems.RemoveWhere(mail => mail.Uid == id);
            var mailGroup = MailGroups.First(group => group.MailItems.Any(mail => mail.Uid == id));
            mailGroup.MailItems.RemoveWhere(mail => mail.Uid == id);
            if(mailGroup.MailItems.Count() == 0)
            {
                MailGroups.Remove(mailGroup);
            }
        }
        public void AddMailToGroup(MailItem mail)
        {
            if (!MailGroups.Any(group => group.Date.Date == mail.Date.Date))
            {
                MailGroups.InsertWhere(grp => grp.Date.Date < mail.Date.Date, new MailGroup(mail.Date));
            }
            MailGroups.First(group => group.Date.Date == mail.Date.Date).AddMail(mail);
        }
        public void ReloadMailGroups()
        {
            MailGroups.Clear();
            foreach (var mail in MailItems.OrderByDescending(mail => mail.Date))
            {
                AddMailToGroup(mail);
            }
        }
    }
}
