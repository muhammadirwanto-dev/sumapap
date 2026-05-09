using Sumapap.Caching.DependencyInjection.Builder;
using Sumapap.DependencyInjection.Builder;

namespace Sumapap.Caching.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        extension(SumapapServiceBuilder builder)
        {
            public SumapapServiceBuilder WithCaching(Action<CachingServiceBuilder> configuration)
            {
                configuration(new CachingServiceBuilder(builder));

                return builder;
            }
        }
    }
}
