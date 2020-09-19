// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase.TokenManager;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http;
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
            provider => new EmailPasswordTokenManager(provider.GetRequiredService<IHttpClientFactory>().CreateClient(), provider.GetRequiredService<IOptions<EmailPasswordOptions>>()),
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

            services.AddFirebaseClient(options =>
            {
                options.DatabaseUrl = "http://test2";
            },
            provider => accessTokenMock.Object,
            "instance2");

            serviceProvider = services.BuildServiceProvider();
            firebaseClient = serviceProvider.GetRequiredService<IFirebaseClient>();
            var tokenAccess = serviceProvider.GetRequiredService<ITokenAccess>();
        }
    }
}
