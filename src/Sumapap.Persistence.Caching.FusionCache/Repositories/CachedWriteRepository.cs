using Sumapap.Caching.Abstractions;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal class CachedWriteRepository<TEntity, TContext>(
        IWriteRepository<TEntity, TContext> _inner,
        ICacheKeyProvider _keyProvider,
        IFusionCache _cache,
        RepositoryCacheRegistry _registry
        ) : CachedRepository(_cache, _registry), IWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity
    {
        public void Add(TEntity entity) => ExecuteSet(
            _inner, "Add", _keyProvider.CreateKey(entity), [GetAllItemTag()], entity, _inner.Add);

        public async ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default) => await ExecuteSetAsync(
            _inner, "Add", _keyProvider.CreateKey(entity), [GetAllItemTag()], entity, (e, ct) => _inner.AddAsync(e, ct).AsTask(), cancellationToken);

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete<TKey>(TKey id) where TKey : IEquatable<TKey>
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey>
        {
            throw new NotImplementedException();
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void DeleteRange<TKey>(IEnumerable<TKey> ids) where TKey : IEquatable<TKey>
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey>
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private string GetAllItemTag() => GetAllItemTag<TEntity>(_keyProvider);
    }
}
