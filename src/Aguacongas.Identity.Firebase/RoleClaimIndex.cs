// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// Role claim index
    /// </summary>
    public class RoleClaimIndex : FirebaseIndex
    {
        /// <inheritdoc />
        public RoleClaimIndex()
        {
            On = "RoleId";
        }
    }
}