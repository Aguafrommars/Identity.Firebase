// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// Login indexes
    /// </summary>
    public class LoginIndex : FirebaseIndexes
    {
        /// <inheritdoc />
        public LoginIndex()
        {
            On = new string[] { "ProviderKey", "UserId" };
        }
    }
}
