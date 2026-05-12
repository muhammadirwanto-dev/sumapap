# Sumapap.Caching

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Caching.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Caching/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Caching.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Caching/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

``Sumapap.Caching`` provides core abstractions and utilities for building consistent caching strategies across Sumapap applications. The package focuses on:

- Cache key provider abstraction for consistent key generation
- Default implementation with configurable options (separator, prefix, type name formatting)
- Fluent configuration API for DI setup
- Provider-agnostic design (works with Redis, FusionCache, MemoryCache, etc.)
- Extensible architecture for custom cache key strategies

The goal is to standardize cache key generation across your application while keeping cache implementation details swappable.

## ✨ Why use ``Sumapap.Caching``?

- **Consistent Key Generation**: Standardized cache key format across your entire application
- **Type-Safe API**: Generic methods provide compile-time safety for cache keys
- **Provider-Agnostic**: Works with any caching implementation (Redis, FusionCache, MemoryCache, IDistributedCache)
- **Fluent Configuration**: Easy DI setup via ``AddSumapap().WithCaching()``
- **Extensible**: Implement ``ICacheKeyProvider`` for custom key generation strategies
- **Testing-Friendly**: Simple abstraction makes testing cache interactions straightforward

## 🚀 Quick start

1. Add the package to your project:

```bash
dotnet add package Sumapap.Caching
```

2. Register caching services in DI:

```csharp
builder.Services.AddSumapap()
    .WithCaching(caching => caching
        .UseDefaultKeyProvider(options =>
        {
            options.KeySeparator = ":";
            options.IncludeTypeName = true;
        })
    );
```

3. Inject ``ICacheKeyProvider`` into your services:

```csharp
public class UserService
{
    private readonly ICacheKeyProvider _keyProvider;
    private readonly IDistributedCache _cache;

    public UserService(ICacheKeyProvider keyProvider, IDistributedCache cache)
    {
        _keyProvider = keyProvider;
        _cache = cache;
    }
}
```

4. Generate cache keys using the provider:

```csharp
// Generate cache key: "User:123"
var cacheKey = _keyProvider.CreateKey<User>(123);

// Try to get from cache
var cached = await _cache.GetStringAsync(cacheKey);
```

5. Use cache keys consistently across your application for all cache operations.

## 🛠 Features and usage

### Cache Key Provider Abstraction

**ICacheKeyProvider** - Core abstraction for cache key generation:

```csharp
public interface ICacheKeyProvider
{
    // Create cache key from object name and parameters
    string CreateKey(string @object, params object[] parameters);

    // Create cache key from generic type and parameters
    string CreateKey<TObject>(params object[] parameters);
}
```

**Usage:**
```csharp
// Type-safe key generation
var userKey = _keyProvider.CreateKey<User>(userId);
// Result: "User:123"

// String-based key generation
var sessionKey = _keyProvider.CreateKey("UserSession", sessionId);
// Result: "UserSession:abc123"

// Multiple parameters
var queryKey = _keyProvider.CreateKey<Product>("Search", category, minPrice, maxPrice);
// Result: "Product:Search:electronics:100:500"
```

### Default Cache Key Provider

**DefaultCacheKeyProvider** - Default implementation with configurable options:

```csharp
public class DefaultCacheKeyProvider : ICacheKeyProvider
{
    public DefaultCacheKeyProvider(CacheKeyProviderOptions options);
    
    public string CreateKey(string @object, params object[] parameters);
    public string CreateKey<TObject>(params object[] parameters);
}
```

**Key Generation Format:**
```
[Prefix:]TypeName:Param1:Param2:...
```

**Examples:**
```csharp
// Default configuration (no prefix, short type name, ":" separator)
var key = _keyProvider.CreateKey<User>(123);
// Result: "User:123"

// With prefix configured
options.Prefix = "myapp";
var key = _keyProvider.CreateKey<User>(123);
// Result: "myapp:User:123"

// With full type name
options.IncludeTypeName = true;
var key = _keyProvider.CreateKey<User>(123);
// Result: "MyApp.Domain.User:123"
```

### Configuration Options

**CacheKeyProviderOptions** - Configuration for default provider:

```csharp
public class CacheKeyProviderOptions
{
    // Separator between cache key components (default: ":")
    public string KeySeparator { get; set; } = ":";

    // Include full type name in cache key (default: false)
    public bool IncludeTypeName { get; set; } = false;

    // Cache key prefix for namespacing (default: null)
    public string? Prefix { get; set; }
}
```

**Configuration Examples:**

**With Prefix:**
```csharp
.WithCaching(caching => caching
    .UseDefaultKeyProvider(options =>
    {
        options.Prefix = "myapp";
    })
)

// Keys: "myapp:User:123", "myapp:Product:456"
```

**With Custom Separator:**
```csharp
.WithCaching(caching => caching
    .UseDefaultKeyProvider(options =>
    {
        options.KeySeparator = "-";
    })
)

// Keys: "User-123", "Product-Search-electronics-100-500"
```

**With Full Type Name:**
```csharp
.WithCaching(caching => caching
    .UseDefaultKeyProvider(options =>
    {
        options.IncludeTypeName = true;
    })
)

// Keys: "MyApp.Domain.Entities.User:123"
```

