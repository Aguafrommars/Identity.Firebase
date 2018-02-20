using Aguacongas.Firebase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Identity.Firebase
{
    public class LoginIndex : FirebaseIndexes
    {
        public LoginIndex()
        {
            On = new string[] { "ProviderKey", "UserId" };
        }
    }
}
