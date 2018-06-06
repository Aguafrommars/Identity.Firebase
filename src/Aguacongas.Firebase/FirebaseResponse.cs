namespace Aguacongas.Firebase
{
    /// <summary>
    /// Firebase response
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public class FirebaseResponse<T>
    {
        /// <summary>
        /// Gets or sets data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets Etag
        /// </summary>
        public string Etag { get; set; }
    }
}
