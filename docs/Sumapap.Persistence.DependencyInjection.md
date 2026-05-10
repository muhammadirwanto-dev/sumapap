# Sumapap.Persistence.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence.DependencyInjection.svg)](https://www.nuget.org/packages/Sumapap.Persistence.DependencyInjection/)

## Overview

`Sumapap.Persistence.DependencyInjection` provides fluent dependency injection builders and extensions for configuring Sumapap.Persistence repositories and caching. This library separates DI concerns from the core `Sumapap.Persistence` abstractions, allowing the persistence layer to remain clean and suitable for the Domain layer while DI configuration lives in the Infrastructure layer.

## Features

- **Fluent Repository Registration**: Type-safe fluent API for registering repositories with scoped or transient lifetimes
- **Generic Repository Support**: Register open generic repositories for automatic resolution
- **Opt-in Caching**: Enable caching per repository with fluent configuration
- **Cache Configuration**: Fine-grained control over which repository methods should be cached
- **Provider-Agnostic Registry**: Cache metadata is stored in a registry for later decoration by cache providers (e.g., FusionCache)
- **Modern C# 14 Extension Syntax**: Uses modern extension methods for better IntelliSense and discoverability

## Installation

```bash
dotnet add package Sumapap.Persistence.DependencyInjection
```

## Architecture

This library implements a **deferred decoration pattern** for caching:

1. **Registration Phase**: Repositories are registered with optional cache configuration
2. **Metadata Recording**: Cache intent is stored in `RepositoryCacheRegistry`
3. **Provider Integration**: Cache providers (like FusionCache) read the registry and apply decorators

This separation keeps the core persistence abstractions independent of any specific caching implementation.

## Basic Usage

### Registering Repositories

```csharp
using Sumapap.DependencyInjection;
using Sumapap.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSumapap()
	.WithRepositories(repos => repos
		// Scoped repository
		.AddScopedRepository<UserRepository, User>()

		// Transient repository with abstract type
		.AddTransientRepository<IProductRepository, ProductRepository, Product>()
	);

var app = builder.Build();
```

### Enabling Caching

```csharp
builder.Services.AddSumapap()
	.WithRepositories(repos => repos
		// Repository with default caching (all read methods)
		.AddScopedRepository<UserRepository, User>()
		.AllowCaching()
		.Builder

		// Repository with custom cache configuration
		.AddScopedRepository<ProductRepository, Product>()
		.AllowCaching(config =>
		{
			config.Duration = TimeSpan.FromMinutes(10);
			config.KeyPrefix = "product";

			// Choose which methods to cache
			config.Methods["FindAsync"] = true;
			config.Methods["GetAllAsync"] = true;
			config.Methods["SingleOrDefaultAsync"] = false; // Don't cache this
		})
	);
```

### Generic Repository Registration

```csharp
builder.Services.AddSumapap()
	.WithRepositories(repos => repos
		// Generic repositories (open types)
		.AddGenericRepository(
			typeof(IRepository<>), 
			typeof(Repository<>), 
			ServiceLifetime.Scoped)
		.AllowCaching(config =>
		{
			config.Duration = TimeSpan.FromMinutes(5);
			config.KeyPrefix = "generic";
		})
		.Builder

		// EF Core generic repositories (if using Sumapap.Persistence.EfCore)
		.AddGenericRepositories() // Adds IReadRepository<,>, IWriteRepository<,>, etc.
	);
```

## Key Types

### RepositoryRegistrationBuilder

The main builder for configuring repository registrations.

```csharp
public class RepositoryRegistrationBuilder
{
	// Register scoped repository
	public RepositoryConfigurator<TImpl, TEntity> AddScopedRepository<TImpl, TEntity>()
		where TImpl : class
		where TEntity : class, IEntity;

	// Register transient repository
	public RepositoryConfigurator<TImpl, TEntity> AddTransientRepository<TImpl, TEntity>()
		where TImpl : class
		where TEntity : class, IEntity;

	// Register generic repository
	public RepositoryConfigurator AddGenericRepository(
		Type serviceType, 
		Type implType, 
		ServiceLifetime serviceLifetime);

	// Build and apply registrations
	public SumapapServiceBuilder Build();
}
```

### RepositoryConfigurator

Fluent configurator for individual repository registration, enabling opt-in caching.

```csharp
public class RepositoryConfigurator
{
	// Enable caching with custom configuration
	public RepositoryConfigurator AllowCaching(
		Action<RepositoryCacheConfiguration> configure);

	// Enable caching with default configuration
	public RepositoryConfigurator AllowCaching();

	// Return to builder for chaining
	public RepositoryRegistrationBuilder Builder { get; }
}
```

### RepositoryCacheConfiguration

Configuration for repository caching behavior.

```csharp
public sealed class RepositoryCacheConfiguration
{
	// Which methods should be cached (method name -> enabled)
	public Dictionary<string, bool> Methods { get; init; }

	// Cache duration (optional, provider-specific default if null)
	public TimeSpan? Duration { get; set; }

	// Cache key prefix (optional)
	public string? KeyPrefix { get; set; }

	// Additional metadata for cache providers
	public Dictionary<string, object> Metadata { get; init; }
}
```

### RepositoryCacheRegistry

Registry tracking all repository registrations with their cache configurations.

