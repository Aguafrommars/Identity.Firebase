// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            provider.GetRequiredService<IUserStore<IdentityUser>>();
            provider.GetRequiredService<IRoleStore<IdentityRole>>();
        }
    }
}
