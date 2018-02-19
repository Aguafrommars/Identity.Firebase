using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase.TokenManager
{
    public class AuthTokenManager : IFirebaseTokenManager
    {
        private readonly ICredential _credentials;

        public string AuthParamName { get; } = "access_token";

        public AuthTokenManager(IOptions<AuthTokenOptions> options)
        {
            var json = JsonConvert.SerializeObject(options?.Value ?? throw new ArgumentNullException(nameof(options)));
            _credentials = GoogleCredential.FromJson(json)
                .CreateScoped("https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/firebase.database")
                .UnderlyingCredential;
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _credentials.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}
