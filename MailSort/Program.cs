using Microsoft.Extensions.DependencyInjection;
using MailSort.Net;
using MailSort.Rules;
using System;
using Serilog;
using Serilog.Extensions.Logging;

namespace MailSort
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
             .AddSingleton<Configuration.IConfiguration, Configuration.Configuration>()
             .AddTransient<IMailRetriever, MailRetriever>()
             .AddTransient<IRulesService, MailRules>()
             .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var retrieverService = serviceProvider.GetService<IMailRetriever>();
            var rulesService = serviceProvider.GetService<IRulesService>();
            retrieverService.Open();
            var inbox = retrieverService.GetInbox();
            int count = 0;
            var startTime = DateTime.Now;
            foreach (var modelMessage in inbox)
            {
                if (count % 10 == 0)
                {
                    var diff = DateTime.Now - startTime;
                    Console.WriteLine($"{count} mail items in {diff.TotalSeconds} seconds.");
                    Log.Information("ping");
                }
                var actions = rulesService.GetActionsForMessage(modelMessage);
                foreach (var action in actions)
                {
                    var actionResult = retrieverService.Execute(action, modelMessage);
                }
                count++;
            }
            retrieverService.Close();
        }
    }
}
