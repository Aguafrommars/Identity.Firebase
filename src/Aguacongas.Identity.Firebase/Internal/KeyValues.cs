using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Identity.Firebase.Internal
{
    public class KeyValues<T>
    {
        public string Key { get; set; }

        public IEnumerable<T> Values { get; set; }
    }
}
