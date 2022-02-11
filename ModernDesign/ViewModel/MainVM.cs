using ModernDesign.Model;
using ModernDesign.ViewModel.Dashboard;
using ModernDesign.ViewModel.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel
{
    public class MainVM : AbstractVM
    {
        public LoginVM LoginVM;
        public DashboardVM DashboardVM;

        private Config config;

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

        public MainVM()
        {
            config = new Config();
            LoginVM = new LoginVM(this);
            ActualVM = LoginVM;
        }

        public void Log()
        {
            DashboardVM = new DashboardVM(this);
            ActualVM = DashboardVM;
        }
    }
}
