using ModernDesign.View.Dashboard.SubView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard.Home
{
    public class HomeVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new HomeView();

        public HomeVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Home";
            Icon = ModernDesign.Properties.Resources.home_page;
        }
    }
}
