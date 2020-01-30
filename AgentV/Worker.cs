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
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace AgentV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        MqttApplicationMessage message;
        DbContext dbContext;
        protected readonly HttpClient _httpClient;
        protected Uri BaseEndpoint { get; set; }

        private IMqttClient mqttClient;// = new IMqttClient("camotemqtt.westeurope.azurecontainer.io", "veraplus",
                                       //"A2uhXG4GLOGfHp47daVA", 2, "CLIENTID"); //LADI
        private MqttFactory factory = new MqttFactory();//LADI
       
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            BaseEndpoint = new Uri("http://10.0.2.134:3480/data_request?id=user_data");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            mqttClient = factory.CreateMqttClient();
            MqttClientAuthenticateResult result = await mqttClient.ConnectAsync(SetConnectionOptions(false, "1234", "camotemqtt.westeurope.azurecontainer.io", 1883, "veraplus", "A2uhXG4GLOGfHp47daVA"), CancellationToken.None);//cleanSession
            while (!stoppingToken.IsCancellationRequested)
            {
                string devices = Get<string>();
                Console.WriteLine(devices);
                Device[] devicess = Get<Device[]>();
                Console.WriteLine(JsonConvert.SerializeObject(devicess));
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
                await Task.Delay(10000, stoppingToken);
            }
        }

        /// <summary>  
        /// Common method for making GET calls  
        /// </summary>  
        protected T Get<T>()
        {

            HttpResponseMessage response = _httpClient.GetAsync(BaseEndpoint, HttpCompletionOption.ResponseHeadersRead).Result;
            var data = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception(data);

            return ConvertResult<T>(data);
        }

        private T ConvertResult<T>(string data)
        {
            data = JObject.Parse(data)["devices"].ToString();
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(data, typeof(T));
            else
                return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
