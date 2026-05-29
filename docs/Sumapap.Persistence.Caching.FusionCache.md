# Sumapap.Persistence.Caching.FusionCache

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Persistence.Caching.FusionCache.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.Caching.FusionCache/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.Caching.FusionCache.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.Caching.FusionCache/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Persistence.Caching.FusionCache` is a concrete caching provider implementation for Sumapap repositories using [FusionCache](https://github.com/ZiggyCreatures/FusionCache). This package provides:

- Automatic repository decorator generation using FusionCache
- Cached implementations for read, write, and read-write repositories
- Tag-based cache invalidation on write operations
- Integration with Sumapap.Persistence.Caching infrastructure
- Support for configurable cache durations and behaviors

The package builds on the metadata-driven caching infrastructure from `Sumapap.Persistence.Caching` and implements the actual caching logic using FusionCache's high-performance caching engine.

## ✨ Why use `Sumapap.Persistence.Caching.FusionCache`?

- **Production-Ready**: Built on FusionCache, a battle-tested caching library with advanced features
- **Automatic Decoration**: Repositories are automatically wrapped with caching based on configuration
- **Intelligent Invalidation**: Write operations automatically invalidate related cached entries using tags
- **Performance**: Reduces database load for frequently accessed data with configurable TTL
- **Flexible Configuration**: Per-repository and per-method caching control
- **Seamless Integration**: Works with existing Sumapap repository infrastructure

## 🚀 Quick start

1. Add the package to your project:

```bash
dotnet add package Sumapap.Persistence.Caching.FusionCache
```

2. Register FusionCache and configure repositories:

```csharp
using Sumapap.Persistence.Caching.FusionCache.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSumapap()
	.WithRepositories(repos =>
	{
		// Register repositories with caching enabled
		repos
			.AddScopedRepository<IUserRepository, UserRepository, User>()
			.AllowCaching(config =>
			{
				config.Duration = TimeSpan.FromMinutes(5);
				config.Methods.EnableAllReads();
			});

		repos
			.AddScopedRepository<IProductRepository, ProductRepository, Product>()
			.AllowCaching(config =>
			{
				config.Duration = TimeSpan.FromMinutes(10);
				config.Methods.EnableAllReads();
			});

		// Enable FusionCache provider
		repos.UseCacheProvider();
	});

// Configure FusionCache
builder.Services.AddFusionCache()
	.WithDefaultEntryOptions(options =>
	{
		options.Duration = TimeSpan.FromMinutes(5);
		options.Priority = CacheItemPriority.Normal;
	});
```

3. Inject and use repositories as normal:

```csharp
public class UserService
{
	private readonly IUserRepository _repository;

	public UserService(IUserRepository repository)
	{
		_repository = repository;
	}

	public async Task<User?> GetUserAsync(int userId)
	{
		// Automatically cached on first call
		return await _repository.FindAsync(userId);
	}

	public async Task UpdateUserAsync(User user)
	{
		// Automatically invalidates cache
		await _repository.UpdateAsync(user);
		await _repository.SaveAsync();
	}
}
```

## 🛠 Features and usage

### UseCacheProvider Extension Method

Registers the FusionCache provider with the repository infrastructure:

```csharp
public static IServiceCollection UseCacheProvider(this IPersistenceBuilder builder)
```

**Usage:**
```csharp
builder.Services.AddSumapap()
	.WithRepositories(repos =>
	{
		repos
			.AddScopedRepository<IUserRepository, UserRepository, User>()
			.AllowCaching(config =>
			{
				config.Duration = TimeSpan.FromMinutes(5);
				config.Methods.EnableAllReads();
			});

		// Register FusionCache provider
		repos.UseCacheProvider();
	});
