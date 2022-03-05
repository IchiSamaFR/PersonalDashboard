using PersonalDashboard.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.ViewModel.Dashboard
{
    public class HomeVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new HomeView();

        public HomeVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Home";
            Icon = PersonalDashboard.Properties.Resources.home_page;
        }
    }
}
