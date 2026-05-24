using System.Collections.Concurrent;
using System.Reflection;

namespace Sumapap.Queries.Utils
{
    public static class ReflectionCache
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo?> _cache = new();

        public static PropertyInfo? GetProperty<T>(string name)
            => _cache.GetOrAdd(
                $"{typeof(T).FullName}.{name}",
                _ => typeof(T).GetProperty(name));
    }
}
