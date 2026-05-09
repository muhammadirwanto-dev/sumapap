using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.DependencyInjection.Builder;
using Sumapap.Persistence.Abstraction;

namespace Sumapap.Persistence.DependencyInjection.Builder
{
    public sealed class RepositoryServiceBuilder(SumapapServiceBuilder _builder) : IBuilder<SumapapServiceBuilder>
    {
        private readonly IServiceCollection _services = _builder.Build();
        private readonly List<RepositoryRegistration> _registrations = [];

        public SumapapServiceBuilder Build()
        {
            foreach (var registration in _registrations)
            {
                RegisterRepository(registration);
            }

            return _builder;
        }

        public RepositoryServiceBuilder AddScopedRepository<TService, TImpl, TEntity>(bool allowCaching = true)
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity
        {
            _registrations.Add(new RepositoryRegistration(
                ServiceLifetime.Scoped,
                typeof(TService),
                typeof(TImpl),
                typeof(TEntity),
                allowCaching
                ));

            return this;
        }

        public RepositoryServiceBuilder AddScopedRepository<TImpl, TEntity>(bool allowCaching = true)
            where TImpl : class
            where TEntity : class, IEntity
        {
            _registrations.Add(new RepositoryRegistration(
                ServiceLifetime.Scoped,
                typeof(TImpl),
                typeof(TImpl),
                typeof(TEntity),
                allowCaching
                ));

            return this;
        }

        public RepositoryServiceBuilder AddTransientRepository<TService, TImpl, TEntity>(bool allowCaching = true)
            where TService : class
            where TImpl : class, TService
            where TEntity : class, IEntity
        {
            _registrations.Add(new RepositoryRegistration(
                ServiceLifetime.Transient,
                typeof(TService),
                typeof(TImpl),
                typeof(TEntity),
                allowCaching
                ));

            return this;
        }

        public RepositoryServiceBuilder AddTransientRepository<TImpl, TEntity>(bool allowCaching = true)
            where TImpl : class
            where TEntity : class, IEntity
        {
            _registrations.Add(new RepositoryRegistration(
                ServiceLifetime.Transient,
                typeof(TImpl),
                typeof(TImpl),
                typeof(TEntity),
                allowCaching
                ));

            return this;
        }

        private void RegisterRepository(RepositoryRegistration registration)
        {
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
            Type rr = typeof(IReadRepository<>).MakeGenericType(entityType);
            Type wr = typeof(IWriteRepository<>).MakeGenericType(entityType);
            Type rw = typeof(IReadWriteRepository<>).MakeGenericType(entityType);
            Type cr = typeof(IRepository<>).MakeGenericType(entityType);

            services.AddScoped(implType);

            if (rr.IsAssignableFrom(implType))
            {
                services.AddScoped(rr, sp => sp.GetRequiredService(implType));
            }

            if (wr.IsAssignableFrom(implType))
            {
                services.AddScoped(wr, sp => sp.GetRequiredService(implType));
            }

            if (rw.IsAssignableFrom(implType))
            {
                services.AddScoped(rw, sp => sp.GetRequiredService(implType));
            }

            if (cr.IsAssignableFrom(implType))
            {
                services.AddScoped(cr, sp => sp.GetRequiredService(implType));
            }
        }

        public static void AddTransientRepository(
            IServiceCollection services,
            Type serviceType,
            Type implType,
            Type entityType)
        {
            AddTransientRepository(services, implType, entityType);
            services.AddTransient(serviceType, sp => sp.GetRequiredService(implType));
        }

        public static void AddTransientRepository(
            IServiceCollection services,
            Type implType,
            Type entityType)
        {
            Type rr = typeof(IReadRepository<>).MakeGenericType(entityType);
            Type wr = typeof(IWriteRepository<>).MakeGenericType(entityType);
            Type rw = typeof(IReadWriteRepository<>).MakeGenericType(entityType);
            Type cr = typeof(IRepository<>).MakeGenericType(entityType);

            services.AddTransient(implType);

            if (rr.IsAssignableFrom(implType))
            {
                services.AddTransient(rr, sp => sp.GetRequiredService(implType));
            }

            if (wr.IsAssignableFrom(implType))
            {
                services.AddTransient(wr, sp => sp.GetRequiredService(implType));
            }

            if (rw.IsAssignableFrom(implType))
            {
                services.AddTransient(rw, sp => sp.GetRequiredService(implType));
            }

            if (cr.IsAssignableFrom(implType))
            {
                services.AddTransient(cr, sp => sp.GetRequiredService(implType));
            }
        }
    }
}
