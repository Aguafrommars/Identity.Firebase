namespace Aguacongas.Firebase.TokenManager
{
    /// <summary>
    /// OAuth service account key mapping
    /// </summary>
    public class OAuthServiceAccountKey
    {
        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public string type { get; set; } = "service_account";
        /// <summary>
        /// Gets or sets the project_id
        /// </summary>
        public string project_id { get; set; }
        /// <summary>
        /// Gets or sets the private_key_id
        /// </summary>
        public string private_key_id { get; set; }
        /// <summary>
        /// Gets or sets the private_key
        /// </summary>
        public string private_key { get; set; }
        /// <summary>
        /// Gets or sets the client_email
        /// </summary>
        public string client_email { get; set; }
        /// <summary>
        /// Gets or sets the client_id
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// Gets or sets the auth_uri
        /// </summary>
        public string auth_uri { get; set; } = "https://accounts.google.com/o/oauth2/auth";
        /// <summary>
        /// Gets or sets the token_uri
        /// </summary>
        public string token_uri { get; set; } = "https://accounts.google.com/o/oauth2/token";
        /// <summary>
        /// Gets or sets the auth_provider_x509_cert_url
        /// </summary>
        public string auth_provider_x509_cert_url { get; set; } = "https://www.googleapis.com/oauth2/v1/certs";
        /// <summary>
        /// Gets or sets the client_x509_cert_url
        /// </summary>
        public string client_x509_cert_url { get; set; } = "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-bmyi7%40identityfirebase.iam.gserviceaccount.com";
    }
}
