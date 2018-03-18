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
    /// Extensions methods to configure an <see cref="IServiceCollection"/>
    ///     for <see cref="IFirebaseClient"/>.
    /// </summary>
    public static class ServiceCollectionExtentions
    {
        /// <summary>
        /// Adds <see cref="IFirebaseClient"/> and related services to the <see cref="IServiceCollection"/>
        ///     and configures the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="configure">An action to configure a FirebaseOptions</param>
        /// <param name="getTokenAccess">A fonction to create the ITokenAccess</param>
        /// <param name="httpClientName">The logical name of the underlying <see cref="HttpClient"/> to configure</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, Action<FirebaseOptions> configure, Func<IServiceProvider, ITokenAccess> getTokenAccess, string httpClientName = "firebase")
        {
            return services.AddHttpClientServices(configure, httpClientName)
                .AddTransient<IFirebaseTokenManager, OAuthTokenManager>()
                .AddSingleton(getTokenAccess);
        }

        /// <summary>
        /// Adds <see cref="IFirebaseClient"/> and related services to the <see cref="IServiceCollection"/>
        ///     and configures the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="url">The firebase url</param>
        /// <param name="getTokenAccess">A fonction to create the ITokenAccess</param>
        /// <param name="httpClientName">The logical name of the underlying <see cref="HttpClient"/> to configure</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, string url, Func<IServiceProvider, ITokenAccess> getTokenAccess, string httpClientName = "firebase")
        {
            return services.AddHttpClientServices(options =>
                {
                    options.DatabaseUrl = url;
                }, httpClientName)
                .AddTransient<IFirebaseTokenManager, OAuthTokenManager>()
                .AddSingleton(getTokenAccess);
        }

        /// <summary>
        /// Adds <see cref="IFirebaseClient"/> and related services to the <see cref="IServiceCollection"/>
        ///     and configures the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="url">The firebase url</param>
        /// <param name="getTokenManager">A fonction to create the IFirebaseTokenManager</param>
        /// <param name="httpClientName">The logical name of the underlying <see cref="HttpClient"/> to configure</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, string url, Func<IServiceProvider, IFirebaseTokenManager> getTokenManager, string httpClientName = "firebase")
        {
            return services.AddHttpClientServices(options =>
                {
                    options.DatabaseUrl = url;
                }, httpClientName)
                .AddSingleton(provider => getTokenManager(provider));
        }

        /// <summary>
        /// Adds the System.Net.Http.IFirebaseClient and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
        ///     and configures the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="configure">An action to configure a FirebaseOptions</param>
        /// <param name="getTokenManager">A fonction to create the IFirebaseTokenManager</param>
        /// <param name="httpClientName">The logical name of the underlying <see cref="HttpClient"/> to configure</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, Action<FirebaseOptions> configure, Func<IServiceProvider, IFirebaseTokenManager> getTokenManager, string httpClientName = "firebase")
        {
            return services.AddHttpClientServices(configure, httpClientName)
                .AddSingleton(provider => getTokenManager(provider));
        }

        /// <summary>
        /// Adds the System.Net.Http.IFirebaseClient and related services to the Microsoft.Extensions.DependencyInjection.IServiceCollection
        ///     and configures the IFirebaseClient type.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <param name="url">The firebase url</param>
        /// <param name="configure">An action to configure FirebaseOptions</param>
        /// <param name="httpClientName">The logical name of the underlying <see cref="HttpClient"/> to configure</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddHttpClientServices(this IServiceCollection services, Action<FirebaseOptions> configure, string httpClientName = "firebase")
        {
            services.Configure<FirebaseOptions>(httpClientName, options =>
                {
                    options.HttpClientName = httpClientName;
                    configure?.Invoke(options);
                })
                .AddTransient<FirebaseAuthenticationHandler>()
                .AddTransient<IFirebaseClient>(provider =>                 
                {
                    var factory = provider.GetRequiredService<IHttpClientFactory>();
                    var httpClient = factory.CreateClient(httpClientName);
                    var snapshot = provider.GetRequiredService<IOptionsSnapshot<FirebaseOptions>>();
                    var options = snapshot.Get(httpClientName);

                    return new FirebaseClient(httpClient, options);
                })
                .AddHttpClient(httpClientName)
                .AddHttpMessageHandler<FirebaseAuthenticationHandler>();

            return services;
        }
    }
}
