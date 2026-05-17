using System.Linq.Expressions;
using Sumapap.Persistence.Abstractions;
using Sumapap.Queries.Abstractions;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal class CachedReadWriteRepository<TEntity, TContext>(
        IServiceProvider _serviceProvider,
        IReadRepository<TEntity, TContext> _read,
        IWriteRepository<TEntity, TContext> _write
        ) : CachedRepository(_serviceProvider), IReadWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity
    {
        #region READ
        public long Count() => ExecuteGetOrSet<long, TEntity>(
            _read, "Count", _keyProvider.CreateKey<TEntity>("*", "Count"), tags: [GetAllItemTag()], () => _read.Count());

        public long Count(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet<long, TEntity>(
            _read, "Count", _keyProvider.CreateKey<TEntity>("*", "Count", predicate), tags: [GetAllItemTag()], () => _read.Count(predicate));

        public Task<long> CountAsync(CancellationToken cancellation = default) => ExecuteGetOrSetAsync<long, TEntity>(
            _read, "Count", _keyProvider.CreateKey<TEntity>("*", "Count"), tags: [GetAllItemTag()], () => _read.CountAsync(cancellation), cancellation);

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<long, TEntity>(
            _read, "Count", _keyProvider.CreateKey<TEntity>("*", "Count", predicate), tags: [GetAllItemTag()], () => _read.CountAsync(predicate, cancellation), cancellation);

        public void DetatchFromTracking(TEntity entity) =>
            _read.DetatchFromTracking(entity);

        public TEntity? Find<TKey>(TKey key) where TKey : IEquatable<TKey> => ExecuteGetOrSet<TEntity?, TEntity>(
            _read, "Find", _keyProvider.CreateKey<TEntity>(key, "*"), tags: [GetAllItemTag()], () => _read.Find(key));

        public async ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default) where TKey : IEquatable<TKey> => await ExecuteGetOrSetAsync<TEntity?, TEntity>(
            _read, "Find", _keyProvider.CreateKey<TEntity>(key, "*"), tags: [GetAllItemTag()], () => _read.FindAsync(key, cancellation).AsTask(), cancellation);

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet<TEntity?, TEntity>(
            _read, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", predicate), tags: [GetAllItemTag()], () => _read.FirstOrDefault(predicate));

        public TEntity? FirstOrDefault(ISpecification<TEntity> specification) => ExecuteGetOrSet<TEntity?, TEntity>(
            _read, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", specification), tags: [GetAllItemTag()], () => _read.FirstOrDefault(specification));

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<TEntity?, TEntity>(
            _read, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", predicate), tags: [GetAllItemTag()], () => _read.FirstOrDefaultAsync(predicate, cancellation), cancellation);

        public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<TEntity?, TEntity>(
            _read, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", specification), tags: [GetAllItemTag()], () => _read.FirstOrDefaultAsync(specification, cancellation), cancellation);

        public IList<TEntity> GetAll() => ExecuteGetOrSet<IList<TEntity>, TEntity>(
            _read, "GetAll", _keyProvider.CreateKey<TEntity>("*"), tags: [GetAllItemTag()], () => _read.GetAll());

        public IList<TEntity> GetAll(ISpecification<TEntity> specification) => ExecuteGetOrSet<IList<TEntity>, TEntity>(
            _read, "GetAll", _keyProvider.CreateKey<TEntity>("*", specification), tags: [GetAllItemTag()], () => _read.GetAll(specification));

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default) => ExecuteGetOrSetAsync<List<TEntity>, TEntity>(
            _read, "GetAll", _keyProvider.CreateKey<TEntity>("*"), tags: [GetAllItemTag()], () => _read.GetAllAsync(cancellation), cancellation);

        public Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<List<TEntity>, TEntity>(
            _read, "GetAll", _keyProvider.CreateKey<TEntity>("*", specification), tags: [GetAllItemTag()], () => _read.GetAllAsync(specification, cancellation), cancellation);

        public bool IsExists(Expression<Func<TEntity, bool>> predicate) =>
            _read.IsExists(predicate);

        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) =>
            _read.IsExistsAsync(predicate, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(IQuery query, CancellationToken cancellation = default) =>
            _read.QueryAsync(query, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) =>
            _read.QueryAsync(specification, cancellation);

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet<TEntity?, TEntity>(
             _read, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", predicate), tags: [GetAllItemTag()], () => _read.SingleOrDefault(predicate));

        public TEntity? SingleOrDefault(ISpecification<TEntity> specification) => ExecuteGetOrSet<TEntity?, TEntity>(
            _read, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", specification), tags: [GetAllItemTag()], () => _read.SingleOrDefault(specification));

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<TEntity?, TEntity>(
            _read, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", predicate), tags: [GetAllItemTag()], () => _read.SingleOrDefaultAsync(predicate, cancellation), cancellation);

        public Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<TEntity?, TEntity>(
            _read, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", specification), tags: [GetAllItemTag()], () => _read.SingleOrDefaultAsync(specification, cancellation), cancellation);

        public IAsyncEnumerable<TEntity> StreamAllAsync() =>
            _read.StreamAllAsync();

        public IAsyncEnumerable<TEntity> StreamAllAsync(ISpecification<TEntity> specification) =>
            _read.StreamAllAsync(specification);

        public IAsyncEnumerable<TEntity> StreamWhereAsync(Expression<Func<TEntity, bool>> predicate) =>
            _read.StreamWhereAsync(predicate);

        public IAsyncEnumerable<TEntity> StreamWhereAsync(ISpecification<TEntity> specification) =>
            _read.StreamWhereAsync(specification);

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet<IList<TEntity>, TEntity>(
            _read, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", predicate), tags: [GetAllItemTag()], () => _read.Where(predicate));

        public IList<TEntity> Where(ISpecification<TEntity> specification) => ExecuteGetOrSet<IList<TEntity>, TEntity>(
            _read, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", specification), tags: [GetAllItemTag()], () => _read.Where(specification));

        public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<List<TEntity>, TEntity>(
            _read, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", predicate), tags: [GetAllItemTag()], () => _read.WhereAsync(predicate, cancellation), cancellation);

        public Task<List<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync<List<TEntity>, TEntity>(
            _read, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", specification), tags: [GetAllItemTag()], () => _read.WhereAsync(specification, cancellation), cancellation);
        #endregion // READ

        #region WRITE
        public void Add(TEntity entity) => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.Add(entity));

        public async ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default) => await ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.AddAsync(entity, ct).AsTask(), cancellationToken);

        public void AddRange(IEnumerable<TEntity> entities) => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.AddRange(entities));

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.AddRangeAsync(entities, ct), cancellationToken);

        public void Delete(TEntity entity) => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.Delete(entity));

        public void Delete<TKey>(TKey id) where TKey : IEquatable<TKey> => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.Delete(id));

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.DeleteAsync(entity, ct), cancellationToken);

        public Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey> => ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.DeleteAsync(id, ct), cancellationToken);

        public void DeleteRange(IEnumerable<TEntity> entities) => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.DeleteRange(entities));

        public void DeleteRange<TKey>(IEnumerable<TKey> ids) where TKey : IEquatable<TKey> => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.DeleteRange(ids));

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.DeleteRangeAsync(entities, ct), cancellationToken);

        public Task DeleteRangeAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey> => ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.DeleteRangeAsync(ids, ct), cancellationToken);

        public void Save() =>
            _write.Save();

        public Task SaveAsync(CancellationToken cancellationToken = default) =>
            _write.SaveAsync(cancellationToken);

        public void Update(TEntity entity) => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.Update(entity));

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.UpdateAsync(entity, ct), cancellationToken);

        public void UpdateRange(IEnumerable<TEntity> entities) => ExecuteSet<TEntity>(
            [GetAllItemTag()], () => _write.UpdateRange(entities));

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => ExecuteSetAsync<TEntity>(
            [GetAllItemTag()], (ct) => _write.UpdateRangeAsync(entities, ct), cancellationToken);
        #endregion // WRITE

        private string GetAllItemTag() => GetAllItemTag<TEntity>();
    }
}
