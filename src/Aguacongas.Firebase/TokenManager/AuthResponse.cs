namespace Aguacongas.Firebase.TokenManager
{
    /// <summary>
    /// Authentitcation response
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Gets or sets the kind
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the local id
        /// </summary>
        public string LocalId { get; set; }

        /// <summary>
        /// Gets or sets the user email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the users display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the id token
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Gets or sets registered flag
        /// </summary>
        public bool Registered { get; set; }

        /// <summary>
        /// Gets or sets the refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expires in
        /// </summary>
        public int ExpiresIn { get; set; }
    }

}
