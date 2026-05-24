using Microsoft.Extensions.DependencyInjection;
using Sumapap.Caching.Abstractions;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal abstract class CachedRepository(
        IServiceProvider _serviceProvider
        )
    {
        protected readonly ICacheKeyProvider _keyProvider = _serviceProvider.GetRequiredService<ICacheKeyProvider>();
        private readonly IFusionCache _cache = _serviceProvider.GetRequiredService<IFusionCache>();
        private readonly RepositoryCacheRegistry _registry = _serviceProvider.GetRequiredService<RepositoryCacheRegistry>();

        protected TResult ExecuteGetOrSet<TResult, TEntity>(
            IRepository inner,
            string methodName,
            string cacheKey,
            string[] tags,
            Func<TResult> operation)
            where TEntity : class, IEntity
        {
            if (_registry.GetCacheEntry(inner) is RepositoryCacheEntry entry
                && entry.IsCached(methodName))
            {
                return entry.Configuration.Duration.HasValue
                    ? _cache.GetOrSet(cacheKey, _ => operation(), tags: tags, duration: entry.Configuration.Duration.Value)
                    : _cache.GetOrSet(cacheKey, _ => operation(), tags: tags);
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
            if (_registry.GetCacheEntry(inner) is RepositoryCacheEntry entry
                && entry.IsCached(methodName))
            {
                return entry.Configuration.Duration.HasValue
                    ? await _cache.GetOrSetAsync(cacheKey, (ct) => operation(), tags: tags, token: cancellationToken, duration: entry.Configuration.Duration.Value)
                    : await _cache.GetOrSetAsync(cacheKey, (ct) => operation(), tags: tags, token: cancellationToken);
            }

            return await operation();
        }

        protected void ExecuteSet<TEntity>(
            string[] tags,
            Action operation)
            where TEntity : class, IEntity
        {
            _cache.RemoveByTag(tags);
            operation();
        }

        protected async Task ExecuteSetAsync<TEntity>(
            string[] tags,
            Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            await _cache.RemoveByTagAsync(tags, token: cancellationToken);
            await operation(cancellationToken);
        }

        protected string GetAllItemTag<TEntity>()
            where TEntity : class, IEntity
            => _keyProvider.CreateKey<TEntity>("*");
    }
}
