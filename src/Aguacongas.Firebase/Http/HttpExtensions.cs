// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    /// <summary>
    /// Extentions methods for <see cref="HttpClient"/>
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Create JSON content
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="httpClient">The <see cref="HttpClient"/></param>
        /// <param name="data">Data to serialize</param>
        /// <param name="jsonSerializerSettings">A <see cref="JsonSerializerSettings"/></param>
        /// <returns>A <see cref="StreamContent"/></returns>
        public static StringContent CreateJsonContent<T>(this HttpClient httpClient, T data, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return new StringContent(JsonConvert.SerializeObject(data, jsonSerializerSettings), Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Sets X-Firebase-ETag header if requestEtag is true
        /// </summary>
        /// <param name="headers">The <see cref="HttpHeaders"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        public static void SetRequestEtag(this HttpHeaders headers, bool requestEtag)
        {
            if (requestEtag)
            {
                headers.TryAddWithoutValidation("X-Firebase-ETag", "true");
            }
        }

        /// <summary>
        /// Sets if-match header if etag is true
        /// </summary>
        /// <param name="headers">The <see cref="HttpHeaders"/></param>
        /// <param name="etag">True to set if-match header</param>
        public static void SetIfMath(this HttpHeaders headers, string etag)
        {
            if (!string.IsNullOrEmpty(etag))
            {
                headers.TryAddWithoutValidation("if-match", etag);
            }
        }

        /// <summary>
        /// Deserialize a Firebase response
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="response">The <see cref="HttpResponseMessage"/></param>
        /// <param name="jsonSerializerSettings">A <see cref="JsonSerializerSettings"/></param>
        /// <returns>A <see cref="FirebaseResponse{T}"/></returns>
        public static async Task<FirebaseResponse<T>> DeserializeResponseAsync<T>(this HttpResponseMessage response, JsonSerializerSettings jsonSerializerSettings = null)
        {
            await response.EnsureIsSuccess();
            
            var jsonResponse = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            return new FirebaseResponse<T>
            {
                Data = JsonConvert.DeserializeObject<T>(jsonResponse, jsonSerializerSettings),
                Etag = response.Headers.SingleOrDefault(h => h.Key.ToUpperInvariant() == "ETAG")
                    .Value?.FirstOrDefault()
            };
        }

        /// <summary>
        /// Ensure response is success
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/></param>
        /// <exception cref="FirebaseException">If response is not success</exception>
        /// <returns></returns>
        public static async Task EnsureIsSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new FirebaseException(response.StatusCode, response.ReasonPhrase, await response.Content.ReadAsStringAsync().ConfigureAwait(false), response.Headers.ETag?.Tag);
            }
        }
    }
}
