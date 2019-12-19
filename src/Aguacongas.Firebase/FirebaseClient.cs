using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    /// <summary>
    /// Firebase client
    /// </summary>
    public class FirebaseClient : IFirebaseClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Create a new instance of <see cref="FirebaseClient"/>
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/></param>
        /// <param name="options">A <see cref="FirebaseOptions"/></param>
        public FirebaseClient(HttpClient httpClient, FirebaseOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(options.DatabaseUrl);
            _jsonSerializerSettings = options.JsonSerializerSettings;
        }

        /// <summary>
        /// Sends a POST request to create data
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="data">Data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <returns>The data id</returns>
        public async Task<FirebaseResponse<string>> PostAsync<T>(string url, T data, CancellationToken cancellationToken = default, bool requestEtag = false)
        {
            CheckIsNotNull(data);

            var response = await SendFirebaseRequest<PostResponse>(url, "POST", _httpClient.CreateJsonContent(data, _jsonSerializerSettings), cancellationToken, requestEtag, null)
                .ConfigureAwait(false);
            return new FirebaseResponse<string>
            {
                Data = response.Data.Name,
                Etag = response.Etag
            };
        }

        /// <summary>
        /// Send a PUT request to update data
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="data">Data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <param name="etag">ETag value</param>
        /// <returns>A <see cref="FirebaseResponse{T}"/></returns>
        public Task<FirebaseResponse<T>> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default, bool requestEtag = false, string etag = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return SendFirebaseRequest<T>(url, "PUT", _httpClient.CreateJsonContent(data, _jsonSerializerSettings), cancellationToken, requestEtag, etag);
        }

        /// <summary>
        /// Send a PATCH request to path data
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="data">Data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A <see cref="FirebaseResponse{T}"/></returns>
        public Task<FirebaseResponse<T>> PatchAsync<T>(string url, T data, CancellationToken cancellationToken = default)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return SendFirebaseRequest<T>(url, "PATCH", _httpClient.CreateJsonContent(data, _jsonSerializerSettings), cancellationToken, false, null);
        }

        /// <summary>
        /// Send a DELETE request to delete data
        /// </summary>
        /// <param name="url">Data uri</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <param name="etag">Etag value</param>
        /// <returns></returns>
        public async Task DeleteAsync(string url, CancellationToken cancellationToken = default, bool requestEtag = false, string etag = null)
        {
            using (var message = new HttpRequestMessage(new HttpMethod("DELETE"), GetFirebaseUrl(url)))
            {
                message.Headers.SetIfMath(etag);
                message.Headers.SetRequestEtag(requestEtag);

                using (var response =
                    await _httpClient.SendAsync(message, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await response.EnsureIsSuccess();
                }
            }
        }

        /// <summary>
        /// Send a GET request to request data
        /// </summary>
        /// <typeparam name="T">The type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <param name="queryString">A query string</param>
        /// <returns>A <see cref="FirebaseResponse{T}"/></returns>
        public Task<FirebaseResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default, bool requestEtag = false, string queryString = null)
        {
            return SendFirebaseRequest<T>(url, "GET", null, cancellationToken, requestEtag, null, queryString);
        }

        private async Task<FirebaseResponse<T>> SendFirebaseRequest<T>(string url, string method, HttpContent content, CancellationToken cancellationToken, bool requestEtag, string eTag, string queryString = null)
        {
            using (var message = new HttpRequestMessage(new HttpMethod(method), GetFirebaseUrl(url, queryString))
            {
                Content = content
            })
            {
                message.Headers.SetIfMath(eTag);
                message.Headers.SetRequestEtag(requestEtag);

                var response = await _httpClient.SendAsync(message, cancellationToken)
                    .ConfigureAwait(false);
                return await GetFirebaseResponse<T>(response)
                    .ConfigureAwait(false);
            }
        }

        private Task<FirebaseResponse<T>> GetFirebaseResponse<T>(HttpResponseMessage response)
        {
            using (response)
            {
                return response.DeserializeResponseAsync<T>(_jsonSerializerSettings);
            }
        }

        private Uri GetFirebaseUrl(string url , string queryString = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var sanetizedUrl = url;
            if (!url.EndsWith(".json"))
            {
                sanetizedUrl = url + ".json";
            }

            var baseAddress = _httpClient.BaseAddress;
            var builder = new UriBuilder(baseAddress.Scheme, baseAddress.Host, baseAddress.Port, sanetizedUrl)
            {
                Query = queryString
            };

            
            return builder.Uri;
        }

        private static void CheckIsNotNull<T>(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
        }

        class PostResponse
        {
            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Firebase service response we don't want to expose")]
            public string Name { get; set; }
        }
    }
}
