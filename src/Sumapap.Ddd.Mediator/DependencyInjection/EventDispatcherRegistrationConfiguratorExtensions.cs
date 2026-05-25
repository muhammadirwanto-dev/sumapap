using Sumapap.Ddd.DependencyInjection;
using Sumapap.Ddd.Mediator.DependencyInjection.Strategies;

namespace Sumapap.Ddd.Mediator.DependencyInjection
{
    public static class EventDispatcherRegistrationConfiguratorExtensions
    {
        extension(EventDispatcherRegistrationConfigurator configurator)
        {
            public EventDispatcherRegistrationConfigurator UseMediatorForDomainEventHandler()
            {
                configurator.Builder.SetStrategy(new MediatorEventDispatcherRegistrationStrategy());

                return configurator;
            }
        }
    }
}
