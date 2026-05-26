using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.DependencyInjection.Abstractions;
using Sumapap.Persistence.Extensions;

namespace Sumapap.Persistence.DependencyInjection
{
    internal class PersistenceBuilder(ISumapapServiceBuilder _builder) : IPersistenceBuilder
    {
        private readonly IServiceCollection _services = _builder.Services;
        private readonly List<RepositoryRegistration> _registrations = [];
        private readonly List<IRepositoryRegistrationVisitor> _visitors = [];

        IList<RepositoryRegistration> IPersistenceBuilder.Registrations => _registrations;

        IServiceCollection IBuilder<ISumapapServiceBuilder>.Services => _services;

        public ISumapapServiceBuilder Build()
        {
            foreach (var registration in _registrations)
            {
                RegisterRepository(registration);
            }

            // allow visitors to process registrations for cross-cutting concerns
            foreach (var visitor in _visitors)
            {
                foreach (var registration in _registrations)
                {
                    registration.Accept(visitor, _services);
                }
            }

            return _builder;
        }

        public RepositoryConfigurator AddGenericRepository(Type serviceType, Type implType, ServiceLifetime serviceLifetime)
        {
            _registrations.Add(new RepositoryRegistration(
                serviceLifetime,
                serviceType,
                implType,
                IsGeneric: true,
                AllowCaching: false,
                Decorator: null
                ));

            return new RepositoryConfigurator(this, _registrations.Count - 1);
        }

        public RepositoryConfigurator AddScopedRepository<TImpl>()
            where TImpl : class
            => AddScopedRepository<TImpl, TImpl>();

        public RepositoryConfigurator AddScopedRepository<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            _registrations.Add(new RepositoryRegistration(
                ServiceLifetime.Scoped,
                typeof(TService),
                typeof(TImpl),
                IsGeneric: false,
                AllowCaching: false,
                Decorator: null
                ));

            return new RepositoryConfigurator(this, _registrations.Count - 1);
        }

        public RepositoryConfigurator AddTransientRepository<TImpl>()
            where TImpl : class
            => AddTransientRepository<TImpl, TImpl>();

        public RepositoryConfigurator AddTransientRepository<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            _registrations.Add(new RepositoryRegistration(
                ServiceLifetime.Transient,
                typeof(TService),
                typeof(TImpl),
                IsGeneric: false,
                AllowCaching: false,
                Decorator: null
                ));

            return new RepositoryConfigurator(this, _registrations.Count - 1);
        }

        public IPersistenceBuilder AddVisitor(IRepositoryRegistrationVisitor visitor)
        {
            ArgumentNullException.ThrowIfNull(visitor);

            _visitors.Add(visitor);

            return this;
        }

        private void RegisterRepository(RepositoryRegistration registration)
        {
            if (registration.IsGeneric)
            {
                RegisterGenericRepository(registration);

                return;
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

        private void RegisterGenericRepository(RepositoryRegistration registration)
        {
            if (!registration.IsGeneric)
            {
                throw new InvalidOperationException("Attempted to register a non-generic repository as generic.");
            }

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

        private static void AddScopedRepository(
            IServiceCollection services,
            Type serviceType,
            Type implType)
        {
            services.AddScoped(implType);
            services.AddScoped(serviceType, sp => sp.GetRequiredService(implType));

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
            services.AddTransient(implType);
            services.AddTransient(serviceType, sp => sp.GetRequiredService(implType));

            foreach (var interf in implType.GetRepositoryInterfacesTypes())
            {
                services.AddTransient(interf, sp => sp.GetRequiredService(implType));
            }
        }
    }
}
