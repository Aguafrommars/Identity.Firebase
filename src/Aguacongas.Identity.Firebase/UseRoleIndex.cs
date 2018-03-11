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