using Microsoft.Extensions.DependencyInjection;
using Sumapap.Ddd.Abstractions;

namespace Sumapap.Ddd.Dispatcher
{
    public sealed class DomainEventDispatcher(IServiceProvider _provider)
        : IDomainEventDispatcher
    {
        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var @event in domainEvents)
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
                var handlers = _provider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    await ((dynamic)handler!).HandleAsync((dynamic)@event, cancellationToken);
                }
            }
        }
    }
}