```csharp
public sealed class RepositoryCacheRegistry
{
	// All registered repositories with caching enabled
	public IReadOnlyList<RepositoryCacheEntry> CachedRepositories { get; }

	// Internal registration method (used by builder)
	internal void Register(RepositoryCacheEntry entry);
}
```

## Cache Provider Integration

Cache providers (like `Sumapap.Persistence.FusionCache`) can access the registry and apply decorators:

```csharp
using Sumapap.Persistence.Caching;

// In your cache provider package
extension(RepositoryRegistrationBuilder builder)
{
	public RepositoryRegistrationBuilder UseFusionCache(
		Action<FusionCacheOptions>? configure = null)
	{
		var registry = builder.Services.GetRepositoryCacheRegistry();

		if (registry != null)
		{
			foreach (var entry in registry.CachedRepositories)
			{
				// Apply caching decorator for this repository
				ApplyCacheDecorator(entry);
			}
		}

		return builder;
	}
}
```

## Best Practices

### 1. Keep Persistence Layer Clean

The `Sumapap.Persistence` library contains only abstractions (interfaces and base classes). All DI and caching concerns live in this package, keeping the domain layer clean.

```csharp
// Domain Layer (Sumapap.Persistence)
public interface IUserRepository : IReadWriteRepository<User> { }

// Infrastructure Layer (Your project)
public class UserRepository : ReadWriteRepository<User>, IUserRepository
{
	// Implementation without any caching concerns
}

// Startup/DI Configuration (Infrastructure Layer)
services.AddSumapap()
	.WithRepositories(repos => repos
		.AddScopedRepository<IUserRepository, UserRepository, User>()
		.AllowCaching() // Caching is opt-in via DI configuration
	);
```

### 2. Use Scoped Lifetime for Repositories

Most repositories should be scoped to the request/unit-of-work lifetime:

```csharp
.AddScopedRepository<UserRepository, User>()
```

Use transient only when you have specific requirements (e.g., parallel processing with isolated contexts).

### 3. Configure Caching Per Repository

Not all repositories benefit from caching. Enable it selectively:

```csharp
.WithRepositories(repos => repos
	// High-read, low-write: cache it
	.AddScopedRepository<ProductCatalogRepository, Product>()
	.AllowCaching()
	.Builder

	// High-write, critical consistency: don't cache
	.AddScopedRepository<OrderRepository, Order>()
)
```

### 4. Use Generic Repositories for Prototyping

Generic repositories are great for rapid development:

```csharp
// Quick setup for all entities
.AddGenericRepository(typeof(IRepository<>), typeof(Repository<>), ServiceLifetime.Scoped)
.AllowCaching()
```

But consider creating specific repositories for complex domain logic.

### 5. Leverage the Builder Property

Use `.Builder` to return to the builder for chaining multiple registrations:

```csharp
.AddScopedRepository<UserRepository, User>()
.AllowCaching()
.Builder // Return to builder
.AddScopedRepository<ProductRepository, Product>()
.AllowCaching()
```

## Examples

### Complete Startup Configuration

```csharp
using Microsoft.EntityFrameworkCore;
using Sumapap.DependencyInjection;
using Sumapap.Persistence.DependencyInjection;
using Sumapap.Persistence.EfCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Configure Sumapap with repositories
builder.Services.AddSumapap()
	.WithRepositories(repos => repos
		// Specific repositories
		.AddScopedRepository<IUserRepository, UserRepository, User>()
		.AllowCaching(config =>
		{
			config.Duration = TimeSpan.FromMinutes(15);
			config.KeyPrefix = "user";
		})
		.Builder

		.AddScopedRepository<ProductRepository, Product>()
		.AllowCaching()
		.Builder

		// Generic EF Core repositories for other entities
		.AddGenericRepositories(ServiceLifetime.Scoped)
	);

var app = builder.Build();
app.Run();
```

### Custom Cache Configuration

```csharp
.AddScopedRepository<UserRepository, User>()
.AllowCaching(config =>
{
	config.Duration = TimeSpan.FromHours(1);
	config.KeyPrefix = "user";

	// Cache only specific methods
	config.Methods.Clear(); // Start fresh
	config.Methods["FindAsync"] = true;
	config.Methods["GetAllAsync"] = true;

	// Add provider-specific metadata
	config.Metadata["Priority"] = "High";
	config.Metadata["Tags"] = new[] { "user", "identity" };
})
```

## Migration from Previous Versions

If you were previously using `Sumapap.Persistence` directly for DI, update your code:

**Before:**
```csharp
using Sumapap.Persistence; // Everything in one package
```

**After:**
```csharp
using Sumapap.Persistence; // Domain abstractions only
using Sumapap.Persistence.DependencyInjection; // DI configuration
```

Your registration code remains the same - only the package reference changes.

## Related Packages

- **Sumapap.Persistence**: Core persistence abstractions (interfaces, specifications)
- **Sumapap.Persistence.EfCore**: Entity Framework Core implementation of repositories
- **Sumapap.Persistence.FusionCache**: FusionCache integration for caching (coming soon)
- **Sumapap.DependencyInjection**: Core Sumapap DI builder infrastructure

## Contributing

Contributions are welcome! Please check the [contributing guidelines](https://github.com/muhammadirwanto-dev/sumapap/blob/main/CONTRIBUTING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.
