using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private Scheduler _scheduler;
        private List<SchedulerItem> schedulerList;

        public Worker(ILogger<Worker> logger, ApiClient veraClient, MqttManager mqttManager, AgentVeraSettings agentVeraSettings, Scheduler scheduler)
        {
            _logger = logger;
            _apiClient = veraClient;
            _mqttManager = mqttManager;
            _agentVeraSettings = agentVeraSettings;
            _scheduler = scheduler;
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
                 */

            while (!stoppingToken.IsCancellationRequested)
            {
                schedulerList = _scheduler.GetScheduler();
                foreach (SchedulerItem element in schedulerList)
                {
                    DateTime lastDateInMillis = element.LastDate.AddMilliseconds(element.Interval * 1000);
                    if (lastDateInMillis > DateTime.Now)
                    {
                        Status status = _apiClient.GetStatusById(element.Id);
                        _mqttManager.Publish(_agentVeraSettings.topic + topicSubscription, JsonConvert.SerializeObject(status));
                        _scheduler.ChangeLastDate(element.Id);
                    }
                }
            }
        }
    }
}
