using Google.Apis.Auth.OAuth2;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase.TokenManager
{
    /// <summary>
    /// OAuth token manager to manage authentication by OAuth
    /// </summary>
    public class OAuthTokenManager : IFirebaseTokenManager
    {
        private readonly ITokenAccess _credential;

        /// <summary>
        /// Gets the authentication param name
        /// </summary>
        public string AuthParamName { get; } = "access_token";

        /// <summary>
        /// Initialize a new instance of <see cref="OAuthTokenManager"/>
        /// </summary>
        /// <param name="credential">A <see cref="ITokenAccess"/></param>
        public OAuthTokenManager(ITokenAccess credential)
        {
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        }

        /// <summary>
        /// Gets the authentication token
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The token</returns>
        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _credential.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
        }
    }
}
