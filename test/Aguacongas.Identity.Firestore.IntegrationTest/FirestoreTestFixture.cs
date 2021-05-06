using Aguacongas.Firebase.TokenManager;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
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
            
            var tableNames = new FirestoreTableNamesConfig().WithSuffix("-tests");
            services.AddSingleton(tableNames);
            
            var p = services.BuildServiceProvider();
            var db = p.GetRequiredService<FirestoreDb>();
            
            Clean(db.Collection(tableNames.UsersTableName));
            Clean(db.Collection(tableNames.UserLoginsTableName));
            Clean(db.Collection(tableNames.UserClaimsTableName));
            Clean(db.Collection(tableNames.UserTokensTableName));
            Clean(db.Collection(tableNames.RolesTableName));
            Clean(db.Collection(tableNames.RoleClaimsTableName));
        }

        private static void Clean(CollectionReference collection)
        {
            var snapShot = collection.GetSnapshotAsync().GetAwaiter().GetResult();
            foreach (var doc in snapShot.Documents)
            {
                doc.Database.Collection(collection.Id).Document(doc.Id).DeleteAsync().GetAwaiter().GetResult();
            }
        }

        public static FirestoreDb CreateFirestoreDb(IServiceProvider provider)
        {
            var authOptions = provider.GetRequiredService<IOptions<OAuthServiceAccountKey>>();

            var path = Path.GetTempFileName();

            var json = JsonConvert.SerializeObject(authOptions.Value);
            using var writer = File.CreateText(path);
            writer.Write(json);
            writer.Flush();
            writer.Close();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            var client = FirestoreClient.Create();
            return FirestoreDb.Create(authOptions.Value.project_id, client: client);
        }
    }
}
