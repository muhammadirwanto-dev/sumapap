using System.Collections.Concurrent;
using System.Reflection;

namespace Sumapap.Queries.Execution.Utils
{
    public static class ReflectionCache
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo?> Cache = new();

        public static PropertyInfo? GetProperty<T>(string name)
            => Cache.GetOrAdd(
                $"{typeof(T).FullName}.{name}",
                _ => typeof(T).GetProperty(name));
    }
}
