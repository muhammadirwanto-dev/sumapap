# Sumapap.Persistence.Caching

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Persistence.Caching.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.Caching/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.Caching.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.Caching/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

``Sumapap.Persistence.Caching`` provides provider-agnostic caching infrastructure for Sumapap persistence repositories using the Visitor pattern. The package focuses on:

- Visitor-based cache decorator registration for extensibility
- Opt-in repository caching via ``AllowCaching()`` configuration
- Metadata-driven caching without tight coupling to specific cache implementations
- Centralized cache registry for inspection and provider consumption
- Granular control over which repository methods should be cached

The goal is to enable flexible, testable repository caching while maintaining clean separation between persistence and caching concerns.

## ✨ Why use ``Sumapap.Persistence.Caching``?

- **Visitor Pattern Architecture**: Cache decoration logic is isolated in visitors, enabling extensibility without modifying core registration code
- **Opt-in by Design**: Repositories explicitly declare caching intent; caching is never forced globally
- **Provider-Agnostic**: Works with any cache implementation (FusionCache, Redis, MemoryCache) through registry consumption
- **Metadata-Driven**: Configuration is stored as metadata and applied later by cache providers, enabling inspection and testing
- **Method-Level Control**: Fine-grained configuration of which repository methods should be cached (reads only, all, or custom)
- **Testable**: Cache registry can be inspected in tests to verify caching configuration

## 🚀 Quick start

1. Add the package to your Infrastructure layer project:

```bash
dotnet add package Sumapap.Persistence.Caching
```

2. Configure repositories with opt-in caching in your DI setup:

```csharp
services.AddSumapap()
    .WithRepositories(builder =>
    {
        builder
            .AddScopedRepository<IUserRepository, UserRepository, User>()
            .AllowCaching(config =>
            {
                config.Duration = TimeSpan.FromMinutes(5);
                config.Methods.EnableAllReads(); // Cache all read operations
            });
            
        builder.UseRepositoryCaching(); // Register visitor
    });
```

3. Add a cache provider (e.g., FusionCache) to consume the registry:

```csharp
services.AddSumapap()
    .WithRepositories(builder =>
    {
        // ... repository registrations with AllowCaching()
        builder.UseRepositoryCaching();
    })
    .UseFusionCache(); // Provider consumes RepositoryCacheRegistry
```

4. The cache provider decorates registered repositories automatically based on metadata.

## 🛠 Features and usage

### Visitor Pattern Architecture

The caching infrastructure uses the Visitor pattern to separate cache decoration from core repository registration:

```
Repository Registration → AllowCaching() → Metadata Stored
                                                 ↓
                                     CachingRepositoryVisitor
                                                 ↓
                                       RepositoryCacheRegistry
                                                 ↓
                                     Cache Provider (FusionCache, etc.)
```

**Key Components:**

1. **CachingRepositoryVisitor**: Inspects registrations and populates cache registry
2. **RepositoryCacheRegistry**: Stores metadata about repositories with caching enabled
3. **RepositoryCacheConfiguration**: Defines which methods to cache and cache behavior
4. **CachedFunctionsMapping**: Default list of cacheable repository methods

### Opt-In Caching Configuration

**AllowCaching()** - Enable caching for a repository with configuration:

```csharp
builder
    .AddScopedRepository<IProductRepository, ProductRepository, Product>()
    .AllowCaching(config =>
    {
        config.Duration = TimeSpan.FromMinutes(10);
        config.Methods.EnableAllReads(); // Cache all read operations
        config.Metadata["Priority"] = "High"; // Custom metadata
    });
```

**Default Configuration** (minimal):
```csharp
builder
    .AddScopedRepository<IOrderRepository, OrderRepository, Order>()
    .AllowCaching(); // Uses default: 5 minutes, all read methods
```

### Method-Level Cache Control

**EnableAllReads()** - Cache all read repository methods (default):
```csharp
config.Methods.EnableAllReads();
// Caches: Find, GetAll, FirstOrDefault, SingleOrDefault, Count, Any, Stream*
```

**EnableSpecific()** - Cache only specific methods:
```csharp
config.Methods.Clear();
config.Methods.EnableMethod("FindAsync");
config.Methods.EnableMethod("GetAllAsync");
// Only FindAsync and GetAllAsync are cached
```

**DisableMethod()** - Exclude specific methods from caching:
```csharp
config.Methods.EnableAllReads();
config.Methods.DisableMethod("StreamAllAsync"); // Disable streaming method cache
```

### Cache Registry Inspection

**RepositoryCacheRegistry** - Central registry of cached repository configurations:

```csharp
public sealed class RepositoryCacheRegistry
{
    public IReadOnlyList<RepositoryCacheEntry> CachedRepositories { get; }
}
```

