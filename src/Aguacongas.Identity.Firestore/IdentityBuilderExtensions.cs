// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase.TokenManager;
using Aguacongas.Identity.Firestore;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
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
        /// <param name="builder">The <see cref="IdentityBuilder" /> instance this method extends.</param>
        /// <param name="configure">Action to configure AuthTokenOptions</param>
        /// <param name="authFilePath">The path where to store the authentication file extracted from config.</param>
        /// <param name="tableNames">Action to configure table names (Firestore Collections). If null, fallbacks to defaults <see cref="FirestoreTableNamesConfig.Defaults"/></param>
        /// <param name="emulatorHostAndPort">Firestore Emulator Host and Port. Connect to the emulator if a value passed, or production otherwise.</param>
        /// <returns>
        /// The <see cref="IdentityBuilder" /> instance this method extends.
        /// </returns>
        public static IdentityBuilder AddFirestoreStores(this IdentityBuilder builder, Action<OAuthServiceAccountKey> configure, string authFilePath, Action<FirestoreTableNamesConfig> tableNames = null, string emulatorHostAndPort = null)
        {
            UseEmulator(emulatorHostAndPort);

            var services = builder.Services;
            services.Configure(configure)
                .AddScoped(provider =>
                {
                    var authOptions = StoreAuthFile(provider, authFilePath);
                    return new FirestoreDbBuilder
                    {
                        ProjectId = authOptions.Value.project_id,
                        EmulatorDetection = Google.Api.Gax.EmulatorDetection.EmulatorOrProduction
                    }.Build();
                });

            AddStores(services, builder.UserType, builder.RoleType, ResolveFirestoreTableNamesConfig(tableNames));
            return builder;
        }

        /// <summary>
        /// Adds an Firebase implementation of identity stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <param name="configure">Action to configure AuthTokenOptions</param>
        /// <param name="tableNames">Action to configure table names (Firestore Collections). If null, fallbacks to defaults <see cref="FirestoreTableNamesConfig.Defaults"/></param>
        /// <param name="emulatorHostAndPort">Firestore Emulator Host and Port. Connect to the emulator if a value passed, or production otherwise.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddFirestoreStores(this IdentityBuilder builder, Action<OAuthServiceAccountKey> configure, Action<FirestoreTableNamesConfig> tableNames = null, string emulatorHostAndPort = null)
        {
            UseEmulator(emulatorHostAndPort);

            var services = builder.Services;
            services.Configure(configure)
                .AddScoped(provider =>
                {
                    var authOptions = StoreAuthFile(provider, Path.GetRandomFileName());
                    return new FirestoreDbBuilder
                    {
                        ProjectId = authOptions.Value.project_id,
                        EmulatorDetection = Google.Api.Gax.EmulatorDetection.EmulatorOrProduction
                    }.Build();
                });
            AddStores(services, builder.UserType, builder.RoleType, ResolveFirestoreTableNamesConfig(tableNames));
            return builder;
        }

        /// <summary>
        /// Adds an Firebase implementation of identity stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder" /> instance this method extends.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="tableNames">Action to configure table names (Firestore Collections). If null, fallbacks to defaults <see cref="FirestoreTableNamesConfig.Defaults"/></param>
        /// <param name="emulatorHostAndPort">Firestore Emulator Host and Port. Connect to the emulator if a value passed, or production otherwise.</param>
        /// <returns>
        /// The <see cref="IdentityBuilder" /> instance this method extends.
        /// </returns>
        public static IdentityBuilder AddFirestoreStores(this IdentityBuilder builder, string projectId, Action<FirestoreTableNamesConfig> tableNames = null, string emulatorHostAndPort = null)
        {
            UseEmulator(emulatorHostAndPort);

            var services = builder.Services;
            services
                .AddScoped(provider =>
                {
                    return new FirestoreDbBuilder
                    {
                        ProjectId = projectId,
                        EmulatorDetection = Google.Api.Gax.EmulatorDetection.EmulatorOrProduction
                    }.Build();
                });
            AddStores(services, builder.UserType, builder.RoleType, ResolveFirestoreTableNamesConfig(tableNames));
            return builder;
        }

        private static FirestoreTableNamesConfig ResolveFirestoreTableNamesConfig(Action<FirestoreTableNamesConfig> tableNames)
        {
            var tableNamesConfig = new FirestoreTableNamesConfig();
            tableNames?.Invoke(tableNamesConfig);
            return tableNamesConfig;
        }

        private static IOptions<OAuthServiceAccountKey> StoreAuthFile(IServiceProvider provider, string authFilePath)
        {
            var authOptions = provider.GetRequiredService<IOptions<OAuthServiceAccountKey>>();
            var json = JsonConvert.SerializeObject(authOptions.Value);
            using (var writer = File.CreateText(authFilePath))
            {
                writer.Write(json);
                writer.Flush();
                writer.Close();
            }
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", authFilePath);
            return authOptions;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, FirestoreTableNamesConfig tableNamesConfig)
        {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<string>));
            if (identityUserType == null)
            {
                throw new InvalidOperationException("AddEntityFrameworkStores can only be called with a user that derives from IdentityUser<string>.");
            }

            services.TryAddSingleton(tableNamesConfig);
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
                var genericType = type.IsGenericType ? type : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return typeInfo;
                }
                type = type.BaseType;
            }
            return null;
        }

        private static void UseEmulator(string emulatorHostAndPort)
        {
            if (!string.IsNullOrEmpty(emulatorHostAndPort))
                Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", emulatorHostAndPort);
        }
    }
}