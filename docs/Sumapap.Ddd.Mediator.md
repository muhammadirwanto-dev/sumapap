# Sumapap.Ddd.Mediator

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Ddd.Mediator.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd.Mediator/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.Mediator.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd.Mediator/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Ddd.Mediator` adapts domain events to a [`Mediator`](https://github.com/martinothamar/Mediator) pipeline so your domain layer remains free from `Mediator` dependencies while allowing application-level `Mediator` handlers to process domain events.

Key pieces in this package:
- `IDomainEventAdapter` — an adapter marker that combines `IDomainEvent` (domain) and `INotification`/`INotification`-like from the `Mediator` abstraction.
- `DomainEventDispatcher` — an implementation of `IDomainEventDispatcher` that publishes events through an `IPublisher` instance.
- `DependencyInjection` — convenience registration that adds the dispatcher to DI (does not register mediator itself).

This package supports the `Mediator.Abstractions` (source-generated mediator) family but the design is generic: the dispatcher only depends on an `IPublisher` abstraction.

## ✨ Why use Sumapap.Ddd.Mediator?

- Keeps domain events in the Domain layer simple and infrastructure-free.
- Allows the Application layer to handle domain events using standard mediator notification handlers.
- Leverages mediator pipelines (behaviors, validation, logging) for domain event handling.
- Supports asynchronous, concurrent dispatch of multiple events using the mediator's publisher.

## 🚀 Quick start

1. Add the package(s) to your project:

```bash
 dotnet add package Sumapap.Ddd.Mediator

 # and the Mediator implementation (https://github.com/martinothamar/Mediator)
 dotnet add package Mediator.SourceGenerator
 dotnet add package Mediator.Abstractions

or

<PackageReference Include="Mediator.SourceGenerator" Version="3.0.*">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
<PackageReference Include="Mediator.Abstractions" Version="3.0.*" />
```

2. Make your domain events compatible with the mediator by implementing `IDomainEventAdapter` which inherits both `IDomainEvent` and the mediator's notification interface (`INotification`). Example:

```csharp
public record OrderPlacedEvent(Guid OrderId, DateTime OccurredAt) : IDomainEventAdapter;
```

3. Register mediator and the dispatcher in DI (example using the `Mediator` source generator):

```csharp
// register mediator (example)
services.AddMediator(options =>
{
    options.Assemblies = new[] { typeof(Program).Assembly };
});

// register the domain event dispatcher
services.AddDomainEventsDispatcher();
```

4. Emit domain events from your entities and dispatch after your unit of work completes:

```csharp
order.AddDomainEvent(new OrderPlacedEvent(order.Id, DateTime.UtcNow));
// after saving changes
await dispatcher.DispatchAsync(order.ConsumeEvents());
```

> [!WARNING]
> Events must implement `IDomainEventAdapter` (or be adapted to it) — the dispatcher casts each `IDomainEvent` to `IDomainEventAdapter` before publishing.

## 🛠  Features and usage

- `IDomainEventAdapter` — tiny adapter interface that marks an event as both a domain event and a mediator notification. This allows the dispatcher to hand the event to the mediator/publisher without leaking mediator types into the Domain project.
- `DomainEventDispatcher` — Accepts a batch of `IDomainEvent` instances and publishes them using an `IPublisher` instance.
  - Uses `Task.WhenAll(...)` to publish events concurrently via the mediator's publish API.
  - Expects events to be `IDomainEventAdapter` instances — otherwise a runtime cast failure will occur.
- `DependencyInjection` — Registers `IDomainEventDispatcher` as a singleton (`DomainEventDispatcher`). The mediator itself must be registered separately.

### Example handler

```csharp
public class NotifyWarehouseHandler : INotificationHandler<OrderPlacedEvent>
{
    public Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        // process notification
        return Task.CompletedTask;
    }
}
```

Depending on your mediator implementation the handler interface may be `INotificationHandler<T>` or `INotificationHandler<TEvent>` provided by the mediator package you're using.

## ⚠️ Notes & best practices

- Ensure events are adapted: The dispatcher uses `(event as IDomainEventAdapter)!` and publishes that. If an event does not implement `IDomainEventAdapter` the cast produces null and the following `!` will throw. Always implement the adapter on your event types or provide a mapping layer.
- Transactional boundaries — dispatch events only after your data changes are committed to avoid publishing events for rolled-back work.
- Handler execution — the dispatcher publishes events concurrently using `Task.WhenAll`. If handler ordering or sequential processing is required, implement a custom dispatcher.
- Lifetimes — the dispatcher is registered as a Singleton; ensure your mediator/publisher resolves handlers with appropriate lifetimes.
- Resiliency — the default implementation does not provide retry or error handling. Wrap dispatch calls or implement policies (Polly) if reliability is required.

## ✅ Example

```csharp
// Domain event (adapter)
public record OrderPlacedEvent(Guid OrderId, DateTime OccurredAt) : IDomainEventAdapter;

// Application-level handler (mediator notification handler)
public class NotifyWarehouseHandler : INotificationHandler<OrderPlacedEvent>
{
    public Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        // notify warehouse
        return Task.CompletedTask;
    }
}

// DI (Program.cs)
services.AddMediator(options => { options.Assemblies = new[] { typeof(Program).Assembly }; });
services.AddDomainEventsDispatcher();

// Usage
var events = order.ConsumeEvents();
await dispatcher.DispatchAsync(events, cancellationToken);
```

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/source/Sumapap.Ddd.Mediator

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>