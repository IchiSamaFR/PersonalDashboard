using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDashboard.Model.Dashboard.Password
{
    public class PasswordItem : ObservableObject
    {
        private string login;
        public string Login
        {
            get
            {
                return login;
            }
            set
            {
                login = value;
                NotifyPropertyChanged();
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                NotifyPropertyChanged();
            }
        }
        
        public string PasswordDisplay
        {
            get
            {
                string str = "";
                return str.PadLeft(Password.Length, '*');
            }
        }

        public PasswordItem(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
