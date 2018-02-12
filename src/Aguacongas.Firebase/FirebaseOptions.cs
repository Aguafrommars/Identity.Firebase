using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Firebase
{
    public class FirebaseOptions
    {
        public string DatabaseUrl { get; set; }

        public JsonSerializerSettings JsonSerializerSettings { get; set; }
    }
}
