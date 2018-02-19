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

        public FirebaseClient(HttpClient httpClient, IFirebaseTokenManager tokenManager, IOptions<FirebaseOptions> options)
        {            
            _httpClient = httpClient;
            var value = options.Value;            
            _tokenManager = tokenManager;
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

            var response = await SendFirebaseRequest<PostResponse>(url, "POST", _httpClient.CreateJsonContent(data, _jsonSerializerSettings), cancellationToken, requestEtag, null);
            return new FirebaseResponse<string>
            {
                Data = response.Data.Name,
                Etag = response.Etag
            };
        }

        public Task<FirebaseResponse<T>> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string etag = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return SendFirebaseRequest<T>(url, "PUT", _httpClient.CreateJsonContent(data, _jsonSerializerSettings), cancellationToken, requestEtag, etag);
        }

        public Task<FirebaseResponse<T>> PatchAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return SendFirebaseRequest<T>(url, "PATCH", _httpClient.CreateJsonContent(data, _jsonSerializerSettings), cancellationToken, false, null);
        }

        public async Task DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string etag = null)
        {
            var message = new HttpRequestMessage(new HttpMethod("DELETE"), await GetFirebaseUrl(url, cancellationToken));
            message.Headers.SetIfMath(etag);
            message.Headers.SetRequestEtag(requestEtag);

            using (var response =
                await _httpClient.SendAsync(message, cancellationToken))
            {
                await response.EnsureIsSuccess();
            }                
        }

        public Task<FirebaseResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string queryString = null)
        {
            return SendFirebaseRequest<T>(url, "GET", null, cancellationToken, requestEtag, null, queryString);
        }

        private async Task<FirebaseResponse<T>> SendFirebaseRequest<T>(string url, string method, HttpContent content, CancellationToken cancellationToken, bool requestEtag, string eTag, string queryString = null)
        {
            var message = new HttpRequestMessage(new HttpMethod(method), await GetFirebaseUrl(url, cancellationToken, queryString))
            {
                Content = content
            };
            message.Headers.SetIfMath(eTag);
            message.Headers.SetRequestEtag(requestEtag);

            var response = await _httpClient.SendAsync(message, cancellationToken);
            return await GetFirebaseResponse<T>(response);
        }
        private async Task<FirebaseResponse<T>> GetFirebaseResponse<T>(HttpResponseMessage response)
        {
            using (response)
            {
                return await response.DeserializeResponseAsync<T>(_jsonSerializerSettings);
            }
        }

        private async Task<string> GetFirebaseUrl(string url, CancellationToken cancellationToken, string queryString = null)
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

            var result = $"{_url}{sanetizedUrl}?{_tokenManager.AuthParamName}={await _tokenManager.GetTokenAsync(cancellationToken)}";
            if(!string.IsNullOrEmpty(queryString))
            {
                result += "&" + queryString; 
            }
            return result;
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
