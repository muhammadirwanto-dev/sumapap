using Microsoft.Extensions.DependencyInjection;
using Sumapap.Ddd.Abstractions.Events;
using Sumapap.Ddd.DependencyInjection;
using Sumapap.Ddd.DependencyInjection.Abstractions;
using Sumapap.Ddd.Mediator.Events;

namespace Sumapap.Ddd.Mediator.DependencyInjection.Strategies
{
    internal class MediatorEventDispatcherRegistrationStrategy : IEventDispatcherRegistrationStrategy
    {
        public void Register(EventDispatcherRegistration registration, IServiceCollection services)
        {
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        }
    }
}
