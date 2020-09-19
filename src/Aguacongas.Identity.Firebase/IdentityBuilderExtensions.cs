// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Aguacongas.Identity.Firebase;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding entity framework stores.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Adds an Firebase implementation of identity stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <param name="url">Firebase url</param>
        /// <param name="getTokenAccess"><see cref="ITokenAccess"/> factory function</param>
        /// <param name="httpClientName">The name of HttpClient</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddFirebaseStores(this IdentityBuilder builder, string url, Func<IServiceProvider, ITokenAccess> getTokenAccess, string httpClientName = "Aguacongas.Identity.Firebase")
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType);
            builder.Services.AddFirebaseClient(url, getTokenAccess, httpClientName);

            return builder;
        }

        /// <summary>
        /// Adds an Firebase implementation of identity stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <param name="url">Firebase url</param>
        /// <param name="getTokenManager"><see cref="IFirebaseTokenManager"/> factory function</param>
        /// <param name="httpClientName">The name of HttpClient</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddFirebaseStores(this IdentityBuilder builder, string url, Func<IServiceProvider, IFirebaseTokenManager> getTokenManager, string httpClientName = "Aguacongas.Identity.Firebase")
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType);
            builder.Services.AddFirebaseClient(url, getTokenManager, httpClientName);

            return builder;
        }

        /// <summary>
        /// Adds an Firebase implementation of identity stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <param name="url">Firebase url</param>
        /// <param name="configure">Action to configure AuthTokenOptions</param>
        /// <param name="httpClientName">The name of HttpClient</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddFirebaseStores(this IdentityBuilder builder, string url, Action<OAuthServiceAccountKey> configure, string httpClientName = "Aguacongas.Identity.Firebase")
        {
            builder.Services.Configure(configure);

            return builder
                .AddFirebaseStores(url, provider =>
                {
                    var options = provider.GetRequiredService<IOptions<OAuthServiceAccountKey>>();
                    var json = JsonConvert.SerializeObject(options?.Value ?? throw new NotSupportedException($"{nameof(options)} is null"));
                    return GoogleCredential.FromJson(json)
                        .CreateScoped("https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/firebase.database")
                        .UnderlyingCredential;
                }, httpClientName);
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType)
        {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<string>));
            if (identityUserType == null)
            {
                throw new InvalidOperationException("AddEntityFrameworkStores can only be called with a user that derives from IdentityUser<string>.");
            }

            var userOnlyStoreType = typeof(UserOnlyStore<>).MakeGenericType(userType);

            if (roleType != null)
            {
                var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<string>));
                if (identityRoleType == null)
                {
                    throw new InvalidOperationException("AddEntityFrameworkStores can only be called with a role that derives from IdentityRole<string>.");
                }

                var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, roleType);
                var roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);

                services.TryAddScoped(typeof(UserOnlyStore<>).MakeGenericType(userType), userOnlyStoreType);
                services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
                services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
            }
            else
            {   // No Roles
                services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userOnlyStoreType);
            }
        }

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType;
            while (type != null)
            {
                var typeInfo = type.GetTypeInfo();
                var genericType = type.IsGenericType ? type: null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return typeInfo;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}