```

This method:
1. Registers the `CachedRepositoryVisitor` to detect repositories with caching enabled
2. Registers the `RepositoryDecorationVisitor` to wrap repositories with cached implementations
3. Connects to the FusionCache instance configured in DI

### Cached Repository Implementations

The package provides three cached repository decorators:

**CachedReadRepository** - Wraps `IReadRepository<TEntity, TContext>`:
- Caches all read operations (Find, GetAll, Count, etc.)
- Uses entity-specific cache keys with tags for invalidation
- Bypasses cache for streaming operations (StreamAllAsync, StreamWhereAsync)

**CachedWriteRepository** - Wraps `IWriteRepository<TEntity, TContext>`:
- Invalidates cache tags on write operations (Add, Update, Delete)
- Ensures cache consistency after data modifications

**CachedReadWriteRepository** - Wraps `IReadWriteRepository<TEntity, TContext>`:
- Combines read caching and write invalidation
- Single decorator for repositories with both read and write operations

### Cache Key Generation

Cache keys are automatically generated using the `ICacheKeyProvider`:

```csharp
// Examples of generated cache keys (with kebab-case):
"user:123:*"              // Find by ID
"user:*:count"            // Count all
"user:*:count:{hash}"     // Count with predicate
"user:*:first-or-default:{hash}"  // FirstOrDefault with predicate
"user:*"                  // GetAll
```

Each key includes:
- **Entity type**: Kebab-cased entity name (e.g., "user", "product")
- **Operation**: Method name or "*" for item-specific queries
- **Parameters**: Hashed representation of predicates/specifications

### Tag-Based Invalidation

Write operations automatically invalidate related cache entries using tags:

```csharp
// When updating a User entity:
await _repository.UpdateAsync(user);
await _repository.SaveAsync();

// FusionCache invalidates all tags:
// - "user:*" (all user queries)
// - "user:{id}:*" (specific user queries)
```

**Tag Structure:**
```csharp
GetAllItemTag()   // Returns "user:*"
GetItemTag(id)    // Returns "user:123:*"
```

### Configuration Options

Caching behavior is controlled through `RepositoryCacheConfiguration`:

```csharp
repos
	.AddScopedRepository<IUserRepository, UserRepository, User>()
	.AllowCaching(config =>
	{
		// Set cache duration (defaults to FusionCache options if not specified)
		config.Duration = TimeSpan.FromMinutes(5);

		// Configure which methods to cache
		config.Methods.EnableAllReads();  // Cache all read methods
		// or
		config.Methods.Enable("Find", "GetAll");  // Cache specific methods
		// or
		config.Methods.EnableAll();  // Cache all methods (read + write)
	});
```

### FusionCache Integration

The package leverages FusionCache features:

```csharp
builder.Services.AddFusionCache()
	.WithDefaultEntryOptions(options =>
	{
		// Default duration (can be overridden per repository)
		options.Duration = TimeSpan.FromMinutes(5);

		// Cache priority
		options.Priority = CacheItemPriority.Normal;

		// Fail-safe settings
		options.IsFailSafeEnabled = true;
		options.FailSafeMaxDuration = TimeSpan.FromHours(1);
		options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
	})
	// Optional: Add distributed cache (Redis, SQL Server, etc.)
	.WithDistributedCache(/* distributed cache configuration */);
```

### Complete Example

```csharp
using Microsoft.EntityFrameworkCore;
using Sumapap.Persistence.Caching.FusionCache.DependencyInjection;
using Sumapap.Persistence.EfCore.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

// Configure database
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure FusionCache
builder.Services.AddFusionCache()
	.WithDefaultEntryOptions(options =>
	{
		options.Duration = TimeSpan.FromMinutes(5);
		options.IsFailSafeEnabled = true;
	})
	.WithSerializer(new FusionCacheSystemTextJsonSerializer())
	.WithDistributedCache(/* optional: add Redis or other distributed cache */);

// Configure Sumapap repositories with caching
builder.Services.AddSumapap()
	.WithRepositories(repos =>
	{
		// User repository with 5-minute cache
		repos
			.AddScopedRepository<IUserRepository, UserRepository, User>()
			.AllowCaching(config =>
			{
				config.Duration = TimeSpan.FromMinutes(5);
				config.Methods.EnableAllReads();
			});

		// Product repository with 10-minute cache
		repos
			.AddScopedRepository<IProductRepository, ProductRepository, Product>()
			.AllowCaching(config =>
			{
				config.Duration = TimeSpan.FromMinutes(10);
				config.Methods.EnableAllReads();
			});

		// Order repository without caching (transactional data)
		repos.AddScopedRepository<IOrderRepository, OrderRepository, Order>();

		// Enable FusionCache provider
		repos.UseCacheProvider();
	});

