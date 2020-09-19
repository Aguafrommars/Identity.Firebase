// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
namespace Aguacongas.Firebase.TokenManager
{
    /// <summary>
    /// Email/Password authentication request
    /// </summary>
    public class EmailPasswordAuthRequest
    {
        /// <summary>
        /// Gets or sets Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a flag to ask for a secure token
        /// </summary>
        public bool ReturnSecureToken { get; set; } = true;
    }
}
