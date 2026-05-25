using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sumapap.Ddd.Abstractions.Events;
using Sumapap.Ddd.DependencyInjection.Abstractions;
using Sumapap.Ddd.Events;

namespace Sumapap.Ddd.DependencyInjection.Strategies
{
    internal class DefaultEventDispatcherRegistrationStrategy : IEventDispatcherRegistrationStrategy
    {
        public void Register(EventDispatcherRegistration registration, IServiceCollection services)
        {
            var assemblies = registration.Assemblies;
            if (assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(t => t is { IsClass: true, IsAbstract: false })
                    .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
                    .Where(x => x.Interface.IsGenericType &&
                                x.Interface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

                foreach (var handler in handlers)
                {
                    services.AddScoped(handler.Interface, handler.Implementation);
                }
            }

            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        }
    }
}
