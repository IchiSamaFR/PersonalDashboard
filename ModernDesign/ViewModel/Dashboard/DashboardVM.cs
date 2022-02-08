using ModernDesign.View.Dashboard;
using ModernDesign.ViewModel.Dashboard.Home;
using ModernDesign.ViewModel.Dashboard.Mail;
using ModernDesign.ViewModel.Dashboard.Setting;
using ModernDesign.ViewModel.Dashboard.Crypto;
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

        private List<AbstractVM> _allVM = new List<AbstractVM>();
        public List<AbstractVM> AllVM
        {
            get
            {
                return _allVM;
            }
            private set
            {
                _allVM = value;
                NotifyPropertyChanged();
            }
        }

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
            ActualVM = GetVM("Home");
        }

        private void BuildVM()
        {
            AllVM = new List<AbstractVM>
            {
                new HomeVM(this),
                new MailVM(this),
                new CryptoVM(this),
                new SettingVM(this)
            };
            MenuVM.SetDashboard(this);
        }

        public AbstractVM GetVM(string name)
        {
            return AllVM.FirstOrDefault(vm => vm.Name.ToLower() == name.ToLower());
        }

        public void ChangeVM(AbstractVM vm)
        {
            ActualVM = GetVM(vm.Name);
        }
    }
}
