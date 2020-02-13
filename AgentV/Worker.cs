using AgentV.DTO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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

                foreach (SchedulerItem element in Scheduler.GetScheduler())
                {
                    if (!Scheduler.GetLaunched().Any(x => x.Id.Equals(element.Id)))
                        _scheduler.AddLaunchedItem(element.Id, element.CreationDate);

                    LaunchedItem launched = Scheduler.GetLaunched().SingleOrDefault(x => x.Id.Equals(element.Id));

                    DateTime lastDateInMillis = launched.LastDate.AddMilliseconds(element.Interval * 1000);
                    if (lastDateInMillis < DateTime.Now)
                    {
                        dynamic status = _apiClient.GetGeneric(ApiClient._constantStatusById + element.Id);
                        _mqttManager.Publish(_agentVeraSettings.topic + topicSubscription, JsonConvert.SerializeObject(status));
                        _scheduler.ChangeLastDate(element.Id);
                    }
                }
                await Task.Delay(1000);
            }
        }
    }
}
