using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public interface IFirebaseClient
    {
        Task DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken));
        Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken));
        Task<T> PatchAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> PostAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken));
        Task<T> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken));
    }
}