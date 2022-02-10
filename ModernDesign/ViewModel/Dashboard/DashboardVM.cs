using ModernDesign.View.Dashboard;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard
{
    public class DashboardVM : AbstractVM
    {
        private MainVM mainVM;
        public override UserControl UserControl { get; } = new DashboardView();

        private MenuVM MenuVM { get; } = new MenuVM();
        public UserControl MenuView
        {
            get
            {
                return MenuVM?.UserControl;
            }
        }

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
            ChangeVM(GetVM("Home"));
        }

        private void BuildVM()
        {
            AllVM = new List<AbstractVM>
            {
                new HomeVM(this),
                new MailVM(this),
                new PasswordVM(this),
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
