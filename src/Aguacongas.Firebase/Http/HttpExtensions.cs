using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public static class HttpExtensions
    {
        public static StringContent CreateJsonContent<T>(this HttpClient httpClient, T data, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return new StringContent(JsonConvert.SerializeObject(data, jsonSerializerSettings), Encoding.UTF8, "application/json");
        }
        public static async Task<T> DeserializeResponseAsync<T>(this HttpResponseMessage response, JsonSerializerSettings jsonSerializerSettings = null)
        {
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse, jsonSerializerSettings);
        }

    }
}
