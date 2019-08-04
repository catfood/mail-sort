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
            var inbox = retrieverService.GetInbox();
            foreach (var modelMessage in inbox)
            {
                var actions = rulesService.GetActionsForMessage(modelMessage);
                foreach (var action in actions)
                {
                    action.Execute(modelMessage);
                }
            }
        }
    }
}
