using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Caching.FusionCache.Visitors;
using Sumapap.Persistence.Caching.Visitors;
using Sumapap.Persistence.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.Caching.FusionCache.DependencyInjection
{
    public static class RepositoryRegistrationBuilderExtensions
    {
        extension(IPersistenceBuilder builder)
        {
            /// <summary>
            /// Registers generic repositories with FusionCache caching enabled for all methods.
            /// </summary>
            /// <param name="serviceLifetime">The lifetime for the repository services.</param>
            /// <returns>The same builder for method chaining.</returns>
            public IServiceCollection UseCacheProvider()
            {
                builder
                    .AddVisitor(new CachedRepositoryVisitor())
                    .AddVisitor(new RepositoryDecorationVisitor());

                return builder.Services;
            }
        }
    }
}
