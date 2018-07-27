using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    internal class RoleStoreStub : RoleStore<TestRole>
    {
        private readonly string _testDb;

        public RoleStoreStub(string testDb, FirestoreDb db, IdentityErrorDescriber describer = null) : base(db, describer)
        {
            _testDb = testDb;
        }
    }
}
