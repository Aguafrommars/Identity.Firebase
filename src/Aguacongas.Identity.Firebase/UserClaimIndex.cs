using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    public class UserClaimIndex : FirebaseIndexes
    {
        public UserClaimIndex()
        {
            On = new string[] { "UserId", "ClaimType" };
        }
    }
}
