using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.DependencyInjection
{
    public static class SumapapServiceCollectionExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Adds Sumapap services to the service collection with a fluent configuration builder.
            /// </summary>
            /// <param name="services">The service collection.</param>
            /// <returns>The service collection for method chaining.</returns>
            public ISumapapServiceBuilder AddSumapap()
            {
                ArgumentNullException.ThrowIfNull(services);

                return new SumapapServiceBuilder(services);
            }
        }
    }
}
