using PersonalDashboard.Model.Dashboard.Mail;
using PersonalDashboard.View.Dashboard.Mail;
using PersonalDashboard.ViewModel.Tools;
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
            string html = !string.IsNullOrEmpty(mail.HtmlBody) ? mail.HtmlBody : HtmlTool.TextToHtml(mail.TextBody);
            (UserControl as MailViewerView).LoadMailBody(html);
        }
    }
}
