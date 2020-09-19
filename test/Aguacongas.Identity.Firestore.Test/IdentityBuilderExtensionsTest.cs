using Aguacongas.Firebase.TokenManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using Xunit;

namespace Aguacongas.Identity.Firestore.Test
{
    public class IdentityBuilderExtensionsTest
    {
        [Fact]
        public void AddFirebaseStores_with_AuthTokenOptionsTest()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../identityfirestore.json"))
                .Build();

            var services = new ServiceCollection();
            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddFirestoreStores(options =>
                {
                    configuration.GetSection("FirestoreAuthTokenOptions").Bind(options);
                });

            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<IUserStore<IdentityUser>>());
            Assert.NotNull(provider.GetService<IRoleStore<IdentityRole>>());
        }

        [Fact]
        public void AddFirebaseStores_with_AuthTokenOptions_and_file_Test()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../identityfirestore.json"))
                .Build();

            var services = new ServiceCollection();
            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddFirestoreStores(options =>
                {
                    configuration.GetSection("FirestoreAuthTokenOptions").Bind(options);
                }, "auth2.json");

            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<IUserStore<IdentityUser>>());
            Assert.NotNull(provider.GetService<IRoleStore<IdentityRole>>());
            Assert.True(File.Exists("auth2.json"));
        }

        [Fact]
        public void AddFirebaseStores_with_project_id_Test()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../identityfirestore.json"))
                .Build();

            var authOptions = configuration.GetSection("FirestoreAuthTokenOptions").Get<OAuthServiceAccountKey>();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
            {
                var path = Path.GetTempFileName();

                var json = JsonConvert.SerializeObject(authOptions);
                using var writer = File.CreateText(path);
                writer.Write(json);
                writer.Flush();
                writer.Close();
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            }

            var services = new ServiceCollection();
            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddFirestoreStores(authOptions.project_id);

            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<IUserStore<IdentityUser>>());
            Assert.NotNull(provider.GetService<IRoleStore<IdentityRole>>());
        }
    }
}
