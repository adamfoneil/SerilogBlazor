using Microsoft.Extensions.Caching.Memory;

namespace SerilogBlazor.ApiConnector;

internal static class CacheExtensions
{
	internal enum ResultType
	{
		Live,
		Cached
	}

	internal static async Task<(ResultType, T)> GetOrExecuteAsync<T>(this IMemoryCache cache, string key, Func<Task<T>> factory, TimeSpan duration)
	{
		if (cache.TryGetValue(key, out T? value) && value is not null)
		{
			return (ResultType.Cached, value);
		}

		value = await factory();
		cache.Set(key, value, new MemoryCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = duration
		});

		return (ResultType.Live, value);
	}
}
