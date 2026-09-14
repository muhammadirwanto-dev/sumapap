using Sumapap.Ddd.DependencyInjection;
using Sumapap.Ddd.Mediator.DependencyInjection.Strategies;

namespace Sumapap.Ddd.Mediator.DependencyInjection
{
    public static class EventDispatcherRegistrationConfiguratorExtensions
    {
        public static EventDispatcherRegistrationConfigurator UseMediatorForDomainEventHandler(this EventDispatcherRegistrationConfigurator configurator)
        {
            configurator.Builder.SetStrategy(new MediatorEventDispatcherRegistrationStrategy());

            return configurator;
        }
    }
}
