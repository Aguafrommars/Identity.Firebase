// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase.Http
{
    /// <summary>
    /// Firebase Authentication Delegating Handler
    /// </summary>
    public class FirebaseAuthenticationHandler: DelegatingHandler
    {
        private readonly IFirebaseTokenManager _tokenManager;

        /// <summary>
        /// Create a new instance of <see cref="FirebaseAuthenticationHandler"/>
        /// </summary>
        /// <param name="tokenManager">The token manager to use</param>
        public FirebaseAuthenticationHandler(IFirebaseTokenManager tokenManager)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        /// <summary>
        /// Create a new instance of <see cref="FirebaseAuthenticationHandler"/>
        /// </summary>
        /// <param name="tokenManager">The token manager to use</param>
        /// <param name="innerHandler">An inner delegating handler</param>
        public FirebaseAuthenticationHandler(IFirebaseTokenManager tokenManager, DelegatingHandler innerHandler): base(innerHandler)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        /// <inheritdoc />
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentUri = request.RequestUri;
            var queryString = new QueryString(currentUri.Query);
            var token = await _tokenManager.GetTokenAsync()
                .ConfigureAwait(false);
            queryString = queryString.Add(_tokenManager.AuthParamName, token);
            var uriBuilder = new UriBuilder(currentUri)
            {
                Query = queryString.Value
            };
            request.RequestUri = uriBuilder.Uri;

            return await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
