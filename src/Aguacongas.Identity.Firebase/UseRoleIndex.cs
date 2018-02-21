using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    public class UseRoleIndex: FirebaseIndexes
    {
        public UseRoleIndex()
        {
            On = new string[] { "UserId", "RoleId" };
        }
    }
}