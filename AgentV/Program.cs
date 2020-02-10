using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AgentV
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    AgentVeraSettings alarmSettings = configuration.Get<AgentVeraSettings>();
                    services.AddSingleton(alarmSettings);
                    services.AddTransient<VeraClient>();
                    services.AddTransient<MqttClient>();
                    services.AddHostedService<Worker>();
                });
    }
}