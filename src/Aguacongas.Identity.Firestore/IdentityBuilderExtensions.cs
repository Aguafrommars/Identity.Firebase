using Aguacongas.Firebase.TokenManager;
using Aguacongas.Identity.Firestore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1Beta1;
using Grpc.Auth;
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
        /// <param name="configure">Action to configure AuthTokenOptions</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddFirestoreStores(this IdentityBuilder builder, Action<OAuthServiceAccountKey> configure)
        {
            var services = builder.Services;
            services.Configure(configure)
                .AddTransient(provider =>
                {
                    var authOptions = provider.GetRequiredService<IOptions<OAuthServiceAccountKey>>();
                    var json = JsonConvert.SerializeObject(authOptions.Value);
                    var credentials = GoogleCredential.FromJson(json)
                        .CreateScoped("https://www.googleapis.com/auth/datastore");
                    var channel = new Grpc.Core.Channel(
                        FirestoreClient.DefaultEndpoint.ToString(),
                        credentials.ToChannelCredentials());
                    var client = FirestoreClient.Create(channel);
                    return FirestoreDb.Create(authOptions.Value.project_id, client: client);
                });
            AddStores(services, builder.UserType, builder.RoleType);
            return builder;
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