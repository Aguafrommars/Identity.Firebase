using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Firebase
{
    public class FirebaseRules
    {
        [JsonProperty(PropertyName = "rules")]
        public Dictionary<string, object> Rules { get; set; }
    }
}
