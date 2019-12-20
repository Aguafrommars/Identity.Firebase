// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
    internal class RoleStoreStub : RoleStore<TestRole>
    {
        private readonly string _testDb;

        public RoleStoreStub(string testDb, IFirebaseClient client, IdentityErrorDescriber describer = null) : base(client, describer)
        {
            _testDb = testDb;
        }

        protected override void SetIndex(Dictionary<string, object> rules, string key, object index)
        {
            var indexOverride = new Dictionary<string, object>();
            var jObject = rules.Where(kv => kv.Key == _testDb).Select(kv => kv.Value).FirstOrDefault() as JObject;
            if (jObject != null)
            {
                foreach(var o in jObject)
                {
                    indexOverride.Add(o.Key, o.Value);
                }
            }
            
            indexOverride[key] = index;
            base.SetIndex(rules, _testDb, indexOverride);
        }

        protected override string GetFirebasePath(params string[] objectPath)
        {
            return _testDb + "/" + base.GetFirebasePath(objectPath);
        }
    }
}
