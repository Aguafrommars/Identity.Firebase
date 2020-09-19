// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Firebase
{
    /// <summary>
    /// Firebase client interface
    /// </summary>
    public interface IFirebaseClient
    {
        /// <summary>
        /// Send a DELETE request to delete data
        /// </summary>
        /// <param name="url">Data uri</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <param name="etag">Etag value</param>
        /// <returns></returns>
        Task DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string etag = null);

        /// <summary>
        /// Send a GET request to request data
        /// </summary>
        /// <typeparam name="T">The type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <param name="queryString">A query string</param>
        /// <returns>A <see cref="FirebaseResponse{T}"/></returns>
        Task<FirebaseResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string queryString = null);

        /// <summary>
        /// Send a PATCH request to path data
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="data">Data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A <see cref="FirebaseResponse{T}"/></returns>
        Task<FirebaseResponse<T>> PatchAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends a POST request to create data
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="data">Data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <returns>The data id</returns>
        Task<FirebaseResponse<string>> PostAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false);

        /// <summary>
        /// Send a PUT request to update data
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="url">Data uri</param>
        /// <param name="data">Data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <param name="requestEtag">True to request ETag</param>
        /// <param name="etag">ETag value</param>
        /// <returns>A <see cref="FirebaseResponse{T}"/></returns>
        Task<FirebaseResponse<T>> PutAsync<T>(string url, T data, CancellationToken cancellationToken = default(CancellationToken), bool requestEtag = false, string etag = null);
    }
}