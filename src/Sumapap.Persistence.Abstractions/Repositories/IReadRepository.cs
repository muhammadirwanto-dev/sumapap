using System.Linq.Expressions;
using Sumapap.Persistence.Abstractions.Entities;
using Sumapap.Persistence.Abstractions.Specifications;
using Sumapap.Queries.Abstractions;

namespace Sumapap.Persistence.Abstractions.Repositories
{
    /// <summary>
    /// Defines a read-only repository for querying entities of a specified type from a data source.
    /// </summary>
    /// <remarks>This interface provides a set of methods for retrieving entities using various query
    /// patterns, including key-based lookup, predicate expressions, and specifications. It supports both synchronous
    /// and asynchronous operations, as well as streaming for large result sets. Implementations are expected to provide
    /// read-only access and should not expose methods for modifying or deleting entities. Thread safety and query
    /// capabilities may vary depending on the underlying data source.</remarks>
    /// <typeparam name="TEntity">The type of entity managed by the repository. Must implement the IEntity interface.</typeparam>
    public interface IReadRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        TEntity? Find<TKey>(TKey key)
            where TKey : IEquatable<TKey>;

        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity? FirstOrDefault(ISpecification<TEntity> specification);

        TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity? SingleOrDefault(ISpecification<TEntity> specification);

        IList<TEntity> GetAll();

        IList<TEntity> GetAll(ISpecification<TEntity> specification);

        IList<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        IList<TEntity> Where(ISpecification<TEntity> specification);

        bool IsExists(Expression<Func<TEntity, bool>> predicate);

        long Count();

        long Count(Expression<Func<TEntity, bool>> predicate);

        void DetatchFromTracking(TEntity entity);

        ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default)
            where TKey : IEquatable<TKey>;

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default);

        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default);

        Task<List<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default);

        IAsyncEnumerable<TEntity> StreamAllAsync();

        IAsyncEnumerable<TEntity> StreamAllAsync(ISpecification<TEntity> specification);

        Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<List<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default);

        IAsyncEnumerable<TEntity> StreamWhereAsync(Expression<Func<TEntity, bool>> predicate);

        IAsyncEnumerable<TEntity> StreamWhereAsync(ISpecification<TEntity> specification);

        Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<long> CountAsync(CancellationToken cancellation = default);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

        Task<IQueryResult<TEntity>> QueryAsync(IQuery query, CancellationToken cancellation = default);

        Task<IQueryResult<TEntity>> QueryAsync(ISpecification<TEntity> specification, CancellationToken cancellation = default);
    }

    public interface IReadRepository<TEntity, TContext> : IReadRepository<TEntity>
        where TEntity : class, IEntity;
}
