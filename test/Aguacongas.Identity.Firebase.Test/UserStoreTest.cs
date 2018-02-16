using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Microsoft.AspNetCore.Identity;
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

        [Fact]
        public async Task FindByIdTest()
        {
            var user = CreateTestUser();
            var manager = await LazyLoadTestSetup(user);

            var userById = await manager.FindByIdAsync(user.Id.ToString());
            Assert.Equal(2, (await manager.GetClaimsAsync(userById)).Count);
            Assert.Equal(1, (await manager.GetLoginsAsync(userById)).Count);
            Assert.Equal(2, (await manager.GetRolesAsync(userById)).Count);
        }

        [Fact]
        public async Task FindByNameTest()
        {
            var user = CreateTestUser();
            var manager = await LazyLoadTestSetup(user);

            var userByName = await manager.FindByNameAsync(user.UserName);
            Assert.Equal(2, (await manager.GetClaimsAsync(userByName)).Count);
            Assert.Equal(1, (await manager.GetLoginsAsync(userByName)).Count);
            Assert.Equal(2, (await manager.GetRolesAsync(userByName)).Count);
        }

        [Fact]
        public async Task FindByLoginTest()
        {
            var user = CreateTestUser();
            var manager = await LazyLoadTestSetup(user);

            var userByLogin = await manager.FindByLoginAsync("provider", user.Id.ToString());
            Assert.Equal(2, (await manager.GetClaimsAsync(userByLogin)).Count);
            Assert.Equal(1, (await manager.GetLoginsAsync(userByLogin)).Count);
            Assert.Equal(2, (await manager.GetRolesAsync(userByLogin)).Count);
        }

        [Fact]
        public async Task FindByEmailTest()
        {
            var user = CreateTestUser();
            var manager = await LazyLoadTestSetup(user);

            var userByEmail = await manager.FindByEmailAsync(user.Email);
            Assert.Equal(2, (await manager.GetClaimsAsync(userByEmail)).Count);
            Assert.Equal(1, (await manager.GetLoginsAsync(userByEmail)).Count);
            Assert.Equal(2, (await manager.GetRolesAsync(userByEmail)).Count);
        }

        private async Task<UserManager<IdentityUser>> LazyLoadTestSetup(IdentityUser user)
        {
            var manager = CreateManager(out Mock<IFirebaseClient> clientMock);
            var role = CreateRoleManager(out Mock<IFirebaseClient> clientMockRole);
            var admin = CreateTestRole("Admin" + Guid.NewGuid().ToString());
            var local = CreateTestRole("Local" + Guid.NewGuid().ToString());
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value.StartsWith("indexes/provider-keys/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>());

            clientMock.Setup(m => m.GetAsync<List<IdentityUserLogin<string>>>(It.Is<string>(value => value== $"users/{user.Id}/logins"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<List<IdentityUserLogin<string>>>());

            List<IdentityUserLogin<string>> logins = new List<IdentityUserLogin<string>>();
            clientMock.Setup(m => m.PutAsync(It.Is<string>(value => value == $"users/{user.Id}/logins"),
               It.IsAny<List<IdentityUserLogin<string>>>(),
               It.IsAny<CancellationToken>(),
               It.IsAny<bool>(),
               It.IsAny<string>()))
               .Callback<string, List<IdentityUserLogin<string>>, CancellationToken, bool, string>((url, data, ct, r, e) => logins.AddRange(data))
               .ReturnsAsync(new FirebaseResponse<List<IdentityUserLogin<string>>>());

            clientMock.Setup(m => m.PutAsync(It.Is<string>(value => value.StartsWith($"indexes/provider-keys")),
              It.IsAny<string>(),
              It.IsAny<CancellationToken>(),
              It.IsAny<bool>(),
              It.IsAny<string>()))
              .ReturnsAsync(new FirebaseResponse<string>());

            clientMock.Setup(m => m.PutAsync(It.Is<string>(value => value == $"users/{user.Id}"),
              It.IsAny<IdentityUser>(),
              It.IsAny<CancellationToken>(),
              It.IsAny<bool>(),
              It.IsAny<string>()))
              .ReturnsAsync(new FirebaseResponse<IdentityUser>
              {
                  Data = user,
                  Etag = "test"
              });

            IdentityResultAssert.IsSuccess(await manager.AddLoginAsync(user, new UserLoginInfo("provider", user.Id.ToString(), "display")));
            IdentityResultAssert.IsSuccess(await role.CreateAsync(admin));
            IdentityResultAssert.IsSuccess(await role.CreateAsync(local));

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value == $"indexes/role-name/{admin.NormalizedName}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>
                {
                    Data = admin.Id
                });

            clientMock.Setup(m => m.GetAsync<IdentityRole>(It.Is<string>(value => value == $"roles/{admin.Id}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityRole>
                {
                    Data = admin
                });

            clientMock.Setup(m => m.GetAsync<IdentityUserRole<string>>(It.Is<string>(value => value == $"users/{user.Id}/roles/{admin.Id}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUserRole<string>>());

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value == $"indexes/role-name/{local.NormalizedName}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>
                {
                    Data = local.Id
                });

            clientMock.Setup(m => m.GetAsync<IdentityRole>(It.Is<string>(value => value == $"roles/{local.Id}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityRole>
                {
                    Data = local
                });

            clientMock.Setup(m => m.GetAsync<IdentityUserRole<string>>(It.Is<string>(value => value == $"users/{user.Id}/roles/{local.Id}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUserRole<string>>());

            List<IdentityUserRole<string>> roles = new List<IdentityUserRole<string>>();
            clientMock.Setup(m => m.PutAsync(It.Is<string>(value => value.StartsWith($"users/{user.Id}/roles/")),
               It.IsAny<IdentityUserRole<string>>(),
               It.IsAny<CancellationToken>(),
               It.IsAny<bool>(),
               It.IsAny<string>()))
               .Callback<string, IdentityUserRole<string>, CancellationToken, bool, string>((url, data, ct, r, e) => roles.Add(data))
               .ReturnsAsync(new FirebaseResponse<IdentityUserRole<string>>());

            IdentityResultAssert.IsSuccess(await manager.AddToRoleAsync(user, admin.Name));
            IdentityResultAssert.IsSuccess(await manager.AddToRoleAsync(user, local.Name));

            clientMock.Setup(m => m.GetAsync<IEnumerable<IdentityUserClaim<string>>>(It.Is<string>(value => value == $"claims/{user.Id}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IEnumerable<IdentityUserClaim<string>>>());

            List<IdentityUserClaim<string>> claims = new List<IdentityUserClaim<string>>();
            clientMock.Setup(m => m.PutAsync(It.Is<string>(value => value == $"claims/{user.Id}"),
              It.IsAny<List<IdentityUserClaim<string>>>(),
              It.IsAny<CancellationToken>(),
              It.IsAny<bool>(),
              It.IsAny<string>()))
              .Callback<string, List<IdentityUserClaim<string>>, CancellationToken, bool , string>((url, data, ct, r, e) => claims.AddRange(data))
              .ReturnsAsync(new FirebaseResponse<List<IdentityUserClaim<string>>>
              {
                  Etag = "test"
              });

            Claim[] userClaims =
            {
                new Claim("Whatever", "Value"),
                new Claim("Whatever2", "Value2")
            };
            foreach (var c in userClaims)
            {
                IdentityResultAssert.IsSuccess(await manager.AddClaimAsync(user, c));
            }

            clientMock.Setup(m => m.GetAsync<IdentityUser>(It.Is<string>(value => value == $"users/{user.Id}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IdentityUser>
                {
                    Data = user,
                    Etag = "test"
                });

            clientMock.Setup(m => m.GetAsync<IEnumerable<IdentityUserClaim<string>>>(It.Is<string>(value => value == $"claims/{user.Id}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IEnumerable<IdentityUserClaim<string>>>
                {
                    Data = claims
                });

            clientMock.Setup(m => m.GetAsync<List<IdentityUserLogin<string>>>(It.Is<string>(value => value == $"users/{user.Id}/logins"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<List<IdentityUserLogin<string>>>
                {
                    Data = logins
                });

            clientMock.Setup(m => m.GetAsync<IEnumerable<IdentityUserRole<string>>>(It.Is<string>(value => value == $"users/{user.Id}/roles"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IEnumerable<IdentityUserRole<string>>>
                {
                    Data = roles
                });

            clientMock.Setup(m => m.GetAsync<IEnumerable<IdentityRole>>(It.Is<string>(value => value == "roles"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IEnumerable<IdentityRole>>
                {
                    Data = new List<IdentityRole> { admin, local }
                });

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value == $"indexes/users-names/{user.NormalizedUserName}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>
                {
                    Data = user.Id
                });

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value == $"indexes/users-email/{user.NormalizedEmail}"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>
                {
                    Data = user.Id
                });

            clientMock.Setup(m => m.GetAsync<string>(It.Is<string>(value => value.StartsWith("indexes/provider-keys/")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<string>
                {
                    Data = user.Id
                });

            clientMock.Setup(m => m.GetAsync<IEnumerable<IdentityUserLogin<string>>>(It.Is<string>(value => value == $"users/{user.Id}/logins"), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new FirebaseResponse<IEnumerable<IdentityUserLogin<string>>>
                {
                    Data = logins
                });
            return manager;
        }

        private IdentityRole CreateTestRole(string roleNamePrefix = "", bool useRoleNamePrefixAsRoleName = false)
        {
            var roleName = useRoleNamePrefixAsRoleName ? roleNamePrefix : string.Format("{0}{1}", roleNamePrefix, Guid.NewGuid());
            return new IdentityRole() { Name = roleName };
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
