using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Firebase
{
    public class FirebaseIndexes
    {
        [JsonProperty(PropertyName = ".indexOn")]
        public string[] On { get; set; }
    }

    public class FirebaseIndex
    {
        [JsonProperty(PropertyName = ".indexOn")]
        public string On { get; set; }
    }
}
