using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PersonalDashboard.Model;
using PersonalDashboard.Model.Dashboard.Password;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersonalDashboard.Model.Dashboard.Mail;

namespace PersonalDashboard.ViewModel.Tools
{
    public static class JsonTool
    {
        private const string CONFIGFOLDER = "config";
        private const string CONFIGFILE = "config.txt";

        private const string PASSFOLDER = "password";
        private const string PASSFILE = "passwords";

        private const string MAILFOLDER = "mail";
        private const string MAILFILE = "mails";

        public static string GetConfigFolder()
        {
            string configPath = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), CONFIGFOLDER);
            if (Directory.Exists(configPath))
            {
                return configPath;
            }
            else
            {
                Directory.CreateDirectory(configPath);
                return configPath;
            }
        }
        
        public static void SaveConfig(ConfigItem configItem)
        {
            SaveJson(Path.Combine(Directory.GetCurrentDirectory(), CONFIGFOLDER, CONFIGFILE), 
                    JsonConvert.SerializeObject(configItem));
        }
        public static ConfigItem LoadConfig()
        {
            ConfigItem configItem = new ConfigItem();
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), CONFIGFOLDER, CONFIGFILE);
                configItem = (JsonConvert.DeserializeObject(File.ReadAllText(path)) as JToken).ToObject<ConfigItem>();
            }
            catch (Exception e)
            {
                SaveConfig(configItem);
            }
            return configItem;
        }
        
        public static void SavePasswords(this List<ServiceItem> serviceItems)
        {
            SaveJson(Path.Combine(Directory.GetCurrentDirectory(), PASSFOLDER, PASSFILE),
                    JsonConvert.SerializeObject(serviceItems));
        }
        public static List<ServiceItem> LoadPasswords()
        {
            List<ServiceItem> passwords = new List<ServiceItem>();
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), CONFIGFOLDER, CONFIGFILE);
                passwords = (JsonConvert.DeserializeObject(File.ReadAllText(path)) as JToken).ToObject<List<ServiceItem>>();
            }
            catch
            {
                SavePasswords(passwords);
            }
            return passwords;
        }

        public static void SaveMails(List<MailItem> mails)
        {
            try
            {
                JArray array = new JArray();
                foreach (var mail in mails)
                {
                    array.Add(new JObject(
                        new JProperty(nameof(MailItem.Uid), mail.Uid),
                        new JProperty(nameof(MailItem.FromEmail), mail.FromEmail),
                        new JProperty(nameof(MailItem.ToEmail), mail.ToEmail),
                        new JProperty(nameof(MailItem.Date), mail.Date),
                        new JProperty(nameof(MailItem.HtmlBody), mail.HtmlBody)
                        ));
                }
                SaveJson(Path.Combine(Directory.GetCurrentDirectory(), MAILFOLDER, MAILFILE),
                        array.ToString());
            }
            catch(Exception e)
            {

            }
        }
        public static List<MailItem> LoadMails()
        {
            List<MailItem> mails = new List<MailItem>();
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), MAILFOLDER, MAILFILE);
                mails = (JsonConvert.DeserializeObject(File.ReadAllText(path)) as JArray).ToObject<List<MailItem>>();
            }
            catch (Exception e)
            {
            }
            return mails;
        }

        public static void SaveJson(string path, string file)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, file);
        }
    }
}