var app = builder.Build();

// Service usage
app.MapGet("/users/{id}", async (int id, IUserRepository repo) =>
{
	// First call: queries database and caches result
	// Subsequent calls: returns from cache
	var user = await repo.FindAsync(id);
	return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPut("/users/{id}", async (int id, User user, IUserRepository repo) =>
{
	user.Id = id;

	// Update database and invalidate cache
	await repo.UpdateAsync(user);
	await repo.SaveAsync();

	return Results.NoContent();
});

app.Run();
```

## ⚠️ Notes & best practices

### When to Cache

**✅ Good candidates for caching:**
- Reference data (categories, countries, settings)
- User profiles and preferences
- Product catalogs
- Frequently accessed, rarely modified data

**❌ Poor candidates for caching:**
- Transactional data (orders, payments)
- Real-time data (stock prices, sensor readings)
- Data with complex invalidation requirements
- Highly personalized data

### Cache Duration Guidelines

```csharp
// Reference data (rarely changes)
config.Duration = TimeSpan.FromHours(1);

// User data (moderate update frequency)
config.Duration = TimeSpan.FromMinutes(15);

// Dynamic content (frequent updates)
config.Duration = TimeSpan.FromMinutes(2);

// No duration specified: uses FusionCache default
config.Methods.EnableAllReads();
```

### Invalidation Strategy

The package uses tag-based invalidation:

```csharp
// Write operations automatically invalidate:
await repo.AddAsync(user);        // Invalidates "user:*"
await repo.UpdateAsync(user);     // Invalidates "user:*" and "user:{id}:*"
await repo.DeleteAsync(user);     // Invalidates "user:*" and "user:{id}:*"
await repo.SaveAsync();           // Triggers invalidation
```

### Performance Considerations

- **Memory usage**: Monitor cache memory consumption with large datasets
- **Serialization**: Use efficient serializers (System.Text.Json is default)
- **Distributed cache**: Add Redis for multi-server deployments
- **Cache key complexity**: Complex predicates result in longer cache keys

### Testing

Mock repositories directly without caching in unit tests:

```csharp
[Fact]
public async Task GetUser_ReturnsUser()
{
	// Arrange
	var mockRepo = new Mock<IUserRepository>();
	mockRepo.Setup(r => r.FindAsync(1, default))
		.ReturnsAsync(new User { Id = 1, Name = "John" });

	var service = new UserService(mockRepo.Object);

	// Act
	var result = await service.GetUserAsync(1);

	// Assert
	Assert.NotNull(result);
	Assert.Equal("John", result.Name);
}
```

For integration tests with caching:

```csharp
[Fact]
public async Task UpdateUser_InvalidatesCache()
{
	// Arrange
	var services = new ServiceCollection();
	services.AddDbContext<TestDbContext>();
	services.AddFusionCache();
	services.AddSumapap()
		.WithRepositories(repos =>
		{
			repos.AddScopedRepository<IUserRepository, UserRepository, User>()
				.AllowCaching(config => config.Methods.EnableAllReads());
			repos.UseCacheProvider();
		});

	var sp = services.BuildServiceProvider();
	var repo = sp.GetRequiredService<IUserRepository>();

	// Act
	var user = await repo.FindAsync(1); // Cached
	user.Name = "Updated";
	await repo.UpdateAsync(user);
	await repo.SaveAsync(); // Invalidates cache
	var updated = await repo.FindAsync(1); // Fresh from database

	// Assert
	Assert.Equal("Updated", updated.Name);
}
```

### Monitoring

Monitor cache effectiveness using FusionCache metrics:

```csharp
builder.Services.AddFusionCache()
	.WithOptions(options =>
	{
		options.EnableSyncEventHandlersExecution = true;
	})
	.WithPostSetup((sp, cache) =>
	{
		cache.Events.Hit += (sender, e) =>
		{
			var logger = sp.GetRequiredService<ILogger<Program>>();
			logger.LogDebug("Cache HIT: {Key}", e.Key);
		};

		cache.Events.Miss += (sender, e) =>
		{
			var logger = sp.GetRequiredService<ILogger<Program>>();
			logger.LogDebug("Cache MISS: {Key}", e.Key);
		};
	});
```

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
	<img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>
