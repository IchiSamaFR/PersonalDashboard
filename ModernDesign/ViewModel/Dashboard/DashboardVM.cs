using ModernDesign.View.Dashboard;
using ModernDesign.ViewModel.Dashboard.Home;
using ModernDesign.ViewModel.Dashboard.Mail;
using ModernDesign.ViewModel.Dashboard.Setting;
using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernDesign.ViewModel.Dashboard
{
    public class DashboardVM : AbstractVM
    {
        private MainVM mainVM;
        private MenuVM MenuVM { get; } = new MenuVM();

        public HomeVM HomeVM { get; } = new HomeVM();
        public MailVM MailVM { get; } = new MailVM();
        public SettingVM SettingVM { get; } = new SettingVM();

        public override UserControl UserControl { get; } = new DashboardView();

        public UserControl MenuView
        {
            get
            {
                return MenuVM?.UserControl;
            }
        }

        private AbstractVM _actualVM;
        public AbstractVM ActualVM
        {
            get
            {
                return _actualVM;
            }
            set
            {
                if (_actualVM != value)
                {
                    _actualVM?.Hide();
                    _actualVM = value;
                    _actualVM.Show();
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ActualView));
                }
            }
        }
        public UserControl ActualView
        {
            get
            {
                return ActualVM?.UserControl;
            }
        }
        
        public DashboardVM(MainVM mainVM)
        {
            this.mainVM = mainVM;
            BuildVM();
            ActualVM = HomeVM;
        }

        private void BuildVM()
        {
            MenuVM.SetDashboard(this);
            HomeVM.SetDashboard(this);
            MailVM.SetDashboard(this);
            SettingVM.SetDashboard(this);
        }

        public void ChangeVM(AbstractVM vm)
        {
            ActualVM = vm;
        }
    }
}
