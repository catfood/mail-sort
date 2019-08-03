using System;
using Microsoft.Extensions.DependencyInjection;

namespace MailSort
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
             .AddSingleton<Configuration.IConfiguration, Configuration.Configuration>()
             .AddTransient<IMailRetriever, MailRetriever>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var service = serviceProvider.GetService<IMailRetriever>();
            service.ListMessages();
            Console.ReadLine();
        }
    }
}
