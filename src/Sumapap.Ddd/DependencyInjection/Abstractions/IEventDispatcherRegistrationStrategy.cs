using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.Ddd.DependencyInjection.Abstractions
{
    public interface IEventDispatcherRegistrationStrategy
    {
        void Register(EventDispatcherRegistration registration, IServiceCollection services);
    }
}
