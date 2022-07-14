using PersonalDashboard.Model;
using PersonalDashboard.View.Dashboard;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalDashboard.ViewModel.Dashboard
{
    public class MenuVM : AbstractVM
    {
        public DashboardVM dashboardVM { get; }
        public override UserControl UserControl { get; } = new MenuView();

        private List<AbstractVM> _menus = new List<AbstractVM>();
        public List<AbstractVM> Menus
        {
            get
            {
                return dashboardVM.AllVM;
            }
        }

        public ICommand ChangeVMCmd { get; }

        public MenuVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;

            ChangeVMCmd = new RelayCommand(o => { ChangeVM((AbstractVM)o); });
        }

        public void ChangeVM(AbstractVM vm)
        {
            dashboardVM.ChangeVM(vm);
        }
    }
}
