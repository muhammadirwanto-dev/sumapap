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
        private readonly System.Collections.Concurrent.ConcurrentQueue<IDomainEvent> _events = [];

        /// <summary>
        /// Adds a domain event to the entity's event queue.
        /// </summary>
        /// <param name="domainEvent">The domain event to add.</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _events.Enqueue(domainEvent);
        }

        /// <summary>
        /// Attempts to consume a single domain event from the queue.
        /// </summary>
        /// <param name="domainEvent">The domain event to consume.</param>
        /// <returns>True if a domain event was successfully consumed; otherwise, false.</returns>
        public bool TryConsumeEvent(out IDomainEvent? domainEvent) => _events.TryDequeue(out domainEvent);

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
