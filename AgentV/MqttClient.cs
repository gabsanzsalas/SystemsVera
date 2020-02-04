using MQTTnet;

using MQTTnet.Client;

using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Protocol;

namespace AgentV
{
    public class MqttClient
    {
        MqttApplicationMessage message;

        private IMqttClient mqttClient;
        private MqttFactory factory = new MqttFactory();

        private IMqttClientOptions SetConnectionOptions(bool cleanSession,
            string clientId, string url, int port, string user, string password)
        {
            var options = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(url, port)
            .WithCredentials(user, password)
            .WithCleanSession(cleanSession)
            .Build();

            return options;
        }

        public async void Publish(string topic, string payload)
        {
             MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

                await mqttClient.PublishAsync(message, CancellationToken.None);
                
         }
    }
}
