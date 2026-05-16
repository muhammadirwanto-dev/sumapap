using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Sumapap.Persistence.Abstractions;
using Sumapap.Queries.Abstractions;

namespace Sumapap.Persistence.Caching.FusionCache.Repositories
{
    internal class CachedReadWriteRepository<TEntity, TContext>() : IReadWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity
    {
        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
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

        public void Save()
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
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
