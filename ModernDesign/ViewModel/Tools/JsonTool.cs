using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ModernDesign.Model;
using ModernDesign.Model.Dashboard.Password;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModernDesign.ViewModel.Tools
{
    public static class JsonTool
    {
        public const string ConfigFolder = "Config";
        public const string ConfigFile = "config";
        public const string PassFile = "passwords";

        public static string GetConfigFolder()
        {
            string configPath = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ConfigFolder);
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

        public static string GetConfigFile(string name)
        {
            return Path.Combine(GetConfigFolder(), name + ".txt");
        }

        public static void SaveConfig(this ConfigItem configItem)
        {
            JObject toSave =
                new JObject(
                    new JProperty("APIKey", configItem.APIKey),
                    new JProperty("SecretKey", configItem.SecretKey),
                    new JProperty("MailAdress", configItem.MailAdress),
                    new JProperty("MailPass", configItem.MailPass));
            File.WriteAllText(GetConfigFile(ConfigFile), toSave.ToString());
            /*
            JObject rss =
                new JObject(
                    new JProperty("channel",
                        new JObject(
                            new JProperty("title", "James Newton-King"),
                            new JProperty("link", "http://james.newtonking.com"),
                            new JProperty("description", "James Newton-King's blog."),
                            new JProperty("item",
                                new JArray(
                                    from p in posts
                                    orderby p.Title
                                    select new JObject(
                                        new JProperty("title", p.Title),
                                        new JProperty("description", p.Description),
                                        new JProperty("link", p.Link),
                                        new JProperty("category",
                                            new JArray(
                                                from c in p.Categories
                                                select new JValue(c)))))))));
                                                */
        }
        public static ConfigItem LoadConfig()
        {
            ConfigItem configItem = new ConfigItem();
            try
            {
                JObject loaded = (JObject)JToken.Parse(File.ReadAllText(GetConfigFile(ConfigFile)));
                configItem.APIKey = loaded.GetValue("APIKey").ToString();
                configItem.SecretKey = loaded.GetValue("SecretKey").ToString();
                configItem.MailAdress = loaded.GetValue("MailAdress").ToString();
                configItem.MailPass = loaded.GetValue("MailPass").ToString();
            }
            catch
            {
                SaveConfig(configItem);
            }

            return configItem;
        }
        
        public static void SavePasswords(this List<ServiceItem> serviceItems)
        {
            JObject toSave =
                new JObject(
                    new JProperty("services",
                        new JArray(
                             from service in serviceItems orderby service.Name select 
                             new JObject(
                                 new JProperty("name", service.Name),
                                 new JProperty("passwords",
                                     new JArray(
                                         from password in service.PasswordItems select 
                                         new JObject(
                                             new JProperty("login", password.Login),
                                             new JProperty("password", password.Password)
                                             )))))));
            File.WriteAllText(GetConfigFile(PassFile), toSave.ToString());
        }
        public static List<ServiceItem> LoadPasswords(List<ServiceItem> serviceItems)
        {
            List<ServiceItem> configItem = new List<ServiceItem>();
            try
            {
                JObject loaded = (JObject)JToken.Parse(File.ReadAllText(GetConfigFile(PassFile)));
                foreach (JObject service in loaded.GetValue("services"))
                {
                    ServiceItem serviceItem = new ServiceItem(service.GetValue("name").ToString());
                    foreach (JObject pass in service.GetValue("passwords"))
                    {
                        serviceItem.PasswordItems.Add(new PasswordItem(pass.GetValue("login").ToString(),
                                                           pass.GetValue("password").ToString()));
                    }

                    configItem.Add(serviceItem);
                }
            }
            catch
            {
                SavePasswords(serviceItems);
            }

            return configItem;
        }

        public static void Save(string name, object toSerializeObject)
        {
            string jsonString = JsonConvert.SerializeObject(toSerializeObject);
            File.WriteAllText(GetConfigFile(name), jsonString);
        }
        public static void Load(string name, object toDerializeObject)
        {
            if (File.Exists(GetConfigFile(name)))
            {
                toDerializeObject = JsonConvert.DeserializeObject(File.ReadAllText(GetConfigFile(name)));
            }
            else
            {
                Save(name, toDerializeObject);
            }
        }
    }
}
