using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var content = new StringContent(JsonConvert.SerializeObject(data, jsonSerializerSettings), Encoding.UTF8, "application/json");
            var headers = content.Headers;
            headers.SetIfMath(etag);
            headers.SetRequestEtag(requestEtag);

            return content;
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

        public static void SetIfMatch(this StringContent content, string etag = null)
        {

        }
        public static async Task<FirebaseResponse<T>> DeserializeResponseAsync<T>(this HttpResponseMessage response, JsonSerializerSettings jsonSerializerSettings = null)
        {
            response.EnsureIsSuccess();
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return new FirebaseResponse<T>
            {
                Data = JsonConvert.DeserializeObject<T>(jsonResponse, jsonSerializerSettings),
                Etag = response.Headers.ETag?.Tag
            };
        }

        public static void EnsureIsSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new FirebaseException(response.StatusCode, response.ReasonPhrase, response.Headers.ETag?.Tag);
            }
        }
    }
}
