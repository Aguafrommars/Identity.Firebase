using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase.TokenManager
{
    /// <summary>
    /// Email password token manager
    /// </summary>
    public class EmailPasswordTokenManager : IFirebaseTokenManager
    {
        private readonly HttpClient _httpClient;
        private readonly EmailPasswordOptions _options;
        private readonly EmailPasswordAuthRequest _authRequest;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private DateTime _nextRenewTime = DateTime.MinValue;
        private string _authKey;

        /// <summary>
        /// Gets the authentication param name
        /// </summary>
        public string AuthParamName { get; } = "auth";

        /// <summary>
        /// Initialize a new instance of <see cref="EmailPasswordTokenManager"/>
        /// </summary>
        /// <param name="httpClient">A <see cref="HttpClient"/></param>
        /// <param name="options">A <see cref="IOptions{EmailPasswordOptions}"/></param>
        public EmailPasswordTokenManager(HttpClient httpClient, IOptions<EmailPasswordOptions> options):
            this(httpClient, options?.Value)
        { }

        /// <summary>
        /// Initialize a new instance of <see cref="EmailPasswordTokenManager"/>
        /// </summary>
        /// <param name="httpClient">A <see cref="HttpClient"/></param>
        /// <param name="options">A <see cref="EmailPasswordOptions"/></param>
        public EmailPasswordTokenManager(HttpClient httpClient, EmailPasswordOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _authRequest = new EmailPasswordAuthRequest
            {
                Email = _options.Email,
                Password = _options.Password
            };
        }

        /// <summary>
        /// Gets the authentication token
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The token</returns>
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
    }
}
