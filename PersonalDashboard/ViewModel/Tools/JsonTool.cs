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
using MimeKit;

namespace PersonalDashboard.ViewModel.Tools
{
    public static class JsonTool
    {
        private const string CONFIGFOLDER = @"config\";
        private const string CONFIGFILE = "config.txt";

        private const string PASSFOLDER = @"password\";
        private const string PASSFILE = "passwords.txt";

        private const string MAILFOLDER = @"mail\";
        private const string MAILFILE = @"mails.txt";
        private const string MAILCACHEFOLDER = @"cache\";

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
            SaveFile(Path.Combine(Directory.GetCurrentDirectory(), CONFIGFOLDER, CONFIGFILE), 
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
            SaveFile(Path.Combine(Directory.GetCurrentDirectory(), PASSFOLDER, PASSFILE),
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

        public static void SaveMails(List<MailItem> mails, string folderName)
        {
            try
            {
                string cachePath = Path.Combine(Directory.GetCurrentDirectory(), MAILFOLDER, MAILCACHEFOLDER);
                JArray array = new JArray();

                SaveFolder(cachePath);
                foreach (var mail in mails)
                {
                    array.Add(new JObject(
                        new JProperty(nameof(MailItem.Uid), mail.Uid),
                        new JProperty(nameof(MailItem.Flags), mail.Flags)
                        ));
                    mail.SaveToEml(Path.Combine(cachePath, mail.Uid.ToString()));
                }
                SaveFile(Path.Combine(Directory.GetCurrentDirectory(), MAILFOLDER, $"{folderName}.txt"),
                        array.ToString());
            }
            catch(Exception e)
            {
                NotificationsVM.instance.AddNotification("JsonSerializer", e.Message);
            }
        }
        public static List<MailItem> LoadMails(string folderName)
        {
            List<MailItem> mails = new List<MailItem>();
            try
            {
                string mailsPath = Path.Combine(Directory.GetCurrentDirectory(), MAILFOLDER, $"{folderName}.txt");
                if (!File.Exists(mailsPath))
                {
                    return mails;
                }

                mails = (JsonConvert.DeserializeObject(File.ReadAllText(mailsPath)) as JArray).ToObject<List<MailItem>>();
                mails.RemoveAll(mail => !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), MAILFOLDER, MAILCACHEFOLDER, mail.Uid.ToString())));
                foreach (var mail in mails)
                {
                    mail.Fill(MimeMessage.Load(Path.Combine(Directory.GetCurrentDirectory(), MAILFOLDER, MAILCACHEFOLDER, mail.Uid.ToString())));
                }
            }
            catch (Exception e)
            {
                NotificationsVM.instance.AddNotification("JsonSerializer", e.Message);
            }
            return mails;
        }

        public static void SaveFile(string path, string file)
        {
            SaveFolder(path);
            File.WriteAllText(path, file);
        }
        public static void SaveFolder(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
