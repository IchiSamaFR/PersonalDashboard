using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDesign.Model.Dashboard.Password
{
    public class PasswordItem : ObservableObject
    {
        private string service;
        public string Service
        {
            get
            {
                return service;
            }
            set
            {
                service = value;
                NotifyPropertyChanged();
            }
        }

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

        public PasswordItem(string service, string login, string password)
        {
            Service = service;
            Login = login;
            Password = password;
        }
    }
}
