using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public static class HttpExtensions
    {
        public static StringContent CreateJsonContent<T>(this HttpClient httpClient, T data, JsonSerializerSettings jsonSerializerSettings = null, bool requestEtag = false, string etag = null)
        {
            return new StringContent(JsonConvert.SerializeObject(data, jsonSerializerSettings), Encoding.UTF8, "application/json");
        }

        public static void SetRequestEtag(this HttpHeaders headers, bool requestEtag)
        {
            if (requestEtag)
            {
                headers.Add("X-Firebase-ETag", new[] { "true" });
            }
        }

        public static void SetIfMath(this HttpHeaders headers, string etag)
        {
            if (!string.IsNullOrEmpty(etag))
            {
                headers.Add("if-match", new[] { etag });
            }
        }

        public static async Task<FirebaseResponse<T>> DeserializeResponseAsync<T>(this HttpResponseMessage response, JsonSerializerSettings jsonSerializerSettings = null)
        {
            await response.EnsureIsSuccess();
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return new FirebaseResponse<T>
            {
                Data = JsonConvert.DeserializeObject<T>(jsonResponse, jsonSerializerSettings),
                Etag = response.Headers.ETag?.Tag
            };
        }

        public static async Task EnsureIsSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new FirebaseException(response.StatusCode, response.ReasonPhrase, await response.Content.ReadAsStringAsync(), response.Headers.ETag?.Tag);
            }
        }
    }
}
