using Aguacongas.Firebase;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
    internal class UserStoreStub: UserStore<TestUser, TestRole>
    {
        private readonly string _testDb;

        public UserStoreStub(string testDb, IFirebaseClient client, UserOnlyStore<TestUser> userOnlyStore, IdentityErrorDescriber describer = null) : base(client, userOnlyStore, describer)
        {
            _testDb = testDb;
        }

        protected override string GetFirebasePath(params string[] objectPath)
        {
            return _testDb + "/" + base.GetFirebasePath(objectPath);
        }
    }
}
