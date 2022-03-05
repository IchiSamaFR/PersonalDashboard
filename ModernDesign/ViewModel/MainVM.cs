using PersonalDashboard.Model;
using PersonalDashboard.ViewModel.Dashboard;
using PersonalDashboard.ViewModel.Login;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.ViewModel
{
    public class MainVM : AbstractVM
    {
        public LoginVM LoginVM;
        public DashboardVM DashboardVM;

        private ConfigItem config;

        private AbstractVM actualVM;
        public AbstractVM ActualVM
        {
            get
            {
                return actualVM;
            }
            set
            {
                if(actualVM != value)
                {
                    actualVM?.Hide();
                    actualVM = value;
                    actualVM.Show();
                    NotifyPropertyChanged("ActualVM");
                    NotifyPropertyChanged("UserControl");
                }
            }
        }

        public override UserControl UserControl
        {
            get
            {
                return ActualVM?.UserControl;
            }
        }
        
        private NotificationsVM notificationsVM;
        public NotificationsVM NotificationsVM
        {
            get
            {
                return notificationsVM;
            }
            set
            {
                notificationsVM = value;
                notificationsVM.Show();
                NotifyPropertyChanged();
            }
        }
        public UserControl NotificationControl
        {
            get
            {
                return NotificationsVM?.UserControl;
            }
        }

        public MainVM()
        {
            config = JsonTool.LoadConfig();

            LoginVM = new LoginVM(this);
            NotificationsVM = new NotificationsVM(this);
            ActualVM = LoginVM;
        }

        public void Log()
        {
            DashboardVM = new DashboardVM(this);
            ActualVM = DashboardVM;
        }
    }
}
