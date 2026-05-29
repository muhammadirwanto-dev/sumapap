using Sumapap.Persistence.Abstractions.Entities;
using Sumapap.Persistence.Abstractions.Repositories;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal class CachedWriteRepository<TEntity, TContext>(
        IServiceProvider _serviceProvider,
        IWriteRepository<TEntity, TContext> _inner
        ) : CachedRepository(_serviceProvider), IWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity
    {
        public void Add(TEntity entity) => ExecuteSet(
            [GetAllItemTag()], () => _inner.Add(entity));

        public async ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default) => await ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.AddAsync(entity, ct).AsTask(), cancellationToken);

        public void AddRange(IEnumerable<TEntity> entities) => ExecuteSet(
            [GetAllItemTag()], () => _inner.AddRange(entities));

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.AddRangeAsync(entities, ct), cancellationToken);

        public void Delete(TEntity entity) => ExecuteSet(
            [GetAllItemTag()], () => _inner.Delete(entity));

        public void Delete<TKey>(TKey id) where TKey : IEquatable<TKey> => ExecuteSet(
            [GetAllItemTag()], () => _inner.Delete(id));

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.DeleteAsync(entity, ct), cancellationToken);

        public Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey> => ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.DeleteAsync(id, ct), cancellationToken);

        public void DeleteRange(IEnumerable<TEntity> entities) => ExecuteSet(
            [GetAllItemTag()], () => _inner.DeleteRange(entities));

        public void DeleteRange<TKey>(IEnumerable<TKey> ids) where TKey : IEquatable<TKey> => ExecuteSet(
            [GetAllItemTag()], () => _inner.DeleteRange(ids));

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.DeleteRangeAsync(entities, ct), cancellationToken);

        public Task DeleteRangeAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey> => ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.DeleteRangeAsync(ids, ct), cancellationToken);

        public void Save()
            => ExecuteSave(_inner);

        public Task SaveAsync(CancellationToken cancellationToken = default)
            => ExecuteSaveAsync(_inner, cancellationToken);

        public void Update(TEntity entity) => ExecuteSet(
            [GetAllItemTag()], () => _inner.Update(entity));

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.UpdateAsync(entity, ct), cancellationToken);

        public void UpdateRange(IEnumerable<TEntity> entities) => ExecuteSet(
            [GetAllItemTag()], () => _inner.UpdateRange(entities));

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => ExecuteSetAsync(
            [GetAllItemTag()], (ct) => _inner.UpdateRangeAsync(entities, ct), cancellationToken);

        private string GetAllItemTag() => GetAllItemTag<TEntity>();
    }
}
