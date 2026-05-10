# FusionCache Integration Guide

## Overview

This guide shows how `Sumapap.Persistence.FusionCache` can integrate with the repository caching mechanism provided by `Sumapap.Persistence`.

## Architecture

The Persistence layer provides:
- ✅ `RepositoryCacheRegistry` - Singleton storing all cached repository configurations
- ✅ `RepositoryCacheEntry` - Individual cache configuration per repository
- ✅ `RepositoryCacheConfiguration` - Rich config (methods, duration, prefix, metadata)
- ✅ `CacheProviderExtensions.GetRepositoryCacheRegistry()` - Helper to access the registry

FusionCache will:
- 🔜 Read the registry to find all repositories with `AllowCaching()`
- 🔜 Apply decorator pattern to wrap repositories with caching logic
- 🔜 Use the configuration to control cache behavior

## Implementation Steps

### Step 1: Create Extension Method in Sumapap.Persistence.FusionCache

**File: `Sumapap.Persistence.FusionCache/FusionCacheBuilderExtensions.cs`**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Caching;
using Sumapap.Persistence.DependencyInjection.Builder;
using ZiggyCreatures.Caching.Fusion;

namespace Sumapap.Persistence.FusionCache
{
	public static class FusionCacheBuilderExtensions
	{
		extension(RepositoryRegistrationBuilder builder)
		{
			/// <summary>
			/// Applies FusionCache decorators to all repositories that have enabled caching.
			/// </summary>
			/// <param name="configureFusion">Optional FusionCache configuration.</param>
			/// <returns>The builder for method chaining.</returns>
			public RepositoryRegistrationBuilder UseFusionCache(
				Action<FusionCacheOptions>? configureFusion = null)
			{
				var services = builder.Services;

				// 1. Register FusionCache if not already registered
				if (!services.Any(d => d.ServiceType == typeof(IFusionCache)))
				{
					services.AddFusionCache().TryWithAutoSetup();
				}

				if (configureFusion != null)
				{
					services.Configure(configureFusion);
				}

				// 2. Get the cache registry
				var registry = services.GetRepositoryCacheRegistry();

				if (registry == null || registry.CachedRepositories.Count == 0)
				{
					// No repositories with caching enabled
					return builder;
				}

				// 3. Apply decorators to each cached repository
				foreach (var entry in registry.CachedRepositories)
				{
					ApplyCacheDecorator(services, entry);
				}

				return builder;
			}
		}

		private static void ApplyCacheDecorator(
			IServiceCollection services,
			RepositoryCacheEntry entry)
		{
			// Build the decorator type: CachedRepository<TEntity>
			var decoratorType = typeof(CachedRepository<>).MakeGenericType(entry.EntityType);

			// For each service type this repository implements
			foreach (var serviceType in entry.ServiceTypes)
			{
				// Find the existing service registration
				var existingDescriptor = services.LastOrDefault(d => d.ServiceType == serviceType);

				if (existingDescriptor == null)
					continue;

				// Remove the original registration
				services.Remove(existingDescriptor);

				// Add decorated version
				services.Add(new ServiceDescriptor(
					serviceType,
					sp =>
					{
						// Resolve the inner repository
						var innerRepo = ActivatorUtilities.CreateInstance(sp, entry.RepositoryType);

						// Resolve FusionCache
						var fusionCache = sp.GetRequiredService<IFusionCache>();

						// Create the decorator with configuration
						return ActivatorUtilities.CreateInstance(
							sp,
							decoratorType,
							innerRepo,
							fusionCache,
							entry.Configuration);
					},
					entry.Lifetime));
			}
		}
	}
}
```

### Step 2: Create the Cached Repository Decorator

**File: `Sumapap.Persistence.FusionCache/Repositories/CachedRepository.cs`**

```csharp
using Sumapap.Persistence.Abstraction;
using Sumapap.Persistence.Caching;
using ZiggyCreatures.Caching.Fusion;

