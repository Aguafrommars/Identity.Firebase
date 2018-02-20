using Aguacongas.Firebase.TokenManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.Firebase.Test
{
    public class AuthTokenManagerIntegrationTest
    {
        [Fact]
        public async Task GetTokenAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<AuthTokenManagerIntegrationTest>()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\testsettings.json"))
                .AddEnvironmentVariables()
                .Build();

            var options = new AuthTokenOptions();
            configuration.GetSection("AuthTokenOptions").Bind(options);
            var mock = new Mock<IOptions<AuthTokenOptions>>();
            mock.SetupGet(m => m.Value).Returns(options);

            var sut = new AuthTokenManager(mock.Object);

            var token = await sut.GetTokenAsync();
        }
    }
}
