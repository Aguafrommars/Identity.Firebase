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