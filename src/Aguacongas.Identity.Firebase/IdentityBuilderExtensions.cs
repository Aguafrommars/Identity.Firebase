// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Reflection;
using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Aguacongas.Identity.Firebase;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding entity framework stores.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Adds an Firebase implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddFirebaseStores(this IdentityBuilder builder)
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType);
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType)
        {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<string>));
            if (identityUserType == null)
            {
                throw new InvalidOperationException("AddEntityFrameworkStores can only be called with a user that derives from IdentityUser<string>.");
            }

            if (roleType != null)
            {
                var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<string>));
                if (identityRoleType == null)
                {
                    throw new InvalidOperationException("AddEntityFrameworkStores can only be called with a role that derives from IdentityRole<string>.");
                }

                Type userStoreType = null;
                Type roleStoreType = null;
                // If its a custom DbContext, we can only add the default POCOs
                userStoreType = typeof(UserStore<,>).MakeGenericType(userType, roleType);
                roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);

                services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
                services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(userType), roleStoreType);
            }
            else
            {   // No Roles
                Type userStoreType = null;
                    // If its a custom DbContext, we can only add the default POCOs
                userStoreType = typeof(UserOnlyStore<>).MakeGenericType(userType);
                services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            }

            services.AddScoped<HttpClient>()
                .AddScoped<IFirebaseClient, FirebaseClient>()
                .AddScoped<IFirebaseTokenManager, EmailPasswordTokenManager>();
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