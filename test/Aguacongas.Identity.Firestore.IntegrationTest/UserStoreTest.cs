using Aguacongas.Firebase.TokenManager;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    public class UserStoreTest : IdentitySpecificationTestBase<TestUser, TestRole>, IClassFixture<FirestoreTestFixture>
    {
        private readonly FirestoreTestFixture _fixture;

        public UserStoreTest(FirestoreTestFixture fixture)
        {
            _fixture = fixture;
        }

        protected override void AddUserStore(IServiceCollection services, object context = null)
        {
            services.Configure<OAuthServiceAccountKey>(options =>
            {
                _fixture.Configuration.GetSection("FirestoreAuthTokenOptions").Bind(options);
            }).AddScoped(provider =>
            {
                return _fixture.CreateFirestoreDb(provider);
            });

            var userType = typeof(TestUser);
            var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, typeof(TestRole));
            services.TryAddSingleton(typeof(UserOnlyStore<>).MakeGenericType(userType), provider => new UserOnlyStoreStub(_fixture.TestDb, provider.GetRequiredService<FirestoreDb>(), provider.GetService<IdentityErrorDescriber>()));
            services.TryAddSingleton(typeof(IUserStore<>).MakeGenericType(userType), provider => new UserStoreStub(_fixture.TestDb, provider.GetRequiredService<FirestoreDb>(), provider.GetRequiredService<UserOnlyStore<TestUser>>(), provider.GetService<IdentityErrorDescriber>()));
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            var roleType = typeof(TestRole);
            services.TryAddSingleton(typeof(IRoleStore<>).MakeGenericType(roleType), provider => new RoleStoreStub(_fixture.TestDb, provider.GetRequiredService<FirestoreDb>()));
        }

        protected override object CreateTestContext()
        {
            return null;
        }

        protected override TestUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "",
            bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = default(DateTimeOffset?), bool useNamePrefixAsUserName = false)
        {
            return new TestUser
            {
                UserName = useNamePrefixAsUserName ? namePrefix : string.Format("{0}{1}", namePrefix, Guid.NewGuid()),
                Email = email,
                PhoneNumber = phoneNumber,
                LockoutEnabled = lockoutEnabled,
                LockoutEnd = lockoutEnd
            };
        }

        protected override TestRole CreateTestRole(string roleNamePrefix = "", bool useRoleNamePrefixAsRoleName = false)
        {
            var roleName = useRoleNamePrefixAsRoleName ? roleNamePrefix : string.Format("{0}{1}", roleNamePrefix, Guid.NewGuid());
            return new TestRole(roleName);
        }

        protected override void SetUserPasswordHash(TestUser user, string hashedPassword)
        {
            user.PasswordHash = hashedPassword;
        }

        protected override Expression<Func<TestUser, bool>> UserNameEqualsPredicate(string userName) => u => u.UserName == userName;

        protected override Expression<Func<TestRole, bool>> RoleNameEqualsPredicate(string roleName) => r => r.Name == roleName;

        protected override Expression<Func<TestRole, bool>> RoleNameStartsWithPredicate(string roleName) => r => r.Name != null && r.Name.StartsWith(roleName);

        protected override Expression<Func<TestUser, bool>> UserNameStartsWithPredicate(string userName) => u => u.UserName != null &&  u.UserName.StartsWith(userName);
    }
}
