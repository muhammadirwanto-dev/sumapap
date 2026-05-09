using Sumapap.DependencyInjection.Builder;
using Sumapap.Persistence.DependencyInjection.Builder;

namespace Sumapap.Persistence.DependencyInjection
{
    public static class SumapapBuilderExtensions
    {
        extension(SumapapServiceBuilder builder)
        {
            /// <summary>
            /// Adds persistence services to the Sumapap builder.
            /// </summary>
            /// <param name="builder">The Sumapap builder.</param>
            /// <returns>The same builder for chaining.</returns>
            public SumapapServiceBuilder WithRepositories(Action<RepositoryServiceBuilder> configuration)
            {
                configuration.Invoke(new RepositoryServiceBuilder(builder));

                return builder;
            }
        }
    }
}