### Fluent DI Configuration

**WithCaching()** - Fluent builder for caching configuration:

```csharp
builder.Services.AddSumapap()
    .WithCaching(caching => caching
        .UseDefaultKeyProvider(options =>
        {
            options.KeySeparator = ":";
            options.Prefix = "myapp";
            options.IncludeTypeName = false;
        })
    );
```

### Complete Usage Example

Full application setup with caching:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register Sumapap with caching
builder.Services.AddSumapap()
    .WithCaching(caching => caching
        .UseDefaultKeyProvider(options =>
        {
            options.Prefix = "myapp";
            options.KeySeparator = ":";
        })
    );

// Add distributed cache implementation
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

var app = builder.Build();

// UserService implementation
public class UserService
{
    private readonly ICacheKeyProvider _keyProvider;
    private readonly IDistributedCache _cache;
    private readonly IUserRepository _repository;

    public UserService(
        ICacheKeyProvider keyProvider, 
        IDistributedCache cache,
        IUserRepository repository)
    {
        _keyProvider = keyProvider;
        _cache = cache;
        _repository = repository;
    }

    public async Task<User?> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        // Generate cache key: "myapp:User:123"
        var cacheKey = _keyProvider.CreateKey<User>(userId);

        // Try to get from cache
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<User>(cached);
        }

        // Fetch from database
        var user = await _repository.FindAsync(userId, cancellationToken);

        // Store in cache
        if (user != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(user),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                },
                cancellationToken);
        }

        return user;
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(
        string searchTerm, 
        CancellationToken cancellationToken = default)
    {
        // Generate cache key: "myapp:User:Search:john"
        var cacheKey = _keyProvider.CreateKey<User>("Search", searchTerm);

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<IEnumerable<User>>(cached) ?? [];
        }

        var users = await _repository.SearchAsync(searchTerm, cancellationToken);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(users),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return users;
    }
}
```

### Custom Cache Key Provider

Implement ``ICacheKeyProvider`` for custom key generation strategies:

```csharp
public class CustomCacheKeyProvider : ICacheKeyProvider
{
    public string CreateKey(string @object, params object[] parameters)
    {
        // Custom implementation: use MD5 hash for long keys
        var raw = $"{@object}:{string.Join(":", parameters)}";
        if (raw.Length > 250)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return Convert.ToBase64String(hash);
        }
        return raw;
    }

    public string CreateKey<TObject>(params object[] parameters)
    {
        return CreateKey(typeof(TObject).Name, parameters);
    }
}

// Register custom provider
builder.Services.AddSumapap()
    .WithCaching(caching => caching
        .UseKeyProvider<CustomCacheKeyProvider>()
    );
```

## ⚠️ Notes & best practices

### ✅ Do

- **Use consistent key generation** across your entire application via ``ICacheKeyProvider``
- **Configure prefix** for multi-tenant or multi-environment scenarios (``options.Prefix = "prod"`` vs ``"dev"``)
- **Include type name** when caching multiple entity types with similar keys
- **Use generic methods** (``CreateKey<T>()``) for type safety when possible
- **Test cache key generation** to verify correct formatting before deploying

### ❌ Don''t

- **Don''t hardcode cache keys** - always use ``ICacheKeyProvider`` for consistency
- **Avoid very long cache keys** - some cache providers have key length limits (Redis: 512MB, but practical limits are much lower)
- **Don''t include sensitive data** in cache keys (passwords, tokens) - keys may be logged or exposed
- **Avoid using ``IncludeTypeName = true`` unnecessarily** - increases key length without value for most scenarios
- **Don''t forget to configure separator** if your cache provider has special character restrictions

### Cache Key Length Limits

Different cache providers have different key length limits:
- **Redis**: 512MB theoretical, but practical limit ~1KB
- **MemoryCache**: No hard limit, but shorter keys improve performance
- **Azure Redis Cache**: 512MB

Keep cache keys concise by:
1. Using short, descriptive object names
2. Avoiding full type names unless necessary (``IncludeTypeName = false``)
3. Using identifiers instead of full objects as parameters

### Multi-Tenancy Support

Use prefix for tenant isolation:

```csharp
.WithCaching(caching => caching
    .UseDefaultKeyProvider(options =>
    {
        options.Prefix = $"tenant-{tenantId}";
    })
)

// Keys: "tenant-123:User:456", "tenant-123:Product:789"
```

### Testing Recommendations

Mock ``ICacheKeyProvider`` in unit tests:

```csharp
[Fact]
public async Task GetUser_GeneratesCorrectCacheKey()
{
    // Arrange
    var keyProviderMock = new Mock<ICacheKeyProvider>();
    keyProviderMock
        .Setup(k => k.CreateKey<User>(123))
        .Returns("User:123");
    
    var service = new UserService(keyProviderMock.Object, cacheMock.Object);

    // Act
    await service.GetUserAsync(123);

    // Assert
    keyProviderMock.Verify(k => k.CreateKey<User>(123), Times.Once);
}
```

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the ``LICENSE`` file in the repository for more information.

# 🚩 Contact

``GitHub`` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
``Project Url`` https://github.com/muhammadirwanto-dev/sumapap

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>