using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.DependencyInjection;
using Sumapap.Persistence.EfCore.Repositories;
using Sumapap.Persistence.EfCore.UnitOfWork;

namespace Sumapap.Persistence.EfCore.DependencyInjection
{
    public static class RepositoryRegistrationBuilderExtensions
    {
        extension(IRepositoryRegistrationBuilder builder)
        {
            public IRepositoryRegistrationBuilder AddGenericRepositories(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            {
                builder.AddGenericRepository(typeof(IReadWriteRepository<,>), typeof(ReadWriteRepository<,>), serviceLifetime);
                builder.AddGenericRepository(typeof(IReadRepository<,>), typeof(ReadRepository<,>), serviceLifetime);
                builder.AddGenericRepository(typeof(IWriteRepository<,>), typeof(WriteRepository<,>), serviceLifetime);

                return builder
                    .RegisterUnitOfWork(serviceLifetime);
            }

            public IRepositoryRegistrationBuilder AddCachedGenericRepositories(
                ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            {
                builder.AddGenericRepository(typeof(IReadWriteRepository<,>), typeof(ReadWriteRepository<,>), serviceLifetime)
                    .AllowCaching();
                builder.AddGenericRepository(typeof(IReadRepository<,>), typeof(ReadRepository<,>), serviceLifetime)
                    .AllowCaching();
                builder.AddGenericRepository(typeof(IWriteRepository<,>), typeof(WriteRepository<,>), serviceLifetime)
                    .AllowCaching();

                return builder
                    .RegisterUnitOfWork(serviceLifetime);
            }

            public IRepositoryRegistrationBuilder AddCachedGenericRepositories(
                Action<RepositoryCacheConfiguration> configuration,
                ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            {
                builder.AddGenericRepository(typeof(IReadWriteRepository<,>), typeof(ReadWriteRepository<,>), serviceLifetime)
                    .AllowCaching(configuration);
                builder.AddGenericRepository(typeof(IReadRepository<,>), typeof(ReadRepository<,>), serviceLifetime)
                    .AllowCaching(configuration);
                builder.AddGenericRepository(typeof(IWriteRepository<,>), typeof(WriteRepository<,>), serviceLifetime)
                    .AllowCaching(configuration);

                return builder
                    .RegisterUnitOfWork(serviceLifetime);
            }

            private IRepositoryRegistrationBuilder RegisterUnitOfWork(ServiceLifetime serviceLifetime)
            {
                if (serviceLifetime is ServiceLifetime.Scoped)
                {
                    builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
                }
                else if (serviceLifetime is ServiceLifetime.Transient)
                {
                    builder.Services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
                }
                else
                {
                    throw new NotSupportedException($"Service lifetime {serviceLifetime} is not supported for generic repositories. Only Scoped and Transient are supported.");
                }

                return builder;
            }
        }
    }
}
