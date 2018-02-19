using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public interface IFirebaseTokenManager: IDisposable
    {
        string AuthParamName { get; }
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
