# Sumapap.Ddd.Abstractions

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Ddd.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd.Abstractions/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Ddd.Abstractions/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Ddd.Abstractions` provides the foundational building blocks for implementing Domain-Driven Design (DDD) patterns in your .NET applications. This package contains:

- **Domain Event Contracts** — interfaces for defining and handling domain events
- **Domain Entity Base Class** — thread-safe implementation for managing domain events within entities
- **Event Dispatcher Contract** — abstraction for dispatching domain events to registered handlers

The package is designed to be lightweight, dependency-free, and focused exclusively on core DDD abstractions, making it suitable for use across domain, application, and infrastructure layers without introducing circular dependencies.

## ✨ Why use `Sumapap.Ddd.Abstractions`?

- **Zero Dependencies** — No external packages required; only standard .NET types
- **Clean Architecture** — Enables strict separation between domain logic and infrastructure concerns
- **Thread-Safe Event Management** — Built-in `ConcurrentQueue` ensures safe event queuing in multi-threaded scenarios
- **Flexible Event Handling** — Supports multiple handlers per event type through generic handler interface
- **Framework Agnostic** — Can be used with any DI container or dispatcher implementation

## 🚀 Quick start

1. Add the package to your domain project:

```bash
dotnet add package Sumapap.Ddd.Abstractions
```

2. Define a domain event by implementing `IDomainEvent`:

```csharp
public record OrderPlacedEvent(Guid OrderId, DateTime PlacedAt) : IDomainEvent;
```

3. Create a domain entity that inherits from `DomainEntity`:

```csharp
public class Order : DomainEntity
{
	public Guid Id { get; private set; }
	public OrderStatus Status { get; private set; }

	public void PlaceOrder()
	{
		Status = OrderStatus.Placed;

		// Raise domain event
		AddDomainEvent(new OrderPlacedEvent(Id, DateTime.UtcNow));
	}
}
```

4. Implement an event handler:

```csharp
public class OrderPlacedEventHandler : IDomainEventHandler<OrderPlacedEvent>
{
	public async Task HandleAsync(OrderPlacedEvent domainEvent, CancellationToken cancellationToken = default)
	{
		// Send confirmation email, update inventory, etc.
		await Task.CompletedTask;
	}
}
```

5. Dispatch events after unit-of-work completes:

```csharp
var order = new Order();
order.PlaceOrder();

// After saving to database
var events = order.ConsumeEvents(); // Gets and clears events
await dispatcher.DispatchAsync(events, cancellationToken);
```

## 🛠 Features and usage

### IDomainEvent

A marker interface representing any domain event. Domain events capture meaningful business occurrences within your domain.

```csharp
public interface IDomainEvent;
```

**Best Practices:**
- Use `record` types for immutability
- Include relevant contextual data (timestamps, entity IDs, user context)
- Name events in past tense (e.g., `OrderPlaced`, `PaymentProcessed`)

**Example:**

```csharp
public record ProductAddedToCartEvent(
	Guid CartId,
	Guid ProductId,
	int Quantity,
	DateTime AddedAt
) : IDomainEvent;
```

### IDomainEventHandler&lt;TEvent&gt;

Generic interface for implementing event handlers. Each handler processes a specific event type.

```csharp
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
	Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
```

**Features:**
- **Contravariant** (`in TEvent`) — allows handler covariance
- **Async-first** — all handlers are asynchronous by design
- **Cancellation support** — respects operation cancellation

**Example with multiple handlers:**

```csharp
// Handler 1: Send notification
public class OrderPlacedNotificationHandler : IDomainEventHandler<OrderPlacedEvent>
{
	private readonly IEmailService _emailService;

	public async Task HandleAsync(OrderPlacedEvent domainEvent, CancellationToken cancellationToken)
	{
		await _emailService.SendOrderConfirmationAsync(domainEvent.OrderId, cancellationToken);
	}
}

// Handler 2: Update analytics
public class OrderPlacedAnalyticsHandler : IDomainEventHandler<OrderPlacedEvent>
{
	private readonly IAnalyticsService _analytics;

	public async Task HandleAsync(OrderPlacedEvent domainEvent, CancellationToken cancellationToken)
	{
		await _analytics.TrackEventAsync("OrderPlaced", domainEvent, cancellationToken);
	}
}
```

### IDomainEventDispatcher

Contract for dispatching domain events to their registered handlers.

```csharp
public interface IDomainEventDispatcher
{
	Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
```

**Responsibilities:**
- Resolve all handlers for each event type
- Invoke handlers in appropriate order
- Handle exceptions or let them bubble (implementation-specific)

> **Note:** This package provides only the abstraction. For a concrete implementation, use [`Sumapap.Ddd`](Sumapap.Ddd.md) or implement your own dispatcher.

### DomainEntity

Abstract base class for domain entities that need to raise domain events. Provides thread-safe event queueing.

```csharp
public abstract class DomainEntity
{
	protected void AddDomainEvent(IDomainEvent domainEvent);
	public IReadOnlyList<IDomainEvent> ConsumeEvents();
	public IReadOnlyList<IDomainEvent> GetEvents();
	public void ClearEvents();
}
```

**Methods:**

| Method | Description |
|--------|-------------|
| `AddDomainEvent(event)` | Protected method to queue a domain event (call from within entity methods) |
| `ConsumeEvents()` | Returns all queued events **and clears** the internal queue (use after persisting) |
| `GetEvents()` | Returns all queued events **without clearing** the queue (read-only inspection) |
| `ClearEvents()` | Clears all queued events without returning them (use for rollback scenarios) |

**Thread Safety:**
- Uses `ConcurrentQueue<IDomainEvent>` internally
- Safe for concurrent access from multiple threads
- No locking required

**Example:**

```csharp
public class ShoppingCart : DomainEntity
{
	private readonly List<CartItem> _items = new();

