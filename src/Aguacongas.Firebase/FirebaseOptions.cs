using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Aguacongas.Firebase
{
    /// <summary>
    /// Firebase options
    /// </summary>
    public class FirebaseOptions
    {
        /// <summary>
        /// Gets or sets serialization settings
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Gets or sets the firebase url
        /// </summary>
        public string DatabaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the HttpClient name
        /// </summary>
        internal string HttpClientName { get; set; } = "firebase";
    }
}
