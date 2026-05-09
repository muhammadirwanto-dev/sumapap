using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstraction;
using Sumapap.Persistence.DependencyInjection.Builder;
using Sumapap.Persistence.EfCore.Repositories;
using Sumapap.Persistence.EfCore.UnitOfWork;

namespace Sumapap.Persistence.EfCore.DependencyInjection
{
    public static class RepositoryServiceBuilderExtensions
    {
        extension(RepositoryServiceBuilder builder)
        {
            public RepositoryServiceBuilder AddGenericRepositories()
            {
                builder.Services
                    .AddScoped(typeof(IReadWriteRepository<,>), typeof(ReadWriteRepository<,>))
                    .AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>))
                    .AddScoped(typeof(IWriteRepository<,>), typeof(WriteRepository<,>))
                    .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

                return builder;
            }
        }
    }
}
