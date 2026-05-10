using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using Sumapap.Caching.Abstractions;
using Sumapap.Persistence.Abstraction;
using Sumapap.Queries.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace Sumapap.Persistence.FusionCache.Repositories
{
    public abstract class CachedReadRepository<TEntity, TContext>(
        IReadRepository<TEntity, TContext> _repository,
        IFusionCache _cache,
        ICacheKeyProvider _keyProvider,
        IOptions<CachedFunctionsMapping> _options)
        : IReadRepository<TEntity, TContext>
        where TEntity : class, IEntity
    {
        private readonly CachedFunctionsMapping _mapping = _options.Value;

        public long Count()
        {
            var key = _keyProvider.CreateKey<TEntity>("*", "Count");

            return _cache.GetOrSet(key, _ => _repository.Count());
        }

        public long Count(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public void DetatchFromTracking(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity? Find<TKey>(TKey key) where TKey : IEquatable<TKey>
        {
            throw new NotImplementedException();
        }

        public ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default) where TKey : IEquatable<TKey>
        {
            throw new NotImplementedException();
        }

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity? FirstOrDefault(ISpecification<TEntity> specification)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> GetAll(ISpecification<TEntity> specification)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public bool IsExists(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryResult<TEntity>> QueryAsync(IQuery query, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity? SingleOrDefault(ISpecification<TEntity> specification)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

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
