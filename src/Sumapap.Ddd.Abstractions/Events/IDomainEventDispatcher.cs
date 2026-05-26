namespace Sumapap.Ddd.Abstractions.Events
{
    /// <summary>
    /// Dispatches domain events to their registered handlers.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Dispatches a collection of domain events to all registered handlers asynchronously.
        /// </summary>
        /// <param name="domainEvents">The domain events to dispatch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
