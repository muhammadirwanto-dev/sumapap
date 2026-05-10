# Sumapap.Persistence Caching Mechanism - Complete Design

## 🎯 Design Goals Achieved

✅ **Fluent API with Type Safety**
- `AllowCaching()` only visible after `AddScopedRepository()` / `AddTransientRepository()`
- Cannot call `builder.AllowCaching()` directly

✅ **Opt-in Caching**
- Each repository explicitly opts into caching
- Repositories without `AllowCaching()` are not cached

✅ **Rich Configuration**
- Method-level control (which methods to cache)
- Duration configuration
- Key prefix customization
- Custom metadata for provider-specific settings

✅ **Deferred Application**
- Cache configuration recorded during registration
- Actual caching applied later when `UseFusionCache()` is called
- Separates registration from caching logic

✅ **Provider-Agnostic**
- `RepositoryCacheRegistry` can be consumed by any cache provider
- FusionCache, Redis, MemoryCache, etc. can all integrate

## 📦 What Was Built

### 1. Core Caching Infrastructure (Sumapap.Persistence)

**New Files:**
- ✅ `Caching/RepositoryCacheRegistry.cs` - Registry storing all cache configurations
- ✅ `Caching/CacheProviderExtensions.cs` - Helper methods for cache providers

**Modified Files:**
- ✅ `DependencyInjection/Builder/RepositoryRegistrationBuilder.cs`
  - Populates `RepositoryCacheRegistry` during `Build()`
  - Passes cache configuration to registry entries
- ✅ `DependencyInjection/Builder/RepositoryServiceRegistration.cs`
  - Changed from `CachedFunctionsMapping` to `RepositoryCacheConfiguration`
- ✅ `DependencyInjection/Builder/RepositoryConfigurator.cs`
  - `AllowCaching(Action<RepositoryCacheConfiguration>)` with rich config
  - `AllowCaching()` overload for default configuration

**Existing Files (Used):**
- ✅ `Caching/CachedFunctionsMapping.cs` - Method-level cache flags

### 2. Documentation

- ✅ `docs/Sumapap.Persistence.FluentBuilder.md` - Complete usage guide
- ✅ `docs/FusionCache-Integration-Guide.md` - Implementation guide for FusionCache
- ✅ `docs/CSharp14-Extension-Syntax-Standard.md` - Modern C# extensions standard

### 3. Copilot Instructions

- ✅ `.github/copilot-instructions.md` - Updated with modern extension syntax rules

## 🔧 Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    User Code (Startup)                      │
├─────────────────────────────────────────────────────────────┤
│  services.AddSumapap()                                      │
│      .WithRepositories(builder =>                           │
│      {                                                      │
│          builder.AddScopedRepository<UserRepo, User>()      │
│              .AllowCaching(config => { ... });              │
│          builder.UseFusionCache(); ← FUTURE                 │
│      });                                                    │
└─────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────┐
│          Sumapap.Persistence (Registration)                 │
├─────────────────────────────────────────────────────────────┤
│  • RepositoryRegistrationBuilder                            │
│    - Registers repository in IServiceCollection             │
│    - Stores cache config in RepositoryCacheRegistry         │
│  • RepositoryConfigurator<TRepo, TEntity>                   │
│    - Provides AllowCaching() method                         │
│  • RepositoryCacheRegistry (Singleton)                      │
│    - Stores List<RepositoryCacheEntry>                      │
└─────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────┐
│    Sumapap.Persistence.FusionCache (Application) FUTURE     │
├─────────────────────────────────────────────────────────────┤
│  • FusionCacheBuilderExtensions.UseFusionCache()            │
│    - Reads RepositoryCacheRegistry                          │
│    - For each RepositoryCacheEntry:                         │
│      * Creates CachedRepository<TEntity> decorator          │
│      * Replaces service registrations                       │
│      * Passes RepositoryCacheConfiguration to decorator     │
│  • CachedRepository<TEntity>                                │
│    - Wraps inner repository                                 │
│    - Uses config.Methods to decide what to cache            │
│    - Uses config.Duration for cache expiration              │
│    - Uses config.KeyPrefix for cache keys                   │
└─────────────────────────────────────────────────────────────┘
```

## 📝 Usage Examples

### Basic Usage

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		// With detailed configuration
		builder.AddScopedRepository<UserRepository, User>()
			.AllowCaching(config =>
			{
				config.Methods[nameof(IReadRepository<User>.FindAsync)] = true;
				config.Methods[nameof(IReadRepository<User>.GetAllAsync)] = true;
				config.Duration = TimeSpan.FromMinutes(10);
				config.KeyPrefix = "user";
			});

		// With default configuration
		builder.AddScopedRepository<ProductRepository, Product>()
			.AllowCaching();

		// Without caching
		builder.AddScopedRepository<OrderRepository, Order>();
	});
```

### Future: With FusionCache

