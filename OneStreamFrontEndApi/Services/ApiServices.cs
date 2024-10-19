using Microsoft.Extensions.Caching.Memory;

namespace OneStreamFrontEndApi.Services
{
    /// <summary>
    /// Provides services for making API calls with caching and retry policies.
    /// </summary>
    public class ApiServices : IApiServices
    {
        private const int CacheDurationInMinutes = 1;

        private readonly IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiServices"/> class.
        /// </summary>
        /// <param name="clientFactory">The factory to create HttpClient instances.</param>
        /// <param name="memoryCache">The in-memory cache for storing API responses.</param>
        public ApiServices(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public async Task<string> CallApiAsync(string cacheKey, string? url)
        {
            return await GetOrAddCacheAsync(cacheKey, async () => await CallApiWithPollyRetry(url), TimeSpan.FromMinutes(CacheDurationInMinutes));
        }

        /// <summary>
        /// Retrieves the cached value or adds a new value to the cache using the provided factory function.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="cacheKey">The cache key to store/retrieve the value.</param>
        /// <param name="factory">A function to create the value if it is not found in the cache.</param>
        /// <param name="cacheDuration">The duration for which the value should be cached.</param>
        /// <returns>The cached or newly created value.</returns>
        private async Task<T> GetOrAddCacheAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan cacheDuration)
        {
            if (_memoryCache.TryGetValue(cacheKey, out T cachedValue))
            {
                return cachedValue;
            }

            // If not in cache, call the factory (API call in this case) to get the data
            var result = await factory();

            // Cache the result for the given duration
            _memoryCache.Set(cacheKey, result, cacheDuration);

            return result;
        }

        /// <summary>
        /// Calls the API using Polly for retry logic to handle transient failures.
        /// </summary>
        /// <param name="url">The URL of the API to call.</param>
        /// <returns>The API response as a string.</returns>
        private async Task<string> CallApiWithPollyRetry(string url)
        {
            // Use the Polly client
            var client = _clientFactory.CreateClient("PollyClient"); 
            return await client.GetStringAsync(url);
        }
    }
}
