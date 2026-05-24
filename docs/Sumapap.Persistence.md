# Sumapap.Persistence

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Persistence.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Persistence` provides a thin, opinionated set of persistence abstractions and helper utilities intended to simplify implementing repositories, specifications and unit-of-work patterns across different data access technologies (`EF Core`, `Dapper`, etc.). The package focuses on:

- Common repository contracts (read, write, read/write)
- Unit of Work abstraction for transactional scope
- Specification pattern helpers (filtering, includes, paging)
- DI helpers for registering repository implementations easily

The goal is to let your domain and application layers depend on a consistent persistence surface while keeping concrete implementations swappable.

## ✨ Why use `Sumapap.Persistence`?

- Provides clear separation between domain and data access layers via small interfaces.
- Encourages use of the Specification pattern to centralize query logic.
- Standardizes repository and unit-of-work surface across different persistence engines.
- Includes convenience DI helpers to register repository implementations with correct lifetimes and service mappings.

## 🚀 Quick start

1. Add the package to your project (when published on NuGet):

```bash
 dotnet add package Sumapap.Persistence
```

2. Implement your entity (must implement `IEntity` or `IEntity<TKey>`):

```csharp
public class Order : IEntity<Guid>
{
    public Guid Id { get; set; }
}
```

3. Implement a repository for your persistence technology (read/write):

```csharp
public class EfOrderRepository : IReadWriteRepository<Order>
{
    // implement methods using your DbContext
}
```

4. Register repository in DI using provided helpers:

```csharp
// when implementing EfOrderRepository as concrete implementation
services.AddScopedRepository<EfOrderRepository, Order>();

// or if you have an interface abstraction IOrderRepository
services.AddScopedRepository<IOrderRepository, EfOrderRepository, Order>();
```

5. Use repository and unit of work in your services:

```csharp
var repo = unitOfWork.GetRepository<Order>();
var orders = await repo.GetAllAsync();
await unitOfWork.SaveChangesAsync();
```

## 🛠 Features and usage

### Repository interfaces
- `IRepository` / `IRepository<TEntity>` — marker base interfaces.
- `IReadRepository<TEntity>` — rich read-only API supporting synchronous and asynchronous queries, streaming, specification-based queries and paging via `IQuery`.
- `IWriteRepository<TEntity>` — mutation API (`Add`, `Update`, `Delete`) with synchronous and asynchronous variants and Save/SaveAsync.
- `IReadWriteRepository<TEntity>` — combination of read and write APIs (not explicitly shown in code but used as returned type in `IUnitOfWork`).

Common patterns:
- Use `Find`, `FirstOrDefault`, `SingleOrDefault`, `Where` or `GetAll` for synchronous reads.
- Prefer the async variants in application code (e.g., `GetAllAsync`, `FirstOrDefaultAsync`).
- Use streaming (`IAsyncEnumerable<T>`) for large result sets to reduce memory pressure.

### Unit of Work
- `IUnitOfWork` provides scoped coordination across repositories and transactional control methods (`BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`).
- Use `GetRepository<TEntity>()` to obtain a repository instance that participates in the unit of work.
- Call `SaveChangesAsync()` to persist changes and optionally control transactions explicitly when needed.

Example:

```csharp
await using var uow = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();

uow.ExecuteAsync(c =>
{
  var repo = uow.GetRepository<Order>();
  repo.Add(newOrder);
}, cancellationToken);
```

### Specification pattern
- `ISpecification<T>` encapsulates query criteria (`Expression<Func<T,bool>>? Criteria`), includes (list of navigation paths) and optional `IQuery` (paging and sorting options).
- `BaseSpecification<T>` provides helpers for building specifications and storing includes and query options.
- `IncludeSpecification<T>` is a simple specialization to specify only Includes (or includes+criteria).
- `PagingSpecification<T>` is a helper that sets paging (Offset or Cursor) and sorting via `IQuery` wrappers.

Usage example:

```csharp
var spec = new PagingSpecification<Order>(o => o.CustomerId == customerId, new OffsetPaginationOptions(0, 20));
var page = await repo.GetAllAsync(spec);
```

### Dependency injection helpers
- `DependencyInjection` exposes extension methods to register repository implementations with correct service mappings and lifetimes:
  - `AddScopedRepository<TImpl, TEntity>()` — registers implementation and maps it to `IReadRepository<TEntity>`, `IWriteRepository<TEntity>`, `IReadWriteRepository<TEntity>` and `IRepository<TEntity>` when applicable.
  - `AddScopedRepository<TService, TImpl, TEntity>()` — same as above plus registers a service abstraction.
  - Equivalent `AddTransientRepository` overloads for transient lifetime.

This helps avoid repetitive registration code and ensures repositories are available via multiple abstraction types.

## ⚠️ Notes & best practices

- Prefer the async APIs throughout your application to avoid thread starvation in server scenarios.
- Keep specifications focused and composable. Specifications should describe the "what" (filter, includes, query options) and not the persistence mechanism.
- Dispatch domain events (if using) only after the unit-of-work completes to avoid notifying stakeholders about rolled-back changes.
- Decide repository lifetime (Scoped vs Transient) based on your persistence technology (`EF Core`: Scoped; `Dapper`: typically Transient).
- Implement concrete repositories to honor the contracts — e.g., `DetatchFromTracking` should detach entities when using `EF Core` to prevent unintended state tracking.

#### Example

```csharp
// Specification
public class OrdersByCustomerSpec : BaseSpecification<Order>
{
    public OrdersByCustomerSpec(Guid customerId)
        : base(o => o.CustomerId == customerId)
    {
        AddInclude("OrderItems.Product");
    }
}

// Registration
services.AddScopedRepository<IOrderRepository, EfOrderRepository, Order>();

// Usage in app service
var repo = unitOfWork.GetRepository<Order>();
var orders = await repo.GetAllAsync(new OrdersByCustomerSpec(customerId));
await unitOfWork.SaveChangesAsync();
```

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/source/Sumapap.Persistence

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>