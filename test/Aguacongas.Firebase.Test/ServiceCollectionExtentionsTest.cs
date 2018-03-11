using Aguacongas.Firebase.TokenManager;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Aguacongas.Firebase.Test
{
    public class ServiceCollectionExtentionsTest
    {
        [Fact]
        public void AddFirebaseClientTest()
        {
            var accessTokenMock = new Mock<ITokenAccess>();
            var services = new ServiceCollection();
            services.AddFirebaseClient(options =>
            {
                options.DatabaseUrl = "http://test1";
            }, 
            provider => new EmailPasswordTokenManager(provider.GetRequiredService<HttpClient>(), provider.GetRequiredService<IOptions<EmailPasswordOptions>>()),
            "instance1")
            .Configure<EmailPasswordOptions>(options =>
            {
                options.ApiKey = "test";
                options.Email = "test";
                options.Password = "test";
            })
            .AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();

            var firebaseClient = serviceProvider.GetRequiredService<IFirebaseClient>();
            var tokenManager = serviceProvider.GetRequiredService<IFirebaseTokenManager>();
        }
    }
}
