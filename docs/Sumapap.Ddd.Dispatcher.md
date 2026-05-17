# Sumapap.Ddd.Dispatcher

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Ddd.Dispatcher.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd.Dispatcher/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.Dispatcher.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd.Dispatcher/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Ddd.Dispatcher` contains a lightweight, DI-friendly implementation for dispatching domain events
produced by domain entities or aggregate roots. It wires event handlers discovered in assemblies and provides a
simple, robust mechanism to publish batches of `IDomainEvent` instances to their corresponding `IDomainEventHandler<TEvent>` implementations.

The package includes:
- `DomainEventDispatcher` — resolves handlers from `IServiceProvider` and invokes them for each event.
- `DependencyInjection` — an extension to scan assemblies and register event handlers and the dispatcher into Microsoft DI.

## ✨ Why use Sumapap.Ddd.Dispatcher?

- Minimal, dependency-free approach for in-process domain event dispatching.
- Automatic discovery and registration of `IDomainEventHandler<TEvent>` implementations.
- Handlers are registered as Scoped so they can depend on per-request services (e.g. DbContext, repositories).
- Works well with `DomainEntity` from Sumapap.Ddd: consume events and dispatch after a successful unit-of-work.

## 🚀 Quick start

1. Add the package to your project (when published on NuGet):

```bash
 dotnet add package Sumapap.Ddd.Dispatcher
```

2. Register the dispatcher and handlers in your DI container. The extension method scans assemblies for handler implementations:

```csharp
// Register handlers from the current calling assembly
services.AddDomainEventsDispatcher();

// Or specify assemblies explicitly
services.AddDomainEventsDispatcher(typeof(Startup).Assembly, typeof(SomeHandler).Assembly);
```

3. Use the dispatcher to publish events (e.g. after saving changes in a transaction):

```csharp
await dispatcher.DispatchAsync(order.ConsumeEvents(), cancellationToken);
```

## 🛠 Features and usage

- `DomainEventDispatcher`
  - For each event in the provided batch, the dispatcher constructs the concrete handler interface type `IDomainEventHandler<T>`
    using reflection and resolves all registered handlers for that type from `IServiceProvider`.
  - It invokes `HandleAsync` on each handler using dynamic dispatch: `await ((dynamic)handler).HandleAsync((dynamic)@event, cancellationToken);`.
  - This approach keeps the dispatcher implementation compact and generic, but relies on runtime dispatch (dynamic) and reflection.

- `DependencyInjection`
  - `AddDomainEventsDispatcher` scans provided assemblies (or the calling assembly by default) to find concrete types
    that implement `IDomainEventHandler<TEvent>` and registers each handler as Scoped under its interface type.
  - Finally registers the `DomainEventDispatcher` as a Singleton implementation of `IDomainEventDispatcher`.

## ⚠️ Notes & best practices

- Transactional boundaries — dispatch events only after the unit-of-work completes (e.g. after `DbContext.SaveChangesAsync()`)
  to avoid publishing events for rolled-back changes.
- Handler lifetime — handlers are registered as Scoped to allow dependency injection of scoped services (`DbContext`, `UnitOfWork`).
  The dispatcher itself is registered as Singleton but resolves handlers from `IServiceProvider` per dispatch call, so using Scoped handlers is safe when Dispatch is called within a scope.
- Ordering & retries — the default dispatcher calls handlers sequentially in the order of discovery. If you need parallelism,
  guaranteed ordering, or retry policies, implement a custom dispatcher or wrap handlers with resiliency policies (e.g. `Polly`).
- Exception handling — the dispatcher does not swallow exceptions; let exceptions bubble or wrap dispatch calls with try/catch where you call the dispatcher. Consider logging or compensating actions in case of handler failure.
- Performance — reflection and dynamic invocation have runtime costs. For high-throughput scenarios consider caching handler resolution,
  using compiled delegates or a source-generated dispatcher.
- Alternatives — for advanced scenarios you may prefer using `MediatR` or an outbox pattern for reliable delivery across process boundaries.

### Example

```csharp
// Event
public record OrderPlacedEvent(Guid OrderId, DateTime OccurredAt) : IDomainEvent;

// Handler
public class NotifyWarehouseHandler : IDomainEventHandler<OrderPlacedEvent>
{
    private readonly ILogger<NotifyWarehouseHandler> _logger;

    public NotifyWarehouseHandler(ILogger<NotifyWarehouseHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderPlacedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Notify warehouse for order {OrderId}", domainEvent.OrderId);
        return Task.CompletedTask;
    }
}

// Registration in Startup / Program
services.AddDomainEventsDispatcher(typeof(NotifyWarehouseHandler).Assembly);

// Usage (e.g. in application service)
await dispatcher.DispatchAsync(order.ConsumeEvents());
```

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/source/Sumapap.Ddd.Dispatcher

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>