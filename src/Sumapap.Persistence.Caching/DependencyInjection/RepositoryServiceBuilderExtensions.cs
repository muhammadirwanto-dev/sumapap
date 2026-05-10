using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using Sumapap.Persistence.Abstraction;
using Sumapap.Persistence.DependencyInjection.Builder;

namespace Sumapap.Persistence.Caching.DependencyInjection
{
    public static class RepositoryServiceBuilderExtensions
    {
        extension(RepositoryRegistrationBuilder builder)
        {
            public RepositoryRegistrationBuilder UseCaching(Action<CachedFunctionsMapping> configuration)
            {
                builder.Build().Build()
                    .Configure(configuration);

                return builder;
            }

            private CachedFunctionsMapping GetDefaultMapping()
                => new()
                {
                    { nameof(IReadRepository<>.Count), true },
                    { nameof(IReadRepository<>.CountAsync), true },
                    { nameof(IReadRepository<>.Find), true },
                    { nameof(IReadRepository<>.FindAsync), true },
                    { nameof(IReadRepository<>.FirstOrDefault), true },
                    { nameof(IReadRepository<>.FirstOrDefaultAsync), true },
                    { nameof(IReadRepository<>.GetAll), true },
                    { nameof(IReadRepository<>.GetAllAsync), true },
                    { nameof(IReadRepository<>.SingleOrDefault), true },
                    { nameof(IReadRepository<>.SingleOrDefaultAsync), true },
                    { nameof(IReadRepository<>.StreamAllAsync), true },
                    { nameof(IReadRepository<>.StreamWhereAsync), true },
                };
        }
    }
}
