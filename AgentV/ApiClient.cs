using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AgentV
{
    public class ApiClient
    {
        private const string _constantGetDevices = "devices";
        private readonly HttpClient _httpClient;

        private AgentVeraSettings agentVeraSettings;

        public ApiClient(AgentVeraSettings agentVeraSettings)
        {
            _httpClient = new HttpClient();
            this.agentVeraSettings = agentVeraSettings;
        }

        /// <summary>  
        /// Common method for making GET calls  
        /// </summary>  
        private T Get<T>(string objectkey)
        {

            HttpResponseMessage response = _httpClient.GetAsync(
                new Uri(agentVeraSettings.baseEndpoint)
                , HttpCompletionOption.ResponseHeadersRead).Result;
            var data = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception(data);

            return ConvertResult<T>(data, objectkey);
        }

        private T ConvertResult<T>(string data, string objectkey)
        {

            data = JObject.Parse(data)[objectkey].ToString();
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(data, typeof(T));
            else
                return JsonConvert.DeserializeObject<T>(data);
        }

        public Device[] GetDevices()
        {
            return this.Get<Device[]>(_constantGetDevices);

        }


    }
}
