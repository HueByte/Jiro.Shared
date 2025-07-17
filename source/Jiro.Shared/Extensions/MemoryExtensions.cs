using Microsoft.Extensions.Caching.Memory;


namespace JiroCloud.Shared.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IMemoryCache"/> to simplify cache retrieval and type-safe access.
/// </summary>
public static class MemoryExtensions
{
	/// <summary>
	/// Retrieves a value from the cache by key.
	/// </summary>
	/// <param name="cache">The memory cache instance.</param>
	/// <param name="key">The key of the cached item.</param>
	/// <returns>The cached value if found; otherwise, <c>null</c>.</returns>
	public static object? Get(this IMemoryCache cache, object key)
	{
		cache.TryGetValue(key, out object? value);
		return value;
	}

	/// <summary>
	/// Retrieves a value from the cache by key and casts it to the specified type.
	/// </summary>
	/// <typeparam name="TItem">The type of the cached item.</typeparam>
	/// <param name="cache">The memory cache instance.</param>
	/// <param name="key">The key of the cached item.</param>
	/// <returns>The cached value cast to <typeparamref name="TItem"/> if found; otherwise, the default value of <typeparamref name="TItem"/>.</returns>
	public static TItem? Get<TItem>(this IMemoryCache cache, object key)
	{
		return (TItem?)(cache.Get(key) ?? default(TItem));
	}

	/// <summary>
	/// Attempts to retrieve a value from the cache by key and cast it to the specified type.
	/// </summary>
	/// <typeparam name="TItem">The type of the cached item.</typeparam>
	/// <param name="cache">The memory cache instance.</param>
	/// <param name="key">The key of the cached item.</param>
	/// <param name="value">When this method returns, contains the cached value cast to <typeparamref name="TItem"/> if found; otherwise, the default value of <typeparamref name="TItem"/>.</param>
	/// <returns><c>true</c> if the value was found and cast successfully; otherwise, <c>false</c>.</returns>
	public static bool TryGetValue<TItem>(this IMemoryCache cache, object key, out TItem? value)
	{
		if (cache.TryGetValue(key, out object? result))
		{
			if (result is TItem item)
			{
				value = item;
				return true;
			}
		}

		value = default;
		return false;
	}
}
