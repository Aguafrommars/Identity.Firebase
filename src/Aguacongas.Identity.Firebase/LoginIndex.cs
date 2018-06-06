using Aguacongas.Firebase;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// Login indexes
    /// </summary>
    public class LoginIndex : FirebaseIndexes
    {
        /// <inheritdoc />
        public LoginIndex()
        {
            On = new string[] { "ProviderKey", "UserId" };
        }
    }
}
