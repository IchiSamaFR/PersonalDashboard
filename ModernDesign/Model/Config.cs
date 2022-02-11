using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDesign.Model
{
    public class Config
    {
        public static Config Instance;
        public string APIKey { get; } = "***REMOVED***";
        public string SecretKey { get; } = "***REMOVED***";
        public string MailAdress { get; } = "***REMOVED***";
        public string MailPass { get; } = "***REMOVED***";

        public Config()
        {
            Instance = this;
        }
    }
}
