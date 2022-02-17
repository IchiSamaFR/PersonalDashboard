using ModernDesign.ViewModel.Dashboard;
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

namespace ModernDesign.View.Dashboard
{
    /// <summary>
    /// Logique d'interaction pour MailView.xaml
    /// </summary>
    public partial class MailView : UserControl
    {
        public WebBrowser WebBrowser { get { return webBrowser; } }

        public MailView()
        {
            InitializeComponent();

            ListView.Items.Clear();

            string test = "#" + App.Current.Resources["col_Background"].ToString().Substring(3);
            webBrowser.NavigateToString($"<html style=\"background-color:{test}\"/>");
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0)
            {
                if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 50)
                {
                    var viewModel = (MailVM)DataContext;
                    if (viewModel.LoadNewMailsCmd.CanExecute(null))
                        viewModel.LoadNewMailsCmd.Execute(null);
                }
            }
        }
    }
}
