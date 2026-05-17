# Sumapap.Queries.Execution

[![NuGet](https://img.shields.io/nuget/v/Sumapap.Queries.Execution.svg)](https://www.nuget.org/packages/Sumapap.Queries.Execution/)

## Overview

`Sumapap.Queries.Execution` provides powerful query execution infrastructure for the Sumapap query abstraction layer. This library implements the execution engine that transforms `IQuery` objects into actual data operations, supporting both in-memory (`IEnumerable<T>`) and database (`IQueryable<T>`) data sources with optimized filtering, sorting, paging, and cursor-based pagination.

## Features

- **Dual Execution Modes**: Support for both `IQueryable<T>` (database) and `IEnumerable<T>` (in-memory) sources
- **Dynamic Query Execution**: Build and execute queries dynamically from `IQuery` objects
- **Filtering**: Complex filtering with dynamic predicate building
- **Sorting**: Multi-column sorting with dynamic expression generation
- **Paging**: Offset-based pagination (Skip/Take)
- **Cursor Pagination**: Efficient cursor-based pagination for large datasets
- **Performance Optimized**: Expression caching and reflection caching for high performance
- **Async Support**: Full async/await support for database operations

## Installation

```bash
dotnet add package Sumapap.Queries.Execution
```

## Architecture

The library follows a factory pattern with specialized executors:

1. **IQueryExecutor**: Abstract interface for query execution
2. **QueryableQueryExecutor**: Optimized for `IQueryable<T>` (EF Core, LINQ to SQL)
3. **EnumerableQueryExecutor**: Optimized for `IEnumerable<T>` (in-memory collections)
4. **ExecutorFactory**: Factory for creating appropriate executors based on source type

## Basic Usage

### Creating Executors

```csharp
using Sumapap.Queries.Execution;
using Sumapap.Queries.Execution.Factories;

// For IQueryable<T> sources (EF Core, databases)
var executor = ExecutorFactory.CreateQueryableExecutor<User, UserDto>();

// For IEnumerable<T> sources (in-memory)
var executor = ExecutorFactory.CreateEnumerableExecutor<User, UserDto>();
```

### Executing Queries

```csharp
// Create a query
var query = QueryBuilder.Create()
	.Where(filter => filter
		.Add("Status", FilterOperator.Equal, "Active")
		.Add("Age", FilterOperator.GreaterThan, 18)
	)
	.OrderBy("Name", SortDirection.Ascending)
	.Page(1, 20);

// Execute against database (IQueryable)
var dbSource = _dbContext.Users.AsQueryable();
var result = await executor.ExecuteAsync(query, dbSource);

// Execute against in-memory collection (IEnumerable)
var memorySource = users.AsEnumerable();
var result = executor.Execute(query, memorySource);

// Access results
Console.WriteLine($"Total: {result.Total}");
Console.WriteLine($"Page: {result.Page} of {result.TotalPages}");
foreach (var user in result.Items)
{
	Console.WriteLine($"{user.Name} - {user.Email}");
}
```

### Extension Methods

```csharp
using Sumapap.Queries.Execution.Extensions;

// Shorthand extension methods
var result = await _dbContext.Users
	.ExecuteQueryAsync<User, UserDto>(query);

var result = users
	.ExecuteQuery<User, UserDto>(query);
```

## Key Types

### IQueryExecutor<TSource, TResult>

The core abstraction for query execution.

```csharp
public interface IQueryExecutor<TSource, TResult>
{
	// Synchronous execution
	IQueryResult<TResult> Execute(IQuery query, TSource source);

	// Asynchronous execution
	Task<IQueryResult<TResult>> ExecuteAsync(
		IQuery query,
		TSource source,
		CancellationToken cancellationToken = default);
}
```

### QueryableQueryExecutor<TSource, TResult>

Optimized executor for `IQueryable<T>` sources.

```csharp
public class QueryableQueryExecutor<TSource, TResult> 
	: IQueryExecutor<IQueryable<TSource>, TResult>
{
	// Executes queries against database with optimal SQL generation
	public async Task<IQueryResult<TResult>> ExecuteAsync(
		IQuery query,
		IQueryable<TSource> source,
		CancellationToken cancellationToken = default)
	{
		// Builds expression tree for database execution
		// Generates efficient SQL
		// Returns paginated results
	}
}
```

### EnumerableQueryExecutor<TSource, TResult>

Optimized executor for `IEnumerable<T>` sources.

```csharp
public class EnumerableQueryExecutor<TSource, TResult> 
	: IQueryExecutor<IEnumerable<TSource>, TResult>
{
	// Executes queries against in-memory collections
	public IQueryResult<TResult> Execute(
		IQuery query,
		IEnumerable<TSource> source)
	{
		// Builds compiled expressions for in-memory execution
		// Returns paginated results
	}
}
```

### ExecutorFactory

Factory for creating appropriate executors.

```csharp
public static class ExecutorFactory
{
	// Create executor for IQueryable<T>
	public static IQueryExecutor<IQueryable<TSource>, TResult> 
		CreateQueryableExecutor<TSource, TResult>();

	// Create executor for IEnumerable<T>
	public static IQueryExecutor<IEnumerable<TSource>, TResult> 
		CreateEnumerableExecutor<TSource, TResult>();
}
```

## Advanced Usage

### Complex Filtering

```csharp
var query = QueryBuilder.Create()
	.Where(filter => filter
		// AND conditions
		.Add("Status", FilterOperator.Equal, "Active")
		.Add("CreatedDate", FilterOperator.GreaterThanOrEqual, DateTime.Now.AddMonths(-6))

		// OR group
		.BeginGroup(LogicalOperator.Or)
			.Add("Role", FilterOperator.Equal, "Admin")
			.Add("Role", FilterOperator.Equal, "Manager")
		.EndGroup()

		// NOT condition
		.Add("IsDeleted", FilterOperator.NotEqual, true)

		// String operations
		.Add("Email", FilterOperator.Contains, "@company.com")
		.Add("Name", FilterOperator.StartsWith, "John")
	);

var result = await _dbContext.Users
	.ExecuteQueryAsync<User, UserDto>(query);
```

### Multi-Column Sorting

```csharp
var query = QueryBuilder.Create()
	.OrderBy("Department", SortDirection.Ascending)
	.ThenBy("Level", SortDirection.Descending)
	.ThenBy("Name", SortDirection.Ascending);

var result = await _dbContext.Users
	.ExecuteQueryAsync<User, UserDto>(query);
```

### Cursor-Based Pagination

```csharp
// First page
var query = QueryBuilder.Create()
	.OrderBy("Id")
	.PageSize(20);

var result = await _dbContext.Users
	.ExecuteQueryAsync<User, UserDto>(query);

Console.WriteLine($"Next Cursor: {result.NextCursor}");

// Next page using cursor
var nextQuery = QueryBuilder.Create()
	.OrderBy("Id")
	.PageSize(20)
	.After(result.NextCursor);

var nextResult = await _dbContext.Users
	.ExecuteQueryAsync<User, UserDto>(nextQuery);
```

### Custom Projections

```csharp
// Define DTO
public class UserSummaryDto
{
	public int Id { get; set; }
	public string FullName { get; set; }
	public string Email { get; set; }
	public int OrderCount { get; set; }
}

// Execute with projection
var query = QueryBuilder.Create()
	.Select(user => new UserSummaryDto
	{
		Id = user.Id,
		FullName = $"{user.FirstName} {user.LastName}",
		Email = user.Email,
		OrderCount = user.Orders.Count
	})
	.Where("IsActive", FilterOperator.Equal, true)
	.OrderBy("FullName")
	.Page(1, 50);

var result = await _dbContext.Users
	.ExecuteQueryAsync<User, UserSummaryDto>(query);
```

### Batch Processing

```csharp
public async Task ProcessUsersInBatchesAsync()
{
	var batchSize = 1000;
	var currentPage = 1;
	IQueryResult<UserDto> result;

	do
	{
		var query = QueryBuilder.Create()
			.Where("ProcessedDate", FilterOperator.Equal, null)
			.OrderBy("Id")
			.Page(currentPage, batchSize);

		result = await _dbContext.Users
			.ExecuteQueryAsync<User, UserDto>(query);

		// Process batch
		await ProcessBatchAsync(result.Items);

		currentPage++;
	}
	while (result.HasNextPage);
}
```

## Performance Features

### Expression Caching

The library caches compiled expressions to avoid repeated compilation overhead:

```csharp
// First execution: builds and caches expression
var result1 = await executor.ExecuteAsync(query, source);

// Subsequent executions: reuses cached expression (much faster)
var result2 = await executor.ExecuteAsync(query, source);
```

### Reflection Caching

Property and type information is cached for performance:

```csharp
// Reflection operations are cached internally
// Repeated queries on the same types benefit from cached metadata
```

### Cursor Encryption

Cursor values are encrypted to prevent tampering:

```csharp
// Cursors are automatically encrypted
var cursor = result.NextCursor; // "encrypted_value_here"

// And decrypted securely when used
var nextQuery = QueryBuilder.Create().After(cursor);
```

## Best Practices

### 1. Use Appropriate Executor for Source Type

```csharp
// For database/IQueryable ✅
var executor = ExecutorFactory.CreateQueryableExecutor<User, UserDto>();
var result = await executor.ExecuteAsync(query, _dbContext.Users);

// For in-memory/IEnumerable ✅
var executor = ExecutorFactory.CreateEnumerableExecutor<User, UserDto>();
var result = executor.Execute(query, usersList);
```

### 2. Reuse Executor Instances

```csharp
public class UserService
{
	// Reuse executor instance
	private readonly IQueryExecutor<IQueryable<User>, UserDto> _executor;

	public UserService()
	{
		_executor = ExecutorFactory.CreateQueryableExecutor<User, UserDto>();
	}

	public Task<IQueryResult<UserDto>> GetUsersAsync(IQuery query)
	{
		return _executor.ExecuteAsync(query, _dbContext.Users);
	}
}
```

### 3. Use Extension Methods for Cleaner Code

```csharp
// Instead of this
var executor = ExecutorFactory.CreateQueryableExecutor<User, UserDto>();
var result = await executor.ExecuteAsync(query, _dbContext.Users);

// Use this ✅
var result = await _dbContext.Users.ExecuteQueryAsync<User, UserDto>(query);
```

### 4. Handle Large Result Sets with Cursor Pagination

```csharp
// For infinite scroll or large datasets
public async IAsyncEnumerable<UserDto> StreamUsersAsync()
{
	string? cursor = null;

	do
	{
		var query = QueryBuilder.Create()
			.OrderBy("Id")
			.PageSize(100)
			.After(cursor);

		var result = await _dbContext.Users
			.ExecuteQueryAsync<User, UserDto>(query);

		foreach (var user in result.Items)
		{
			yield return user;
		}

		cursor = result.NextCursor;
	}
	while (!string.IsNullOrEmpty(cursor));
}
```

### 5. Project to DTOs for API Responses

```csharp
// Always project to DTOs, never expose entities
var result = await _dbContext.Users
	.ExecuteQueryAsync<User, UserApiDto>(query);

// DTO controls API contract
public class UserApiDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	// Only expose what API clients need
}
```

## Integration with Repository Pattern

```csharp
public class UserRepository : IUserRepository
{
	private readonly DbContext _dbContext;
	private readonly IQueryExecutor<IQueryable<User>, UserDto> _executor;

	public UserRepository(DbContext dbContext)
	{
		_dbContext = dbContext;
		_executor = ExecutorFactory.CreateQueryableExecutor<User, UserDto>();
	}

	public Task<IQueryResult<UserDto>> QueryAsync(IQuery query)
	{
		return _executor.ExecuteAsync(query, _dbContext.Set<User>());
	}
}
```

## Testing

```csharp
public class QueryExecutorTests
{
	[Fact]
	public async Task Execute_WithFiltering_ReturnsFilteredResults()
	{
		// Arrange
		var users = new List<User>
		{
			new User { Id = 1, Name = "John", Age = 25 },
			new User { Id = 2, Name = "Jane", Age = 30 },
			new User { Id = 3, Name = "Bob", Age = 35 }
		}.AsQueryable();

		var query = QueryBuilder.Create()
			.Where("Age", FilterOperator.GreaterThan, 25);

		var executor = ExecutorFactory.CreateQueryableExecutor<User, User>();

		// Act
		var result = await executor.ExecuteAsync(query, users);

		// Assert
		Assert.Equal(2, result.Total);
		Assert.All(result.Items, u => Assert.True(u.Age > 25));
	}
}
```

## Related Packages

- **Sumapap.Queries**: Core query abstractions and builders
- **Sumapap.Persistence**: Repository patterns and specifications
- **Sumapap.Persistence.EfCore**: Entity Framework Core implementations

## Contributing

Contributions are welcome! Please check the [contributing guidelines](https://github.com/muhammadirwanto-dev/sumapap/blob/main/CONTRIBUTING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.
