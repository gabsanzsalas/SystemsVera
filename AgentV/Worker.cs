using MQTTnet; //LADI

using MQTTnet.Client;//LADI

using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Protocol;
using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace AgentV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        DbContext dbContext;
        protected readonly HttpClient _httpClient;
        protected Uri BaseEndpoint { get; set; }
       
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            BaseEndpoint = new Uri("http://10.0.2.134:3480/data_request?id=user_data");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            mqttClient = factory.CreateMqttClient();
            MqttClientAuthenticateResult result = await mqttClient.ConnectAsync(SetConnectionOptions(false, "1234", "camotemqtt.westeurope.azurecontainer.io", 1883, "veraplus", "A2uhXG4GLOGfHp47daVA"), CancellationToken.None);//cleanSession
            while (!stoppingToken.IsCancellationRequested)
            {
             //   string devices = Get<string>();
             //   Console.WriteLine(devices);
                Device[] devices = Get<Device[]>();
             //   Console.WriteLine();
                
                Publish("45fea213", JsonConvert.SerializeObject(devices));
                _logger.LogInformation("Worker published at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
