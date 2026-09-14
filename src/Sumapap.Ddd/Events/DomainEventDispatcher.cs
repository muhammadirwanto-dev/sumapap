using Microsoft.Extensions.DependencyInjection;
using Sumapap.Ddd.Abstractions.Events;

namespace Sumapap.Ddd.Events
{
    internal sealed class DomainEventDispatcher(IServiceProvider _provider)
        : IDomainEventDispatcher
    {
        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            using var scoped = _provider.CreateScope();

            foreach (var @event in domainEvents)
            {
                await InternalDispatchAsync(scoped, @event, cancellationToken);
            }
        }

        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            using var scoped = _provider.CreateScope();

            await InternalDispatchAsync(scoped, domainEvent, cancellationToken);
        }

        private static async Task InternalDispatchAsync(IServiceScope scope, IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                if (method is not null)
                {
                    await (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;
                }
            }
        }
    }
}
