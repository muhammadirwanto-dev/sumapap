using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Caching.FusionCache.Visitors;
using Sumapap.Persistence.Caching.Visitors;
using Sumapap.Persistence.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.Caching.FusionCache.DependencyInjection
{
    public static class RepositoryRegistrationBuilderExtensions
    {
        /// <summary>
        /// Registers generic repositories with FusionCache caching enabled for all methods.
        /// </summary>
        /// <param name="builder">The persistence builder.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection UseCacheProvider(this IPersistenceBuilder builder)
        {
            builder
                .AddVisitor(new CachedRepositoryVisitor())
                .AddVisitor(new RepositoryDecorationVisitor());

            return builder.Services;
        }
    }
}
