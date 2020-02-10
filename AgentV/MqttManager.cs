using Microsoft.Extensions.DependencyInjection;
using MQTTnet;

using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgentV
{
    public class MqttManager
    {
        private const string topicSubscription = ".request";
        private IMqttClient mqttClient;
        private AgentVeraSettings agentVeraSettings;
        private IMqttClientOptions mqttOptions;

        public MqttManager(AgentVeraSettings agentVeraSettings, MqttFactory mqttFactory)
        {
            this.agentVeraSettings = agentVeraSettings;
            SetConnectionOptions();
            mqttClient = mqttFactory.CreateMqttClient();
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
            await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);

            var result = await mqttClient.SubscribeAsync(new TopicFilterBuilder()
                     .WithTopic(agentVeraSettings.topic+ topicSubscription)
                     .WithAtLeastOnceQoS().Build());

        }
    }
}
