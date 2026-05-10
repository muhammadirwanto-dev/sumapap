using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.DependencyInjection.Builder;
using Sumapap.Persistence.Abstraction;

namespace Sumapap.Persistence.DependencyInjection.Builder
{
    public class RepositoryRegistrationBuilder(SumapapServiceBuilder _builder)
        : IBuilder<SumapapServiceBuilder>
    {
        internal readonly IServiceCollection _services = _builder.Services;
        internal readonly List<RepositoryRegistrationEntry> _registrations = [];

        public SumapapServiceBuilder Build()
        {
            var cacheRegistry = GetOrCreateCacheRegistry(_services);

            foreach (var registration in _registrations)
            {
                if (registration.IsGeneric)
                {
                    RegisterGenericRepository(registration);
                }
                else
                {
                    RegisterRepository(registration);
                }

                if (registration.AllowCaching && registration.CachingConfiguration != null)
                {
                    var cacheEntry = new RepositoryCacheEntry
                    {
                        RepositoryType = registration.ImplType,
                        EntityType = registration.EntityType,
                        Lifetime = registration.ServiceLifetime,
                        Configuration = registration.CachingConfiguration,
                        ServiceTypes = registration.IsGeneric
                            ? GetGenericServiceTypes(registration)
                            : GetServiceTypes(registration)
                    };

                    cacheRegistry.Register(cacheEntry);
                }
            }

            return _builder;
        }

        public RepositoryConfigurator AddGenericRepository(Type serviceType, Type implType, ServiceLifetime serviceLifetime)
        {
            _registrations.Add(new RepositoryRegistrationEntry(
                serviceLifetime,
                serviceType,
                implType,
                EntityType: null,
                IsGeneric: true,
                AllowCaching: false
                ));

            return new RepositoryConfigurator(this, _registrations.Count - 1);
        }

        public RepositoryConfigurator<TImpl, TEntity> AddScopedRepository<TImpl, TEntity>()
            where TImpl : class
            where TEntity : class, IEntity
            => AddScopedRepository<TImpl, TImpl, TEntity>();

        public RepositoryConfigurator<TImpl, TEntity> AddScopedRepository<TService, TImpl, TEntity>()
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity
        {
            _registrations.Add(new RepositoryRegistrationEntry(
                ServiceLifetime.Scoped,
                typeof(TService),
                typeof(TImpl),
                typeof(TEntity),
                IsGeneric: false,
                AllowCaching: false
                ));

            return new RepositoryConfigurator<TImpl, TEntity>(this, _registrations.Count - 1);
        }

        public RepositoryConfigurator<TImpl, TEntity> AddTransientRepository<TImpl, TEntity>()
            where TImpl : class
            where TEntity : class, IEntity
            => AddTransientRepository<TImpl, TImpl, TEntity>();

        public RepositoryConfigurator<TImpl, TEntity> AddTransientRepository<TService, TImpl, TEntity>()
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity
        {
            _registrations.Add(new RepositoryRegistrationEntry(
                ServiceLifetime.Transient,
                typeof(TService),
                typeof(TImpl),
                typeof(TEntity),
                IsGeneric: false,
                AllowCaching: false
                ));

            return new RepositoryConfigurator<TImpl, TEntity>(this, _registrations.Count - 1);
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

        private static List<Type> GetServiceTypes(RepositoryRegistrationEntry registration)
        {
            var serviceTypes = new List<Type>();
            var entityType = registration.EntityType;
            var implType = registration.ImplType;

            if (entityType == null)
            {
                throw new InvalidOperationException("EntityType cannot be null for non-generic repository service type resolution.");
            }

            if (registration.AbstractType != implType)
            {
                serviceTypes.Add(registration.AbstractType);
            }

            foreach (var interf in GetInterfaceTypes(entityType))
            {
                if (interf.IsAssignableFrom(implType))
                {
                    serviceTypes.Add(interf);
                }
            }

            return serviceTypes;
        }

        private static List<Type> GetGenericServiceTypes(RepositoryRegistrationEntry registration)
        {
            var serviceTypes = new List<Type>();
            if (registration.AbstractType != registration.ImplType)
            {
                serviceTypes.Add(registration.AbstractType);
            }

            // For generic repositories, we add the open generic types
            // The implementation type is typically the open generic
            serviceTypes.Add(registration.ImplType);

            return serviceTypes;
        }

        private void RegisterRepository(RepositoryRegistrationEntry registration)
        {
            if (registration.EntityType == null)
            {
                throw new InvalidOperationException("EntityType cannot be null for non-generic repository registration.");
            }

            switch (registration.ServiceLifetime)
            {
                case ServiceLifetime.Scoped:
                    AddScopedRepository(_services, registration.AbstractType, registration.ImplType, registration.EntityType);
                    break;
                case ServiceLifetime.Transient:
                    AddTransientRepository(_services, registration.AbstractType, registration.ImplType, registration.EntityType);
                    break;
                default:
                    throw new NotSupportedException($"Service lifetime {registration.ServiceLifetime} is not supported for repository registration.");
            }
        }

        private void RegisterGenericRepository(RepositoryRegistrationEntry registration)
        {
            switch (registration.ServiceLifetime)
            {
                case ServiceLifetime.Scoped:
                    _services.AddScoped(registration.AbstractType, registration.ImplType);
                    break;
                case ServiceLifetime.Transient:
                    _services.AddTransient(registration.AbstractType, registration.ImplType);
                    break;
                default:
                    throw new NotSupportedException($"Service lifetime {registration.ServiceLifetime} is not supported for repository registration.");
            }
        }

        private static void AddScopedRepository(
            IServiceCollection services,
            Type serviceType,
            Type implType,
            Type entityType)
        {
            AddScopedRepository(services, implType, entityType);
            services.AddScoped(serviceType, sp => sp.GetRequiredService(implType));
        }

        private static void AddScopedRepository(
            IServiceCollection services,
            Type implType,
            Type entityType)
        {
            services.AddScoped(implType);

            foreach (var interf in GetInterfaceTypes(entityType))
            {
                if (interf.IsAssignableFrom(implType))
                {
                    services.AddScoped(interf, sp => sp.GetRequiredService(implType));
                }
            }
        }

        private static void AddTransientRepository(
            IServiceCollection services,
            Type serviceType,
            Type implType,
            Type entityType)
        {
            AddTransientRepository(services, implType, entityType);
            services.AddTransient(serviceType, sp => sp.GetRequiredService(implType));
        }

        private static void AddTransientRepository(
            IServiceCollection services,
            Type implType,
            Type entityType)
        {
            services.AddTransient(implType);

            foreach (var interf in GetInterfaceTypes(entityType))
            {
                if (interf.IsAssignableFrom(implType))
                {
                    services.AddTransient(interf, sp => sp.GetRequiredService(implType));
                }
            }
        }

        private static IEnumerable<Type> GetInterfaceTypes(Type entityType) => [
            typeof(IReadRepository<>).MakeGenericType(entityType),
            typeof(IWriteRepository<>).MakeGenericType(entityType),
            typeof(IReadWriteRepository<>).MakeGenericType(entityType),
            typeof(IRepository<>).MakeGenericType(entityType)
        ];
    }
}
