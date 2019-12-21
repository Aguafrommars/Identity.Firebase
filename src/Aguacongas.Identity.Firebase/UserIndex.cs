// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// User indexes
    /// </summary>
    public class UserIndex: FirebaseIndexes
    {
        public UserIndex()
        {
            On = new string[] { "NormalizedEmail", "NormalizedUserName" };
        }
    }
}
