namespace Sumapap.Ddd.Abstractions.Events
{
    /// <summary>
    /// Defines a handler for domain events of a specific type.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event to handle.</typeparam>
    public interface IDomainEventHandler<in TEvent>
        where TEvent : IDomainEvent
    {
        /// <summary>
        /// Handles the specified domain event asynchronously.
        /// </summary>
        /// <param name="domainEvent">The domain event to handle.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
