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
