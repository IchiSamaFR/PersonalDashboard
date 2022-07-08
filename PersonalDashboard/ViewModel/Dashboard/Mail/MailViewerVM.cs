using PersonalDashboard.Model.Dashboard.Mail;
using PersonalDashboard.View.Dashboard.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.ViewModel.Dashboard.Mail
{
    public class MailViewerVM : AbstractVM
    {
        public override UserControl UserControl { get; } = new MailViewerView();

        public void LoadMail(MailItem mail)
        {
            (UserControl as MailViewerView).LoadMailBody(mail.HtmlBody);
        }
    }
}
