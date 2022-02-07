﻿using ModernDesign.View.Dashboard;
using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernDesign.ViewModel.Dashboard
{
    public class MenuVM : AbstractVM
    {
        public DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new MenuView();

        private List<AbstractVM> _menus = new List<AbstractVM>();
        public List<AbstractVM> Menus
        {
            get
            {
                return _menus;
            }
            set
            {
                _menus = value;
                NotifyPropertyChanged();
            }
        }
        
        public void SetDashboard(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Menus.Add(dashboardVM.HomeVM);
            Menus.Add(dashboardVM.MailVM);
            Menus.Add(dashboardVM.SettingVM);
        }

        private ICommand _changeVMCmd;
        public ICommand ChangeVMCmd
        {
            get
            {
                if (_changeVMCmd == null)
                {
                    _changeVMCmd = new RelayCommand(o => { ChangeVM((AbstractVM)o); });
                }
                return _changeVMCmd;
            }
        }

        public void ChangeVM(AbstractVM vm)
        {
            dashboardVM.ChangeVM(vm);
        }
    }
}