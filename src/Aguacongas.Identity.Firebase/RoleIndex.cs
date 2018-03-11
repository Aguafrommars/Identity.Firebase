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