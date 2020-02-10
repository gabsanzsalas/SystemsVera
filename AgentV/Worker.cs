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
        private readonly ApiClient _apiClient;
        private MqttManager _mqttManager;

        public Worker(ILogger<Worker> logger, ApiClient veraClient, MqttManager mqttManager)
        {
            _logger = logger;
            _apiClient = veraClient;
            _mqttManager = mqttManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                Device[] devices = _apiClient.GetDevices();
                _mqttManager.Publish("45fea213.response", JsonConvert.SerializeObject(devices));
                _logger.LogInformation("Worker published at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
