// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.Identity.Firestore.IntegrationTest
{
    internal class UserStoreStub: UserStore<TestUser, TestRole>
    {
        private readonly string _testDb;

        public UserStoreStub(string testDb, FirestoreDb db, UserOnlyStore<TestUser> userOnlyStore, FirestoreTableNamesConfig tableNamesConfig, IdentityErrorDescriber describer = null) : base(db, userOnlyStore, tableNamesConfig, describer)
        {
            _testDb = testDb;
        }
    }
}
