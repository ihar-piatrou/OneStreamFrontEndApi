using Microsoft.Extensions.Caching.Memory;

namespace OneStreamFrontEndApi.Services
{
    public class ApiServices : IApiServices
    {
        private const int CacheDurationInMinutes = 1;

        private readonly IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _memoryCache;
       
        public ApiServices(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public async Task<string> CallApiAsync(string cacheKey, string url)
        {
            return await GetOrAddCacheAsync(cacheKey, async () => await CallApiWithPollyRetry(url), TimeSpan.FromMinutes(CacheDurationInMinutes));
        }

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
        private async Task<string> CallApiWithPollyRetry(string url)
        {
            // Use the Polly client
            var client = _clientFactory.CreateClient("PollyClient"); 
            return await client.GetStringAsync(url);
        }
    }
}
