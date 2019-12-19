using Newtonsoft.Json;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Aguacongas.Firebase
{
    /// <summary>
    /// Firebase exception
    /// </summary>
    [Serializable]
    public class FirebaseException: Exception
    {
        /// <summary>
        /// Gets the HTTP statuc code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the reason phrase
        /// </summary>
        public string ReasonPhrase { get; private set; }

        /// <summary>
        /// Gets the Etag
        /// </summary>
        public string Etag { get; private set; }

        /// <summary>
        /// Gets the error content
        /// </summary>

        public string Error { get; private set; }

        /// <summary>
        /// Gets the <see cref="FirebaseError"/>
        /// </summary>
        public FirebaseError FirebaseError {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<FirebaseError>(Error);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Initialize a new instance of <see cref="FirebaseException"/>
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="reasonPhrase">The reason phrase</param>
        /// <param name="error">The error content</param>
        /// <param name="eTag">The ETag value</param>
        public FirebaseException(HttpStatusCode statusCode, string reasonPhrase, string error, string eTag): base($"HTTP Error {statusCode}: {reasonPhrase}")
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Etag = eTag;
            Error = error;
        }

        protected FirebaseException(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo, streamingContext)
        {
        }
    }
}
