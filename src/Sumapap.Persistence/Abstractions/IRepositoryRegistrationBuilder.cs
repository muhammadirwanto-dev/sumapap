using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Abstractions
{
    public interface IRepositoryRegistrationBuilder : IBuilder<ISumapapServiceBuilder>
    {
        internal IList<RepositoryRegistrationEntry> Registrations { get; }

        RepositoryConfigurator AddGenericRepository(Type serviceType, Type implType, ServiceLifetime serviceLifetime);

        RepositoryConfigurator<TImpl, TEntity> AddScopedRepository<TImpl, TEntity>()
            where TImpl : class
            where TEntity : class, IEntity;

        RepositoryConfigurator<TImpl, TEntity> AddScopedRepository<TService, TImpl, TEntity>()
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity;

        RepositoryConfigurator<TImpl, TEntity> AddTransientRepository<TImpl, TEntity>()
            where TImpl : class
            where TEntity : class, IEntity;

        RepositoryConfigurator<TImpl, TEntity> AddTransientRepository<TService, TImpl, TEntity>()
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity;

        IRepositoryRegistrationBuilder AddVisitor(IRepositoryRegistrationVisitor visitor);
    }
}
