using System.Reflection;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Ddd.DependencyInjection.Abstractions
{
    public interface IDddBuilder : IBuilder<ISumapapServiceBuilder>
    {
        EventDispatcherRegistrationConfigurator AddDomainEventsDispatcher(Assembly[] assemblies);

        IDddBuilder SetStrategy(IEventDispatcherRegistrationStrategy strategy);
    }
}
