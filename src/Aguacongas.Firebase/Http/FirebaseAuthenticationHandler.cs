using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase.Http
{
    public class FirebaseAuthenticationHandler: DelegatingHandler
    {
        private readonly IFirebaseTokenManager _tokenManager;

        public FirebaseAuthenticationHandler(IFirebaseTokenManager tokenManager)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        public FirebaseAuthenticationHandler(IFirebaseTokenManager tokenManager, DelegatingHandler innerHandler): base(innerHandler)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentUri = request.RequestUri;
            var queryString = new QueryString(currentUri.Query);
            var token = await _tokenManager.GetTokenAsync();
            queryString = queryString.Add(_tokenManager.AuthParamName, token);
            var uriBuilder = new UriBuilder(currentUri)
            {
                Query = queryString.Value
            };
            request.RequestUri = uriBuilder.Uri;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
