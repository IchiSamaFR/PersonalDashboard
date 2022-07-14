using PersonalDashboard.Model;
using PersonalDashboard.Model.Dashboard.Password;
using PersonalDashboard.View.Dashboard;
using PersonalDashboard.ViewModel.Dashboard.Password;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PersonalDashboard.ViewModel.Dashboard
{
    public class PasswordVM : AbstractVM
    {
        private readonly DashboardVM dashboardVM;
        public override UserControl UserControl { get; } = new PasswordView();

        private ServiceItem serviceSelected;
        private Double actualWidth = Double.NaN;
        private ObservableCollection<ServiceItem> serviceItems = new ObservableCollection<ServiceItem>();

        public ServiceItem ServiceSelected
        {
            get
            {
                return serviceSelected;
            }
            set
            {
                serviceSelected = value;
                ServiceVM.LoadService(serviceSelected);
                NotifyPropertyChanged(nameof(ServiceVisibility));

                if(serviceSelected != null)
                {
                    ActualWidth = 480;
                }
                else
                {
                    ActualWidth = Double.NaN;
                }
            }
        }
        public UserControl ActualView
        {
            get
            {
                return ServiceVM?.UserControl;
            }
        }
        public Visibility ServiceVisibility
        {
            get
            {
                return ServiceSelected == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Double ActualWidth
        {
            get
            {
                return actualWidth;
            }
            set
            {
                actualWidth = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<ServiceItem> ServiceItems
        {
            get
            {
                return serviceItems;
            }
            set
            {
                serviceItems = value;
            }
        }

        public ServiceVM ServiceVM;
        public string ServiceNameToAdd { get; set; }
        public string LoginToAdd { get; set; }
        public string PassToAdd { get; set; }

        public PasswordVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Password";
            Icon = PersonalDashboard.Properties.Resources.security_shield_green;

            ServiceVM = new ServiceVM(this);

            LoadPasswords();

            SavePasswords();
        }

        public void LoadPasswords()
        {
            var fbservice = new ServiceItem("Facebook");
            fbservice.AddPassword("mylogin", "pass");
            fbservice.AddPassword("fblogin", "pass2");

            var isservice = new ServiceItem("Insta");
            isservice.AddPassword("mylogin", "pass");
            isservice.AddPassword("fblogin", "pass2");

            ServiceItems.Add(fbservice);
        }

        public void SavePasswords()
        {
            JsonTool.SavePasswords(ServiceItems.ToList());
        }

        public void AddPassword()
        {
            var service = ServiceItems.First(item => item.Name == ServiceNameToAdd);
            service.AddPassword(LoginToAdd, PassToAdd);

            SavePasswords();
        }
    }
}
