# Sumapap.Persistence Fluent Builder Design

## Overview

The fluent builder pattern has been implemented for Sumapap.Persistence to provide a clean, type-safe API for repository registration with **opt-in caching** support. The caching configuration is stored in a registry that can be consumed by cache providers like FusionCache.

## Architecture

### Key Components

1. **`SumapapServiceBuilderExtensions`** - Entry point extension method
2. **`RepositoryRegistrationBuilder`** - Main builder for registering repositories
3. **`RepositoryConfigurator<TRepository, TEntity>`** - Type-safe configurator returned after registration
4. **`RepositoryServiceRegistration`** - Internal record storing registration metadata
5. **`RepositoryCacheConfiguration`** - Rich configuration model for cache behavior
6. **`RepositoryCacheRegistry`** - Singleton registry storing all cached repository metadata
7. **`RepositoryCacheEntry`** - Individual cache entry with full configuration
8. **`CachedFunctionsMapping`** - Method-level cache enablement (legacy, wrapped in configuration)

### Design Principles

✅ **AllowCaching() is only visible after repository registration**
- `AllowCaching()` is a method on `RepositoryConfigurator<TRepository, TEntity>`
- You cannot call `builder.AllowCaching()` directly
- Must call `builder.AddScopedRepository<>().AllowCaching()`

✅ **Opt-in caching per repository**
- Each repository independently opts into caching
- Caching configuration is stored in the registration metadata
- Can configure which methods are cacheable, cache duration, key prefix, and custom metadata

✅ **Deferred cache application**
- Cache configuration is recorded during repository registration
- Actual cache decorator application happens when `UseFusionCache()` (or other provider) is called
- FusionCache can read `RepositoryCacheRegistry` to apply decorators

✅ **Method chaining support**
- Can chain multiple repository registrations
- Can return to builder via `.Builder` property if needed

## Usage Examples

### Basic Usage with Full Configuration

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		// Repository with detailed caching configuration
		builder.AddScopedRepository<UserRepository, User>()
			.AllowCaching(config =>
			{
				// Configure which methods to cache
				config.Methods[nameof(IReadRepository<User>.FindAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.GetAllAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.FirstOrDefaultAsync)] = false;

				// Configure cache duration
				config.Duration = TimeSpan.FromMinutes(10);

				// Configure cache key prefix
				config.KeyPrefix = "user";

				// Add custom metadata for cache provider
				config.Metadata["Priority"] = "High";
			});

		// Repository without caching
		builder.AddScopedRepository<ProductRepository, Product>();

		// Repository with default caching (all default methods cached)
		builder.AddScopedRepository<OrderRepository, Order>()
			.AllowCaching();
	});
```

### With FusionCache Integration (Future)

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		builder.AddScopedRepository<UserRepository, User>()
			.AllowCaching(config =>
			{
				config.Methods[nameof(IReadRepository<User>.FindAsync)] = true;
				config.Duration = TimeSpan.FromMinutes(5);
			});

		builder.AddScopedRepository<ProductRepository, Product>()
			.AllowCaching(); // Default configuration

		// Apply FusionCache decorators to all repositories with AllowCaching
		builder.UseFusionCache(); // <-- This will be implemented in Sumapap.Persistence.FusionCache
	});
```

### Advanced Configuration

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		builder.AddScopedRepository<IUserRepository, UserRepository, User>()
			.AllowCaching(config =>
			{
				// Only cache read operations
				config.Methods[nameof(IReadRepository<User>.FindAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.GetAllAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.FirstOrDefaultAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.SingleOrDefaultAsync)] = true;

				// Long cache duration for rarely-changing data
				config.Duration = TimeSpan.FromHours(1);

				// Custom key prefix
				config.KeyPrefix = "user_cache";

				// FusionCache-specific metadata
				config.Metadata["FusionCache:FailSafe"] = true;
				config.Metadata["FusionCache:Priority"] = 10;
			});
	});
```

## Type Safety

The design ensures compile-time type safety:

### ❌ This won't compile:
```csharp
builder.AllowCaching()  // Error: AllowCaching is not a method on RepositoryRegistrationBuilder
```

### ✅ This will compile:
```csharp
builder.AddScopedRepository<UserRepository, User>()
	.AllowCaching()  // OK: AllowCaching is a method on RepositoryConfigurator<,>
