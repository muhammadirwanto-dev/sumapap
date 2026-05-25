using Sumapap.Persistence.Abstractions.Entities;
using Sumapap.Persistence.Abstractions.Repositories;

namespace Sumapap.Persistence.Abstractions.UnitOfWorks
{
    /// <summary>
    /// Defines the contract for a Unit of Work.
    /// Manages transactions and coordinates saving changes for multiple repositories.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets the current repository of the specified type.
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        IReadWriteRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, IEntity;

        /// <summary>
        /// Saves all changes made within the current unit of work scope to the underlying data store.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of state entries written to the database (if applicable, like in EF Core).</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Optional: Starts a new transaction explicitly.
        /// Often SaveChangesAsync implicitly handles transactions, but this allows finer control.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Optional: Commits the current transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Optional: Rolls back the current transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the provided action within the unit of work scope.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork;
}
