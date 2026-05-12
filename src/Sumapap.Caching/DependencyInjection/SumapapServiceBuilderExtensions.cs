using Sumapap.Caching.DependencyInjection.Builder;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Caching.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        extension(ISumapapServiceBuilder builder)
        {
            public ISumapapServiceBuilder WithCaching(Action<CachingServiceBuilder> configuration)
            {
                configuration(new CachingServiceBuilder(builder));

                return builder;
            }
        }
    }
}
