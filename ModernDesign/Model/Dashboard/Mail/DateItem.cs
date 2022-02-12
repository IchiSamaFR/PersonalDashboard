using ModernDesign.View.Dashboard.Mail;
using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.Model.Dashboard.Mail
{
    public class DateItem : ObservableObject
    {
        public UserControl UserControl = new MailDateView();

        public DateTime TimeReceived { get; set; }
        public string TimeDisplay
        {
            get
            {
                return TimeReceived.ToString("ddd dd/MM/yy");
            }
        }

        public DateItem()
        {
            UserControl.DataContext = this;
        }
    }
}
