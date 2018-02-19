using Aguacongas.Firebase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Identity.Firebase
{
    public class UserIndex
    {
        [JsonProperty(PropertyName = "users")]
        public FirebaseIndexes Users { get; } = new FirebaseIndexes
        {
            On = new string[] { "NormalizedEmail", "NormalizedUserName" }
        };
    }
}
