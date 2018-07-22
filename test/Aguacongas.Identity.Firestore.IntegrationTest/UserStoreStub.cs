using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    internal class UserStoreStub: UserStore<TestUser, TestRole>
    {
        private readonly string _testDb;

        public UserStoreStub(string testDb, FirestoreDb db, UserOnlyStore<TestUser> userOnlyStore, IdentityErrorDescriber describer = null) : base(db, userOnlyStore, describer)
        {
            _testDb = testDb;
        }
    }
}
