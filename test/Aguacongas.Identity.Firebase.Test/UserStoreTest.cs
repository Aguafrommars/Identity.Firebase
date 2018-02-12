using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.Identity.Firebase.Test
{
    public class UserStoreTest
    {
        [Fact]
        public async Task UserStoreMethodsThrowWhenDisposedTest()
        {
            var clientMock = new Mock<IFirebaseClient>();
            var store = new UserStore(clientMock.Object);
            store.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.AddClaimsAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.AddLoginAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.AddToRoleAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.GetClaimsAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.GetLoginsAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.GetRolesAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.IsInRoleAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.RemoveClaimsAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.RemoveLoginAsync(null, null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await store.RemoveFromRoleAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.RemoveClaimsAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.ReplaceClaimAsync(null, null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.FindByLoginAsync(null, null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.FindByIdAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.FindByNameAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.CreateAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.UpdateAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.DeleteAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await store.SetEmailConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await store.GetEmailConfirmedAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await store.SetPhoneNumberConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await store.GetPhoneNumberConfirmedAsync(null));
        }

        [Fact]
        public async Task UserStorePublicNullCheckTest()
        {
            Assert.Throws<ArgumentNullException>("client", () => new UserStore(null));
            var clientMock = new Mock<IFirebaseClient>();
            var store = new UserStore(clientMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetUserIdAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetUserNameAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetUserNameAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.CreateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.UpdateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.DeleteAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.AddClaimsAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.ReplaceClaimAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.RemoveClaimsAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetClaimsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetLoginsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetRolesAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.AddLoginAsync(null, null));
            await
                Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.RemoveLoginAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.AddToRoleAsync(null, null));
            await
                Assert.ThrowsAsync<ArgumentNullException>("user",
                    async () => await store.RemoveFromRoleAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.IsInRoleAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetPasswordHashAsync(null));
            await
                Assert.ThrowsAsync<ArgumentNullException>("user",
                    async () => await store.SetPasswordHashAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetSecurityStampAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                async () => await store.SetSecurityStampAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("login", async () => await store.AddLoginAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("claims",
                async () => await store.AddClaimsAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("claims",
                async () => await store.RemoveClaimsAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetEmailConfirmedAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                async () => await store.SetEmailConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetEmailAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetEmailAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetPhoneNumberAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetPhoneNumberAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                async () => await store.GetPhoneNumberConfirmedAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                async () => await store.SetPhoneNumberConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetTwoFactorEnabledAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",
                async () => await store.SetTwoFactorEnabledAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetLockoutEnabledAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetLockoutEnabledAsync(null, false));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetLockoutEndDateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetLockoutEndDateAsync(null, new DateTimeOffset()));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.ResetAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.IncrementAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("normalizedRoleName", async () => await store.AddToRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("normalizedRoleName", async () => await store.RemoveFromRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("normalizedRoleName", async () => await store.IsInRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("normalizedRoleName", async () => await store.AddToRoleAsync(new IdentityUser("fake"), ""));
            await Assert.ThrowsAsync<ArgumentNullException>("normalizedRoleName", async () => await store.RemoveFromRoleAsync(new IdentityUser("fake"), ""));
            await Assert.ThrowsAsync<ArgumentNullException>("normalizedRoleName", async () => await store.IsInRoleAsync(new IdentityUser("fake"), ""));
        }

        [Fact]
        public async Task CanCreateUsingManager()
        {
            var manager = CreateManager(out Mock<IFirebaseClient> clientMock);
            var guid = Guid.NewGuid().ToString();
            var user = new IdentityUser { UserName = "New" + guid };

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value.StartsWith("indexes/users-names/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>());

            clientMock.Setup(m => m.PostAsync("users", It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>
                {
                    Data = "test",
                    Etag = "test"
                });

            clientMock.Setup(m => m.PutAsync<string>(It.Is<string>(value => value.StartsWith("indexes/users-names/")),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>(),
                It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<string>());

            clientMock.Setup(m => m.PutAsync<string>(It.Is<string>(value => value.StartsWith("indexes/users-email/")),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>(),
                It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<string>());

            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));

            clientMock.Verify(m => m.PostAsync("users", user, It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
            clientMock.Verify(m => m.PutAsync($"indexes/users-names/{user.NormalizedUserName}", 
                user.Id, 
                It.IsAny<CancellationToken>(), 
                It.IsAny<bool>(),
                It.IsAny<string>()), Times.Once);

            clientMock.Verify(m => m.PutAsync($"indexes/users-email/{user.NormalizedEmail}",
                user.Id,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>(),
                It.IsAny<string>()), Times.Never);

            clientMock.Setup(m => m.DeleteAsync($"users/{user.Id}", It.IsAny<CancellationToken>(), user.ConcurrencyStamp))
                .Returns(Task.CompletedTask);
            
            IdentityResultAssert.IsSuccess(await manager.DeleteAsync(user));

            clientMock.Verify(m => m.DeleteAsync($"users/{user.Id}", It.IsAny<CancellationToken>(), user.ConcurrencyStamp), Times.Once);

            user.Email = "test@test.com";

            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));

            clientMock.Verify(m => m.PutAsync($"indexes/users-email/{user.NormalizedEmail}",
                user.Id,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>(),
                It.IsAny<string>()), Times.Once);
        }

        private UserManager<IdentityUser> CreateManager(out Mock<IFirebaseClient> clientMock)
        {
            clientMock = new Mock<IFirebaseClient>();
            var client = clientMock.Object;
            var services = new ServiceCollection();

            services.AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddFirebaseStores();

            services.AddScoped(b => client);
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<UserManager<IdentityUser>>();
        }
    }
}
