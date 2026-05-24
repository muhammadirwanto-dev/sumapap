using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.DependencyInjection.Abstractions
{
    public interface IPersistenceBuilder : IBuilder<ISumapapServiceBuilder>
    {
        internal IList<RepositoryRegistration> Registrations { get; }

        RepositoryConfigurator AddGenericRepository(Type serviceType, Type implType, ServiceLifetime serviceLifetime);

        RepositoryConfigurator AddScopedRepository<TImpl>()
            where TImpl : class;

        RepositoryConfigurator AddScopedRepository<TService, TImpl>()
            where TService : class
            where TImpl : class, TService;

        RepositoryConfigurator AddTransientRepository<TImpl>()
            where TImpl : class;

        RepositoryConfigurator AddTransientRepository<TService, TImpl>()
            where TService : class
            where TImpl : class, TService;

        IPersistenceBuilder AddVisitor(IRepositoryRegistrationVisitor visitor);
    }
}
