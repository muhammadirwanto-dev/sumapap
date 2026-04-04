using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sumapap.Persistence.Abstraction;
using Sumapap.Persistence.EfCore.Specifications;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.EfCore.Extensions;

namespace Sumapap.Persistence.EfCore.Repositories
{
    /// <summary>
    /// Generic repository implementation using Entity Framework Core.
    /// This class is designed to be inheritable by specific repositories in consuming applications.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TContext">The DbContext type.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity, TKey, TContext}"/> class.
    /// </remarks>
    /// <param name="dbContext">The EF Core DbContext.</param>
    public class ReadRepository<TEntity, TContext>(TContext @context) : IReadRepository<TEntity, TContext>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        protected readonly TContext _context = @context;
        protected readonly DbSet<TEntity> _set = @context.Set<TEntity>();

        public long Count()
        {
            return _set.Count();
        }

        public long Count(Expression<Func<TEntity, bool>> predicate)
        {
            return _set.Count(predicate);
        }

        public Task<long> CountAsync(CancellationToken cancellation = default)
        {
            return _set.LongCountAsync(cancellation);
        }

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            return _set.LongCountAsync(predicate, cancellation);
        }

        public void DetatchFromTracking(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public TEntity? Find<TKey>(TKey key)
            where TKey : IEquatable<TKey>
        {
            return _set.Find(key);
        }

        public ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default)
            where TKey : IEquatable<TKey>
        {
            return _set.FindAsync([key], cancellation);
        }

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _set.AsNoTracking().FirstOrDefault(predicate);
        }

        public TEntity? FirstOrDefault(ISpecification<TEntity> specification)
        {
            return ApplySpecification(specification).FirstOrDefault();
        }

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            return _set.AsNoTracking().FirstOrDefaultAsync(predicate, cancellation);
        }

        public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            return ApplySpecification(specification).FirstOrDefaultAsync(cancellation);
        }

        public IList<TEntity> GetAll()
        {
            return _set.AsNoTracking().ToList();
        }

        public IList<TEntity> GetAll(ISpecification<TEntity> specification)
        {
            return ApplySpecification(specification).ToList();
        }

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default)
        {
            return _set.AsNoTracking().ToListAsync(cancellation);
        }

        public Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            return ApplySpecification(specification).ToListAsync(cancellation);
        }

        public IAsyncEnumerable<TEntity> StreamAllAsync()
        {
            return _set.AsNoTracking().AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> StreamAllAsync(ISpecification<TEntity> specification)
        {
            return ApplySpecification(specification).AsAsyncEnumerable();
        }

        public bool IsExists(Expression<Func<TEntity, bool>> predicate)
        {
            return _set.Any(predicate);
        }

        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            return _set.AnyAsync(predicate, cancellation);
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _set.AsNoTracking().SingleOrDefault(predicate);
        }

        public TEntity? SingleOrDefault(ISpecification<TEntity> specification)
        {
            return ApplySpecification(specification).SingleOrDefault();
        }

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            return _set.AsNoTracking().SingleOrDefaultAsync(predicate, cancellation);
        }

        public Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            return ApplySpecification(specification).SingleOrDefaultAsync(cancellation);
        }

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _set.AsNoTracking().Where(predicate).ToList();
        }

        public IList<TEntity> Where(ISpecification<TEntity> specification)
        {
            return ApplySpecification(specification).ToList();
        }

        public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
        {
            return _set.AsNoTracking().Where(predicate).ToListAsync(cancellation);
        }

        public Task<List<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            return ApplySpecification(specification).ToListAsync(cancellation);
        }

        public IAsyncEnumerable<TEntity> StreamWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _set.AsNoTracking().Where(predicate).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> StreamWhereAsync(ISpecification<TEntity> specification)
        {
            return ApplySpecification(specification).AsAsyncEnumerable();
        }

        public Task<IQueryResult<TEntity>> QueryAsync(IQuery query, CancellationToken cancellation = default)
        {
            return query.ExecuteAsync(_set.AsNoTracking(), cancellation);
        }

        public Task<IQueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(specification.QueryOptions, nameof(specification));

            var queryable = ApplySpecification(specification);

            return specification.QueryOptions.ExecuteAsync(queryable, cancellation);
        }

        /// <summary>
        /// Central method to apply the logic from an ISpecification object to the DbSet IQueryable.
        /// Delegates the actual application logic to the SpecificationEvaluator helper class.
        /// </summary>
        /// <param name="specification">The specification object containing the query logic.</param>
        /// <param name="evaluateCriteriaOnly">If true, instructs the evaluator to only apply filtering criteria (optimisation for Count/Exists).</param>
        /// <returns>An IQueryable<TEntity> representing the query defined by the specification, ready for execution.</returns>
        protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification, bool evaluateCriteriaOnly = false, bool track = false)
        {
            // Use AsNoTracking() for read operations by default to improve performance.
            return SpecificationEvaluator.Default.Evaluate(track ? _set.AsTracking() : _set.AsNoTracking(), specification, evaluateCriteriaOnly);
        }
    }
}
