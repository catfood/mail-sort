using System;
using Microsoft.Extensions.Configuration;

namespace MailSort.Configuration
{
    public interface IConfiguration
    {
        Models.Config Get();
    }

    public sealed class Configuration: IConfiguration
    {
        private static readonly Lazy<Configuration> _instance = new Lazy<Configuration>(() => new Configuration());
        private Models.Config Value;

        public Models.Config Get()
        {
            return _instance.Value.Value;
        }

        public Configuration()
        {
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), false);
            var conf = builder.Build();
            foreach (var x in conf.GetChildren())
            {
                System.Diagnostics.Debug.WriteLine($"Child element is {x.Key}.");
            }
            Value = new Models.Config
            {
                IMAPHost = conf["MailSort:IMAPHost"],
                IMAPPort = Convert.ToInt32(conf["MailSort:IMAPPort"]),
                IMAPMode = conf["MailSort:IMAPMode"],
                IMAPUser = conf["MailSort:IMAPUser"],
                IMAPPassword = conf["MailSort:IMAPPassword"]
            };
        }
    }
}
