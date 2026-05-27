using Microsoft.Extensions.Options;
using Sumapap.Caching.Abstractions;
using Sumapap.Caching.DependencyInjection.Options;

namespace Sumapap.Caching.Tests
{
    public class DefaultCacheKeyProviderTests
    {
        private readonly ICacheKeyProvider _provider;
        private readonly CacheKeyProviderOptions _options;

        public DefaultCacheKeyProviderTests()
        {
            _options = new CacheKeyProviderOptions
            {
                Separator = ":",
                Tenant = null
            };
            _provider = new DefaultCacheKeyProvider(Options.Create(_options));
        }

        [Fact]
        public void CreateKey_WithStringObject_ReturnsKebabCaseKey()
        {
            var result = _provider.CreateKey("TestString");

            Assert.Equal("test-string:", result);
        }

        [Fact]
        public void CreateKey_WithStringObjectAndParameters_ReturnsCombinedKey()
        {
            var result = _provider.CreateKey("TestString", "param1", "param2");

            Assert.Equal("test-string:param1:param2", result);
        }

        [Fact]
        public void CreateKey_WithComplexObject_ReturnsHashedKey()
        {
            var testObj = new TestClass { Id = 1, Name = "Test" };

            var result = _provider.CreateKey(testObj);

            Assert.NotEmpty(result);
            Assert.Contains(":", result);
            Assert.Equal(result.ToLower(), result);
        }

        [Fact]
        public void CreateKey_WithComplexObjectAndParameters_ReturnsCombinedHashedKey()
        {
            var testObj = new TestClass { Id = 1, Name = "Test" };

            var result = _provider.CreateKey(testObj, "param1", 123);

            Assert.NotEmpty(result);
            Assert.Contains(":", result);
            var parts = result.Split(':');
            Assert.True(parts.Length >= 2);
        }

        [Fact]
        public void CreateKey_WithTypeParameter_ReturnsTypeNameBasedKey()
        {
            var result = _provider.CreateKey<TestClass>();

            Assert.Contains("test-class", result);
        }

        [Fact]
        public void CreateKey_WithTypeParameterAndParameters_ReturnsCombinedTypeNameKey()
        {
            var result = _provider.CreateKey<TestClass>("param1", "param2");

            Assert.Contains("test-class", result);
            Assert.Contains("param1", result);
            Assert.Contains("param2", result);
        }

        [Fact]
        public void CreateKey_WithTenantOption_IncludesTenantInKey()
        {
            var options = new CacheKeyProviderOptions
            {
                Separator = ":",
                Tenant = "tenant-a"
            };
            var provider = new DefaultCacheKeyProvider(Options.Create(options));

            var result = provider.CreateKey("TestString");

            Assert.StartsWith("tenant-a:", result);
        }

        [Fact]
        public void CreateKey_WithCustomSeparator_UsesCustomSeparator()
        {
            var options = new CacheKeyProviderOptions
            {
                Separator = "-",
                Tenant = null
            };
            var provider = new DefaultCacheKeyProvider(Options.Create(options));

            var result = provider.CreateKey("TestString", "param1", "param2");

            Assert.Contains("-", result);
            Assert.Equal("test-string-param1-param2", result);
        }

        [Fact]
        public void CreateKey_WithTenantAndCustomSeparator_FormatsCorrectly()
        {
            var options = new CacheKeyProviderOptions
            {
                Separator = "|",
                Tenant = "my-tenant"
            };
            var provider = new DefaultCacheKeyProvider(Options.Create(options));

            var result = provider.CreateKey("TestString", "param1");

            Assert.StartsWith("my-tenant|", result);
            Assert.Contains("|param1", result);
        }

        [Fact]
        public void CreateKey_SameObjectTwice_ReturnsSameKey()
        {
            var testObj = new TestClass { Id = 1, Name = "Test" };

            var result1 = _provider.CreateKey(testObj);
            var result2 = _provider.CreateKey(testObj);

            Assert.Equal(result1, result2);
        }

        [Fact]
        public void CreateKey_DifferentObjects_ReturnsDifferentKeys()
        {
            var testObj1 = new TestClass { Id = 1, Name = "Test1" };
            var testObj2 = new TestClass { Id = 2, Name = "Test2" };

            var result1 = _provider.CreateKey(testObj1);
            var result2 = _provider.CreateKey(testObj2);

            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void CreateKey_WithEmptyParameters_HandlesGracefully()
        {
            var result = _provider.CreateKey<TestClass>([]);

            Assert.Contains("test-class", result);
        }

        [Fact]
        public void CreateKey_WithNumericParameters_ConvertsToString()
        {
            var result = _provider.CreateKey("TestString", 123, 456);

            Assert.NotEmpty(result);
            Assert.Contains(":", result);
        }

        [Fact]
        public void CreateKey_WithMixedTypeParameters_HandlesAllTypes()
        {
            var result = _provider.CreateKey("TestString", 123, "string", true, 45.67);

            Assert.NotEmpty(result);
            var parts = result.Split(':');
            Assert.True(parts.Length >= 4);
        }

        [Fact]
        public void CreateKey_WithComplexNestedObject_ProducesConsistentHash()
        {
            var nestedObj = new NestedTestClass
            {
                Inner = new TestClass { Id = 1, Name = "Inner" },
                Value = "Outer"
            };

            var result1 = _provider.CreateKey(nestedObj);
            var result2 = _provider.CreateKey(nestedObj);

            Assert.Equal(result1, result2);
            Assert.NotEmpty(result1);
        }

        [Fact]
        public void CreateKey_WithWhitespaceTenant_IgnoresTenant()
        {
            var options = new CacheKeyProviderOptions
            {
                Separator = ":",
                Tenant = "   "
            };
            var provider = new DefaultCacheKeyProvider(Options.Create(options));

            var result = provider.CreateKey("TestString");

            Assert.DoesNotContain("   :", result);
        }

        [Fact]
        public void CreateKey_WithNullTenant_DoesNotIncludeTenant()
        {
            var options = new CacheKeyProviderOptions
            {
                Separator = ":",
                Tenant = null
            };
            var provider = new DefaultCacheKeyProvider(Options.Create(options));

            var result = provider.CreateKey("TestString");

            Assert.DoesNotContain("null", result.ToLower());
            Assert.StartsWith("test-string", result);
        }

        [Fact]
        public void CreateKey_ResultIsAlwaysKebabCase()
        {
            var result = _provider.CreateKey("TestStringWithUpperCase", "ParamWithUpper");

            Assert.Equal(result, result.ToLower());
            Assert.DoesNotContain(" ", result);
        }

        private class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class NestedTestClass
        {
            public TestClass? Inner { get; set; }
            public string Value { get; set; } = string.Empty;
        }
    }
}
