using ModernDesign.Model.Dashboard.Password;
using ModernDesign.View.Dashboard;
using ModernDesign.ViewModel.Dashboard.Password;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard
{
    public class PasswordVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new PasswordView();

        public string ServiceNameToAdd { get; set; }
        public string LoginToAdd { get; set; }
        public string PassToAdd { get; set; }

        private ObservableCollection<ServiceItem> serviceItems = new ObservableCollection<ServiceItem>();
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

        private ServiceItem serviceSelected;
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

        public ServiceVM ServiceVM;
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

        private Double actualWidth = Double.NaN;
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

        public PasswordVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Password";
            Icon = ModernDesign.Properties.Resources.security_shield_green;

            ServiceVM = new ServiceVM(this);

            LoadPasswords();
        }

        public void LoadPasswords()
        {
            var fbservice = new ServiceItem("Facebook");
            fbservice.AddPassword("log", "pass");
            fbservice.AddPassword("log2", "pass2");

            var isservice = new ServiceItem("Insta");
            isservice.AddPassword("log", "pass");
            isservice.AddPassword("log2", "pass2");
            isservice.AddPassword("log3", "pass3");

            var ytservice = new ServiceItem("Youtube");
            ytservice.AddPassword("log", "pass");

            ServiceItems.Add(fbservice);
            ServiceItems.Add(isservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
            ServiceItems.Add(ytservice);
        }

        public void SavePasswords()
        {

        }

        public void AddPassword()
        {
            var service = ServiceItems.First(item => item.Name == ServiceNameToAdd);
            service.AddPassword(LoginToAdd, PassToAdd);

            SavePasswords();
        }
    }
}
