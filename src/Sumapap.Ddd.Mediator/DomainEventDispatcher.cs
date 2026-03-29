using Mediator;
using Sumapap.Ddd.Abstractions;
using Sumapap.Ddd.Mediator.Abstractions;

namespace Sumapap.Ddd.Mediator
{
    public sealed class DomainEventDispatcher(IPublisher _publisher)
        : IDomainEventDispatcher
    {
        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(domainEvents.Select(@event => _publisher.Publish((@event as IDomainEventAdapter)!, cancellationToken).AsTask()));
        }
    }
}
