using Microsoft.Extensions.DependencyInjection;
using MailSort.Net;
using MailSort.Rules;
using System;

namespace MailSort
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
             .AddSingleton<Configuration.IConfiguration, Configuration.Configuration>()
             .AddTransient<IMailRetriever, MailRetriever>()
             .AddTransient<IRulesService, MailRules>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var retrieverService = serviceProvider.GetService<IMailRetriever>();
            var rulesService = serviceProvider.GetService<IRulesService>();
            retrieverService.Open();
            var inbox = retrieverService.GetInbox();
            retrieverService.Close();
            foreach (var modelMessage in inbox)
            {
                var actions = rulesService.GetActionsForMessage(modelMessage);
                foreach (var action in actions)
                {
                    var actionResult = retrieverService.Execute(action, modelMessage);
                }
            }
        }
    }
}
