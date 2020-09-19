// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// Role index
    /// </summary>
    public class RoleIndex : FirebaseIndex
    {
        /// <inheritdoc />
        public RoleIndex()
        {
            On = "NormalizedName";
        }
    }
}