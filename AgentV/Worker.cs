using MQTTnet;

using MQTTnet.Client;

using MQTTnet.Client.Connecting;
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace AgentV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly VeraClient _veraClient;
        private readonly MqttFactory factory;
        private IMqttClient imqttClient;
        private MqttClient _mqttClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _veraClient = new VeraClient(new Uri("http://10.0.2.134:3480/data_request?id=user_data"));
            factory = new MqttFactory();
            _mqttClient = new MqttClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            imqttClient = factory.CreateMqttClient();
            _mqttClient.ConnectAndSet();
            while (!stoppingToken.IsCancellationRequested)
            {
                Device[] devices = _veraClient.Get<Device[]>();
                
                _mqttClient.Publish("45fea213", JsonConvert.SerializeObject(devices));
                _logger.LogInformation("Worker published at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
