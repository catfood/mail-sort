using System;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MailSort.Configuration
{
    public interface IConfiguration
    {
        Models.IMAPConfig IMAP();
    }

    public sealed class Configuration: IConfiguration
    {
        private static readonly Lazy<Configuration> _instance = new Lazy<Configuration>(() => new Configuration());
        private Models.IMAPConfig IMAPValue;

        public Models.IMAPConfig IMAP()
        {
            return _instance.Value.IMAPValue;
        }

        /*
        public Models.RulesConfig Rules()
        {
            return _instance.Value.RulesList;
        }
        */

        public Configuration()
        {
            var builder = new ConfigurationBuilder();
            string appSettingsFileName = GetAppSettingsFileName();
            appSettingsFileName = "appsettings.json";
            builder.AddJsonFile(appSettingsFileName);
            builder.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), false);
            var conf = builder.Build();
            var foo = conf.GetChildren();
            foreach (var x in conf.GetChildren())
            {
                System.Diagnostics.Debug.WriteLine($"Child element is {x.Key}.");
            }
            IMAPValue = new Models.IMAPConfig
            {
                IMAPHost = conf["MailSort:IMAPHost"],
                IMAPPort = Convert.ToInt32(conf["MailSort:IMAPPort"]),
                IMAPMode = conf["MailSort:IMAPMode"],
                IMAPUser = conf["MailSort:IMAPUser"],
                IMAPPassword = conf["MailSort:IMAPPassword"]
            };
            object rules = foo.FirstOrDefault(section => section.Key == "Rules");
            System.Diagnostics.Debug.Assert(rules != null);
        }

        public string GetAppSettingsFileName()
        {
            return $"{Environment.GetEnvironmentVariable("CONFIG_DIRECTORY")}\\appsettings.json";
        }
    }
}
