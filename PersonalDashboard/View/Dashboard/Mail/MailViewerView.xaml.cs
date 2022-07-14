using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PersonalDashboard.View.Dashboard.Mail
{
    public partial class MailViewerView : UserControl
    {
        public WebBrowser WebBrowser { get { return webBrowser; } }

        public MailViewerView()
        {
            InitializeComponent();
        }
        public void LoadMailBody(string htmlBody)
        {
            WebBrowser.NavigateToString(htmlBody);
        }
    }
}
