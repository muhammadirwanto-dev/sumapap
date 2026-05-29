using Sumapap.Caching.DependencyInjection.Options;

namespace Sumapap.Caching.Tests.DependencyInjection.Options
{
    public class CacheKeyProviderOptionsTests
    {
        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var options = new CacheKeyProviderOptions();

            Assert.Null(options.Tenant);
            Assert.Equal(":", options.Separator);
        }

        [Fact]
        public void Tenant_CanBeSet()
        {
            var options = new CacheKeyProviderOptions
            {
                Tenant = "test-tenant"
            };

            Assert.Equal("test-tenant", options.Tenant);
        }

        [Fact]
        public void Separator_CanBeSet()
        {
            var options = new CacheKeyProviderOptions
            {
                Separator = "-"
            };

            Assert.Equal("-", options.Separator);
        }

        [Fact]
        public void AllProperties_CanBeSetTogether()
        {
            var options = new CacheKeyProviderOptions
            {
                Tenant = "my-tenant",
                Separator = "|"
            };

            Assert.Equal("my-tenant", options.Tenant);
            Assert.Equal("|", options.Separator);
        }
    }
}
