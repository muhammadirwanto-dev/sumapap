using Sumapap.Persistence.Abstractions.Entities;
using Sumapap.Persistence.Abstractions.Repositories;
using Sumapap.Persistence.Abstractions.Specifications;
using Sumapap.Queries.Abstractions;
using System.Linq.Expressions;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal class CachedReadRepository<TEntity, TContext>(
        IServiceProvider _serviceProvider,
        IReadRepository<TEntity, TContext> _inner
        ) : CachedRepository(_serviceProvider), IReadRepository<TEntity, TContext>
        where TEntity : class, IEntity
    {
        public long Count() => ExecuteGetOrSet(
            _inner, "Count", _keyProvider.CreateKey<TEntity>("*", "Count"), tags: [GetAllItemTag()], () => _inner.Count());

        public long Count(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet(
            _inner, "Count", _keyProvider.CreateKey<TEntity>("*", "Count", predicate), tags: [GetAllItemTag()], () => _inner.Count(predicate));

        public Task<long> CountAsync(CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "Count", _keyProvider.CreateKey<TEntity>("*", "Count"), tags: [GetAllItemTag()], () => _inner.CountAsync(cancellation), cancellation);

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "Count", _keyProvider.CreateKey<TEntity>("*", "Count", predicate), tags: [GetAllItemTag()], () => _inner.CountAsync(predicate, cancellation), cancellation);

        public void DetatchFromTracking(TEntity entity) =>
            _inner.DetatchFromTracking(entity);

        public TEntity? Find<TKey>(TKey key) where TKey : IEquatable<TKey> => ExecuteGetOrSet(
            _inner, "Find", _keyProvider.CreateKey<TEntity>(key, "*"), tags: [GetAllItemTag()], () => _inner.Find(key));

        public async ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default) where TKey : IEquatable<TKey> => await ExecuteGetOrSetAsync(
            _inner, "Find", _keyProvider.CreateKey<TEntity>(key, "*"), tags: [GetAllItemTag()], () => _inner.FindAsync(key, cancellation).AsTask(), cancellation);

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet(
            _inner, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", predicate), tags: [GetAllItemTag()], () => _inner.FirstOrDefault(predicate));

        public TEntity? FirstOrDefault(ISpecification<TEntity> specification) => ExecuteGetOrSet(
            _inner, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", specification), tags: [GetAllItemTag()], () => _inner.FirstOrDefault(specification));

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", predicate), tags: [GetAllItemTag()], () => _inner.FirstOrDefaultAsync(predicate, cancellation), cancellation);

        public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "FirstOrDefault", _keyProvider.CreateKey<TEntity>("*", "FirstOrDefault", specification), tags: [GetAllItemTag()], () => _inner.FirstOrDefaultAsync(specification, cancellation), cancellation);

        public IList<TEntity> GetAll() => ExecuteGetOrSet(
            _inner, "GetAll", _keyProvider.CreateKey<TEntity>("*"), tags: [GetAllItemTag()], () => _inner.GetAll());

        public IList<TEntity> GetAll(ISpecification<TEntity> specification) => ExecuteGetOrSet(
            _inner, "GetAll", _keyProvider.CreateKey<TEntity>("*", specification), tags: [GetAllItemTag()], () => _inner.GetAll(specification));

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "GetAll", _keyProvider.CreateKey<TEntity>("*"), tags: [GetAllItemTag()], () => _inner.GetAllAsync(cancellation), cancellation);

        public Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "GetAll", _keyProvider.CreateKey<TEntity>("*", specification), tags: [GetAllItemTag()], () => _inner.GetAllAsync(specification, cancellation), cancellation);

        public bool IsExists(Expression<Func<TEntity, bool>> predicate) =>
            _inner.IsExists(predicate);

        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) =>
            _inner.IsExistsAsync(predicate, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(IQuery query, CancellationToken cancellation = default) =>
            _inner.QueryAsync(query, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) =>
            _inner.QueryAsync(specification, cancellation);

        public IList<T> Select<T>(Expression<Func<TEntity, T>> selector) => ExecuteGetOrSet(
            _inner, "Select", _keyProvider.CreateKey<TEntity>("*", "Select", selector), tags: [GetAllItemTag()], () => _inner.Select(selector));

        public IList<T> Select<T>(Expression<Func<TEntity, T>> selector, ISpecification<TEntity> specification) => ExecuteGetOrSet(
            _inner, "Select", _keyProvider.CreateKey<TEntity>("*", "Select", selector, specification), tags: [GetAllItemTag()], () => _inner.Select(selector, specification));

        public Task<List<T>> SelectAsync<T>(Expression<Func<TEntity, T>> selector, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "Select", _keyProvider.CreateKey<TEntity>("*", "Select", selector), tags: [GetAllItemTag()], () => _inner.SelectAsync(selector, cancellation), cancellation);

        public Task<List<T>> SelectAsync<T>(Expression<Func<TEntity, T>> selector, ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "Select", _keyProvider.CreateKey<TEntity>("*", "Select", selector, specification), tags: [GetAllItemTag()], () => _inner.SelectAsync(selector, specification, cancellation), cancellation);

        public IList<T> SelectMany<T>(Expression<Func<TEntity, IEnumerable<T>>> selector) => ExecuteGetOrSet(
            _inner, "SelectMany", _keyProvider.CreateKey<TEntity>("*", "SelectMany", selector), tags: [GetAllItemTag()], () => _inner.SelectMany(selector));

        public IList<T> SelectMany<T>(Expression<Func<TEntity, IEnumerable<T>>> selector, ISpecification<TEntity> specification) => ExecuteGetOrSet(
            _inner, "SelectMany", _keyProvider.CreateKey<TEntity>("*", "SelectMany", selector, specification), tags: [GetAllItemTag()], () => _inner.SelectMany(selector, specification));

        public Task<List<T>> SelectManyAsync<T>(Expression<Func<TEntity, IEnumerable<T>>> selector, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "SelectMany", _keyProvider.CreateKey<TEntity>("*", "SelectMany", selector), tags: [GetAllItemTag()], () => _inner.SelectManyAsync(selector, cancellation), cancellation);

        public Task<List<T>> SelectManyAsync<T>(Expression<Func<TEntity, IEnumerable<T>>> selector, ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "SelectMany", _keyProvider.CreateKey<TEntity>("*", "SelectMany", selector, specification), tags: [GetAllItemTag()], () => _inner.SelectManyAsync(selector, specification, cancellation), cancellation);

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet(
             _inner, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", predicate), tags: [GetAllItemTag()], () => _inner.SingleOrDefault(predicate));

        public TEntity? SingleOrDefault(ISpecification<TEntity> specification) => ExecuteGetOrSet(
            _inner, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", specification), tags: [GetAllItemTag()], () => _inner.SingleOrDefault(specification));

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", predicate), tags: [GetAllItemTag()], () => _inner.SingleOrDefaultAsync(predicate, cancellation), cancellation);

        public Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "SingleOrDefault", _keyProvider.CreateKey<TEntity>("*", "SingleOrDefault", specification), tags: [GetAllItemTag()], () => _inner.SingleOrDefaultAsync(specification, cancellation), cancellation);

        public IAsyncEnumerable<TEntity> StreamAllAsync() =>
            _inner.StreamAllAsync();

        public IAsyncEnumerable<TEntity> StreamAllAsync(ISpecification<TEntity> specification) =>
            _inner.StreamAllAsync(specification);

        public IAsyncEnumerable<TEntity> StreamWhereAsync(Expression<Func<TEntity, bool>> predicate) =>
            _inner.StreamWhereAsync(predicate);

        public IAsyncEnumerable<TEntity> StreamWhereAsync(ISpecification<TEntity> specification) =>
            _inner.StreamWhereAsync(specification);

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate) => ExecuteGetOrSet(
            _inner, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", predicate), tags: [GetAllItemTag()], () => _inner.Where(predicate));

        public IList<TEntity> Where(ISpecification<TEntity> specification) => ExecuteGetOrSet(
            _inner, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", specification), tags: [GetAllItemTag()], () => _inner.Where(specification));

        public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", predicate), tags: [GetAllItemTag()], () => _inner.WhereAsync(predicate, cancellation), cancellation);

        public Task<List<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) => ExecuteGetOrSetAsync(
            _inner, "Where", _keyProvider.CreateKey<TEntity>("*", "Where", specification), tags: [GetAllItemTag()], () => _inner.WhereAsync(specification, cancellation), cancellation);

        private string GetAllItemTag() => GetAllItemTag<TEntity>();
    }
}
