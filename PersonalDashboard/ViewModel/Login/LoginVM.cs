using InputFields;
using PersonalDashboard.Model;
using PersonalDashboard.View.Login;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalDashboard.ViewModel.Login
{
    public class LoginVM : AbstractVM
    {
        private MainVM mainVM;
        public override UserControl UserControl { get; } = new LoginView();

        public StringField LoginInput { get; set; } = new StringField() { Placeholder = "Login..." };
        public StringField PassInput { get; set; } = new StringField() { Placeholder = "Password..." };
        public StringField ConnextionInput { get; set; } = new StringField() { Value = "Connexion" };

        private ICommand _logCmd;
        public ICommand LogCmd
        {
            get
            {
                if(_logCmd == null)
                {
                    _logCmd = new RelayCommand(o => { Log(); });
                }
                return _logCmd;
            }
        }

        public LoginVM(MainVM mainVM)
        {
            Name = "Login";
            this.mainVM = mainVM;
        }

        public void Log()
        {
            NotificationsVM.instance.AddNotification(Name, "Connecté");
            mainVM.Log();
        }
    }
}
