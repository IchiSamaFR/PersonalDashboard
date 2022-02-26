using ModernDesign.Model.Dashboard.Password;
using ModernDesign.View.Dashboard.Password;
using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernDesign.ViewModel.Dashboard.Password
{
    public class ServiceVM : AbstractVM
    {
        private PasswordVM passwordVM { get; set; }
        public override UserControl UserControl { get; } = new ServiceView();

        #region Commands

        private ICommand _copyPasswordCmd;
        public ICommand CopyPasswordCmd
        {
            get
            {
                if (_copyPasswordCmd == null)
                {
                    _copyPasswordCmd = new RelayCommand(o => { CopyPass((PasswordItem)o); });
                }
                return _copyPasswordCmd;
            }
        }
        private ICommand _copyLogin;
        public ICommand CopyLoginCmd
        {
            get
            {
                if (_copyLogin == null)
                {
                    _copyLogin = new RelayCommand(o => { CopyLogin((PasswordItem)o); });
                }
                return _copyLogin;
            }
        }
        #endregion


        public void CopyPass(PasswordItem passwordItem)
        {
            Clipboard.SetText(passwordItem.Password);
        }
        public void CopyLogin(PasswordItem passwordItem)
        {
            Clipboard.SetText(passwordItem.Login);
        }

        private ServiceItem serviceItem;
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

        private ObservableCollection<PasswordItem> passwordItems = new ObservableCollection<PasswordItem>();
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

        public ServiceVM(PasswordVM passwordVM)
        {
            this.passwordVM = passwordVM;
            Name = "Service";
        }

        public void LoadService(ServiceItem serviceItem)
        {
            ServiceItem = serviceItem;
            PasswordItems = ServiceItem.PasswordItems;
        }
    }
}
