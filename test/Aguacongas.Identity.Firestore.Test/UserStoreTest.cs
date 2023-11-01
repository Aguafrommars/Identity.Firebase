using Aguacongas.Firebase.TokenManager;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.Identity.Firestore.Test
{
    public class UserStoreTest
    {
        [Fact]
        public async Task UserStoreMethodsThrowWhenDisposedTest()
        {
            var db = CreateDb();

            var tableNamesConfig = new FirestoreTableNamesConfig();
            var userOnlyStore = new UserOnlyStore(db, tableNamesConfig);
            var store = new UserStore(db, userOnlyStore, tableNamesConfig);
            store.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.AddClaimsAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.AddLoginAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.AddToRoleAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.GetClaimsAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.GetLoginsAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.GetRolesAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.IsInRoleAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.RemoveClaimsAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.RemoveLoginAsync(null, null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                () => store.RemoveFromRoleAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.RemoveClaimsAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.ReplaceClaimAsync(null, null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.FindByLoginAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.FindByIdAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.FindByNameAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.CreateAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.UpdateAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.DeleteAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                () => store.SetEmailConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => store.GetEmailConfirmedAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                () => store.SetPhoneNumberConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                () => store.GetPhoneNumberConfirmedAsync(null));
        }

        [Fact]
        public async Task UserStorePublicNullCheckTest()
        {
            Assert.Throws<ArgumentNullException>("db", () => new UserStore(null, null, null));
            var db = CreateDb();

            Assert.Throws<ArgumentNullException>("tableNamesConfig", () => new UserStore(db, null, null));

            var tableNamesConfig = new FirestoreTableNamesConfig();
            Assert.Throws<ArgumentNullException>("userOnlyStore", () => new UserStore(db, null, tableNamesConfig));
            
            var userOnlyStore = new UserOnlyStore(db, tableNamesConfig);
            var store = new UserStore(db, userOnlyStore, tableNamesConfig);
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetUserIdAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetUserNameAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.SetUserNameAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.CreateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.UpdateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.DeleteAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.AddClaimsAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.ReplaceClaimAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.RemoveClaimsAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetClaimsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetLoginsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetRolesAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.AddLoginAsync(null, null));
            await
                Assert.ThrowsAsync<ArgumentNullException>("user", () => store.RemoveLoginAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.AddToRoleAsync(null, null));
            await
                Assert.ThrowsAsync<ArgumentNullException>("user",
                    () => store.RemoveFromRoleAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.IsInRoleAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetPasswordHashAsync(null));
            await
                Assert.ThrowsAsync<ArgumentNullException>("user",
                    () => store.SetPasswordHashAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetSecurityStampAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                () => store.SetSecurityStampAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("login", () => store.AddLoginAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("claims",
                () => store.AddClaimsAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("claims",
                () => store.RemoveClaimsAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetEmailConfirmedAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                () => store.SetEmailConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetEmailAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.SetEmailAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetPhoneNumberAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.SetPhoneNumberAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                () => store.GetPhoneNumberConfirmedAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                () => store.SetPhoneNumberConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetTwoFactorEnabledAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                () => store.SetTwoFactorEnabledAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetLockoutEnabledAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.SetLockoutEnabledAsync(null, false));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.GetLockoutEndDateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.SetLockoutEndDateAsync(null, new DateTimeOffset()));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.ResetAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => store.IncrementAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", () => store.AddToRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", () => store.RemoveFromRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", () => store.IsInRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", () => store.AddToRoleAsync(new IdentityUser("fake"), ""));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", () => store.RemoveFromRoleAsync(new IdentityUser("fake"), ""));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", () => store.IsInRoleAsync(new IdentityUser("fake"), ""));
        }

        private FirestoreDb CreateDb()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../identityfirestore.json"))
                .Build();
            var services = new ServiceCollection();
            services.Configure((Action<OAuthServiceAccountKey>)(options =>
            {
                configuration.GetSection("FirestoreAuthTokenOptions").Bind(options);
            }))
            .AddScoped(provider =>
            {
                CreateAuthFile(provider, out IOptions<OAuthServiceAccountKey> authOptions);

                var client = FirestoreClient.Create();
                return FirestoreDb.Create(authOptions.Value.project_id, client: client);
            });

            return services.BuildServiceProvider().GetRequiredService<FirestoreDb>();
        }

        private static void CreateAuthFile(IServiceProvider provider, out IOptions<OAuthServiceAccountKey> authOptions)
        {
            authOptions = provider.GetRequiredService<IOptions<OAuthServiceAccountKey>>();
            var json = JsonConvert.SerializeObject(authOptions.Value);
            var path = Path.GetTempFileName();
            using var writer = File.CreateText(path);
            writer.Write(json);
            writer.Flush();
            writer.Close();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        }
    }
}