```

## Caching Configuration Model

### RepositoryCacheConfiguration

```csharp
public sealed class RepositoryCacheConfiguration
{
	// Method-level cache control
	public CachedFunctionsMapping Methods { get; set; } = new();

	// Cache duration (null = use provider default)
	public TimeSpan? Duration { get; set; }

	// Cache key prefix (null = use repository type name)
	public string? KeyPrefix { get; set; }

	// Custom metadata for cache providers
	public Dictionary<string, object> Metadata { get; } = new();
}
```

### RepositoryCacheEntry

```csharp
public sealed class RepositoryCacheEntry
{
	public required Type RepositoryType { get; init; }      // UserRepository
	public required Type EntityType { get; init; }           // User
	public required ServiceLifetime Lifetime { get; init; }  // Scoped/Transient
	public required RepositoryCacheConfiguration Configuration { get; init; }
	public List<Type> ServiceTypes { get; init; }           // IRepository<User>, IReadRepository<User>, etc.
}
```

## Cache Registry

The `RepositoryCacheRegistry` is a singleton that stores all repository cache configurations:

```csharp
public sealed class RepositoryCacheRegistry
{
	public IReadOnlyList<RepositoryCacheEntry> CachedRepositories { get; }
}
```

### Accessing the Registry (for Cache Providers)

```csharp
// In your cache provider (e.g., FusionCache extensions)
extension(RepositoryRegistrationBuilder builder)
{
	public RepositoryRegistrationBuilder UseFusionCache()
	{
		var registry = builder.Services.GetRepositoryCacheRegistry();

		if (registry != null)
		{
			foreach (var entry in registry.CachedRepositories)
			{
				// Apply FusionCache decorator based on entry.Configuration
				ApplyCacheDecorator(builder.Services, entry);
			}
		}

		return builder;
	}
}
```

## Integration with Cache Providers

### For FusionCache (Implementation Guide)

In `Sumapap.Persistence.FusionCache`, you can create:

```csharp
extension(RepositoryRegistrationBuilder builder)
{
	public RepositoryRegistrationBuilder UseFusionCache(
		Action<FusionCacheOptions>? configure = null)
	{
		var services = builder.Services;

		// Register FusionCache
		services.AddFusionCache().TryWithAutoSetup();

		if (configure != null)
		{
			services.Configure(configure);
		}

		// Get all cached repository configurations
		var registry = services.GetRepositoryCacheRegistry();

		if (registry != null)
		{
			foreach (var entry in registry.CachedRepositories)
			{
				// Create decorator type
				var decoratorType = typeof(CachedRepository<>).MakeGenericType(entry.EntityType);

				// For each service type registered (IRepository<T>, IReadRepository<T>, etc.)
				foreach (var serviceType in entry.ServiceTypes)
				{
					ApplyDecorator(services, serviceType, entry.RepositoryType, decoratorType, entry);
				}
			}
		}

		return builder;
	}
}
```

### Cache Decorator Implementation

The cache decorator can read the configuration:

```csharp
internal class CachedRepository<TEntity> : IReadRepository<TEntity>
{
	private readonly IReadRepository<TEntity> _inner;
	private readonly IFusionCache _cache;
	private readonly RepositoryCacheConfiguration _config;

	public CachedRepository(
		IReadRepository<TEntity> inner,
		IFusionCache cache,
		RepositoryCacheConfiguration config)
	{
		_inner = inner;
		_cache = cache;
		_config = config;
	}

