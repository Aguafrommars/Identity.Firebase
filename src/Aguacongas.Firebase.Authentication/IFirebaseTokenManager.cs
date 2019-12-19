using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    /// <summary>
    /// Firebase token manager
    /// </summary>
    public interface IFirebaseTokenManager
    {
        /// <summary>
        /// Gets the authentication param name
        /// </summary>
        string AuthParamName { get; }

        /// <summary>
        /// Gets the authentication token
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The token</returns>
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
