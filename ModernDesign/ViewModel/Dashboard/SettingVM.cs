using ModernDesign.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard
{
    public class SettingVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new SettingView();

        public SettingVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Setting";
            Icon = ModernDesign.Properties.Resources.settings;
        }
    }
}
