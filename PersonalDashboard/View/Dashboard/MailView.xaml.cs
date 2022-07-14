using PersonalDashboard.ViewModel.Dashboard;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace PersonalDashboard.View.Dashboard
{
    /// <summary>
    /// Logique d'interaction pour MailView.xaml
    /// </summary>
    public partial class MailView : UserControl
    {
        public MailView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0)
            {
                if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 50)
                {
                    var viewModel = (MailVM)DataContext;
                    if (viewModel.LoadAncientMailsCmd.CanExecute(null))
                        viewModel.LoadAncientMailsCmd.Execute(null);
                }
            }
        }
    }
}
