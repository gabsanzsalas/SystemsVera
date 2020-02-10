using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AgentV
{
    public class VeraClient
    {
        private readonly HttpClient _httpClient;
        private Uri BaseEndpoint { get; set; }
        private AgentVeraSettings agentVeraSettings;

        public VeraClient(AgentVeraSettings agentVeraSettings)
        {
            _httpClient = new HttpClient();
            
            this.agentVeraSettings = agentVeraSettings;
            BaseEndpoint = new Uri(agentVeraSettings.baseEndpoint);
        }

        /// <summary>  
        /// Common method for making GET calls  
        /// </summary>  
        public T Get<T>()
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
