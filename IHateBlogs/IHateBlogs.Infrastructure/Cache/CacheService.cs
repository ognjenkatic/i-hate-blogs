using IHateBlogs.Application.Common.Interfaces;
using System;

namespace IHateBlogs.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, object> _cache = new();
        public T? Get<T>(string key)
        {
            _cache.TryGetValue(key, out var value);

            if (value is not null)
            {
                return (T?)value;
            }

            return default;
        }

        public T Set<T>(string key, T value)
        {
            if (value is null)
            {
                throw new InvalidOperationException("Cannot add null to cache");
            }

            _cache[key] = value;

            return value;
        }
    }
}