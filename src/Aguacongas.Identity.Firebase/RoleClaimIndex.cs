using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    public class RoleClaimIndex : FirebaseIndex
    {
        public RoleClaimIndex()
        {
            On = "RoleId";
        }
    }
}