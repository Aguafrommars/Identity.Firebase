// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    internal class UserOnlyStoreStub : UserOnlyStore<TestUser>
    {
        private readonly string _testDb;

        public UserOnlyStoreStub(string testDb, FirestoreDb db, FirestoreTableNamesConfig tableNamesConfig, IdentityErrorDescriber describer = null) : base(db, tableNamesConfig, describer)
        {
            _testDb = testDb;
        }
    }
}
