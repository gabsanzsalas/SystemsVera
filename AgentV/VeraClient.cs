using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AgentV
{
    class VeraClient
    {
        private readonly HttpClient _httpClient;
        private Uri _BaseEndpoint { get; set; }


        public VeraClient(Uri baseEndpoint)
        {
            _httpClient = new HttpClient();
            _BaseEndpoint = baseEndpoint;
        }

        /// <summary>  
        /// Common method for making GET calls  
        /// </summary>  
        public T Get<T>()
        {

            HttpResponseMessage response = _httpClient.GetAsync(_BaseEndpoint, HttpCompletionOption.ResponseHeadersRead).Result;
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
