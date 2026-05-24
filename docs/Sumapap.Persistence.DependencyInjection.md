# Sumapap.Persistence.DependencyInjection

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Persistence.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.DependencyInjection/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.DependencyInjection/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Persistence.DependencyInjection` provides fluent dependency injection builders and extensions for configuring Sumapap persistence repositories and caching. The package focuses on:

- Type-safe fluent API for repository registration (scoped, transient, generic)
- Visitor pattern architecture for extensible repository decoration
- Opt-in caching configuration with fine-grained method control
- Separation of DI concerns from core persistence abstractions
- Modern C# 14 extension syntax for better IntelliSense

The goal is to enable clean Infrastructure layer DI configuration while keeping `Sumapap.Persistence.Domain` abstractions safe for the Domain layer.

## ✨ Why use `Sumapap.Persistence.DependencyInjection`?

- **Clean Architecture Compliance**: Keeps DI configuration separate from domain abstractions, allowing `Sumapap.Persistence.Domain` to remain in the Domain layer
- **Fluent Registration API**: Type-safe, discoverable fluent syntax for repository registration with compile-time safety
- **Visitor Pattern Extensibility**: Add caching, logging, validation, or other cross-cutting concerns via visitors without modifying core code
- **Opt-in Caching**: Per-repository cache configuration with granular method control (not forced globally)
- **Generic Repository Support**: Register open generic repositories for automatic entity resolution
- **Provider-Agnostic**: Cache metadata stored in registry for consumption by any cache provider (FusionCache, Redis, etc.)

## 🚀 Quick start

1. Add the package to your Infrastructure layer project:

``bash
dotnet add package Sumapap.Persistence.DependencyInjection
``

2. Register repositories with the fluent builder:

``csharp
builder.Services.AddSumapap()
    .WithRepositories(repos => repos
        .AddScopedRepository<UserRepository, User>()
        .AddTransientRepository<IProductRepository, ProductRepository, Product>()
    );
``

3. Enable opt-in caching for specific repositories:

``csharp
builder.Services.AddSumapap()
    .WithRepositories(repos => repos
        .AddScopedRepository<UserRepository, User>()
        .AllowCaching(config =>
        {
            config.Duration = TimeSpan.FromMinutes(10);
        })
        .Builder
        
        .UseRepositoryCaching() // Register visitor
    );
``

4. Add a cache provider to consume the registry (optional):

``csharp
builder.Services.AddSumapap()
    .WithRepositories(repos => repos
        // ... registrations with AllowCaching()
        .UseRepositoryCaching()
    )
    .UseFusionCache(); // Provider decorates based on metadata
``

## 🛠 Features and usage

### Fluent Repository Registration

**AddScopedRepository()** - Register scoped repository (most common):

``csharp
.WithRepositories(repos => repos
    // Concrete implementation only
    .AddScopedRepository<UserRepository, User>()
    
    // With abstraction
    .AddScopedRepository<IOrderRepository, OrderRepository, Order>()
)
``

**AddTransientRepository()** - Register transient repository:

``csharp
.WithRepositories(repos => repos
    .AddTransientRepository<IProductRepository, ProductRepository, Product>()
)
``

**AddSingletonRepository()** - Register singleton repository (rare):

``csharp
.WithRepositories(repos => repos
    .AddSingletonRepository<ICatalogRepository, CatalogRepository, Catalog>()
)
``

### Generic Repository Registration

**AddGenericRepository()** - Register open generic repositories:

``csharp
.WithRepositories(repos => repos
    // Register IRepository<> for all entities
    .AddGenericRepository(
        typeof(IRepository<>), 
        typeof(EfRepository<>), 
        ServiceLifetime.Scoped
    )
    .AllowCaching() // Applies to all entity types
)
``

**AddGenericRepositories()** - Register multiple EF Core generic repository types:

``csharp
.WithRepositories(repos => repos
    .AddGenericRepositories(ServiceLifetime.Scoped) // Registers IReadRepository<>, IWriteRepository<>, etc.
)
``

### Visitor Pattern Architecture

The library uses the **Visitor Pattern** for extensible repository decoration:

``
Repository Registration → Visitor Processing → Service Registration
                              ↓
                     IRepositoryRegistrationVisitor
                              ↓
                 (e.g., CachingRepositoryVisitor)
``

**IRepositoryRegistrationVisitor** - Interface for processing registrations:

``csharp
public interface IRepositoryRegistrationVisitor
{
    void Visit(RepositoryRegistrationEntry entry, IServiceCollection services);
}
``

**UseRepositoryCaching()** - Register the caching visitor:

``csharp
.WithRepositories(repos => repos
    .AddScopedRepository<UserRepository, User>()
    .AllowCaching()
    .Builder
    
    .UseRepositoryCaching() // Registers CachingRepositoryVisitor
)
``

**Custom Visitors** - Implement for logging, validation, etc.:

``csharp
public class LoggingRepositoryVisitor : IRepositoryRegistrationVisitor
{
    public void Visit(RepositoryRegistrationEntry entry, IServiceCollection services)
    {
        Console.WriteLine($"Registered {entry.ImplementationType.Name} for {entry.EntityType.Name}");
    }
}

// Register custom visitor
.WithRepositories(repos => repos
    .AddScopedRepository<UserRepository, User>()
    .Builder
    .UseVisitor(new LoggingRepositoryVisitor())
)
``

### Opt-In Caching Configuration

**AllowCaching()** - Enable caching with default configuration:

``csharp
.AddScopedRepository<UserRepository, User>()
.AllowCaching() // Default: 5 minutes, all read methods
``

**AllowCaching(config => ...)** - Fine-grained cache configuration:

``csharp
.AddScopedRepository<ProductRepository, Product>()
.AllowCaching(config =>
{
    config.Duration = TimeSpan.FromMinutes(10);
    config.KeyPrefix = "product";
    
    // Granular method control
    config.Methods.Clear();
    config.Methods["FindAsync"] = true;
    config.Methods["GetAllAsync"] = true;
    config.Methods["CountAsync"] = false; // Don''t cache Count
    
    // Provider-specific metadata
    config.Metadata["Priority"] = "High";
    config.Metadata["Tags"] = new[] { "catalog", "inventory" };
})
``

### Repository Registration Entry

**RepositoryRegistrationEntry** - Metadata about a registered repository:

``csharp
public sealed class RepositoryRegistrationEntry
{
    public Type? AbstractionType { get; init; }
    public required Type ImplementationType { get; init; }
    public required Type EntityType { get; init; }
    public ServiceLifetime Lifetime { get; init; }
    public bool AllowCaching { get; set; }
    public RepositoryCacheConfiguration? CachingConfiguration { get; set; }
}
``

### Repository Configurator

**RepositoryConfigurator** - Fluent configurator returned after registration:

``csharp
public sealed class RepositoryConfigurator<TImpl, TEntity>
{
    public RepositoryConfigurator<TImpl, TEntity> AllowCaching(
        Action<RepositoryCacheConfiguration>? configure = null);
    
    public RepositoryRegistrationBuilder Builder { get; }
}
``

**Chaining** - Return to builder for next registration:

``csharp
.AddScopedRepository<UserRepository, User>()
.AllowCaching()
.Builder // Return to RepositoryRegistrationBuilder

.AddScopedRepository<ProductRepository, Product>()
.AllowCaching()
.Builder

.UseRepositoryCaching()
``

### Complete Example

Full DI setup with repositories, generics, and caching:

``csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSumapap()
    .WithRepositories(repos => repos
        // Specific repositories with caching
        .AddScopedRepository<IUserRepository, UserRepository, User>()
        .AllowCaching(config =>
        {
            config.Duration = TimeSpan.FromMinutes(15);
            config.KeyPrefix = "user";
            config.Methods.EnableAllReads();
        })
        .Builder
        
        .AddScopedRepository<IProductRepository, ProductRepository, Product>()
        .AllowCaching(config =>
        {
            config.Duration = TimeSpan.FromMinutes(30);
            config.KeyPrefix = "product";
        })
        .Builder
        
        // Generic repositories for other entities
        .AddGenericRepository(
            typeof(IRepository<>), 
            typeof(EfRepository<>), 
            ServiceLifetime.Scoped
        )
        .AllowCaching(config =>
        {
            config.Duration = TimeSpan.FromMinutes(5);
        })
        .Builder
        
        // Register caching visitor
        .UseRepositoryCaching()
    )
    .UseFusionCache(); // Cache provider decorates based on metadata

var app = builder.Build();
app.Run();
``

### Cache Registry Inspection

Access the cache registry for testing or runtime inspection:

``csharp
var registry = serviceProvider.GetRequiredService<RepositoryCacheRegistry>();

foreach (var entry in registry.CachedRepositories)
{
    Console.WriteLine($"Repository: {entry.RepositoryType.Name}");
    Console.WriteLine($"Entity: {entry.EntityType.Name}");
    Console.WriteLine($"Duration: {entry.Configuration.Duration}");
    Console.WriteLine($"Methods: {string.Join(", ", entry.Configuration.Methods.Keys)}");
}
``

## ⚠️ Notes & best practices

### ✅ Do

- **Reference from Infrastructure layer only** - this package should never be referenced from Domain or Application layers
- **Use `Sumapap.Persistence.Domain` in Domain layer** for abstractions (`IEntity`, repository interfaces)
- **Use scoped lifetime for most repositories** (aligns with EF Core `DbContext` lifetime)
- **Call `UseRepositoryCaching()`** after all `AllowCaching()` calls to register the visitor
- **Chain via `.Builder`** to return to `RepositoryRegistrationBuilder` for next registration
- **Customize cache duration** based on data volatility (shorter for frequently changing data)
- **Use generic repositories** for entities without custom query logic

### ❌ Don''t

- **Never reference this package from Domain layer** - violates Clean Architecture dependency rules
- **Avoid calling `AllowCaching()` without `UseRepositoryCaching()`** - cache metadata is recorded but never consumed
- **Don''t cache write operations** - only read methods should be cached (enforced by default)
- **Avoid singleton lifetime** for repositories that depend on scoped `DbContext`
- **Don''t forget `.Builder`** when chaining registrations - IntelliSense won''t show next registration methods

### Migration from Previous Versions

If you were using `Sumapap.Persistence` directly for DI:

**Before:**
``csharp
using Sumapap.Persistence; // Everything in one package
``

**After:**
``csharp
using Sumapap.Persistence.Domain; // Domain abstractions
using Sumapap.Persistence.DependencyInjection; // DI configuration
``

Registration code remains the same - only package references change.

### Testing Recommendations

When testing DI configuration:
1. **Verify registrations** by resolving services from `IServiceProvider`
2. **Inspect cache registry** via `RepositoryCacheRegistry` to validate caching configuration
3. **Test visitor behavior** by implementing custom visitors and asserting side effects
4. **Mock repository implementations** in unit tests, not DI configuration

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