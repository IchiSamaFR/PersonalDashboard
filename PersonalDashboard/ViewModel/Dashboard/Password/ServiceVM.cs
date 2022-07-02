using PersonalDashboard.Model.Dashboard.Password;
using PersonalDashboard.View.Dashboard.Password;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalDashboard.ViewModel.Dashboard.Password
{
    public class ServiceVM : AbstractVM
    {
        private PasswordVM passwordVM { get; set; }
        public override UserControl UserControl { get; } = new ServiceView();

        #region Commands
        public ICommand CopyPasswordCmd { get; }
        public ICommand CopyLoginCmd { get; }
        #endregion

        private ServiceItem serviceItem;
        private ObservableCollection<PasswordItem> passwordItems = new ObservableCollection<PasswordItem>();

        public ServiceItem ServiceItem
        {
            get
            {
                return serviceItem;
            }
            set
            {
                serviceItem = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<PasswordItem> PasswordItems
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


        public void CopyPass(PasswordItem passwordItem)
        {
            NotificationsVM.instance.AddNotification(this, "Password copied.");
            Clipboard.SetText(passwordItem.Password);
        }
        public void CopyLogin(PasswordItem passwordItem)
        {
            NotificationsVM.instance.AddNotification(this, "Login copied.");
            Clipboard.SetText(passwordItem.Login);
        }

        public ServiceVM(PasswordVM passwordVM)
        {
            this.passwordVM = passwordVM;
            Name = "Service";
            CopyPasswordCmd = new RelayCommand(o => { CopyPass((PasswordItem)o); });
            CopyLoginCmd = new RelayCommand(o => { CopyLogin((PasswordItem)o); });
        }

        public void LoadService(ServiceItem serviceItem)
        {
            ServiceItem = serviceItem;
            PasswordItems = ServiceItem.Passwords;
        }
    }
}
