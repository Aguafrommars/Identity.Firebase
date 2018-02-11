using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public class FirebaseClient : IFirebaseClient, IDisposable
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

        public async Task<FirebaseResponse<string>> PostAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var response = await _httpClient.PostAsync(await GetFirebaseUrl(url, cancellationToken), _httpClient.CreateJsonContent(data, _jsonSerializerSettings, requestEtag), cancellationToken);
            var postResponse = await response.DeserializeResponseAsync<PostResponse>(_jsonSerializerSettings);
            return new FirebaseResponse<string>
            {
                Data = postResponse.Data.Name,
                Etag = postResponse.Etag
            };
        }

        public async Task<FirebaseResponse<T>> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string etag = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var response = await _httpClient.PutAsync(await GetFirebaseUrl(url, cancellationToken), _httpClient.CreateJsonContent(data, _jsonSerializerSettings, requestEtag, etag), cancellationToken);
            return await GetResponse<T>(response);
        }

        public async Task<FirebaseResponse<T>> PatchAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), string etag = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var message = new HttpRequestMessage(new HttpMethod("PATCH"), await GetFirebaseUrl(url, cancellationToken))
            {
                Content = _httpClient.CreateJsonContent(data, _jsonSerializerSettings, etag: etag)
            };

            var response = await _httpClient.SendAsync(message, cancellationToken);
            return await GetResponse<T>(response);
        }

        public async Task DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken), string etag = null)
        {
            var message = new HttpRequestMessage(new HttpMethod("DELETE"), await GetFirebaseUrl(url, cancellationToken));
            message.Headers.SetIfMath(etag);

            using (var response =
                await _httpClient.SendAsync(message, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
            }                
        }

        public async Task<FirebaseResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false)
        {
            var message = new HttpRequestMessage(new HttpMethod("DELETE"), await GetFirebaseUrl(url, cancellationToken));
            message.Headers.SetRequestEtag(requestEtag);

            var response = await _httpClient.GetAsync(await GetFirebaseUrl(url, cancellationToken), cancellationToken);
            return await GetResponse<T>(response);
        }

        private async Task<FirebaseResponse<T>> GetResponse<T>(HttpResponseMessage response)
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                    _tokenManager.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
