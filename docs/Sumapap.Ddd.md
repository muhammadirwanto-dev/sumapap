# Sumapap.Ddd

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Ddd.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Ddd` provides a small set of domain-driven design (DDD) abstractions intended to be lightweight,
framework-agnostic and easy to adopt in .NET applications. The project focuses on core building blocks used
to model rich domains: domain events, domain entities and a minimal event dispatching contract.

Core concepts included in this package:
- `IDomainEvent` — marker interface for domain events.
- `IDomainEventHandler<TEvent>` — consumer contract for a specific domain event.
- `IDomainEventDispatcher` — a dispatcher that publishes/dispatches domain events to handlers.
- `DomainEntity` — a base entity helper that collects domain events emitted by an aggregate or entity.

## ✨ Why Sumapap.Ddd?

The library keeps the domain model clean and decoupled from infrastructure by providing small, focused
abstractions you can implement or plug into your application. Use it when you want:
- A consistent pattern for publishing domain events from entities or aggregate roots.
- A simple, testable contract for handling domain events.
- Minimal dependencies so the core domain remains portable across projects and runtimes.

## 🚀 Quick start

1. Add the package to your project (when published on NuGet):

   ```dotnetcli
   dotnet add package Sumapap.Ddd
   ```

2. Model a domain event:

   ```csharp
   public record OrderPlacedEvent(Guid OrderId, DateTime OccurredAt) : IDomainEvent;
   ```

3. Implement an event handler:

   ```csharp
   public class NotifyWarehouseHandler : IDomainEventHandler<OrderPlacedEvent>
   {
       public Task HandleAsync(OrderPlacedEvent domainEvent, CancellationToken cancellationToken = default)
       {
           // send message to warehouse
           return Task.CompletedTask;
       }
   }
   ```

4. Emit events from your domain entity:

   ```csharp
   public class Order : DomainEntity
   {
       public void Place()
       {
           // domain logic
           AddDomainEvent(new OrderPlacedEvent(Id, DateTime.UtcNow));
       }
   }
   ```

5. Dispatch events using `IDomainEventDispatcher` (register your dispatcher and handlers in DI):

   ```csharp
   await dispatcher.DispatchAsync(order.ConsumeEvents());
   ```

   > [!NOTE]
   > `DomainEntity` provides basic collection and consumption methods for events. In larger systems
   you may prefer a richer AggregateRoot base class with explicit child-management, versioning and
   domain event semantics.

## 🛎️ Features and usage

- `IDomainEvent`
  - A marker interface to identify domain events. Keep events immutable (records are a good fit).

- `IDomainEventHandler<TEvent>`
  - Implement this generic interface for each event you want to handle. Handlers receive a single
    domain event and a CancellationToken. Prefer async handlers.

- `IDomainEventDispatcher`
  - A small contract that accepts a batch of domain events and publishes them to registered handlers.
  - Typical implementations either resolve handlers from an IServiceProvider (DI) and invoke them
    or map events to an in-process message bus. Implementations should be robust to handler
    exceptions and support cancellation.

- `DomainEntity`
  - A convenience base class for entities that can raise domain events. It exposes methods to add,
    access, consume and clear events produced by the entity.
  - Use `ConsumeEvents()` to atomically read and remove events (useful when flushing events after
    a successful transaction/commit).

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/source/Sumapap.Ddd

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>