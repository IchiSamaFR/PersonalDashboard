using ModernDesign.View.Dashboard.SubView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard.Setting
{
    public class SettingVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new SettingView();

        public SettingVM()
        {
            Name = "Setting";
            Icon = ModernDesign.Properties.Resources.settings;
        }
        public void SetDashboard(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
        }
    }
}
