using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Caching.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        public static ISumapapServiceBuilder WithCaching(this ISumapapServiceBuilder builder)
        {
            return new CachingServiceBuilder(builder)
                .AddKeyProvider()
                .Build();
        }

        public static ISumapapServiceBuilder WithCaching(this ISumapapServiceBuilder builder, Action<CachingServiceBuilder> configuration)
        {
            var cachingBuilder = new CachingServiceBuilder(builder);

            configuration(cachingBuilder);

            return cachingBuilder.Build();
        }
    }
}
