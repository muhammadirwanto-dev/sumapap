using System.Linq.Expressions;
using Sumapap.Caching.Abstractions;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.DependencyInjection;
using Sumapap.Queries.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal class CachedReadRepository<TEntity, TContext>(
        IReadRepository<TEntity, TContext> _inner,
        ICacheKeyProvider _keyProvider,
        IFusionCache _cache,
        RepositoryCacheRegistry _registry
        ) : CachedRepository(_cache, _registry), IReadRepository<TEntity, TContext>
        where TEntity : class, IEntity
    {
        public long Count() => ExecuteCacheIfAllowed<long, TEntity>(
            _inner, "Count", _keyProvider.CreateKey<TEntity>("*", "Count"), () => _inner.Count());

        public long Count(Expression<Func<TEntity, bool>> predicate) =>
            _inner.Count(predicate);

        public Task<long> CountAsync(CancellationToken cancellation = default) => ExecuteCacheIfAllowedAsync<long, TEntity>(
            _inner, "Count", _keyProvider.CreateKey<TEntity>("*", "Count"), () => _inner.CountAsync(cancellation), cancellation);

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) =>
            _inner.CountAsync(predicate, cancellation);

        public void DetatchFromTracking(TEntity entity) =>
            _inner.DetatchFromTracking(entity);

        public TEntity? Find<TKey>(TKey key) where TKey : IEquatable<TKey> => ExecuteCacheIfAllowed<TEntity?, TEntity>(
            _inner, "Find", _keyProvider.CreateKey<TEntity>(key, "*"), () => _inner.Find(key));

        public async ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default) where TKey : IEquatable<TKey> => await ExecuteCacheIfAllowedAsync<TEntity?, TEntity>(
            _inner, "Find", _keyProvider.CreateKey<TEntity>(key, "*"), () => _inner.FindAsync(key, cancellation).AsTask(), cancellation);

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate) =>
            _inner.FirstOrDefault(predicate);

        public TEntity? FirstOrDefault(ISpecification<TEntity> specification) =>
            _inner.FirstOrDefault(specification);

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) =>
            _inner.FirstOrDefaultAsync(predicate, cancellation);

        public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) =>
            _inner.FirstOrDefaultAsync(specification, cancellation);

        public IList<TEntity> GetAll() => ExecuteCacheIfAllowed<IList<TEntity>, TEntity>(
            _inner, "GetAll", _keyProvider.CreateKey<TEntity>("*"), () => _inner.GetAll());

        public IList<TEntity> GetAll(ISpecification<TEntity> specification) =>
            _inner.GetAll(specification);

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default) => ExecuteCacheIfAllowedAsync<List<TEntity>, TEntity>(
            _inner, "GetAll", _keyProvider.CreateKey<TEntity>("*"), () => _inner.GetAllAsync(cancellation), cancellation);

        public Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) =>
            _inner.GetAllAsync(specification, cancellation);

        public bool IsExists(Expression<Func<TEntity, bool>> predicate) =>
            _inner.IsExists(predicate);

        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) =>
            _inner.IsExistsAsync(predicate, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(IQuery query, CancellationToken cancellation = default) =>
            _inner.QueryAsync(query, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) =>
            _inner.QueryAsync(specification, cancellation);

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate) =>
            _inner.SingleOrDefault(predicate);

        public TEntity? SingleOrDefault(ISpecification<TEntity> specification) =>
            _inner.SingleOrDefault(specification);

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default) =>
            _inner.SingleOrDefaultAsync(predicate, cancellation);

        public Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default) =>
            _inner.SingleOrDefaultAsync(specification, cancellation);

        public IAsyncEnumerable<TEntity> StreamAllAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TEntity> StreamAllAsync(ISpecification<TEntity> specification)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TEntity> StreamWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TEntity> StreamWhereAsync(ISpecification<TEntity> specification)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> Where(ISpecification<TEntity> specification)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }
    }
}
