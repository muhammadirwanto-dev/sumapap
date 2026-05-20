using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sumapap.Persistence.Abstractions;
using Sumapap.Queries.Abstractions;

namespace Sumapap.Persistence.EfCore.Repositories
{
    /// <summary>
    /// Generic repository implementation using Entity Framework Core.
    /// This class is designed to be inheritable by specific repositories in consuming applications.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The entity's primary key type.</typeparam>
    /// <typeparam name="TContext">The DbContext type.</typeparam>
    public class ReadWriteRepository<TEntity, TContext>(
        TContext @context
        ) : IReadWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        protected readonly TContext _context = context;
        protected readonly IReadRepository<TEntity, TContext> _read = new ReadRepository<TEntity, TContext>(context);
        protected readonly IWriteRepository<TEntity, TContext> _write = new WriteRepository<TEntity, TContext>(context);

        public void Add(TEntity entity)
            => _write.Add(entity);

        public ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => _write.AddAsync(entity, cancellationToken);

        public void AddRange(IEnumerable<TEntity> entities)
            => _write.AddRange(entities);

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            => _write.AddRangeAsync(entities, cancellationToken);

        public long Count()
            => _read.Count();

        public long Count(Expression<Func<TEntity, bool>> predicate)
            => _read.Count(predicate);

        public Task<long> CountAsync(CancellationToken cancellation = default)
            => _read.CountAsync(cancellation);

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
            => _read.CountAsync(predicate, cancellation);

        public void Delete(TEntity entity)
            => _write.Delete(entity);

        public void Delete<TKey>(TKey id) where TKey : IEquatable<TKey>
            => _write.Delete(id);

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
            => _write.DeleteAsync(entity, cancellationToken);

        public Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey>
            => _write.DeleteAsync(id, cancellationToken);

        public void DeleteRange(IEnumerable<TEntity> entities)
            => _write.DeleteRange(entities);

        public void DeleteRange<TKey>(IEnumerable<TKey> ids) where TKey : IEquatable<TKey>
            => _write.DeleteRange(ids);

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            => _write.DeleteRangeAsync(entities, cancellationToken);

        public Task DeleteRangeAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) where TKey : IEquatable<TKey>
            => _write.DeleteRangeAsync(ids, cancellationToken);

        public void DetatchFromTracking(TEntity entity)
            => _read.DetatchFromTracking(entity);

        public TEntity? Find<TKey>(TKey key) where TKey : IEquatable<TKey>
            => _read.Find(key);

        public ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default) where TKey : IEquatable<TKey>
            => _read.FindAsync(key, cancellation);

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
            => _read.FirstOrDefault(predicate);

        public TEntity? FirstOrDefault(ISpecification<TEntity> specification)
            => _read.FirstOrDefault(specification);

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
            => _read.FirstOrDefaultAsync(predicate, cancellation);

        public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
            => _read.FirstOrDefaultAsync(specification, cancellation);

        public IList<TEntity> GetAll()
            => _read.GetAll();

        public IList<TEntity> GetAll(ISpecification<TEntity> specification)
            => _read.GetAll(specification);

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default)
            => _read.GetAllAsync(cancellation);

        public Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
            => _read.GetAllAsync(specification, cancellation);

        public bool IsExists(Expression<Func<TEntity, bool>> predicate)
            => _read.IsExists(predicate);

        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
            => _read.IsExistsAsync(predicate, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(IQuery query, CancellationToken cancellation = default)
            => _read.QueryAsync(query, cancellation);

        public Task<IQueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
            => _read.QueryAsync(specification, cancellation);

        public void Save()
            => _write.Save();

        public Task SaveAsync(CancellationToken cancellationToken = default)
            => _write.SaveAsync(cancellationToken);

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
            => _read.SingleOrDefault(predicate);

        public TEntity? SingleOrDefault(ISpecification<TEntity> specification)
            => _read.SingleOrDefault(specification);

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
            => _read.SingleOrDefaultAsync(predicate, cancellation);

        public Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
            => _read.SingleOrDefaultAsync(specification, cancellation);

        public IAsyncEnumerable<TEntity> StreamAllAsync()
            => _read.StreamAllAsync();

        public IAsyncEnumerable<TEntity> StreamAllAsync(ISpecification<TEntity> specification)
            => _read.StreamAllAsync(specification);

        public IAsyncEnumerable<TEntity> StreamWhereAsync(Expression<Func<TEntity, bool>> predicate)
            => _read.StreamWhereAsync(predicate);

        public IAsyncEnumerable<TEntity> StreamWhereAsync(ISpecification<TEntity> specification)
            => _read.StreamWhereAsync(specification);

        public void Update(TEntity entity)
            => _write.Update(entity);

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
            => _write.UpdateAsync(entity, cancellationToken);

        public void UpdateRange(IEnumerable<TEntity> entities)
            => _write.UpdateRange(entities);

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            => _write.UpdateRangeAsync(entities, cancellationToken);

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
            => _read.Where(predicate);

        public IList<TEntity> Where(ISpecification<TEntity> specification)
            => _read.Where(specification);

        public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
            => _read.WhereAsync(predicate, cancellation);

        public Task<List<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
            => _read.WhereAsync(specification, cancellation);
    }
}
