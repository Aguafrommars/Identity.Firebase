using Aguacongas.Firebase.TokenManager;
using Aguacongas.Identity.Firestore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1Beta1;
using Grpc.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    public class FirestoreTestFixture
    {
        public IConfigurationRoot Configuration { get; private set; }

        public string TestDb { get; } = DateTime.Now.ToString("s");
        public FirestoreTestFixture()
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../identityfirestore.json"))
                .Build();

            var services = new ServiceCollection();
            services.Configure<OAuthServiceAccountKey>(options =>
            {
                Configuration.GetSection("FirestoreAuthTokenOptions").Bind(options);
            }).AddScoped(provider =>
            {
                return CreateFirestoreDb(provider);
            });
            var p = services.BuildServiceProvider();
            var db = p.GetRequiredService<FirestoreDb>();
            Clean(db.Collection("users"));
            Clean(db.Collection("user-logins"));
            Clean(db.Collection("user-claims"));
            Clean(db.Collection("user-tokens"));
            Clean(db.Collection("roles"));
            Clean(db.Collection("role-claims"));
        }

        private static void Clean(CollectionReference collection)
        {
            var snapShot = collection.GetSnapshotAsync().GetAwaiter().GetResult();
            foreach (var doc in snapShot.Documents)
            {
                doc.Database.Collection(collection.Id).Document(doc.Id).DeleteAsync().GetAwaiter().GetResult();
            }
        }

        public FirestoreDb CreateFirestoreDb(IServiceProvider provider)
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
        }
    }
}
