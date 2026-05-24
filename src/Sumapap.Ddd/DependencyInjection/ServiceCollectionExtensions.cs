using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sumapap.Ddd.Abstractions;
using Sumapap.Ddd.Dispatcher;

namespace Sumapap.Ddd.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainEventsDispatcher(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
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
                    // Register as Scoped so handlers can inject your Repository/DbContext
                    services.AddScoped(handler.Interface, handler.Implementation);
                }
            }

            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

            return services;
        }
    }
}
