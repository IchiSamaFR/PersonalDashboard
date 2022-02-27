using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDesign.Model
{
    public class ConfigItem
    {
        public static ConfigItem Instance;
        public string APIKey { get; set; } = "***REMOVED***";
        public string SecretKey { get; set; } = "***REMOVED***";
        public string MailAdress { get; set; } = "***REMOVED***";
        public string MailPass { get; set; } = "***REMOVED***";

        public ConfigItem()
        {
            Instance = this;
        }
    }
}