**Access the registry** (useful for testing or runtime inspection):
```csharp
var registry = serviceProvider.GetRequiredService<RepositoryCacheRegistry>();

foreach (var entry in registry.CachedRepositories)
{
    Console.WriteLine($"Repository: {entry.RepositoryType.Name}");
    Console.WriteLine($"Duration: {entry.Configuration.Duration}");
    Console.WriteLine($"Cached Methods: {string.Join(", ", entry.Configuration.Methods)}");
}
```

### Repository Cache Entry

**RepositoryCacheEntry** - Represents a cached repository registration:

```csharp
public sealed class RepositoryCacheEntry
{
    public required Type RepositoryType { get; init; }
    public required Type EntityType { get; init; }
    public required ServiceLifetime Lifetime { get; init; }
    public required RepositoryCacheConfiguration Configuration { get; init; }
}
```

### Caching Repository Visitor

**CachingRepositoryVisitor** - Processes registrations with caching metadata:

```csharp
public class CachingRepositoryVisitor : IRepositoryRegistrationVisitor
{
    public void Visit(RepositoryRegistrationEntry entry, IServiceCollection services)
    {
        if (!entry.AllowCaching || entry.CachingConfiguration is null)
            return;
            
        var cacheRegistry = GetOrCreateCacheRegistry(services);
        cacheRegistry.Register(new RepositoryCacheEntry
        {
            RepositoryType = entry.ImplementationType,
            EntityType = entry.EntityType,
            Lifetime = entry.Lifetime,
            Configuration = entry.CachingConfiguration
        });
    }
}
```

### Default Cached Methods

By default, the following read-only repository methods are cached:

**Synchronous:**
- ``Find(id)``
- ``GetAll()``
- ``FirstOrDefault()``
- ``SingleOrDefault()``
- ``Count()``
- ``Any()``

**Asynchronous:**
- ``FindAsync(id)``
- ``GetAllAsync()``
- ``FirstOrDefaultAsync()``
- ``SingleOrDefaultAsync()``
- ``CountAsync()``
- ``AnyAsync()``
- ``StreamAllAsync()``
- ``StreamWhereAsync()``

**Write operations are never cached** (``Add``, ``Update``, ``Delete``, ``SaveChanges``).

### Cache Provider Integration

Cache providers consume the ``RepositoryCacheRegistry`` to apply decorators:

```csharp
// Inside a cache provider (e.g., Sumapap.Persistence.FusionCache)
public static ISumapapBuilder UseFusionCache(this ISumapapBuilder builder)
{
    builder.Services.AddFusionCache();
    
    // Consume the cache registry
    var registry = builder.Services.BuildServiceProvider()
        .GetRequiredService<RepositoryCacheRegistry>();
    
    foreach (var cacheEntry in registry.CachedRepositories)
    {
        // Decorate the repository service with caching logic
        builder.Services.Decorate(cacheEntry.RepositoryType, (inner, sp) =>
        {
            var cache = sp.GetRequiredService<IFusionCache>();
            return new CachedRepository(inner, cache, cacheEntry.Configuration);
        });
    }
    
    return builder;
}
```

## ⚠️ Notes & best practices

### ✅ Do

- **Always call ``UseRepositoryCaching()``** after adding repositories with ``AllowCaching()`` to register the visitor
- **Use default configuration** for typical CRUD repositories (covers common scenarios)
- **Customize cache duration** based on data volatility (short for frequently changing data, longer for reference data)
- **Inspect the registry in tests** to verify caching is configured correctly
- **Use ``DisableMethod()``** for expensive queries that should not be cached (e.g., large streaming operations)

### ❌ Don''t

- **Never cache write operations** - the default configuration only caches reads; do not enable caching for ``Add``, ``Update``, ``Delete``
- **Avoid very long cache durations** for frequently changing data (leads to stale reads)
- **Don''t forget to register the visitor** - calling ``AllowCaching()`` without ``UseRepositoryCaching()`` has no effect
- **Avoid caching streaming methods** for very large datasets (can cause memory pressure)
- **Don''t use caching for real-time data** where staleness is unacceptable

### Cache Invalidation

This package only handles cache decoration and configuration. **Cache invalidation is the responsibility of the cache provider** (e.g., ``Sumapap.Persistence.FusionCache``). Refer to your cache provider documentation for invalidation strategies.

### Testing Recommendations

When testing repositories with caching:
1. **Test without cache first** to verify repository logic
2. **Verify cache configuration** by inspecting ``RepositoryCacheRegistry``
3. **Test cache behavior** by verifying cache provider decorators are applied
4. **Mock the cache provider** when testing repository consumers to avoid cache side effects

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