	public async ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default)
	{
		// Check if this method should be cached
		if (!_config.Methods.TryGetValue(nameof(FindAsync), out var shouldCache) || !shouldCache)
		{
			return await _inner.FindAsync(key, cancellation);
		}

		var cacheKey = $"{_config.KeyPrefix ?? typeof(TEntity).Name}:Find:{key}";
		var duration = _config.Duration ?? TimeSpan.FromMinutes(5);

		return await _cache.GetOrSetAsync(
			cacheKey,
			async ct => await _inner.FindAsync(key, ct),
			new FusionCacheEntryOptions { Duration = duration },
			cancellation);
	}
}
```

## Benefits

✅ **Type-safe** - Compiler prevents calling `AllowCaching()` in wrong context
✅ **Opt-in** - Caching is explicitly enabled per repository
✅ **Flexible** - Rich configuration model (methods, duration, prefix, metadata)
✅ **Composable** - Integrates with existing `AddSumapap()` builder pattern
✅ **Clean API** - Fluent, readable, self-documenting
✅ **Extensible** - Easy to add new cache providers
✅ **Deferred** - Cache application happens separately from registration
✅ **Observable** - Cache providers can inspect all cached repositories

## File Structure

```
Sumapap.Persistence/
├── DependencyInjection/
│   ├── SumapapServiceBuilderExtensions.cs       # Entry point
│   └── Builder/
│       ├── RepositoryRegistrationBuilder.cs     # Main builder + RepositoryConfigurator<,>
│       ├── RepositoryServiceRegistration.cs     # Registration records
│       └── RepositoryRegistration.cs            # Legacy class
└── Caching/
	├── CachedFunctionsMapping.cs                # Method-level cache flags
	├── RepositoryCacheRegistry.cs               # Cache configuration registry
	└── CacheProviderExtensions.cs               # Integration helpers for cache providers
```

## Migration Path

### Old Approach (Before)

```csharp
builder.AddScopedRepository<UserRepository, User>(allowCaching: true);
```

### New Approach (After)

```csharp
builder.AddScopedRepository<UserRepository, User>()
	.AllowCaching(config =>
	{
		config.Methods[nameof(IReadRepository<User>.FindAsync)] = true;
		config.Duration = TimeSpan.FromMinutes(10);
	});
```

## Notes

- ✅ The implementation is complete for the Persistence layer
- ✅ `RepositoryCacheRegistry` is populated during `Build()`
- ✅ Cache configuration is stored and ready for consumption
- 🔜 FusionCache integration will read the registry and apply decorators
- 🔜 Other cache providers can also integrate using the same registry
- ✅ The design allows for custom metadata per cache provider

## Future: UseFusionCache() Implementation

When implemented in `Sumapap.Persistence.FusionCache`, it will:

1. Call `services.GetRepositoryCacheRegistry()` to get all cached repositories
2. For each `RepositoryCacheEntry`:
   - Read the `Configuration` (methods, duration, prefix, metadata)
   - Create a decorator (`CachedRepository<TEntity>`)
   - Replace the service registration with the decorated version
   - Pass the `RepositoryCacheConfiguration` to the decorator
3. The decorator uses the configuration to decide what to cache and how


### With Service Abstraction

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		builder.AddScopedRepository<IUserRepository, UserRepository, User>()
			.AllowCaching(config =>
			{
				// Configure specific methods
				config[nameof(IReadRepository<User>.FindAsync)] = true;
				config[nameof(IReadRepository<User>.GetAllAsync)] = true;
			});
	});
```

### Transient Repositories

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		builder.AddTransientRepository<TempDataRepository, TempData>()
			.AllowCaching();  // Even transient repos can be cached
	});
```

### Returning to Builder

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		builder.AddScopedRepository<UserRepository, User>()
			.AllowCaching()
			.Builder  // Return to builder
			.AddScopedRepository<ProductRepository, Product>();
	});
```

## Type Safety

The design ensures compile-time type safety:

### ❌ This won't compile:
```csharp
builder.AllowCaching()  // Error: AllowCaching is not a method on RepositoryRegistrationBuilder
```

### ✅ This will compile:
```csharp
builder.AddScopedRepository<UserRepository, User>()
	.AllowCaching()  // OK: AllowCaching is a method on RepositoryConfigurator<,>
```

## Caching Configuration

### CachedFunctionsMapping

The `CachedFunctionsMapping` class is a dictionary that maps method names to boolean values:

