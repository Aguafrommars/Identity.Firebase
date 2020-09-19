// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// User roles indexes
    /// </summary>
    public class UseRoleIndex: FirebaseIndexes
    {
        public UseRoleIndex()
        {
            On = new string[] { "UserId", "RoleId" };
        }
    }
}