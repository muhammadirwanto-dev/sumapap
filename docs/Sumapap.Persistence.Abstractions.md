# Sumapap.Persistence.Abstractions

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Persistence.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.Abstractions/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.Abstractions/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Persistence.Abstractions` provides core domain-safe persistence contracts that can be safely referenced from your Domain layer without introducing infrastructure dependencies. The package focuses on:

- Entity marker interfaces (`IEntity`, `IEntity<TKey>`)
- Aggregate root marker (`IAggregateRoot`)
- Repository contracts (`IReadRepository<T>`, `IWriteRepository<T>`, `IReadWriteRepository<T>`, `IRepository<T>`)
- Specification pattern contract (`ISpecification<T>`, `ISpecificationEvaluator`)
- Unit of Work abstraction (`IUnitOfWork`)

The goal is to enable Clean Architecture by allowing your domain to depend on abstractions while keeping all infrastructure concerns (EF Core, Dapper, caching, DI) in separate packages.

## ✨ Why use `Sumapap.Persistence.Abstractions`?

- **Domain-Safe**: Zero dependencies on EF Core, DI, caching, or other infrastructure concerns - only abstractions
- **Clean Architecture Compliance**: Enables proper dependency inversion - domain depends on abstractions, infrastructure implements them
- **Framework-Agnostic**: Works with any ORM or data access technology (EF Core, Dapper, NHibernate, ADO.NET)
- **DDD-Friendly**: Provides marker interfaces for entities and aggregate roots to support Domain-Driven Design patterns
- **Type-Safe**: Generic constraints ensure compile-time safety for entity operations
- **Specification Pattern**: Built-in support for the Specification pattern to encapsulate complex query logic

## 🚀 Quick start

1. Add the package to your Domain layer project:

``bash
dotnet add package Sumapap.Persistence.Abstractions
``

2. Mark your domain entities with `IEntity` or `IEntity<TKey>`:

``csharp
public class Product : IEntity<Guid>
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public decimal Price { get; set; }
}
``

3. Mark aggregate roots with `IAggregateRoot`:

``csharp
public class Order : IAggregateRoot, IEntity<int>
{
	public int Id { get; set; }
	public List<OrderLine> Lines { get; set; } = new();

	public void AddLine(Product product, int quantity)
	{
		Lines.Add(new OrderLine(product, quantity));
	}
}
``

4. Define domain repository interfaces extending the base contracts:

``csharp
public interface IProductRepository : IReadRepository<Product>
{
	Task<IEnumerable<Product>> GetExpensiveProductsAsync(decimal minPrice);
}
``

5. Implement repositories in your Infrastructure layer using `Sumapap.Persistence` or `Sumapap.Persistence.EfCore`.

## 🛠 Features and usage

### Entity Interfaces

**IEntity** - Marker interface for all entities:

``csharp
public interface IEntity;
``

Use this as a base marker for types that represent entities:

``csharp
public class AuditLog : IEntity
{
	public Guid Id { get; private set; }
	public string Action { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
}
``

**IEntity<TKey>** - Entity with strongly-typed identifier:

``csharp
public interface IEntity<TKey> : IEntity
	where TKey : IEquatable<TKey>
{
	TKey Id { get; set; }
}
``

Most common entity pattern with type-safe ID:

``csharp
public class User : IEntity<Guid>
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
}

public class Category : IEntity<int>
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
}
``

### Aggregate Root

**IAggregateRoot** - Marker interface for DDD aggregate roots:

``csharp
public interface IAggregateRoot : IEntity;
``

Aggregates enforce consistency boundaries and encapsulate domain logic:

``csharp
public class ShoppingCart : IAggregateRoot, IEntity<Guid>
{
	public Guid Id { get; set; }
	private readonly List<CartItem> _items = new();
	public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

	public void AddItem(Product product, int quantity)
	{
		ArgumentNullException.ThrowIfNull(product);
		if (quantity <= 0)
			throw new ArgumentException("Quantity must be positive", nameof(quantity));

		var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
		if (existingItem != null)
		{
			existingItem.IncreaseQuantity(quantity);
		}
		else
		{
			_items.Add(new CartItem(product.Id, quantity, product.Price));
		}
	}

	public decimal GetTotal() => _items.Sum(i => i.Subtotal);
}
``

### Repository Interfaces

**IRepository<TEntity>** - Base marker for all repositories:

``csharp
public interface IRepository<TEntity> where TEntity : class, IEntity;
``

**IReadRepository<TEntity>** - Read-only repository operations:

``csharp
public interface IReadRepository<TEntity> : IRepository<TEntity>
	where TEntity : class, IEntity
{
	// Synchronous queries
	TEntity? Find(object id);
	IEnumerable<TEntity> GetAll();
	TEntity? FirstOrDefault(ISpecification<TEntity> specification);
	TEntity? SingleOrDefault(ISpecification<TEntity> specification);
	int Count();
	bool Any();

	// Asynchronous queries
	Task<TEntity?> FindAsync(object id, CancellationToken cancellationToken = default);
	Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
	Task<TEntity?> SingleOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
	Task<int> CountAsync(CancellationToken cancellationToken = default);
	Task<bool> AnyAsync(CancellationToken cancellationToken = default);

	// Streaming
	IAsyncEnumerable<TEntity> StreamAllAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<TEntity> StreamWhereAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
``

**IWriteRepository<TEntity>** - Write operations:

``csharp
public interface IWriteRepository<TEntity> : IRepository<TEntity>
	where TEntity : class, IEntity
{
	// Synchronous mutations
	TEntity Add(TEntity entity);
	void Update(TEntity entity);
	void Delete(TEntity entity);
	int SaveChanges();

	// Asynchronous mutations
	Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
	Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
	Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
``

**IReadWriteRepository<TEntity>** - Combined read-write repository:

``csharp
public interface IReadWriteRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
	where TEntity : class, IEntity
{
}
``

**Usage in Domain:**

``csharp
// Define domain-specific repository interface
public interface IOrderRepository : IReadWriteRepository<Order>
{
	Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId);
	Task<Order?> GetOrderWithItemsAsync(int orderId);
}

// Use in domain service
public class OrderService
{
	private readonly IOrderRepository _orderRepository;
	private readonly IUnitOfWork _unitOfWork;

	public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
	{
		_orderRepository = orderRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Order> CreateOrderAsync(Guid customerId, IEnumerable<OrderLine> lines)
	{
		var order = new Order { CustomerId = customerId };
		foreach (var line in lines)
		{
			order.AddLine(line);
		}

		await _orderRepository.AddAsync(order);
		await _unitOfWork.SaveChangesAsync();

		return order;
	}
}
``

### Specification Pattern

**ISpecification<T>** - Contract for reusable query specifications:

``csharp
public interface ISpecification<T>;
``

**ISpecificationEvaluator** - Evaluates specifications against queryable data:

``csharp
public interface ISpecificationEvaluator
{
	IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> specification) 
		where T : class;
}
``

**Define specifications in your domain:**

``csharp
public class ActiveProductsSpecification : ISpecification<Product>
{
	// Specification implementation provided by infrastructure layer
}

public class ExpensiveProductsSpecification : ISpecification<Product>
{
	public decimal MinimumPrice { get; }

	public ExpensiveProductsSpecification(decimal minimumPrice)
	{
		MinimumPrice = minimumPrice;
	}
}
``

**Use specifications with repositories:**

``csharp
var activeProducts = await _productRepository
	.GetAllAsync(new ActiveProductsSpecification());

var expensiveProducts = await _productRepository
	.GetAllAsync(new ExpensiveProductsSpecification(minPrice: 1000m));
``

### Unit of Work

**IUnitOfWork** - Transactional boundary and repository access:

``csharp
public interface IUnitOfWork : IDisposable
{
	IReadWriteRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;

	int SaveChanges();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	void BeginTransaction();
	Task BeginTransactionAsync(CancellationToken cancellationToken = default);

	void CommitTransaction();
	Task CommitTransactionAsync(CancellationToken cancellationToken = default);

	void RollbackTransaction();
	Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
``

**Usage:**

``csharp
public class TransferService
{
	private readonly IUnitOfWork _unitOfWork;

	public TransferService(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
	{
		await _unitOfWork.BeginTransactionAsync();

		try
		{
			var accountRepo = _unitOfWork.GetRepository<Account>();

			var fromAccount = await accountRepo.FindAsync(fromAccountId);
			var toAccount = await accountRepo.FindAsync(toAccountId);

			if (fromAccount == null || toAccount == null)
				throw new InvalidOperationException("Account not found");

			fromAccount.Withdraw(amount);
			toAccount.Deposit(amount);

			await accountRepo.UpdateAsync(fromAccount);
			await accountRepo.UpdateAsync(toAccount);

			await _unitOfWork.SaveChangesAsync();
			await _unitOfWork.CommitTransactionAsync();
		}
		catch
		{
			await _unitOfWork.RollbackTransactionAsync();
			throw;
		}
	}
}
``

## ⚠️ Notes & best practices

### ✅ Do

- **Reference from Domain layer** - this package is safe for domain projects (zero infrastructure dependencies)
- **Use `IAggregateRoot` for consistency boundaries** - mark aggregate roots to clarify domain boundaries
- **Define specific repository interfaces** - extend `IReadRepository<T>` or `IReadWriteRepository<T>` with domain-specific methods
- **Use specifications for complex queries** - encapsulate query logic in reusable specification objects
- **Prefer async methods** - use `*Async` methods in application code to avoid thread starvation
- **Use `IEntity<TKey>` with appropriate key types** - `Guid` for distributed systems, `int` for simple scenarios

### ❌ Don't

- **Never reference infrastructure packages from Domain** - keep domain clean by only depending on abstractions
- **Avoid exposing `IQueryable<T>` from repositories** - use specifications instead to maintain encapsulation
- **Don't create repository per entity** - create repository per aggregate root only
- **Avoid generic repository leakage** - define specific interfaces (`IProductRepository`) rather than using `IRepository<T>` directly in services
- **Don't implement repository logic in domain** - keep domain focused on business rules, not data access
- **Avoid using `IUnitOfWork.GetRepository<T>()` everywhere** - prefer injecting specific repository interfaces

### Repository Per Aggregate

**Correct approach:**

``csharp
// One repository per aggregate root
public interface IOrderRepository : IReadWriteRepository<Order> { }

// Order aggregate manages OrderLines internally
public class Order : IAggregateRoot, IEntity<int>
{
	private readonly List<OrderLine> _lines = new();
	public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
}
``

**Incorrect approach:**

``csharp
// ❌ Don't create separate repositories for child entities
public interface IOrderLineRepository : IRepository<OrderLine> { }
``

### Specification Pattern Best Practices

- Specifications should be **reusable** across the application
- Keep specifications **focused** on a single query concern
- Use **composition** to combine specifications rather than creating complex monolithic ones
- Implement specification logic in **infrastructure layer**, not domain
- Specifications should describe **what** to query, not **how** to query

### Testing Recommendations

When testing domain logic:
1. **Mock repository interfaces** in unit tests
2. **Test domain logic independently** of data access
3. **Use in-memory implementations** for integration tests
4. **Verify specification correctness** by testing against real queries in infrastructure tests

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
