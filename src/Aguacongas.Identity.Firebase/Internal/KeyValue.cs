using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Identity.Firebase.Internal
{
    public class KeyValue<T>
    {
        public string Key { get; set; }

        public T Value { get; set; }
    }
}
