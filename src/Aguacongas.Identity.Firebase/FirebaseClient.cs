using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Identity.Firebase
{
    public class FirebaseClient : IFirebaseClient
    {
        private readonly HttpClient _httpClient;
        private readonly FirebaseOptions _options;
        private readonly AuthRequest _authRequest;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private DateTime _nextRenewTime = DateTime.MinValue;
        private string _authKey;

        public FirebaseClient(HttpClient httpClient, IOptions<FirebaseOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _authRequest = new AuthRequest
            {
                Email = _options.UserName,
                Password = _options.Password
            };
        }

        public async Task<string> PostAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var response = await _httpClient.PostAsync(await GetFirebaseUrl(url, cancellationToken), CreateContent(data), cancellationToken);
            var postResponse = await GetResponse<PostResponse>(response);
            return postResponse.Name;
        }

        public async Task<T> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var response = await _httpClient.PutAsync(await GetFirebaseUrl(url, cancellationToken), CreateContent(data), cancellationToken);
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
                Content = CreateContent(data)
            };

            var response = await _httpClient.SendAsync(message, cancellationToken);            
            return await GetResponse<T>(response);
        }

        public async Task DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.DeleteAsync(await GetFirebaseUrl(url, cancellationToken), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync(await GetFirebaseUrl(url, cancellationToken), cancellationToken);
            return await GetResponse<T>(response);
        }

        private static async Task<T> GetResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
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

            if (sanetizedUrl.StartsWith("/") && _options.DatabaseUrl.EndsWith("/"))
            {
                sanetizedUrl = sanetizedUrl.Substring(1);
            }

            if (!sanetizedUrl.StartsWith("/") && !_options.DatabaseUrl.EndsWith("/"))
            {
                sanetizedUrl = "/" + sanetizedUrl;
            }

            return $"{_options.DatabaseUrl}{sanetizedUrl}?auth={await GetAuthKey(cancellationToken)}";
        }

        private async Task<string> GetAuthKey(CancellationToken cancellationToken)
        {
            if (_nextRenewTime > DateTime.UtcNow.AddSeconds(-1))
            {
                return _authKey;
            }

            var content = CreateContent(_authRequest, _jsonSerializerSettings);

            var response = await _httpClient.PostAsync($"{_options.SignUpUrl}?key={_options.ApiKey}", content, cancellationToken);

            var authResponse = await GetResponse<AuthResponse>(response);
            _authKey = authResponse.IdToken;
            _nextRenewTime = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn);

            return _authKey;
        }

        private StringContent CreateContent<T>(T data)
        {
            return CreateContent(data, _options.JsonSerializerSettings);
        }

        private StringContent CreateContent<T>(T data, JsonSerializerSettings jsonSerializerSettings)
        {
            return new StringContent(JsonConvert.SerializeObject(data, jsonSerializerSettings), Encoding.UTF8, "application/json");
        }

        class PostResponse
        {
            public string Name { get; set; }
        }
        class AuthRequest
        {
            public string Email { get; set; }

            public string Password { get; set; }

            public bool ReturnSecureToken { get; set; } = true;
        }

        class AuthResponse
        {
            public string Kind { get; set; }

            public string LocalId { get; set; }

            public string Email { get; set; }

            public string DisplayName { get; set; }

            public string IdToken { get; set; }

            public bool Registered { get; set; }

            public string RefreshToken { get; set; }

            public int ExpiresIn { get; set; }
        }
    }
}
