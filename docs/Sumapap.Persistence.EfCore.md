# Sumapap.Persistence.EFCore

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Persistence.EFCore.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.EFCore/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.EFCore.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.EFCore/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Persistence.EFCore` provides concrete, EF Core-based implementations of the persistence abstractions defined
in `Sumapap.Persistence`. It ships generic repositories, a Unit of Work implementation and a specification evaluator
that integrate with Entity Framework Core to let you focus on domain logic rather than plumbing.

Included implementations:
- `ReadRepository<TEntity, TContext>` — EF Core read-side repository (AsNoTracking by default, streaming, queries, specs).
- `WriteRepository<TEntity, TContext>` — EF Core write-side repository (Add/Update/Delete + Save/SaveAsync).
- `ReadWriteRepository<TEntity, TContext>` — composition of read and write behaviors exposing the full CRUD surface.
- `UnitOfWork<TContext>` — manages a DbContext instance and (optional) explicit transactions with Begin/Commit/Rollback support.
- `SpecificationEvaluator` — applies `ISpecification<T>` includes, filters and query options to `IQueryable`.
- `DependencyInjection` helpers — convenient registration for generic repositories and unit-of-work.

## ✨ Why Sumapap.Persistence.EFCore?

- Reuses the small, testable persistence contracts from `Sumapap.Persistence` while leveraging EF Core features (tracking, change detection, transactions).
- Provides sensible defaults for read operations (`AsNoTracking`) and streaming large result sets.
- Centralizes specification evaluation so query composition is consistent across repositories.
- Easy DI integration via extension methods; works well in layered applications.

## 🚀 Quick start

1. Add the package to your project (when published on NuGet):

```bash
dotnet add package Sumapap.Persistence.EFCore
```

2. Create your EF Core DbContext and entities (entities implement `IEntity` or `IEntity<TKey>`):

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
}

public class Order : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
}
```

3. Register EF Core persistence in DI (recommended):

```csharp
services.AddEfCorePersistence<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("Default")));
```

This registers the generic repository types and the `IUnitOfWork<TContext>` implementation and also registers your `DbContext`.

4. Use the repositories / unit of work in application services:

```csharp
public class OrderService
{
    private readonly IUnitOfWork<AppDbContext> _uow;

    public OrderService(IUnitOfWork<AppDbContext> uow) => _uow = uow;

    public async Task CreateOrderAsync(Order order)
    {
        var repo = _uow.GetRepository<Order>();
        repo.Add(order);
        await _uow.SaveChangesAsync();
    }
}
```

## 🛠 Features & Usage Details

### ReadRepository
- Uses `DbSet<TEntity>` and EF Core's query APIs.
- Default read operations use `AsNoTracking()` for improved read performance and to avoid accidental state changes.
- Supports synchronous and asynchronous operations, streaming (`IAsyncEnumerable<T>`), and specification-based queries via `ISpecification<T>`.
- `ApplySpecification` delegates include/filter/paging logic to `SpecificationEvaluator`.

### WriteRepository
- Implements add/update/delete operations using `DbSet.Add` / `Update` / `Remove` and their async variants where appropriate.
- Save operations call `_context.SaveChanges()` / `_context.SaveChangesAsync()`; changes are persisted only when save is invoked.
- Delete by id operations use `Find`/`FindAsync` to materialize entity before removal.

### ReadWriteRepository
- Thin facade that composes `ReadRepository` and `WriteRepository` to provide a single read/write surface for convenience.

### UnitOfWork
- Wraps a `DbContext` instance and maintains an internal cache of created repository instances so all repo instances in a UoW share the same context.
- Supports:
  - `SaveChangesAsync()` for committing changes (EF Core creates implicit transactions by default).
  - `BeginTransactionAsync` / `CommitTransactionAsync` / `RollbackTransactionAsync` for explicit transaction control.
  - `ExecuteAsync(Func<CancellationToken, Task>)` that runs the provided operation inside an explicit transaction and commits/rolls back appropriately.
- Disposal: The UoW implements both `IDisposable` and `IAsyncDisposable`. It ensures explicit transactions are rolled back/disposed if still active on dispose. It does not dispose the underlying DbContext (DI typically manages that lifetime).

### SpecificationEvaluator
- Applies the `ISpecification<T>` to an `IQueryable<T>` by:
  - Applying the Criteria filter expression if present.
  - Optionally skipping includes/paging when only criteria evaluation is required (useful for Count/Exists optimizations).
  - Applying Includes (string-based navigation paths using EF Core `Include` overload that accepts string).
  - Executing optional `IQuery` query options (paging/sorting) via the query execution helpers.

### DependencyInjection
- `AddEfCorePersistence()` registers the open generic types:
  - `IReadWriteRepository<,>` -> `ReadWriteRepository<,>`
  - `IReadRepository<,>` -> `ReadRepository<,>`
  - `IWriteRepository<,>` -> `WriteRepository<,>`
  - `IUnitOfWork<>` -> `UnitOfWork<>`
- `AddEfCorePersistence<TContext>(Action<DbContextOptionsBuilder>)` also registers the specified `DbContext` with the provided configuration.

## ⚠️ Notes & Best Practices

- Transaction boundaries — prefer dispatching domain events or notifying external systems only after `SaveChangesAsync` completes successfully.
- Handler lifetimes — repositories and DbContext are typically registered as Scoped. Do not hold DbContext across threads or beyond its scope.
- Explicit transactions — use `BeginTransactionAsync` only when you need to coordinate multiple `SaveChanges` calls or cross-cutting transactional work.
- Performance — `SpecificationEvaluator` uses string-based includes for simplicity; prefer expression-based includes in custom repositories when you need compile-time safety.
- Large result sets — use `StreamAllAsync` / `StreamWhereAsync` to process results without loading all rows into memory.

## ✅ Example

```csharp
// Register in Program.cs
services.AddEfCorePersistence<AppDbContext>(opts => opts.UseSqlServer(connString));

// Application service
public class OrdersAppService
{
    private readonly IUnitOfWork<AppDbContext> _uow;
    private readonly IDomainEventDispatcher _dispatcher; // if using domain events

    public OrdersAppService(IUnitOfWork<AppDbContext> uow, IDomainEventDispatcher dispatcher)
    {
        _uow = uow;
        _dispatcher = dispatcher;
    }

    public async Task PlaceOrderAsync(Order order, CancellationToken ct)
    {
        var repo = _uow.GetRepository<Order>();
        repo.Add(order);

        await _uow.SaveChangesAsync(ct);

        // dispatch domain events after successful commit
        if (order is DomainEntity domainEntity)
        {
            var events = domainEntity.ConsumeEvents();
            await _dispatcher.DispatchAsync(events, ct);
        }
    }
}
```

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/src/Sumapap.Persistence.EfCore

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>