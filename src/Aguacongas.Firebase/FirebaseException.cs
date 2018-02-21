using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Aguacongas.Firebase
{
    public class FirebaseException: Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }

        public string Etag { get; private set; }

        public string Error { get; private set; }

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

        public FirebaseException(HttpStatusCode statusCode, string reasonPhrase, string error, string eTag): base($"HTTP Error {statusCode}: {reasonPhrase}")
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Etag = eTag;
            Error = error;
        }
    }
}
