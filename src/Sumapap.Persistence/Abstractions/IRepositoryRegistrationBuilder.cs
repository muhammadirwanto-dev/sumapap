using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Abstractions
{
    public interface IRepositoryRegistrationBuilder : IBuilder<ISumapapServiceBuilder>
    {
        public RepositoryConfigurator AddGenericRepository(Type serviceType, Type implType, ServiceLifetime serviceLifetime);

        public RepositoryConfigurator<TImpl, TEntity> AddScopedRepository<TImpl, TEntity>()
            where TImpl : class
            where TEntity : class, IEntity;

        public RepositoryConfigurator<TImpl, TEntity> AddScopedRepository<TService, TImpl, TEntity>()
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity;

        public RepositoryConfigurator<TImpl, TEntity> AddTransientRepository<TImpl, TEntity>()
            where TImpl : class
            where TEntity : class, IEntity;

        public RepositoryConfigurator<TImpl, TEntity> AddTransientRepository<TService, TImpl, TEntity>()
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity;

        public IRepositoryRegistrationBuilder AddVisitor(IRepositoryRegistrationVisitor visitor);
    }
}
