using Microsoft.Extensions.DependencyInjection;
using Sumapap.Caching.Abstractions;
using Sumapap.Caching.DependencyInjection;
using Sumapap.DependencyInjection;

namespace Sumapap.Caching.Tests.DependencyInjection
{
    public class SumapapServiceBuilderExtensionsTests
    {
        [Fact]
        public void WithCaching_WithoutConfiguration_RegistersDefaultKeyProvider()
        {
            var services = new ServiceCollection();
            var builder = services.AddSumapap();

            var result = builder.WithCaching();

            Assert.Same(builder, result);
            var builtServices = builder.Build();
            var serviceProvider = builtServices.BuildServiceProvider();
            var provider = serviceProvider.GetService<ICacheKeyProvider>();
            Assert.NotNull(provider);
            Assert.IsType<DefaultCacheKeyProvider>(provider);
        }

        [Fact]
        public void WithCaching_WithConfiguration_AppliesConfiguration()
        {
            var services = new ServiceCollection();
            var builder = services.AddSumapap();

            var result = builder.WithCaching(cachingBuilder =>
            {
                cachingBuilder.AddKeyProvider(options =>
                {
                    options.Tenant = "configured-tenant";
                    options.Separator = "-";
                });
            });

            Assert.Same(builder, result);
            var builtServices = builder.Build();
            var serviceProvider = builtServices.BuildServiceProvider();
            var provider = serviceProvider.GetService<ICacheKeyProvider>();
            Assert.NotNull(provider);
        }

        [Fact]
        public void WithCaching_WithCustomProvider_RegistersCustomProvider()
        {
            var services = new ServiceCollection();
            var builder = services.AddSumapap();

            var result = builder.WithCaching(cachingBuilder =>
            {
                cachingBuilder.AddKeyProvider<CustomCacheKeyProvider>();
            });

            Assert.Same(builder, result);
            var builtServices = builder.Build();
            var serviceProvider = builtServices.BuildServiceProvider();
            var provider = serviceProvider.GetService<ICacheKeyProvider>();
            Assert.NotNull(provider);
            Assert.IsType<CustomCacheKeyProvider>(provider);
        }

        [Fact]
        public void WithCaching_ReturnsOriginalBuilder()
        {
            var services = new ServiceCollection();
            var builder = services.AddSumapap();

            var result = builder.WithCaching();

            Assert.Same(builder, result);
        }

        [Fact]
        public void WithCaching_CanBeChained()
        {
            var services = new ServiceCollection();
            var builder = services.AddSumapap();

            var result = builder
                .WithCaching(cachingBuilder =>
                {
                    cachingBuilder.AddKeyProvider(options => options.Tenant = "tenant1");
                });

            Assert.Same(builder, result);
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