	public void AddItem(Product product, int quantity)
	{
		ArgumentNullException.ThrowIfNull(product);

		if (quantity <= 0)
			throw new ArgumentException("Quantity must be positive", nameof(quantity));

		var item = new CartItem(product.Id, quantity, product.Price);
		_items.Add(item);

		// Raise domain event
		AddDomainEvent(new ProductAddedToCartEvent(
			CartId: Id,
			ProductId: product.Id,
			Quantity: quantity,
			AddedAt: DateTime.UtcNow
		));
	}

	public void Checkout()
	{
		if (!_items.Any())
			throw new InvalidOperationException("Cannot checkout an empty cart");

		// Business logic...

		AddDomainEvent(new CartCheckedOutEvent(Id, _items.Sum(i => i.Total)));
	}
}

// Usage in application layer
var cart = await _repository.GetByIdAsync(cartId);
cart.AddItem(product, quantity);

await _repository.SaveAsync(cart);

// Dispatch events AFTER successful save
var events = cart.ConsumeEvents();
await _dispatcher.DispatchAsync(events, cancellationToken);
```

## ⚠️ Notes & best practices

### Event Dispatching Timing
- **Always dispatch events AFTER** the unit-of-work/transaction completes successfully
- Dispatching before save risks publishing events for uncommitted changes
- Dispatching before commit risks publishing events that get rolled back

```csharp
// ✅ CORRECT
await _unitOfWork.SaveChangesAsync();
var events = entity.ConsumeEvents();
await _dispatcher.DispatchAsync(events, cancellationToken);

// ❌ WRONG - events dispatched before save
var events = entity.ConsumeEvents();
await _dispatcher.DispatchAsync(events, cancellationToken);
await _unitOfWork.SaveChangesAsync(); // What if this fails?
```

### Event Immutability
- Use `record` types for events to ensure immutability
- Avoid exposing mutable properties
- Include all necessary context at creation time

```csharp
// ✅ CORRECT - immutable record
public record OrderPlacedEvent(Guid OrderId, decimal Total, DateTime PlacedAt) : IDomainEvent;

// ❌ WRONG - mutable class
public class OrderPlacedEvent : IDomainEvent
{
	public Guid OrderId { get; set; }
	public decimal Total { get; set; }
}
```

### Multiple Events
- An entity can raise multiple events during a single operation
- Events are queued in the order they are raised
- All events are dispatched together after persistence

```csharp
public void CompleteOrder()
{
	Status = OrderStatus.Completed;
	AddDomainEvent(new OrderCompletedEvent(Id, DateTime.UtcNow));

	if (IsFirstOrder)
		AddDomainEvent(new FirstOrderCompletedEvent(CustomerId, Id));

	if (Total > 1000)
		AddDomainEvent(new HighValueOrderCompletedEvent(Id, Total));
}
```

### Handler Scope
- Register handlers as **Scoped** services to allow access to scoped resources (DbContext, repositories)
- Handlers should be side-effect operations (send emails, update read models, trigger workflows)
- Keep handlers focused on a single responsibility

### Error Handling
- Decide on error handling strategy at the dispatcher level:
  - **Fail-fast**: Stop on first handler exception (propagate up)
  - **Continue**: Log exceptions but continue processing other handlers
  - **Retry**: Use resilience policies (e.g., Polly) for transient failures

### Testing
- `DomainEntity` makes testing easy — inspect raised events without needing a dispatcher:

```csharp
[Fact]
public void PlaceOrder_RaisesOrderPlacedEvent()
{
	// Arrange
	var order = new Order();

	// Act
	order.PlaceOrder();

	// Assert
	var events = order.GetEvents();
	var orderPlacedEvent = events.OfType<OrderPlacedEvent>().Single();
	Assert.Equal(order.Id, orderPlacedEvent.OrderId);
}
```

### Integration with Outbox Pattern
- For reliable event delivery across process boundaries, consider using an outbox table:

```csharp
// 1. Persist events to outbox table in same transaction as entity save
await _outboxRepository.AddAsync(events.Select(e => new OutboxMessage(e)));
await _unitOfWork.SaveChangesAsync();

// 2. Background worker reads outbox and publishes to message bus
// 3. Mark as processed after successful publish
```

# ⭐ License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

# 🚩 Contact

- **GitHub**: [muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)
- **Project URL**: [https://github.com/muhammadirwanto-dev/sumapap](https://github.com/muhammadirwanto-dev/sumapap)

# ☕ Support

If you find this project helpful, consider supporting the developer:

<a href="https://www.buymeacoffee.com/muhammadirwanto" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
