using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public class FirebaseClient : IFirebaseClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly IFirebaseTokenManager _tokenManager;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public FirebaseClient(HttpClient httpClient, IOptions<FirebaseOptions> options)
        {            
            _httpClient = httpClient;
            var value = options.Value;            
            _tokenManager = value.FirebaseTokenManager;
            _url = value.DatabaseUrl;
            _jsonSerializerSettings = value.JsonSerializerSettings;

            if (!_url.EndsWith("/"))
            {
                _url = _url + "/";
            }
        }

        public async Task<string> PostAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var response = await _httpClient.PostAsync(await GetFirebaseUrl(url, cancellationToken), _httpClient.CreateJsonContent(data, _jsonSerializerSettings), cancellationToken);
            var postResponse = await response.DeserializeResponseAsync<PostResponse>(_jsonSerializerSettings);
            return postResponse.Name;
        }

        public async Task<T> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var response = await _httpClient.PutAsync(await GetFirebaseUrl(url, cancellationToken), _httpClient.CreateJsonContent(data), cancellationToken);
            return await GetResponse<T>(response);
        }

        public async Task<T> PatchAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var message = new HttpRequestMessage(new HttpMethod("PATCH"), await GetFirebaseUrl(url, cancellationToken))
            {
                Content = _httpClient.CreateJsonContent(data, _jsonSerializerSettings)
            };

            var response = await _httpClient.SendAsync(message, cancellationToken);
            return await GetResponse<T>(response);
        }

        public async Task DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response =
                await _httpClient.DeleteAsync(await GetFirebaseUrl(url, cancellationToken), cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }                
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync(await GetFirebaseUrl(url, cancellationToken), cancellationToken);
            return await GetResponse<T>(response);
        }

        private async Task<T> GetResponse<T>(HttpResponseMessage response)
        {
            using (response)
            {
                return await response.DeserializeResponseAsync<T>(_jsonSerializerSettings);
            }
        }

        private async Task<string> GetFirebaseUrl(string url, CancellationToken cancellationToken)
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

            while (sanetizedUrl.StartsWith("/"))
            {
                sanetizedUrl = sanetizedUrl.Substring(1);
            }

            return $"{_url}{sanetizedUrl}?auth={await _tokenManager.GetTokenAsync(cancellationToken)}";
        }

        class PostResponse
        {
            public string Name { get; set; }
        }
    }
}
