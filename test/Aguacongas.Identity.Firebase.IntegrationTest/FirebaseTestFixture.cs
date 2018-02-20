using Aguacongas.Firebase;
using Aguacongas.Firebase.TokenManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
    public class FirebaseTestFixture
    {
        public IConfigurationRoot Configuration { get; private set; }
        public FirebaseOptions FirebaseOptions { get; private set; }

        public string TestDb { get; } = DateTime.Now.ToString("s");
        public FirebaseTestFixture()
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder.AddUserSecrets<UserStoreTest>()
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\testsettings.json"))
                .Build();

            FirebaseOptions = new FirebaseOptions();
            Configuration.GetSection("FirebaseOptions").Bind(FirebaseOptions);
        }
    }
}
