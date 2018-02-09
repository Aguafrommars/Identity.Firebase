using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Firebase.TokenManager
{
    public class EmailPasswordOptions
    {
        public string Email { get; set; }

        public string Password { get; set; }
        public string ApiKey { get; set; }

        public string SignUpUrl { get; set; } = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword";
    }
}
