# Sumapap.Caching

[![NuGet](https://img.shields.io/nuget/v/Sumapap.Caching.svg)](https://www.nuget.org/packages/Sumapap.Caching/)

## Overview

`Sumapap.Caching` provides core abstractions and utilities for building consistent caching strategies across Sumapap applications. This library offers a provider-agnostic approach to cache key generation and management, making it easy to integrate any caching implementation (Redis, FusionCache, MemoryCache, etc.).

## Features

- **Cache Key Provider Abstraction**: Consistent cache key generation across your application
- **Default Implementation**: Out-of-the-box cache key provider with sensible defaults
- **Fluent Configuration**: Type-safe fluent API for configuring caching behavior
- **Extensible**: Easy to implement custom cache key strategies
- **Provider-Agnostic**: Works with any caching implementation

## Installation

```bash
dotnet add package Sumapap.Caching
```

## Architecture

The library follows a provider pattern:

1. **ICacheKeyProvider**: Abstract interface for cache key generation
2. **DefaultCacheKeyProvider**: Default implementation using object names and parameters
3. **Fluent Builder**: Integrate caching configuration into your DI setup

## Basic Usage

### Registering Caching Services

```csharp
using Sumapap.DependencyInjection;
using Sumapap.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSumapap()
	.WithCaching(caching => caching
		.UseDefaultKeyProvider(options =>
		{
			options.KeySeparator = ":";
			options.IncludeTypeName = true;
		})
	);

var app = builder.Build();
```

### Using Cache Key Provider

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

	public async Task<User?> GetUserAsync(int userId)
	{
		// Generate cache key: "User:123"
		var cacheKey = _keyProvider.CreateKey<User>(userId);

		// Try to get from cache
		var cached = await _cache.GetStringAsync(cacheKey);
		if (cached != null)
		{
			return JsonSerializer.Deserialize<User>(cached);
		}

		// Fetch from database
		var user = await _dbContext.Users.FindAsync(userId);

		// Store in cache
		if (user != null)
		{
			await _cache.SetStringAsync(
				cacheKey,
				JsonSerializer.Serialize(user),
				new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
				});
		}

		return user;
	}
}
```

## Key Types

### ICacheKeyProvider

The core abstraction for cache key generation.

```csharp
public interface ICacheKeyProvider
{
	// Create cache key from object name and parameters
	string CreateKey(string @object, params object[] parameters);

	// Create cache key from generic type and parameters
	string CreateKey<TObject>(params object[] parameters);
}
```

### DefaultCacheKeyProvider

Default implementation that generates cache keys in the format: `TypeName:Param1:Param2:...`

```csharp
public class DefaultCacheKeyProvider : ICacheKeyProvider
{
	public string CreateKey(string @object, params object[] parameters)
	{
		// Generates: "ObjectName:param1:param2"
	}

	public string CreateKey<TObject>(params object[] parameters)
	{
		// Generates: "TObject:param1:param2"
	}
}
```

### CacheKeyProviderOptions

Configuration options for the default cache key provider.

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

## Advanced Usage

### Custom Cache Key Provider

```csharp
public class CustomCacheKeyProvider : ICacheKeyProvider
{
	private readonly string _environment;

	public CustomCacheKeyProvider(IHostEnvironment environment)
	{
		_environment = environment.EnvironmentName;
	}

	public string CreateKey(string @object, params object[] parameters)
	{
		// Include environment in cache key for isolation
		var parts = new List<string> { _environment, @object };
		parts.AddRange(parameters.Select(p => p?.ToString() ?? "null"));
		return string.Join(":", parts);
	}

	public string CreateKey<TObject>(params object[] parameters)
	{
		return CreateKey(typeof(TObject).Name, parameters);
	}
}

// Registration
builder.Services.AddSumapap()
	.WithCaching(caching => caching
		.UseCustomKeyProvider<CustomCacheKeyProvider>()
	);
```

### Configuring Cache Key Options

```csharp
builder.Services.AddSumapap()
	.WithCaching(caching => caching
		.UseDefaultKeyProvider(options =>
		{
			// Use pipe separator instead of colon
			options.KeySeparator = "|";

			// Include full type names (e.g., "MyApp.Domain.User")
			options.IncludeTypeName = true;

			// Add prefix for namespacing (e.g., "myapp:User:123")
			options.Prefix = "myapp";
		})
	);
```

### Using with Repository Pattern

```csharp
public class CachedUserRepository : IUserRepository
{
	private readonly IUserRepository _innerRepository;
	private readonly ICacheKeyProvider _keyProvider;
	private readonly IDistributedCache _cache;

