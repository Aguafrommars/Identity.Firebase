using Google.Apis.Auth.OAuth2;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase.TokenManager
{
    public class AuthTokenManager : IFirebaseTokenManager
    {
        private readonly ITokenAccess _credential;

        public string AuthParamName { get; } = "access_token";

        public AuthTokenManager(ITokenAccess credential)
        {
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _credential.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}
