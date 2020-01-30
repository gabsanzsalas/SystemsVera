using MQTTnet; //LADI

using MQTTnet.Client;//LADI

using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Protocol;
using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AgentV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        MqttApplicationMessage message;
        DbContext dbContext;
        private IMqttClient mqttClient;// = new IMqttClient("camotemqtt.westeurope.azurecontainer.io", "veraplus",
                                       //"A2uhXG4GLOGfHp47daVA", 2, "CLIENTID"); //LADI
        private MqttFactory factory = new MqttFactory();//LADI
       
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            mqttClient = factory.CreateMqttClient();
            MqttClientAuthenticateResult result = await mqttClient.ConnectAsync(SetConnectionOptions(false, "1234", "camotemqtt.westeurope.azurecontainer.io", 1883, "veraplus", "A2uhXG4GLOGfHp47daVA"), CancellationToken.None);//cleanSession
            while (!stoppingToken.IsCancellationRequested)
            {
                Publish("45fea213","ABCD");
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private IMqttClientOptions SetConnectionOptions(bool cleanSession,
            String clientId, String url, int port, String user, String password)
        {
            var options = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(url, port)
            .WithCredentials(user, password)
            .WithCleanSession(cleanSession)
            .Build();

            return options;
        }//LADI

        public async void Publish(string topic, string payload)////
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
