using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using MQTTnet;
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

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            BaseEndpoint = new Uri("http://10.0.2.134:3480/data_request?id=user_data");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            {
                string devices = Get<string>();
                Console.WriteLine(devices);
                Device[] devicess = Get<Device[]>();
                Console.WriteLine(JsonConvert.SerializeObject(devicess));
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
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
