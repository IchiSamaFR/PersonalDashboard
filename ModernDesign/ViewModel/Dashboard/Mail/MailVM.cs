using ModernDesign.View.Dashboard.SubView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard.Mail
{
    public class MailVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new MailView();

        public MailVM()
        {
            Name = "Mail";
            Icon = ModernDesign.Properties.Resources.envelope;
        }
        public void SetDashboard(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
        }
    }
}
