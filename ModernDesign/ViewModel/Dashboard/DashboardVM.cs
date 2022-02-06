using ModernDesign.Model.Dashboard;
using ModernDesign.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard
{
    public class DashboardVM : AbstractVM
    {
        private MainVM mainVM;
        public override UserControl UserControl { get; } = new DashboardView();

        private List<MenuButton> _menuButtons = new List<MenuButton>();
        public List<MenuButton> MenuButtons
        {
            get
            {
                return _menuButtons;
            }
            set
            {
                _menuButtons = value;
                NotifyPropertyChanged("MenuButtons");
            }
        }
        
        public DashboardVM(MainVM mainVM)
        {
            this.mainVM = mainVM;
            MenuButtons.Add(new MenuButton() { BtnText = "Home", ImgResource = ModernDesign.Properties.Resources.home_page });
            MenuButtons.Add(new MenuButton() { BtnText = "Settings", ImgResource = ModernDesign.Properties.Resources.settings });
        }
    }
}
