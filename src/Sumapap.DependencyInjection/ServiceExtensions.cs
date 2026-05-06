using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.DependencyInjection
{
    public static class ServiceExtensions
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Adds Sumapap services to the service collection with a fluent configuration builder.
            /// </summary>
            /// <param name="services">The service collection.</param>
            /// <returns>The service collection for method chaining.</returns>
            public SumapapBuilder AddSumapap()
            {
                ArgumentNullException.ThrowIfNull(services);

                return new(services);
            }
        }
    }
}
