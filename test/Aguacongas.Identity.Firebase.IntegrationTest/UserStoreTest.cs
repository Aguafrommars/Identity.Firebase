using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
    public class UserStoreTest : IdentitySpecificationTestBase<IdentityUser, IdentityRole>
    {
        protected override void AddUserStore(IServiceCollection services, object context = null)
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.AddUserSecrets<UserStoreTest>()
                .AddEnvironmentVariables()
                .Build();

            services.Configure<EmailPasswordOptions>(options =>
            {
                configuration.GetSection("EmailPasswordOptions").Bind(options);
            });
            services.Configure<FirebaseOptions>(options =>
            {
                configuration.GetSection("FirebaseOptions").Bind(options);
            });

            var userType = typeof(IdentityUser);
            var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, typeof(IdentityRole));
            services.TryAddSingleton(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);

            services.AddSingleton<HttpClient>()
                .AddSingleton<IFirebaseClient, FirebaseClient>()
                .AddSingleton<IFirebaseTokenManager, EmailPasswordTokenManager>()
                .AddSingleton<ILookupNormalizer, FirebaseLookupNormalizer>();
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            var roleType = typeof(IdentityRole);
            var roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);
            services.TryAddSingleton(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
        }

        protected override object CreateTestContext()
        {
            return null;
        }

        protected override IdentityUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "",
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

        protected override IdentityRole CreateTestRole(string roleNamePrefix = "", bool useRoleNamePrefixAsRoleName = false)
        {
            var roleName = useRoleNamePrefixAsRoleName ? roleNamePrefix : string.Format("{0}{1}", roleNamePrefix, Guid.NewGuid());
            return new IdentityRole(roleName);
        }

        protected override void SetUserPasswordHash(IdentityUser user, string hashedPassword)
        {
            user.PasswordHash = hashedPassword;
        }

        protected override Expression<Func<IdentityUser, bool>> UserNameEqualsPredicate(string userName) => u => u.UserName == userName;

        protected override Expression<Func<IdentityRole, bool>> RoleNameEqualsPredicate(string roleName) => r => r.Name == roleName;

        protected override Expression<Func<IdentityRole, bool>> RoleNameStartsWithPredicate(string roleName) => r => r.Name.StartsWith(roleName);

        protected override Expression<Func<IdentityUser, bool>> UserNameStartsWithPredicate(string userName) => u => u.UserName.StartsWith(userName);
    }
}
