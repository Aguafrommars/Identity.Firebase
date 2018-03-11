using Aguacongas.Firebase;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.IO;
using Xunit;

namespace Aguacongas.Identity.Firebase.Test
{
    public class IdentityBuilderExtensionsTest
    {
        [Fact]
        public void AddFirebaseStores_with_AuthTokenOptionsTest()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.AddUserSecrets<IdentityBuilderExtensionsTest>()
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../testsettings.json"))
                .Build();

            var services = new ServiceCollection();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddFirebaseStores("http://test", options =>
                {
                    configuration.GetSection("AuthTokenOptions").Bind(options);
                });

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<IUserStore<IdentityUser>>();
            provider.GetRequiredService<IRoleStore<IdentityRole>>();
        }

        [Fact]
        public void AddFirebaseStores_with_FirebaseTokenMamagerTest()
        {
            var mock = new Mock<IFirebaseTokenManager>();
            var services = new ServiceCollection();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddFirebaseStores("http://test", p =>
                {
                    return mock.Object;
                });

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<IUserStore<IdentityUser>>();
            provider.GetRequiredService<IRoleStore<IdentityRole>>();
        }
    }
}
