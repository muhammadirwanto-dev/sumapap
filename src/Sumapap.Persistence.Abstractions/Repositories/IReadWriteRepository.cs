using Sumapap.Persistence.Abstractions.Entities;

namespace Sumapap.Persistence.Abstractions.Repositories
{
    /// <summary>
    /// Defines a repository with both read and write operations for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public interface IReadWriteRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
        where TEntity : class, IEntity;

    /// <summary>
    /// Defines a repository with both read and write operations, associated with a specific context.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TContext">The type of the data context.</typeparam>
    public interface IReadWriteRepository<TEntity, TContext> : IReadWriteRepository<TEntity>,
        IReadRepository<TEntity, TContext>, IWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity;
}
