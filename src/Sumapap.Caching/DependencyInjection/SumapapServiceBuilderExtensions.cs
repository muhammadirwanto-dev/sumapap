using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Caching.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        extension(ISumapapServiceBuilder builder)
        {
            public ISumapapServiceBuilder WithCaching()
            {
                return new CachingServiceBuilder(builder)
                    .AddKeyProvider()
                    .Build();
            }

            public ISumapapServiceBuilder WithCaching(Action<CachingServiceBuilder> configuration)
            {
                var cachingBuilder = new CachingServiceBuilder(builder);

                configuration(cachingBuilder);

                return cachingBuilder.Build();
            }
        }
    }
}
