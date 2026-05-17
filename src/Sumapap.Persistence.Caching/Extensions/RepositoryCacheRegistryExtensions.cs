using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.DependencyInjection;

namespace Sumapap.Persistence.Caching.Extensions
{
    internal static class RepositoryCacheRegistryExtensions
    {
        extension(RepositoryCacheRegistry registry)
        {
            public RepositoryCacheEntry? GetCacheEntry<T>(T repository, Type entityType)
                where T : IRepository
            {
                return registry.CachedRepositories
                    .FirstOrDefault(x =>
                    {
                        return x.RepositoryType == repository.GetType()
                            && x.EntityType == entityType;
                    });
            }

            public bool IsCached<T>(T repository, Type entityType, string key)
                where T : IRepository
            {
                var entry = registry.GetCacheEntry(repository, entityType);
                if (entry == null)
                {
                    return false;
                }

                return entry.IsCached(key);
            }
        }
    }
}
