using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Firebase
{
    public class FirebaseResponse<T>
    {
        public T Data { get; set; }

        public string Etag { get; set; }
    }
}
