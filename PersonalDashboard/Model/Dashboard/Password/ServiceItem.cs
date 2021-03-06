using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDashboard.Model.Dashboard.Password
{
    public class ServiceItem : ObservableObject
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PasswordItem> passwordItems = new ObservableCollection<PasswordItem>();
        public ObservableCollection<PasswordItem> Passwords
        {
            get
            {
                return passwordItems;
            }
            set
            {
                passwordItems = value;
                NotifyPropertyChanged();
            }
        }

        public ServiceItem(string name)
        {
            Name = name;
        }

        public void AddPassword(string login, string password)
        {
            Passwords.Add(new PasswordItem(login, password));
        }
    }
}
