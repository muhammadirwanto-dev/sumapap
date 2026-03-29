using System.Collections.Concurrent;
using System.Reflection;

namespace Sumapap.Queries.Execution.Internals
{
    public static class ReflectionCache
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo?> Cache = new();

        public static PropertyInfo? GetProperty<T>(string name)
        {
            var key = $"{typeof(T).FullName}.{name}";
            return Cache.GetOrAdd(
                key,
                _ => typeof(T).GetProperty(name));
        }
    }
}