	public CachedUserRepository(
		IUserRepository innerRepository,
		ICacheKeyProvider keyProvider,
		IDistributedCache cache)
	{
		_innerRepository = innerRepository;
		_keyProvider = keyProvider;
		_cache = cache;
	}

	public async Task<User?> FindAsync(int id, CancellationToken ct = default)
	{
		var key = _keyProvider.CreateKey<User>("Find", id);

		// Try cache first
		var cached = await _cache.GetStringAsync(key, ct);
		if (cached != null)
		{
			return JsonSerializer.Deserialize<User>(cached);
		}

		// Fetch from inner repository
		var user = await _innerRepository.FindAsync(id, ct);

		// Cache the result
		if (user != null)
		{
			await _cache.SetStringAsync(
				key,
				JsonSerializer.Serialize(user),
				new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
				},
				ct);
		}

		return user;
	}
}
```

## Best Practices

### 1. Consistent Key Generation

Always use `ICacheKeyProvider` instead of manually constructing keys:

```csharp
// Good ✅
var key = _keyProvider.CreateKey<User>(userId);

// Bad ❌
var key = $"User:{userId}";
```

### 2. Include All Relevant Parameters

Include all parameters that affect the result:

```csharp
// Cache key for filtered query
var key = _keyProvider.CreateKey<User>(
	"Query",
	filter.Status,
	filter.Role,
	filter.PageNumber,
	filter.PageSize
);
```

### 3. Use Prefixes for Namespacing

Configure a prefix to avoid key collisions across applications:

```csharp
options.Prefix = "myapp";  // Results in "myapp:User:123"
```

### 4. Choose Appropriate Separators

Pick a separator that won't appear in your parameters:

```csharp
// If your IDs might contain colons
options.KeySeparator = "|";  // "User|123|active"
```

### 5. Consider Environment Isolation

Include environment name in keys for development/staging isolation:

```csharp
public class EnvironmentCacheKeyProvider : ICacheKeyProvider
{
	private readonly string _env;

	public EnvironmentCacheKeyProvider(IHostEnvironment environment)
	{
		_env = environment.EnvironmentName.ToLowerInvariant();
	}

	public string CreateKey<TObject>(params object[] parameters)
	{
		return $"{_env}:{typeof(TObject).Name}:{string.Join(":", parameters)}";
	}
}
```

## Integration Examples

### With Redis

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddSumapap()
	.WithCaching(caching => caching
		.UseDefaultKeyProvider(options =>
		{
			options.Prefix = "myapp";
			options.KeySeparator = ":";
		})
	);
```

### With Memory Cache

```csharp
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSumapap()
	.WithCaching(caching => caching
		.UseDefaultKeyProvider()
	);
```

### With FusionCache

```csharp
builder.Services.AddFusionCache();

builder.Services.AddSumapap()
	.WithCaching(caching => caching
		.UseDefaultKeyProvider(options =>
		{
			options.Prefix = builder.Configuration["App:Name"];
		})
	);
```

## Cache Key Examples

```csharp
var keyProvider = new DefaultCacheKeyProvider(new CacheKeyProviderOptions
{
	KeySeparator = ":",
	Prefix = "myapp"
});

// Simple entity by ID
keyProvider.CreateKey<User>(123);
// Result: "myapp:User:123"

// Method with multiple parameters
keyProvider.CreateKey<Order>("GetByCustomer", customerId, year);
// Result: "myapp:Order:GetByCustomer:456:2024"

// Query with filter object
keyProvider.CreateKey<Product>("Search", category, minPrice, maxPrice);
// Result: "myapp:Product:Search:electronics:100:500"

// Using string key
keyProvider.CreateKey("UserSession", sessionId);
// Result: "myapp:UserSession:abc123"
```

## Testing

```csharp
public class UserServiceTests
{
	[Fact]
	public async Task GetUser_UsesCacheKey()
	{
		// Arrange
		var keyProvider = new DefaultCacheKeyProvider(new CacheKeyProviderOptions());
		var cacheMock = new Mock<IDistributedCache>();
		var service = new UserService(keyProvider, cacheMock.Object);

		// Act
		await service.GetUserAsync(123);

		// Assert
		cacheMock.Verify(c => c.GetStringAsync(
			"User:123",
			It.IsAny<CancellationToken>()), 
			Times.Once);
	}
}
```

## Related Packages

- **Sumapap.Persistence.DependencyInjection**: Fluent repository registration with caching support
- **Sumapap.Persistence.FusionCache**: FusionCache integration for repository caching
- **Sumapap.DependencyInjection**: Core Sumapap DI builder infrastructure

## Contributing

Contributions are welcome! Please check the [contributing guidelines](https://github.com/muhammadirwanto-dev/sumapap/blob/main/CONTRIBUTING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.
