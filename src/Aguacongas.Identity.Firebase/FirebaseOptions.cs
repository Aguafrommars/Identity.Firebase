using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Identity.Firebase
{
    public class FirebaseOptions
    {
        public string DatabaseUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ApiKey { get; set; }

        public string SignUpUrl { get; set; } = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword";

        public JsonSerializerSettings JsonSerializerSettings { get; set; }
    }
}
