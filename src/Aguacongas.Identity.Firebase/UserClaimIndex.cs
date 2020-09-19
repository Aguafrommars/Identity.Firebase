// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// User claim indexes
    /// </summary>
    public class UserClaimIndex : FirebaseIndexes
    {
        public UserClaimIndex()
        {
            On = new string[] { "UserId", "ClaimType" };
        }
    }
}
