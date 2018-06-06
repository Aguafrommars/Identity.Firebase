namespace Aguacongas.Firebase.TokenManager
{
    /// <summary>
    /// Eamil password authentication options
    /// </summary>
    public class EmailPasswordOptions
    {
        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the api key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the sign up url
        /// </summary>
        public string SignUpUrl { get; set; } = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword";
    }
}
