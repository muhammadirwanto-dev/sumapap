using Microsoft.Extensions.DependencyInjection;
using Sumapap.Caching.Abstractions;
using Sumapap.Caching.DependencyInjection.Options;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.DependencyInjection.Builder;

namespace Sumapap.Caching.DependencyInjection.Builder
{
    public class CachingServiceBuilder(SumapapServiceBuilder _builder) : IBuilder<SumapapServiceBuilder>
    {
        private readonly IServiceCollection _services = _builder.Services;

        public IServiceCollection Services => _services;

        public SumapapServiceBuilder Build() => _builder;

        public CachingServiceBuilder AddKeyProvider(Action<CacheKeyProviderOptions>? configuration)
            => AddKeyProvider<DefaultCacheKeyProvider>(configuration);

        public CachingServiceBuilder AddKeyProvider<TImpl>(Action<CacheKeyProviderOptions>? configuration)
            where TImpl : class, ICacheKeyProvider
        {
            _services.Configure(configuration ?? (opt => { }));
            _services.AddSingleton<ICacheKeyProvider, TImpl>();

            return this;
        }
    }
}
