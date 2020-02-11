using AgentV.DTO;
using MQTTnet;

using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgentV
{
    public class MqttManager
    {
        private const string topicPublish = ".response";
        private const string topicSubscription = ".request";
        private IMqttClient mqttClient;
        private AgentVeraSettings agentVeraSettings;
        private IMqttClientOptions mqttOptions;
        private ApiClient apiClient;
        private Scheduler scheduler;

        public MqttManager(AgentVeraSettings agentVeraSettings, MqttFactory mqttFactory, ApiClient apiClient, Scheduler scheduler)
        {
            this.agentVeraSettings = agentVeraSettings;
            SetConnectionOptions();
            mqttClient = mqttFactory.CreateMqttClient();
            this.apiClient = apiClient;
            this.scheduler = scheduler;
            SubscribeMqttClientAsync();
        }




        private async Task SubscribeMqttClientAsync()
        {
            this.Connect();
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        Message messageObject = JsonConvert.DeserializeObject<Message>(message);
                        switch (messageObject.commando)
                        {
                            case "GetDevices":
                                Device[] devices = apiClient.GetDevices();
                                this.Publish(agentVeraSettings.topic + topicPublish, JsonConvert.SerializeObject(devices));
                                break;
                            case "GetStatus":
                                Status statuses = apiClient.GetStatus();
                                this.Publish(agentVeraSettings.topic + topicPublish, JsonConvert.SerializeObject(statuses));
                                break;
                            case "GetStatusById":
                                DeviceStatus status = apiClient.GetStatusById(messageObject.parameters[0].value);
                                this.Publish(agentVeraSettings.topic + topicPublish, JsonConvert.SerializeObject(status));
                                break;
                            case "ScheduleStatus":
                                scheduler.AddScheduleItem(messageObject.parameters[0].value, messageObject.parameters[1].value);
                                break;
                        }

                        ///Get devices()
                        ///Get Status() 
                        ///Schedule 
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });
        }
        private void SetConnectionOptions()
        {
            mqttOptions = new MqttClientOptionsBuilder()
            .WithClientId(agentVeraSettings.clientId)
            .WithTcpServer(agentVeraSettings.url, agentVeraSettings.port)
            .WithCredentials(agentVeraSettings.user, agentVeraSettings.password)
            .WithCleanSession(agentVeraSettings.cleanSession)
            .Build();
        }

        public async void Publish(string topic, string payload)
        {
            if (!mqttClient.IsConnected)
                Connect();


            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
               .WithTopic(topic)
               .WithPayload(payload)
               .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
               .WithRetainFlag(false)
               .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

        }

        async internal void Connect()
        {
            while (!mqttClient.IsConnected)
            {
                await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);
            }

            var result = await mqttClient.SubscribeAsync(new TopicFilterBuilder()
                     .WithTopic(agentVeraSettings.topic + topicSubscription)
                     .WithAtLeastOnceQoS().Build());
        }
    }

    public class Message
    {
        public string commando { get; set; }
        public Parameters[] parameters { get; set; }
    }

    public class Parameters
    {
        public string name { get; set; }
        public int value { get; set; }

    }
}