namespace Sumapap.Persistence.FusionCache.Repositories
{
	/// <summary>
	/// Decorator that adds FusionCache to repository operations.
	/// Uses RepositoryCacheConfiguration to determine what and how to cache.
	/// </summary>
	internal class CachedRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
		where TEntity : class, IEntity
	{
		private readonly IReadRepository<TEntity> _innerRead;
		private readonly IWriteRepository<TEntity>? _innerWrite;
		private readonly IFusionCache _cache;
		private readonly RepositoryCacheConfiguration _config;
		private readonly string _keyPrefix;

		public CachedRepository(
			object innerRepository,  // Can be IReadRepository, IWriteRepository, or both
			IFusionCache cache,
			RepositoryCacheConfiguration config)
		{
			_innerRead = (IReadRepository<TEntity>)innerRepository;
			_innerWrite = innerRepository as IWriteRepository<TEntity>;
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_keyPrefix = config.KeyPrefix ?? typeof(TEntity).Name;
		}

		// Helper to check if method should be cached
		private bool ShouldCache(string methodName)
		{
			return _config.Methods.TryGetValue(methodName, out var shouldCache) && shouldCache;
		}

		private FusionCacheEntryOptions GetCacheOptions()
		{
			var duration = _config.Duration ?? TimeSpan.FromMinutes(5);
			var options = new FusionCacheEntryOptions { Duration = duration };

			// Apply custom metadata if specified
			if (_config.Metadata.TryGetValue("FusionCache:FailSafe", out var failSafe))
			{
				options.IsFailSafeEnabled = (bool)failSafe;
			}

			if (_config.Metadata.TryGetValue("FusionCache:Priority", out var priority))
			{
				options.Priority = (int)priority;
			}

			return options;
		}

		// IReadRepository<TEntity> implementation

		public async ValueTask<TEntity?> FindAsync<TKey>(TKey key, CancellationToken cancellation = default)
			where TKey : IEquatable<TKey>
		{
			if (!ShouldCache(nameof(FindAsync)))
			{
				return await _innerRead.FindAsync(key, cancellation);
			}

			var cacheKey = $"{_keyPrefix}:Find:{key}";
			return await _cache.GetOrSetAsync(
				cacheKey,
				async ct => await _innerRead.FindAsync(key, ct),
				GetCacheOptions(),
				cancellation);
		}

		public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellation = default)
		{
			if (!ShouldCache(nameof(GetAllAsync)))
			{
				return await _innerRead.GetAllAsync(cancellation);
			}

			var cacheKey = $"{_keyPrefix}:GetAll";
			return await _cache.GetOrSetAsync(
				cacheKey,
				async ct => await _innerRead.GetAllAsync(ct),
				GetCacheOptions(),
				cancellation);
		}

		public async Task<TEntity?> FirstOrDefaultAsync(
			ISpecification<TEntity> specification,
			CancellationToken cancellation = default)
		{
			if (!ShouldCache(nameof(FirstOrDefaultAsync)))
			{
				return await _innerRead.FirstOrDefaultAsync(specification, cancellation);
			}

			// Use specification hash for cache key
			var specHash = specification.GetHashCode();
			var cacheKey = $"{_keyPrefix}:FirstOrDefault:{specHash}";

			return await _cache.GetOrSetAsync(
				cacheKey,
				async ct => await _innerRead.FirstOrDefaultAsync(specification, ct),
				GetCacheOptions(),
				cancellation);
		}

		// ... Implement other IReadRepository methods similarly ...

		// IWriteRepository<TEntity> implementation
		// Write operations should invalidate cache

		public void Add(TEntity entity)
		{
			if (_innerWrite == null)
				throw new NotSupportedException("This repository does not support write operations.");

			_innerWrite.Add(entity);
			InvalidateCache(entity);
		}

		public void Update(TEntity entity)
		{
			if (_innerWrite == null)
				throw new NotSupportedException("This repository does not support write operations.");

			_innerWrite.Update(entity);
			InvalidateCache(entity);
		}

		public void Delete(TEntity entity)
		{
			if (_innerWrite == null)
				throw new NotSupportedException("This repository does not support write operations.");

			_innerWrite.Delete(entity);
			InvalidateCache(entity);
		}

		private void InvalidateCache(TEntity entity)
		{
			// Invalidate entity-specific cache
			if (entity is IEntity<Guid> guidEntity)
			{
				_cache.Remove($"{_keyPrefix}:Find:{guidEntity.Id}");
			}
			else if (entity is IEntity<int> intEntity)
			{
				_cache.Remove($"{_keyPrefix}:Find:{intEntity.Id}");
			}

			// Invalidate collection caches
			_cache.Remove($"{_keyPrefix}:GetAll");

			// For more sophisticated invalidation, consider using cache tags
			// or implementing IChangeTracker pattern
		}

