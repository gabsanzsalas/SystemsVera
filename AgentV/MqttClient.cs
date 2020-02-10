using MQTTnet;

using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AgentV
{
    public class MqttClient
    {
        private IMqttClient mqttClient;

        public IMqttClientOptions SetConnectionOptions(bool cleanSession,
            String clientId, String url, int port, String user, String password)
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

        async internal void ConnectAndSet()
        {
            await mqttClient.ConnectAsync(this.SetConnectionOptions(false, "1234", "camotemqtt.westeurope.azurecontainer.io", 1883, "veraplus", "A2uhXG4GLOGfHp47daVA"), CancellationToken.None);//cleanSession        
        }
}
