using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Caching.DependencyInjection;
using Sumapap.Persistence.DependencyInjection;
using Sumapap.Persistence.DependencyInjection.Abstractions;
using Sumapap.Persistence.Extensions;

namespace Sumapap.Persistence.Caching.Visitors;

/// <summary>
/// Visitor that processes repository registrations with caching metadata
/// and populates the RepositoryCacheRegistry for provider-specific decoration.
/// </summary>
public class CachedRepositoryVisitor : IRepositoryRegistrationVisitor
{
    public void Visit(RepositoryRegistration entry, IServiceCollection services)
    {
        if (!entry.AllowCaching || entry.Decorator is not CachedRepositoryRegistration cachedEntry)
        {
            return;
        }

        var cacheRegistry = GetOrCreateCacheRegistry(services);
        var cacheEntry = new RepositoryCacheEntry
        {
            RepositoryType = entry.ImplType,
            Configuration = cachedEntry.Configuration,
            ServiceTypes = [.. entry.GetRepositoryInterfacesTypes()]
        };

        cacheRegistry.Register(cacheEntry);
    }

    private static RepositoryCacheRegistry GetOrCreateCacheRegistry(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(RepositoryCacheRegistry) &&
            d.Lifetime == ServiceLifetime.Singleton);

        if (descriptor?.ImplementationInstance is RepositoryCacheRegistry existingRegistry)
        {
            return existingRegistry;
        }

        var newRegistry = new RepositoryCacheRegistry();
        services.AddSingleton(newRegistry);

        return newRegistry;
    }
}
