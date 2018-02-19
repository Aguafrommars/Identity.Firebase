using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
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
        public async Task ConcurrentRoleUpdatesWillFail()
        {
            var role = new IdentityRole(Guid.NewGuid().ToString());
            var manager = CreateRoleManager(out Mock<IFirebaseClient> clientMock);

            IdentityResultAssert.IsSuccess(await manager.CreateAsync(role));

            var manager1 = CreateRoleManager(out Mock<IFirebaseClient> clientMock1);
            clientMock1.Setup(m => m.PutAsync(It.Is<string>(value => value.StartsWith("roles/")), It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<IdentityRole>
                {
                    Data = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                    },
                    Etag = Guid.NewGuid().ToString()
                });

            var manager2 = CreateRoleManager(out Mock<IFirebaseClient> clientMock2);
            clientMock2.Setup(m => m.PutAsync(It.Is<string>(value => value.StartsWith("roles/")), It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
               .ThrowsAsync(new FirebaseException(HttpStatusCode.PreconditionFailed, "test", "test", "test"));

            var role1 = await manager1.FindByIdAsync(role.Id);
            var role2 = await manager2.FindByIdAsync(role.Id);
            Assert.NotNull(role1);
            Assert.NotNull(role2);
            Assert.NotSame(role1, role2);
            role1.Name = Guid.NewGuid().ToString();
            role2.Name = Guid.NewGuid().ToString();
            IdentityResultAssert.IsSuccess(await manager1.UpdateAsync(role1));
            IdentityResultAssert.IsFailure(await manager2.UpdateAsync(role2), new IdentityErrorDescriber().ConcurrencyFailure());            
        }

        [Fact]
        public async Task DeleteAModifiedRoleWillFail()
        {
            var role = new IdentityRole(Guid.NewGuid().ToString());
            var manager = CreateRoleManager(out Mock<IFirebaseClient> clientMock);

            IdentityResultAssert.IsSuccess(await manager.CreateAsync(role));

            var manager1 = CreateRoleManager(out Mock<IFirebaseClient> clientMock1);
            clientMock1.Setup(m => m.PutAsync(It.Is<string>(value => value.StartsWith("roles/")), It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<IdentityRole>
                {
                    Data = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                    },
                    Etag = Guid.NewGuid().ToString()
                });

            var manager2 = CreateRoleManager(out Mock<IFirebaseClient> clientMock2);
            clientMock2.Setup(m => m.DeleteAsync(It.Is<string>(value => value.StartsWith("roles/")), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
               .ThrowsAsync(new FirebaseException(HttpStatusCode.PreconditionFailed, "test", "test", "test"));

            var role1 = await manager1.FindByIdAsync(role.Id);
            var role2 = await manager2.FindByIdAsync(role.Id);
            Assert.NotNull(role1);
            Assert.NotNull(role2);
            Assert.NotSame(role1, role2);
            role1.Name = Guid.NewGuid().ToString();
            role2.Name = Guid.NewGuid().ToString();
            IdentityResultAssert.IsSuccess(await manager1.UpdateAsync(role1));
            IdentityResultAssert.IsFailure(await manager2.DeleteAsync(role2), new IdentityErrorDescriber().ConcurrencyFailure());
        }

        private RoleManager<IdentityRole> CreateRoleManager(out Mock<IFirebaseClient> clientMock)
        {
            var provider = SetupIdentity(out clientMock);
            return provider.GetRequiredService<RoleManager<IdentityRole>>();
        }

        private static ServiceProvider SetupIdentity(out Mock<IFirebaseClient> clientMock)
        {
            clientMock = new Mock<IFirebaseClient>();
            var client = clientMock.Object;

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value.StartsWith("indexes/users-names/")), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
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
            
            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value.StartsWith("indexes/role-name/")), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<string>());

            clientMock.Setup(m => m.GetAsync<IdentityRole>(It.Is<string>(value => value.StartsWith("roles/")), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<IdentityRole>
                {
                    Data = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString()
                    }
                });

            clientMock.Setup(m => m.PostAsync("roles", It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => new FirebaseResponse<string>
                {
                    Data = Guid.NewGuid().ToString(),
                    Etag = "test"
                });

            var services = new ServiceCollection();

            services.AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password = new PasswordOptions
                    {
                        RequireDigit = false,
                        RequiredLength = 0,
                        RequiredUniqueChars = 0,
                        RequireLowercase = false,
                        RequireNonAlphanumeric = false,
                        RequireUppercase = false
                    };
                })
                .AddDefaultTokenProviders()
                .AddFirebaseStores();

            services.AddScoped(b => client);
            return services.BuildServiceProvider();
        }
    }
}
