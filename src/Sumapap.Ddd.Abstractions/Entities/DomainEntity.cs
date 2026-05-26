using System.Collections.Concurrent;
using Sumapap.Ddd.Abstractions.Events;

namespace Sumapap.Ddd.Abstractions.Entities
{
    /// <summary>
    /// Base class for domain entities that can raise and manage domain events.
    /// Uses a thread-safe queue for event storage.
    /// </summary>
    public abstract class DomainEntity
    {
        private readonly ConcurrentQueue<IDomainEvent> _events = [];

        /// <summary>
        /// Adds a domain event to the entity's event queue.
        /// </summary>
        /// <param name="domainEvent">The domain event to add.</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _events.Enqueue(domainEvent);
        }

        /// <summary>
        /// Retrieves all pending domain events and clears the queue.
        /// </summary>
        /// <returns>A read-only list of domain events.</returns>
        public IReadOnlyList<IDomainEvent> ConsumeEvents()
        {
            var events = _events.ToArray();

            _events.Clear();

            return events;
        }

        /// <summary>
        /// Retrieves all pending domain events without clearing the queue.
        /// </summary>
        /// <returns>A read-only list of domain events.</returns>
        public IReadOnlyList<IDomainEvent> GetEvents() => [.. _events];

        /// <summary>
        /// Clears all pending domain events from the queue.
        /// </summary>
        public void ClearEvents() => _events.Clear();
    }
}
