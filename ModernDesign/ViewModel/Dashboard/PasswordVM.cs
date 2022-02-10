using ModernDesign.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard
{
    public class PasswordVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new PasswordView();

        public PasswordVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Password";
            Icon = ModernDesign.Properties.Resources.security_shield_green;
        }
    }
}
