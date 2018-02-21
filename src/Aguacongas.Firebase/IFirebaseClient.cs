using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    public interface IFirebaseClient
    {
        Task DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string etag = null);
        Task<FirebaseResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string queryString = null);
        Task<FirebaseResponse<T>> PatchAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken));
        Task<FirebaseResponse<string>> PostAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false);
        Task<FirebaseResponse<T>> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string etag = null);
    }
}