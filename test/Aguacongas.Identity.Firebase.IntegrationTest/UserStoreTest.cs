using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
    public class UserStoreTest : IdentitySpecificationTestBase<TestUser, TestRole>, IClassFixture<FirebaseTestFixture>
    {
        private readonly FirebaseTestFixture _fixture;

        public UserStoreTest(FirebaseTestFixture fixture)
        {
            _fixture = fixture;
        }

        protected override void AddUserStore(IServiceCollection services, object context = null)
        {
            services.Configure<AuthTokenOptions>(options =>
            {
                _fixture.Configuration.GetSection("AuthTokenOptions").Bind(options);
            });

            services.AddFirebaseClient(_fixture.Configuration["FirebaseOptions:DatabaseUrl"], provider =>
            {
                var options = provider.GetRequiredService<IOptions<AuthTokenOptions>>();
                var json = JsonConvert.SerializeObject(options?.Value ?? throw new ArgumentNullException(nameof(options)));
                return GoogleCredential.FromJson(json)
                    .CreateScoped("https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/firebase.database")
                    .UnderlyingCredential;
            });

            var userType = typeof(TestUser);
            var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, typeof(TestRole));
            services.TryAddSingleton(typeof(UserOnlyStore<>).MakeGenericType(userType), provider => new UserOnlyStoreStub(_fixture.TestDb, provider.GetRequiredService<IFirebaseClient>(), provider.GetService<IdentityErrorDescriber>()));
            services.TryAddSingleton(typeof(IUserStore<>).MakeGenericType(userType), provider => new UserStoreStub(_fixture.TestDb, provider.GetRequiredService<IFirebaseClient>(), provider.GetRequiredService<UserOnlyStore<TestUser>>(), provider.GetService<IdentityErrorDescriber>()));
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            var roleType = typeof(TestRole);
            services.TryAddSingleton(typeof(IRoleStore<>).MakeGenericType(roleType), provider => new RoleStoreStub(_fixture.TestDb, provider.GetRequiredService<IFirebaseClient>()));
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

        [Fact]
        [Trait("firebase", "firebase")]
        public async Task DeserializeDictionary()
        {
            var services = new ServiceCollection();
            SetupIdentityServices(services, null);

            var client = services.BuildServiceProvider().GetRequiredService<IFirebaseClient>();
            await client.PostAsync(_fixture.TestDb + "/users", new TestUser
            {
                NormalizedEmail = "1"
            });

            await client.PostAsync(_fixture.TestDb + "/users", new TestUser
            {
                NormalizedEmail = "2"
            });

            var rules = await client.GetAsync<FirebaseRules>(".settings/rules.json");

            rules.Data.Rules[_fixture.TestDb] = new Dictionary<string, object>(){ { "users", new UserIndex() } };
            await client.PutAsync(".settings/rules.json", rules.Data);

            var users = await client.GetAsync<Dictionary<string, TestUser>>(_fixture.TestDb + "/users", queryString: "orderBy=\"NormalizedEmail\"&equalTo=\"2\"");
        }
    }
}
