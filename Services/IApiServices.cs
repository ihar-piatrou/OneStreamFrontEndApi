namespace OneStreamFrontEndApi.Services
{
    /// <summary>
    /// Defines a contract for making API calls and caching the results.
    /// This service provides functionality to call an external API asynchronously and store the response in a cache
    /// to minimize redundant network requests.
    /// </summary>
    public interface IApiServices
    {
        /// <summary>
        /// Makes an asynchronous API call to the provided URL, caches the result with the specified cache key,
        /// and returns the response. If the data is already cached, it will return the cached value instead of making
        /// another network request.
        /// </summary>
        /// <param name="cacheKey">The key used to store and retrieve the API response from the cache.</param>
        /// <param name="url">The URL of the API endpoint to call.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the API response as a string.</returns>
        Task<string> CallApiAsync(string cacheKey, string? url);
    }
}
