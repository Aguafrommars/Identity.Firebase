using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Firebase.TokenManager
{
    public class AuthResponse
    {
        public string Kind { get; set; }

        public string LocalId { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string IdToken { get; set; }

        public bool Registered { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }
    }

}
