using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstraction;
using Sumapap.Persistence.EfCore.Repositories;
using Sumapap.Persistence.EfCore.UnitOfWork;

namespace Sumapap.Persistence.EfCore
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEfCorePersistence(
            this IServiceCollection services
            ) => services
                .AddScoped(typeof(IReadWriteRepository<,>), typeof(ReadWriteRepository<,>))
                .AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>))
                .AddScoped(typeof(IWriteRepository<,>), typeof(WriteRepository<,>))
                .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        public static IServiceCollection AddEfCorePersistence<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> dbContextOptionsAction
            )
            where TContext : DbContext => services
                .AddEfCorePersistence()
                .AddDbContext<TContext>(dbContextOptionsAction);
    }
}
