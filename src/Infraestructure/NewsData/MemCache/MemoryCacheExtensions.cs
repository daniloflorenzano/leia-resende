using System.Runtime.CompilerServices;
using Microsoft.Extensions.Caching.Memory;

namespace Infraestructure.NewsData.MemCache;

public static class MemoryCacheExtensions
{
    private static readonly ConditionalWeakTable<IMemoryCache, List<object>> CacheKeys = new();

    public static void TrackKey(this IMemoryCache cache, object key)
    {
        if (!CacheKeys.TryGetValue(cache, out var keys))
        {
            keys = [];
            CacheKeys.Add(cache, keys);
        }
        
        if (!keys.Contains(key))
        {
            keys.Add(key);
        }
    }

    private static IEnumerable<object> GetTrackedKeys(this IMemoryCache cache)
    {
        return CacheKeys.TryGetValue(cache, out var keys) 
            ? keys 
            : Enumerable.Empty<object>();
    }

    public static IEnumerable<TItem> GetKeys<TItem>(this IMemoryCache cache)
    {
        var trackedKeys = cache.GetTrackedKeys();
        var items = new List<TItem>();

        foreach (var key in trackedKeys)
        {
            if (cache.TryGetValue(key, out TItem? item) && item != null)
            {
                items.Add(item);
            }
        }

        return items;
    }
}