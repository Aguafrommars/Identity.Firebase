using System;
using System.Collections.Generic;
using System.Text;
using Aguacongas.Firebase;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
    internal class UserOnlyStoreStub : UserOnlyStore<TestUser>
    {
        private readonly string _testDb;

        public UserOnlyStoreStub(string testDb, IFirebaseClient client, IdentityErrorDescriber describer = null) : base(client, describer)
        {
            _testDb = testDb;
        }

        protected override void AddUserIndexes(Dictionary<string, object> rules)
        {
            var subSet = new Dictionary<string, object>();
            base.AddUserIndexes(subSet);
            rules.Add(_testDb, subSet);
        }

        protected override string GetFirebasePath(params string[] objectPath)
        {
            return _testDb + "/" + base.GetFirebasePath(objectPath);
        }
    }
}
