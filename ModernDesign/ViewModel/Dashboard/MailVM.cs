using ModernDesign.Model.Dashboard.Mail;
using ModernDesign.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard
{
    public class MailVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new MailView();

        private List<MailItem> _mailItems = new List<MailItem>();
        public List<MailItem> MailItems
        {
            get
            {
                return _mailItems;
            }
            set
            {
                _mailItems = value;
                NotifyPropertyChanged();
            }
        }

        public MailVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Mail";
            Icon = ModernDesign.Properties.Resources.envelope;

            LoadMails();
        }
        public void LoadMails()
        {
            MailItems.Add(new MailItem());
            MailItems.Add(new MailItem());
            MailItems.Add(new MailItem());
            MailItems.Add(new MailItem());
            MailItems.Add(new MailItem());
        }
    }
}
