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

        private void RegisterRepository(RepositoryRegistrationEntry registration)
        {
            if (registration.EntityType == null)
            {
                throw new InvalidOperationException("EntityType cannot be null for non-generic repository registration.");
            }

            switch (registration.ServiceLifetime)
            {
                case ServiceLifetime.Scoped:
                    AddScopedRepository(_services, registration.AbstractType, registration.ImplType);
                    break;
                case ServiceLifetime.Transient:
                    AddTransientRepository(_services, registration.AbstractType, registration.ImplType);
                    break;
                default:
                    throw new NotSupportedException($"Service lifetime {registration.ServiceLifetime} is not supported for repository registration.");
            }
        }

        private void RegisterGenericRepository(RepositoryRegistrationEntry registration)
        {
            if (registration.IsReadWriteRepository())
            {
                switch (registration.ServiceLifetime)
                {
                    case ServiceLifetime.Scoped:
                        _services.TryAddScoped(registration.AbstractType, registration.ImplType);
                        break;
                    case ServiceLifetime.Transient:
                        _services.TryAddTransient(registration.AbstractType, registration.ImplType);
                        break;
                    default:
                        throw new NotSupportedException($"Service lifetime {registration.ServiceLifetime} is not supported for repository registration.");
                }
            }
            else
            {
                switch (registration.ServiceLifetime)
                {
                    case ServiceLifetime.Scoped:
                        AddScopedRepository(_services, registration.ImplType);
                        break;
                    case ServiceLifetime.Transient:
                        AddTransientRepository(_services, registration.ImplType);
                        break;
                    default:
                        throw new NotSupportedException($"Service lifetime {registration.ServiceLifetime} is not supported for repository registration.");
                }
            }
        }

        private static void AddScopedRepository(
            IServiceCollection services,
            Type serviceType,
            Type implType)
        {
            AddScopedRepository(services, implType);
            services.AddScoped(serviceType, sp => sp.GetRequiredService(implType));
        }

        private static void AddScopedRepository(
            IServiceCollection services,
            Type implType)
        {
            services.AddScoped(implType);

            foreach (var interf in implType.GetRepositoryInterfacesTypes())
            {
                services.AddScoped(interf, sp => sp.GetRequiredService(implType));
            }
        }

        private static void AddTransientRepository(
            IServiceCollection services,
            Type serviceType,
            Type implType)
        {
            AddTransientRepository(services, implType);
            services.AddTransient(serviceType, sp => sp.GetRequiredService(implType));
        }

        private static void AddTransientRepository(
            IServiceCollection services,
            Type implType)
        {
            services.AddTransient(implType);

            foreach (var interf in implType.GetRepositoryInterfacesTypes())
            {
                services.AddTransient(interf, sp => sp.GetRequiredService(implType));
            }
        }
    }
}
