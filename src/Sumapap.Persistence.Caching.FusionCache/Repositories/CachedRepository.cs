using Sumapap.Caching.Abstractions;
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
        protected TResult ExecuteGetOrSet<TResult, TEntity>(
            IRepository inner,
            string methodName,
            string cacheKey,
            string[] tags,
            Func<TResult> operation)
            where TEntity : class, IEntity
        {
            if (_registry.IsCached(inner, typeof(TEntity), methodName))
            {
                return _cache.GetOrSet(cacheKey, _ => operation(), tags: tags);
            }

            return operation();
        }

        protected async Task<TResult> ExecuteGetOrSetAsync<TResult, TEntity>(
            IRepository inner,
            string methodName,
            string cacheKey,
            string[] tags,
            Func<Task<TResult>> operation,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (_registry.IsCached(inner, typeof(TEntity), methodName))
            {
                return await _cache.GetOrSetAsync(cacheKey, (ct) => operation(), tags: tags, token: cancellationToken);
            }

            return await operation();
        }

        protected void ExecuteSet<TEntity>(
            IRepository inner,
            string methodName,
            string cacheKey,
            string[] tags,
            TEntity value,
            Action<TEntity> operation)
            where TEntity : class, IEntity
        {
            if (_registry.IsCached(inner, typeof(TEntity), methodName))
            {
                _cache.RemoveByTag(tags);
                _cache.Set(cacheKey, value, tags: tags);
            }

            operation(value);
        }

        protected async Task ExecuteSetAsync<TEntity>(
            IRepository inner,
            string methodName,
            string cacheKey,
            string[] tags,
            TEntity value,
            Func<TEntity, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (_registry.IsCached(inner, typeof(TEntity), methodName))
            {
                await _cache.RemoveByTagAsync(tags, token: cancellationToken);
                await _cache.SetAsync(cacheKey, value, tags: tags, token: cancellationToken);
            }

            await operation(value, cancellationToken);
        }

        protected static string GetAllItemTag<TEntity>(ICacheKeyProvider provider)
            where TEntity : class, IEntity
            => provider.CreateKey<TEntity>("*");
    }
}
