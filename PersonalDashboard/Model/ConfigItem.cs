using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDashboard.Model
{
    public class ConfigItem
    {
        public static ConfigItem Instance;
        public string APIKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string MailAdress { get; set; } = "";
        public string MailPass { get; set; } = "";

        public ConfigItem()
        {
            Instance = this;
        }
    }
}
