using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.DependencyInjection;
using Sumapap.Persistence.Caching.Extensions;
using ZiggyCreatures.Caching.Fusion;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal abstract class CachedRepository(
        IFusionCache _cache,
        RepositoryCacheRegistry _registry
        )
    {
        protected TResult ExecuteCacheIfAllowed<TResult, TEntity>(
            IRepository inner,
            string methodName,
            string cacheKey,
            Func<TResult> operation)
            where TEntity : class, IEntity
        {
            if (_registry.IsCached(inner, typeof(TEntity), methodName))
            {
                return _cache.GetOrSet(cacheKey, _ => operation());
            }

            return operation();
        }

        protected async Task<TResult> ExecuteCacheIfAllowedAsync<TResult, TEntity>(
            IRepository inner,
            string methodName,
            string cacheKey,
            Func<Task<TResult>> operation,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (_registry.IsCached(inner, typeof(TEntity), methodName))
            {
                return await _cache.GetOrSetAsync(cacheKey, (ct) => operation(), token: cancellationToken);
            }

            return await operation();
        }
    }
}
