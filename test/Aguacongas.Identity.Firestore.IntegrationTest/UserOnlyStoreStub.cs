using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    internal class UserOnlyStoreStub : UserOnlyStore<TestUser>
    {
        private readonly string _testDb;

        public UserOnlyStoreStub(string testDb, FirestoreDb db, IdentityErrorDescriber describer = null) : base(db, describer)
        {
            _testDb = testDb;
        }
    }
}
