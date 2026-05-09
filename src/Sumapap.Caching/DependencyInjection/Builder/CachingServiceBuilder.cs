using Microsoft.Extensions.DependencyInjection;
using Sumapap.Caching.Abstractions;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.DependencyInjection.Builder;

namespace Sumapap.Caching.DependencyInjection.Builder
{
    public class CachingServiceBuilder(SumapapServiceBuilder _builder) : IBuilder<SumapapServiceBuilder>
    {
        private readonly IServiceCollection _services = _builder.Build();

        public IServiceCollection Services => _services;

        public SumapapServiceBuilder Build() => _builder;

        public CachingServiceBuilder WithOptions(Action<CachingServiceBuilderOptions> configuration)
        {
            _services.Configure(configuration);

            return this;
        }

        public CachingServiceBuilder AddKeyProvider() => AddKeyProvider<DefaultCacheKeyProvider>();

        public CachingServiceBuilder AddKeyProvider<TImpl>()
            where TImpl : class, ICacheKeyProvider
        {
            _services.AddSingleton<ICacheKeyProvider, TImpl>();

            return this;
        }
    }
}
