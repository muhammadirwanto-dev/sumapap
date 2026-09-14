using Sumapap.DependencyInjection.Abstractions;
using System.Reflection;

namespace Sumapap.Ddd.DependencyInjection.Abstractions
{
    public interface IDddBuilder : IBuilder<ISumapapServiceBuilder>
    {
        EventDispatcherRegistrationConfigurator AddDomainEventsDispatcher(Assembly[] assemblies);

        IDddBuilder SetStrategy(IEventDispatcherRegistrationStrategy strategy);
    }
}
