using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AgentV
{
    public class Worker : BackgroundService
    {
        private const string topicSubscription = ".response";
        private readonly ILogger<Worker> _logger;
        private readonly ApiClient _apiClient;
        private MqttManager _mqttManager;
        private AgentVeraSettings _agentVeraSettings;

        public Worker(ILogger<Worker> logger, ApiClient veraClient, MqttManager mqttManager, AgentVeraSettings agentVeraSettings)
        {
            _logger = logger;
            _apiClient = veraClient;
            _mqttManager = mqttManager;
            _agentVeraSettings = agentVeraSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
       /*     while (!stoppingToken.IsCancellationRequested)
            {
                Device[] devices = _apiClient.GetDevices();
                _mqttManager.Publish(_agentVeraSettings.topic + topicSubscription, JsonConvert.SerializeObject(devices));
                _logger.LogInformation("Worker published at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }

        public Schedule(SchedulerItem list)
        {
            
            while (true)
            {
                double lastDateInMillis = lastDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                double NowInMillis = DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                if (intervall * 1000 + lastDateInMillis < NowInMillis)
                {
                    lastDate = DateTime.Now;
                    addScheduleItem(id, intervall, lastDate);
                }
            }
        }
    }
}
