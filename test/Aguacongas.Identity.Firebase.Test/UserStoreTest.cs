using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
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

            IdentityResultAssert.IsSuccess(await manager.DeleteAsync(user));

            clientMock.Verify(m => m.DeleteAsync($"users/{user.Id}", It.IsAny<CancellationToken>(), true, user.ConcurrencyStamp), Times.Once);

            user.Email = "test@test.com";

            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));

            clientMock.Verify(m => m.PutAsync($"indexes/users-email/{user.NormalizedEmail}",
                user.Id,
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TwoUsersSamePasswordDifferentHash()
        {
            var manager = CreateManager(out Mock<IFirebaseClient> clientMock);
            var userA = new IdentityUser(Guid.NewGuid().ToString());
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(userA, "password"));
            var userB = new IdentityUser(Guid.NewGuid().ToString());
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(userB, "password"));

            Assert.NotEqual(userA.PasswordHash, userB.PasswordHash);
        }

        [Fact]
        public async Task AddUserToUnknownRoleFails()
        {
            var manager = CreateManager(out Mock<IFirebaseClient> clientMock);
            var u = CreateTestUser();
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(u));

            clientMock.Setup(m => m.GetAsync<IdentityRole>(It.Is<string>(value => value.StartsWith("roles/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityRole>());

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await manager.AddToRoleAsync(u, "bogus"));
        }

        [Fact]
        public async Task ConcurrentUpdatesWillFail()
        {
            var user = CreateTestUser();
            var manager1 = CreateManager(out Mock<IFirebaseClient> clientMock1);

            clientMock1.Setup(m => m.GetAsync<IdentityUser>(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUser>
                {
                    Data = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    Etag = Guid.NewGuid().ToString()
                });
            clientMock1.Setup(m => m.PutAsync<IdentityUser>(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUser>
                {
                    Etag = Guid.NewGuid().ToString()
                });

            var manager2 = CreateManager(out Mock<IFirebaseClient> clientMock2);

            clientMock2.Setup(m => m.GetAsync<IdentityUser>(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUser>
                {
                    Data = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    Etag = Guid.NewGuid().ToString()
                });
            clientMock2.Setup(m => m.PutAsync(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
               .ThrowsAsync(new FirebaseException(HttpStatusCode.PreconditionFailed, "test", "test"));

            var user1 = await manager1.FindByIdAsync(user.Id);
            var user2 = await manager2.FindByIdAsync(user.Id);
            Assert.NotNull(user1);
            Assert.NotNull(user2);
            Assert.NotSame(user1, user2);
            user1.UserName = Guid.NewGuid().ToString();
            user2.UserName = Guid.NewGuid().ToString();
            IdentityResultAssert.IsSuccess(await manager1.UpdateAsync(user1));
            IdentityResultAssert.IsFailure(await manager2.UpdateAsync(user2), new IdentityErrorDescriber().ConcurrencyFailure());
        }

        [Fact]
        public async Task DeleteAModifiedUserWillFail()
        {
            var user = CreateTestUser();
            var manager1 = CreateManager(out Mock<IFirebaseClient> clientMock1);

            clientMock1.Setup(m => m.GetAsync<IdentityUser>(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUser>
                {
                    Data = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    Etag = Guid.NewGuid().ToString()
                });
            clientMock1.Setup(m => m.PutAsync(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUser>
                {
                    Etag = Guid.NewGuid().ToString()
                });

            var manager2 = CreateManager(out Mock<IFirebaseClient> clientMock2);

            clientMock2.Setup(m => m.GetAsync<IdentityUser>(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUser>
                {
                    Data = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    Etag = Guid.NewGuid().ToString()
                });
            clientMock2.Setup(m => m.DeleteAsync(It.Is<string>(value => value.StartsWith("users/")), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<string>()))
                .ThrowsAsync(new FirebaseException(HttpStatusCode.PreconditionFailed, "test", "test"));

            var user1 = await manager1.FindByIdAsync(user.Id);
            var user2 = await manager2.FindByIdAsync(user.Id);
            Assert.NotNull(user1);
            Assert.NotNull(user2);
            Assert.NotSame(user1, user2);
            user1.UserName = Guid.NewGuid().ToString();
            IdentityResultAssert.IsSuccess(await manager1.UpdateAsync(user1));
            IdentityResultAssert.IsFailure(await manager2.DeleteAsync(user2), new IdentityErrorDescriber().ConcurrencyFailure());
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
               .ThrowsAsync(new FirebaseException(HttpStatusCode.PreconditionFailed, "test", "test"));

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
               .ThrowsAsync(new FirebaseException(HttpStatusCode.PreconditionFailed, "test", "test"));

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

        private UserManager<IdentityUser> CreateManager(out Mock<IFirebaseClient> clientMock)
        {
            var provider = SetupIdentity(out clientMock);
            return provider.GetRequiredService<UserManager<IdentityUser>>();
        }

        private RoleManager<IdentityRole> CreateRoleManager(out Mock<IFirebaseClient> clientMock)
        {
            var provider = SetupIdentity(out clientMock);
            return provider.GetRequiredService<RoleManager<IdentityRole>>();
        }

        protected IdentityUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "",
           bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = default(DateTimeOffset?), bool useNamePrefixAsUserName = false)
        {
            return new IdentityUser
            {
                UserName = useNamePrefixAsUserName ? namePrefix : string.Format("{0}{1}", namePrefix, Guid.NewGuid()),
                Email = email,
                PhoneNumber = phoneNumber,
                LockoutEnabled = lockoutEnabled,
                LockoutEnd = lockoutEnd
            };
        }

        private static ServiceProvider SetupIdentity(out Mock<IFirebaseClient> clientMock)
        {
            clientMock = new Mock<IFirebaseClient>();
            var client = clientMock.Object;

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
            
            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value.StartsWith("indexes/role-name/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>());

            clientMock.Setup(m => m.GetAsync<IdentityRole>(It.Is<string>(value => value.StartsWith("roles/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityRole>
                {
                    Data = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString()
                    }
                });

            clientMock.Setup(m => m.PostAsync("roles", It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>
                {
                    Data = "test",
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