```csharp
services.AddSumapap()
	.WithRepositories(builder =>
	{
		builder.AddScopedRepository<UserRepository, User>()
			.AllowCaching(config =>
			{
				config.Methods[nameof(IReadRepository<User>.FindAsync)] = true;
				config.Duration = TimeSpan.FromMinutes(10);
			});

		// This will apply FusionCache to all repositories with AllowCaching()
		builder.UseFusionCache();  // ← TO BE IMPLEMENTED IN Sumapap.Persistence.FusionCache
	});
```

## 🔍 Key Classes

### RepositoryCacheConfiguration

```csharp
public sealed class RepositoryCacheConfiguration
{
	public CachedFunctionsMapping Methods { get; set; }        // Which methods to cache
	public TimeSpan? Duration { get; set; }                    // Cache duration
	public string? KeyPrefix { get; set; }                     // Cache key prefix
	public Dictionary<string, object> Metadata { get; }        // Provider-specific metadata
}
```

### RepositoryCacheEntry

```csharp
public sealed class RepositoryCacheEntry
{
	public required Type RepositoryType { get; init; }         // UserRepository
	public required Type EntityType { get; init; }             // User
	public required ServiceLifetime Lifetime { get; init; }    // Scoped/Transient
	public required RepositoryCacheConfiguration Configuration { get; init; }
	public List<Type> ServiceTypes { get; init; }             // [IRepository<User>, IReadRepository<User>, ...]
}
```

### RepositoryCacheRegistry

```csharp
public sealed class RepositoryCacheRegistry
{
	public IReadOnlyList<RepositoryCacheEntry> CachedRepositories { get; }
	internal void Register(RepositoryCacheEntry entry);
}
```

## 🎯 Type Safety Guarantees

### ❌ Won't Compile

```csharp
builder.AllowCaching();  // Error: method not found on RepositoryRegistrationBuilder
```

### ✅ Will Compile

```csharp
builder.AddScopedRepository<UserRepository, User>()
	.AllowCaching();  // OK: method exists on RepositoryConfigurator<,>
```

## 🚀 Next Steps for FusionCache Integration

The mechanism is **ready**. To complete the integration:

1. **In `Sumapap.Persistence.FusionCache`:**
   - Create `FusionCacheBuilderExtensions.cs`
   - Implement `extension(RepositoryRegistrationBuilder) { UseFusionCache() }`
   - Create `CachedRepository<TEntity>` decorator
   - Read `RepositoryCacheRegistry` and apply decorators

2. **Reference the guides:**
   - See `docs/FusionCache-Integration-Guide.md` for complete implementation
   - Follow the decorator pattern shown in the guide

## ✅ What's Complete

- ✅ Fluent builder API with type safety
- ✅ Cache configuration model
- ✅ Cache registry infrastructure
- ✅ Registry population during Build()
- ✅ Helper methods for cache providers
- ✅ Documentation and guides
- ✅ Modern C# 14 extension syntax
- ✅ Ready for FusionCache consumption

## 🔜 What's Next (Not Done Yet)

- 🔜 Implement `UseFusionCache()` in Sumapap.Persistence.FusionCache
- 🔜 Create `CachedRepository<TEntity>` decorator
- 🔜 Test end-to-end caching behavior

## 📊 Testing

You can test the registry population:

```csharp
[Fact]
public void Should_Populate_Cache_Registry()
{
	var services = new ServiceCollection();

	services.AddSumapap()
		.WithRepositories(builder =>
		{
			builder.AddScopedRepository<UserRepository, User>()
				.AllowCaching(config =>
				{
					config.Duration = TimeSpan.FromMinutes(10);
					config.KeyPrefix = "user";
				});
		});

	// Build the service provider to trigger registration
	var provider = services.BuildServiceProvider();

	// Get the registry
	var registry = services.GetRepositoryCacheRegistry();

	Assert.NotNull(registry);
	Assert.Single(registry.CachedRepositories);

	var entry = registry.CachedRepositories[0];
	Assert.Equal(typeof(UserRepository), entry.RepositoryType);
	Assert.Equal(typeof(User), entry.EntityType);
	Assert.Equal(ServiceLifetime.Scoped, entry.Lifetime);
	Assert.Equal(TimeSpan.FromMinutes(10), entry.Configuration.Duration);
	Assert.Equal("user", entry.Configuration.KeyPrefix);
}
```

## 🎉 Summary

The Sumapap.Persistence caching mechanism is **complete and ready for consumption**.

- **API Design**: ✅ Done
- **Registry Infrastructure**: ✅ Done
- **Configuration Model**: ✅ Done
- **Documentation**: ✅ Done
- **FusionCache Ready**: ✅ Ready (just needs implementation following the guide)

The design achieves all your requirements:
- ✅ `AllowCaching()` only visible after repository registration
- ✅ Opt-in caching per repository
- ✅ Rich configuration (methods, duration, prefix, metadata)
- ✅ Deferred application via `UseFusionCache()`
- ✅ Method-level cache control
- ✅ Type-safe, fluent API
