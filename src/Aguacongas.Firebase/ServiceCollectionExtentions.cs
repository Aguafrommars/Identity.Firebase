using Aguacongas.Firebase;
using Aguacongas.Firebase.Http;
using Aguacongas.Firebase.TokenManager;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions methods to configure an Microsoft.Extensions.DependencyInjection.IServiceCollection
    ///     for Aguacongas.Firebase.IFirebaseClient.
    /// </summary>
    public static class ServiceCollectionExtentions
    {
        /// <summary>
        /// Adds the System.Net.Http.IFirebaseClient and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
        ///     and configures a binding between the IFirebaseClient
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="configure">An action to configure a FirebaseOptions</param>
        /// <param name="getTokenAccess">A fonction to create the ITokenAccess</param>
        /// <param name="databaseName">The logical name of the System.Net.Http.HttpClient to configure</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IHttpClientBuilder that can be used
        ///     to configure the client.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, Action<FirebaseOptions> configure, Func<IServiceProvider, ITokenAccess> getTokenAccess, string databaseName = "firebase")
        {
            return services.AddHttpMiddleware(configure, databaseName)
                .AddTransient<IFirebaseTokenManager, AuthTokenManager>()
                .AddSingleton(getTokenAccess);
        }

        /// <summary>
        /// Adds the System.Net.Http.IFirebaseClient and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
        ///     and configures a binding between the IFirebaseClient
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="url">The firebase url</param>
        /// <param name="getTokenAccess">A fonction to create the ITokenAccess</param>
        /// <param name="databaseName">The logical name of the System.Net.Http.HttpClient to configure</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IHttpClientBuilder that can be used
        ///     to configure the client.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, string url, Func<IServiceProvider, ITokenAccess> getTokenAccess, string databaseName = "firebase")
        {
            return services.AddHttpMiddleware(options =>
                {
                    options.DatabaseUrl = url;
                }, databaseName)
                .AddTransient<IFirebaseTokenManager, AuthTokenManager>()
                .AddSingleton(getTokenAccess);
        }

        /// <summary>
        /// Adds the System.Net.Http.IFirebaseClient and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
        ///     and configures a binding between the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="url">The firebase url</param>
        /// <param name="getTokenManager">A fonction to create the IFirebaseTokenManager</param>
        /// <param name="databaseName">The logical name of the System.Net.Http.HttpClient to configure</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IHttpClientBuilder that can be used
        ///     to configure the client.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, string url, Func<IServiceProvider, IFirebaseTokenManager> getTokenManager, string databaseName = "firebase")
        {
            return services.AddHttpMiddleware(options =>
                {
                    options.DatabaseUrl = url;
                }, databaseName)
                .AddSingleton(provider => getTokenManager(provider));
        }

        /// <summary>
        /// Adds the System.Net.Http.IFirebaseClient and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
        ///     and configures a binding between the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="configure">An action to configure a FirebaseOptions</param>
        /// <param name="getTokenManager">A fonction to create the IFirebaseTokenManager</param>
        /// <param name="databaseName">The logical name of the System.Net.Http.HttpClient to configure</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IHttpClientBuilder that can be used
        ///     to configure the client.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, Action<FirebaseOptions> configure, Func<IServiceProvider, IFirebaseTokenManager> getTokenManager, string databaseName = "firebase")
        {
            return services.AddHttpMiddleware(configure, databaseName)
                .AddSingleton(provider => getTokenManager(provider));
        }

        /// <summary>
        /// Adds the System.Net.Http.IFirebaseClient and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
        ///     and configures a binding between the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="url">The firebase url</param>
        /// <param name="configure">An action to configure FirebaseOptions</param>
        /// <param name="databaseName">The logical name of the System.Net.Http.HttpClient to configure</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IHttpClientBuilder that can be used
        ///     to configure the client.</returns>
        private static IServiceCollection AddHttpMiddleware(this IServiceCollection services, Action<FirebaseOptions> configure, string databaseName = "firebase")
        {
            services.Configure<FirebaseOptions>(databaseName, options =>
                {
                    options.DatabaseName = databaseName;
                    configure?.Invoke(options);
                })
                .AddTransient<FirebaseAuthenticationHandler>()
                .AddScoped<IFirebaseClient>(provider =>
                    new FirebaseClient(provider.GetRequiredService<IHttpClientFactory>(),
                        provider.GetRequiredService<IOptionsSnapshot<FirebaseOptions>>().Get(databaseName)))
                .AddHttpClient<FirebaseClient>(databaseName)
                .AddHttpMessageHandler<FirebaseAuthenticationHandler>();

            return services;
        }
    }
}
