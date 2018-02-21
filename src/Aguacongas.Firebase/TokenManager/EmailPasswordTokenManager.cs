using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase.TokenManager
{
    public class EmailPasswordTokenManager : IFirebaseTokenManager, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly EmailPasswordOptions _options;
        private readonly AuthRequest _authRequest;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private DateTime _nextRenewTime = DateTime.MinValue;
        private string _authKey;

        public string AuthParamName { get; } = "auth";

        public EmailPasswordTokenManager(HttpClient httpClient, IOptions<EmailPasswordOptions> options):
            this(httpClient, options?.Value)
        { }

        public EmailPasswordTokenManager(HttpClient httpClient, EmailPasswordOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _authRequest = new AuthRequest
            {
                Email = _options.Email,
                Password = _options.Password
            };
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_nextRenewTime > DateTime.UtcNow.AddSeconds(-1))
            {
                return _authKey;
            }

            var content = _httpClient.CreateJsonContent(_authRequest, _jsonSerializerSettings);

            var response = await _httpClient.PostAsync($"{_options.SignUpUrl}?key={_options.ApiKey}", content, cancellationToken);

            var authResponse = await response.DeserializeResponseAsync<AuthResponse>(_jsonSerializerSettings);
            var data = authResponse.Data;
            _authKey = data.IdToken;
            _nextRenewTime = DateTime.UtcNow.AddSeconds(data.ExpiresIn);

            return _authKey;
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
