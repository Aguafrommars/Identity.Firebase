using Aguacongas.Firebase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Identity.Firebase
{
    public class UserIndex: FirebaseIndexes
    {
        public UserIndex()
        {
            On = new string[] { "NormalizedEmail", "NormalizedUserName" };
        }
    }
}
