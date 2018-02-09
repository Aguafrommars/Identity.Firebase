using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Firebase.TokenManager
{
    public class AuthRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool ReturnSecureToken { get; set; } = true;
    }
}
