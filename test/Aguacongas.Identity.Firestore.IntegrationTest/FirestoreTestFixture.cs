using Aguacongas.Identity.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    public class FirestoreTestFixture
    {
        public IConfigurationRoot Configuration { get; private set; }
        public FirestoreOptions FirestoreOptions { get; private set; }

        public string TestDb { get; } = DateTime.Now.ToString("s");
        public FirestoreTestFixture()
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../identityfirestore.json"))
                .Build();

            FirestoreOptions = new FirestoreOptions();
            Configuration.GetSection("FirestoreOptions").Bind(FirestoreOptions);
        }
    }
}
