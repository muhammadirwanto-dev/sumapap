using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sumapap.Caching.Abstractions;
using Sumapap.Caching.DependencyInjection;
using Sumapap.Caching.DependencyInjection.Options;
using Sumapap.DependencyInjection;

namespace Sumapap.Caching.Tests.DependencyInjection
{
    public class CachingServiceBuilderTests
    {
        [Fact]
        public void AddKeyProvider_WithoutConfiguration_RegistersDefaultProvider()
        {
            var services = new ServiceCollection();
            var sumapapBuilder = services.AddSumapap();
            var cachingBuilder = new CachingServiceBuilder(sumapapBuilder);

            var result = cachingBuilder.AddKeyProvider();

            Assert.NotNull(result);
            var serviceProvider = cachingBuilder.Services.BuildServiceProvider();
            var provider = serviceProvider.GetService<ICacheKeyProvider>();
            Assert.NotNull(provider);
            Assert.IsType<DefaultCacheKeyProvider>(provider);
        }

        [Fact]
        public void AddKeyProvider_WithConfiguration_RegistersWithOptions()
        {
            var services = new ServiceCollection();
            var sumapapBuilder = services.AddSumapap();
            var cachingBuilder = new CachingServiceBuilder(sumapapBuilder);

            var result = cachingBuilder.AddKeyProvider(options =>
            {
                options.Tenant = "test-tenant";
                options.Separator = "-";
            });

            Assert.NotNull(result);
            var serviceProvider = cachingBuilder.Services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<CacheKeyProviderOptions>>();
            Assert.Equal("test-tenant", options.Value.Tenant);
            Assert.Equal("-", options.Value.Separator);
        }

        [Fact]
        public void AddKeyProvider_WithCustomImplementation_RegistersCustomProvider()
        {
            var services = new ServiceCollection();
            var sumapapBuilder = services.AddSumapap();
            var cachingBuilder = new CachingServiceBuilder(sumapapBuilder);

            var result = cachingBuilder.AddKeyProvider<CustomCacheKeyProvider>();

            Assert.NotNull(result);
            var serviceProvider = cachingBuilder.Services.BuildServiceProvider();
            var provider = serviceProvider.GetService<ICacheKeyProvider>();
            Assert.NotNull(provider);
            Assert.IsType<CustomCacheKeyProvider>(provider);
        }

        [Fact]
        public void AddKeyProvider_WithCustomImplementationAndConfiguration_RegistersBoth()
        {
            var services = new ServiceCollection();
            var sumapapBuilder = services.AddSumapap();
            var cachingBuilder = new CachingServiceBuilder(sumapapBuilder);

            var result = cachingBuilder.AddKeyProvider<CustomCacheKeyProvider>(options =>
            {
                options.Tenant = "custom-tenant";
                options.Separator = "|";
            });

            Assert.NotNull(result);
            var serviceProvider = cachingBuilder.Services.BuildServiceProvider();
            var provider = serviceProvider.GetService<ICacheKeyProvider>();
            Assert.NotNull(provider);
            Assert.IsType<CustomCacheKeyProvider>(provider);

            var options = serviceProvider.GetRequiredService<IOptions<CacheKeyProviderOptions>>();
            Assert.Equal("custom-tenant", options.Value.Tenant);
            Assert.Equal("|", options.Value.Separator);
        }

        [Fact]
        public void Build_ReturnsOriginalBuilder()
        {
            var services = new ServiceCollection();
            var sumapapBuilder = services.AddSumapap();
            var cachingBuilder = new CachingServiceBuilder(sumapapBuilder);

            var result = cachingBuilder.Build();

            Assert.Same(sumapapBuilder, result);
        }

        [Fact]
        public void Services_ReturnsSameServiceCollection()
        {
            var services = new ServiceCollection();
            var sumapapBuilder = services.AddSumapap();
            var cachingBuilder = new CachingServiceBuilder(sumapapBuilder);

            var builtServices = sumapapBuilder.Build();
            var cachingServices = cachingBuilder.Services;

            Assert.Same(builtServices, cachingServices);
        }

        [Fact]
        public void AddKeyProvider_CanBeCalledMultipleTimes_LastOneWins()
        {
            var services = new ServiceCollection();
            var sumapapBuilder = services.AddSumapap();
            var cachingBuilder = new CachingServiceBuilder(sumapapBuilder);

            cachingBuilder
                .AddKeyProvider(options => options.Tenant = "first")
                .AddKeyProvider(options => options.Tenant = "second");

            var serviceProvider = cachingBuilder.Services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<CacheKeyProviderOptions>>();
            Assert.Equal("second", options.Value.Tenant);
        }

        private class CustomCacheKeyProvider : ICacheKeyProvider
        {
            public string CreateKey<TObject>(TObject @object, params object[] parameters) where TObject : class
                => "custom-key";

            public string CreateKey<TObject>(params object[] parameters) where TObject : class
                => "custom-key";
        }
    }
}
