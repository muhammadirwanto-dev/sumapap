using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Extensions;

namespace Sumapap.Persistence.DependencyInjection
{
    internal class RepositoryRegistrationBuilder(ISumapapServiceBuilder _builder) : IRepositoryRegistrationBuilder
    {
        private readonly IServiceCollection _services = _builder.Services;
        private readonly List<RepositoryRegistrationEntry> _registrations = [];
        private readonly List<IRepositoryRegistrationVisitor> _visitors = [];

        IList<RepositoryRegistrationEntry> IRepositoryRegistrationBuilder.Registrations => _registrations;

        IServiceCollection IBuilder<ISumapapServiceBuilder>.Services => _services;

        public ISumapapServiceBuilder Build()
        {
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
            }

            // allow visitors to process registrations for cross-cutting concerns
            foreach (var visitor in _visitors)
            {
                foreach (var registration in _registrations)
                {
                    visitor.Visit(registration, _services);
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
                AllowCaching: false,
                Decorator: null
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
                AllowCaching: false,
                Decorator: null
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
                AllowCaching: false,
                Decorator: null
                ));

            return new RepositoryConfigurator<TImpl, TEntity>(this, _registrations.Count - 1);
        }

        public IRepositoryRegistrationBuilder AddVisitor(IRepositoryRegistrationVisitor visitor)
        {
            ArgumentNullException.ThrowIfNull(visitor);

            _visitors.Add(visitor);

            return this;
        }

        internal static List<Type> GetServiceTypes(RepositoryRegistrationEntry registration)
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

            foreach (var interf in GetRepositoryInterfaceTypes(entityType))
            {
                if (interf.IsAssignableFrom(implType))
                {
                    serviceTypes.Add(interf);
                }
            }

            return serviceTypes;
        }

        internal static List<Type> GetGenericServiceTypes(RepositoryRegistrationEntry registration)
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
                    if (registration.IsReadWriteRepository())
                    {
                        _services.TryAddScoped(sp => RegisterReadWriteRepository(registration, sp));
                    }
                    else
                    {
                        _services.TryAddScoped(registration.AbstractType, registration.ImplType);
                    }
                    break;
                case ServiceLifetime.Transient:
                    if (registration.IsReadWriteRepository())
                    {
                        _services.TryAddTransient(sp => RegisterReadWriteRepository(registration, sp));
                    }
                    else
                    {
                        _services.TryAddTransient(registration.AbstractType, registration.ImplType);
                    }
                    break;
                default:
                    throw new NotSupportedException($"Service lifetime {registration.ServiceLifetime} is not supported for repository registration.");
            }
        }

        private object RegisterReadWriteRepository(
            RepositoryRegistrationEntry registration,
            IServiceProvider provider)
        {
            var typeArguments = registration.ImplType.GetGenericArguments();
            var entityType = typeArguments.FirstOrDefault(x => typeof(IEntity).IsAssignableFrom(x)) ?? throw new InvalidOperationException("Entity type cannot be determined.");
            var contextType = typeArguments.FirstOrDefault(x => !typeof(IEntity).IsAssignableFrom(x)) ?? throw new InvalidOperationException("Context type cannot be determined.");
            var context = provider.GetRequiredService(contextType);

            var readRepoInterfaceType = typeof(IReadRepository<,>).MakeGenericType(entityType, contextType);
            var writeRepoInterfaceType = typeof(IWriteRepository<,>).MakeGenericType(entityType, contextType);

            var readRepositoryImpl = provider.GetRequiredService(readRepoInterfaceType);
            var writeRepositoryImpl = provider.GetRequiredService(writeRepoInterfaceType);
            var closedReadWriteType = registration.ImplType.IsGenericTypeDefinition
                ? registration.ImplType.MakeGenericType(entityType, contextType)
                : registration.ImplType;

            return Activator.CreateInstance(
                closedReadWriteType,
                context,
                readRepositoryImpl,
                writeRepositoryImpl
            ) ?? throw new InvalidOperationException($"Failed to create an instance of {closedReadWriteType.Name}");
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

            foreach (var interf in GetRepositoryInterfaceTypes(entityType))
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

            foreach (var interf in GetRepositoryInterfaceTypes(entityType))
            {
                if (interf.IsAssignableFrom(implType))
                {
                    services.AddTransient(interf, sp => sp.GetRequiredService(implType));
                }
            }
        }

        private static IEnumerable<Type> GetRepositoryInterfaceTypes(Type entityType) => [
            typeof(IReadRepository<>).MakeGenericType(entityType),
            typeof(IWriteRepository<>).MakeGenericType(entityType),
            typeof(IReadWriteRepository<>).MakeGenericType(entityType),
            typeof(IRepository<>).MakeGenericType(entityType)
        ];
    }
}