		// Delegate other methods to inner repository...
	}
}
```

### Step 3: Usage Example

```csharp
// In your application startup
services.AddSumapap()
	.WithRepositories(builder =>
	{
		// Register repositories with caching
		builder.AddScopedRepository<UserRepository, User>()
			.AllowCaching(config =>
			{
				// Configure which methods to cache
				config.Methods[nameof(IReadRepository<User>.FindAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.GetAllAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.FirstOrDefaultAsync)] = true;

				// Configure cache duration
				config.Duration = TimeSpan.FromMinutes(10);

				// Configure cache key prefix
				config.KeyPrefix = "user";

				// FusionCache-specific settings
				config.Metadata["FusionCache:FailSafe"] = true;
				config.Metadata["FusionCache:Priority"] = 10;
			});

		builder.AddScopedRepository<ProductRepository, Product>()
			.AllowCaching(); // Use default configuration

		builder.AddScopedRepository<OrderRepository, Order>();
		// No caching for OrderRepository

		// Apply FusionCache to all repositories with AllowCaching
		builder.UseFusionCache(options =>
		{
			// Global FusionCache configuration
			options.DefaultEntryOptions = new FusionCacheEntryOptions
			{
				Duration = TimeSpan.FromMinutes(5),
				IsFailSafeEnabled = true
			};
		});
	});
```

## What Happens

1. **Registration Phase** (`WithRepositories`):
   - `AddScopedRepository<UserRepository, User>()` registers the repository
   - `.AllowCaching(config => {...})` stores the configuration in `RepositoryCacheRegistry`
   - Same for `ProductRepository`
   - `OrderRepository` is registered without caching

2. **Cache Application Phase** (`UseFusionCache`):
   - FusionCache extension reads `RepositoryCacheRegistry`
   - Finds 2 entries: `UserRepository` and `ProductRepository`
   - For each entry:
	 - Creates `CachedRepository<User>` / `CachedRepository<Product>`
	 - Replaces `IRepository<User>` → `CachedRepository<User>(innerRepo: UserRepository)`
	 - Replaces `IReadRepository<User>` → same decorator instance
	 - Passes `RepositoryCacheConfiguration` to the decorator

3. **Runtime**:
   - When you inject `IRepository<User>`, you get `CachedRepository<User>`
   - The decorator checks `config.Methods["FindAsync"]` → true → cache it
   - Uses `config.Duration` for cache entry options
   - Uses `config.KeyPrefix` for cache key construction

## Benefits

✅ **Separation of Concerns**: Registration logic separate from caching logic
✅ **Lazy Application**: Cache decorators only applied when `UseFusionCache()` is called
✅ **Configuration-Driven**: All caching behavior driven by `RepositoryCacheConfiguration`
✅ **Type-Safe**: Compile-time guarantees on method availability
✅ **Flexible**: Can support multiple cache providers (Redis, MemoryCache, etc.)
✅ **Observable**: Cache provider can inspect all configurations before applying

## Testing

The registry can be inspected in tests:

```csharp
[Fact]
public void Should_Register_Cached_Repositories()
{
	var services = new ServiceCollection();

	services.AddSumapap()
		.WithRepositories(builder =>
		{
			builder.AddScopedRepository<UserRepository, User>()
				.AllowCaching(config => config.Duration = TimeSpan.FromMinutes(10));
		});

	var registry = services.GetRepositoryCacheRegistry();

	Assert.NotNull(registry);
	Assert.Single(registry.CachedRepositories);
	Assert.Equal(typeof(UserRepository), registry.CachedRepositories[0].RepositoryType);
	Assert.Equal(TimeSpan.FromMinutes(10), registry.CachedRepositories[0].Configuration.Duration);
}
```

## Alternative Cache Providers

Other cache providers can follow the same pattern:

```csharp
// In Sumapap.Persistence.Redis
extension(RepositoryRegistrationBuilder builder)
{
	public RepositoryRegistrationBuilder UseRedisCache(
		Action<RedisOptions>? configure = null)
	{
		var registry = builder.Services.GetRepositoryCacheRegistry();

		// Apply Redis-based caching decorators
		// ...

		return builder;
	}
}
```

## Summary

The mechanism is now ready:
- ✅ `RepositoryCacheRegistry` captures all caching intent
- ✅ `RepositoryCacheConfiguration` provides rich configuration
- ✅ `CacheProviderExtensions.GetRepositoryCacheRegistry()` provides easy access
- 🔜 FusionCache can implement `UseFusionCache()` as shown above
- 🔜 Other providers can follow the same pattern
