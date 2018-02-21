using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    public class RoleIndex : FirebaseIndex
    {
        public RoleIndex()
        {
            On = "NormalizedName";
        }
    }
}