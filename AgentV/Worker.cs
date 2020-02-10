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

        public Worker(ILogger<Worker> logger, VeraClient veraClient, MqttClient mqttClient)
        {
            _logger = logger;
            _veraClient = veraClient;
            factory = new MqttFactory();
            _mqttClient = mqttClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            imqttClient = factory.CreateMqttClient();
            MqttClientAuthenticateResult result = await imqttClient.ConnectAsync(_mqttClient.SetConnectionOptions(false, "1234", "camotemqtt.westeurope.azurecontainer.io", 1883, "veraplus", "A2uhXG4GLOGfHp47daVA"), CancellationToken.None);//cleanSession
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