```csharp
public sealed class CachedFunctionsMapping : Dictionary<string, bool>
{
	public static readonly CachedFunctionsMapping Default = new()
	{
		{ nameof(IReadRepository<>.Find), true },
		{ nameof(IReadRepository<>.FindAsync), true },
		{ nameof(IReadRepository<>.FirstOrDefault), true },
		{ nameof(IReadRepository<>.FirstOrDefaultAsync), true },
		{ nameof(IReadRepository<>.GetAll), true },
		{ nameof(IReadRepository<>.GetAllAsync), true },
		{ nameof(IReadRepository<>.SingleOrDefault), true },
		{ nameof(IReadRepository<>.SingleOrDefaultAsync), true },
		{ nameof(IReadRepository<>.StreamAllAsync), true },
		{ nameof(IReadRepository<>.StreamWhereAsync), true },
	};
}
```

### Custom Configuration

```csharp
.AllowCaching(config =>
{
	// Only cache specific methods
	config[nameof(IReadRepository<User>.FindAsync)] = true;
	config[nameof(IReadRepository<User>.GetAllAsync)] = false;
})
```

### Default Configuration

```csharp
.AllowCaching()  // Uses CachedFunctionsMapping.Default
```

## Internal Implementation

### RepositoryServiceRegistration Record

```csharp
internal sealed record RepositoryServiceRegistration(
	ServiceLifetime ServiceLifetime,
	Type AbstractType,
	Type ImplType,
	Type EntityType,
	bool AllowCaching,
	CachedFunctionsMapping? CachingConfiguration = null);
```

### Registration Flow

1. User calls `AddScopedRepository<TRepository, TEntity>()`
2. `RepositoryRegistrationBuilder` creates a `RepositoryServiceRegistration` with `AllowCaching = false`
3. Returns `RepositoryConfigurator<TRepository, TEntity>`
4. User optionally calls `.AllowCaching(config => { ... })`
5. `RepositoryConfigurator` updates the registration record with caching enabled
6. When `Build()` is called, all registrations are processed

### Method Visibility Control

- `RepositoryConfigurator<TRepository, TEntity>` has `AllowCaching()` method
- `RepositoryRegistrationBuilder` does **not** have `AllowCaching()` method
- Type system enforces this at compile time

## Future Integration

The caching metadata stored in `RepositoryServiceRegistration` can be consumed by:

1. **Sumapap.Persistence.FusionCache** - Can read `AllowCaching` and `CachingConfiguration` to apply decorators
2. **Sumapap.Persistence.Caching** - Can provide base caching infrastructure
3. **Custom cache providers** - Can implement their own cache decoration logic

### Example FusionCache Integration (Future)

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		builder.AddScopedRepository<UserRepository, User>()
			.AllowCaching(config => { ... });

		builder.UseFusionCache();  // <-- Future: Apply FusionCache to all cached repos
	});
```

The `UseFusionCache()` method would:
1. Iterate through all `RepositoryServiceRegistration` records
2. Find ones with `AllowCaching = true`
3. Apply FusionCache decorators based on `CachingConfiguration`

## Backward Compatibility

The existing `RepositoryRegistration` class is kept for backward compatibility but no longer inherits from `RepositoryRegistrationBuilder`. It's a standalone class for legacy code.

## Benefits

✅ **Type-safe** - Compiler prevents calling `AllowCaching()` in wrong context
✅ **Opt-in** - Caching is explicitly enabled per repository
✅ **Flexible** - Can configure which methods are cached
✅ **Composable** - Integrates with existing `AddSumapap()` builder pattern
✅ **Clean API** - Fluent, readable, self-documenting
✅ **Extensible** - Easy to add new cache providers

## File Structure

```
Sumapap.Persistence/
├── DependencyInjection/
│   ├── SumapapServiceBuilderExtensions.cs       # Entry point
│   └── Builder/
│       ├── RepositoryRegistrationBuilder.cs     # Main builder
│       ├── RepositoryConfigurator<,>.cs         # Inside RepositoryRegistrationBuilder.cs
│       ├── RepositoryServiceRegistration.cs     # Registration records
│       └── RepositoryRegistration.cs            # Legacy class
└── Caching/
	└── CachedFunctionsMapping.cs                # Method cache configuration
```

## Notes

- The implementation is complete and compiles successfully
- The design follows your requirements exactly
- Ready for FusionCache integration in the future
- `AllowCaching()` visibility is enforced by the type system
