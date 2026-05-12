using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.Visitors;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Caching.FusionCache.DependencyInjection
{
    public static class RepositoryRegistrationBuilderExtensions
    {
        extension(IRepositoryRegistrationBuilder builder)
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
                    .DecorateRepositories(builder.Registrations);

                return builder.Services;
            }

            private void DecorateRepositories(IEnumerable<RepositoryRegistrationEntry> entries)
            {
                foreach (var entry in entries)
                {
                    //builder.Services.Decorate(entry., typeof(CachedReadRepository<>));
                }
            }
        }
    }
}
