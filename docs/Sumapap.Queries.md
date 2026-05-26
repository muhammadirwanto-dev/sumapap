# Sumapap.Queries

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Queries.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Queries.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Queries` provides a powerful query execution infrastructure that transforms `IQuery` objects into actual data operations. This library includes both the query abstraction layer and execution engines, supporting both in-memory (`IEnumerable<T>`) and database (`IQueryable<T>`) data sources with optimized filtering, sorting, and pagination (offset and cursor-based).

The package includes:
- **Query Builder** — fluent API for constructing queries
- **Query Executors** — execution engines for IQueryable and IEnumerable sources
- **Expression Builders** — dynamic LINQ expression generation with caching
- **Extension Methods** — convenient syntax for query execution

## ✨ Why use `Sumapap.Queries`?

- **Unified API** — Execute queries against both database (IQueryable) and in-memory (IEnumerable) sources with the same interface
- **Performance Optimized** — Expression caching and reflection optimization for high-performance query execution
- **Dynamic Query Building** — Generates efficient SQL for databases and compiled delegates for in-memory collections
- **Cursor Pagination** — Efficient pagination for large datasets without offset performance degradation
- **Type-Safe** — Strongly-typed query building with compile-time safety

## 🚀 Quick start

1. Add the package to your project:

```bash
dotnet add package Sumapap.Queries
```

2. Create a query using the builder:

```csharp
using Sumapap.Queries;
using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Sorting;

var query = new QueryBuilder()
    .WithFilters(new FilterConfiguration()
        .AddFilter(new FilterDescriptor("Status", FilterOperator.Equals, "Active"))
        .AddFilter(new FilterDescriptor("Age", FilterOperator.GreaterThan, 18)))
    .WithSort(new SortConfiguration()
        .AddSort(new SortDescriptor("Name", SortDirection.Ascending)))
    .UseOffsetPaging(limit: 20, offset: 0)
    .Build();
```

3. Execute the query against your data source:

```csharp
using Sumapap.Queries.Executors;

// For database queries (IQueryable)
var queryableExecutor = new QueryableQueryExecutor<User>();
var result = await queryableExecutor.ExecuteAsync(query, dbContext.Users);

// For in-memory collections (IEnumerable)
var enumerableExecutor = new EnumerableQueryExecutor<User>();
var result = enumerableExecutor.Execute(query, usersList);
```

4. Use extension methods for cleaner syntax:

```csharp
using Sumapap.Queries.Extensions;

var result = await dbContext.Users.ExecuteQueryAsync(query);
```

5. Access the paginated results:

```csharp
Console.WriteLine($"Total: {result.Total}");
foreach (var user in result.Data)
{
    Console.WriteLine($"{user.Name} - {user.Email}");
}

if (result.PageInfo.HasNextPage)
{
    Console.WriteLine($"Next cursor: {result.PageInfo.EndCursor}");
}
```

## 🛠 Features and usage

### QueryBuilder

Fluent API for constructing queries:

```csharp
var query = new QueryBuilder()
    .WithFilters(filterConfig)
    .WithSort(sortConfig)
    .UseOffsetPaging(limit: 20, offset: 40)  // Traditional pagination
    .Build();

// Or with cursor pagination
var query = new QueryBuilder()
    .WithFilters(filterConfig)
    .WithSort(sortConfig)
    .UseCursorPaging(
        cursorField: "Id",
        cursor: lastSeenCursor,
        limit: 20,
        direction: CursorDirection.Forward)
    .Build();
```

**Helper methods:**
- `WithOptionalFilter(FilterConfiguration?)` — adds filters only if not null
- `WithOptionalSort(SortConfiguration?)` — adds sorting only if not null

### Query executors

Two specialized executor implementations:

**QueryableQueryExecutor&lt;T&gt;** — Optimized for IQueryable sources (EF Core, database queries):
```csharp
var executor = new QueryableQueryExecutor<Order>();
var result = await executor.ExecuteAsync(query, dbContext.Orders, cancellationToken);
```

**EnumerableQueryExecutor&lt;T&gt;** — Optimized for IEnumerable sources (in-memory collections):
```csharp
var executor = new EnumerableQueryExecutor<Order>();
var result = executor.Execute(query, ordersList);
```

Both executors implement `IQueryExecutor<TSource, T>` with synchronous and asynchronous execution methods.

### Dynamic filtering

Build complex filter expressions:

```csharp
var filters = new FilterConfiguration()
    // Simple filters
    .AddFilter(new FilterDescriptor("Status", FilterOperator.Equals, "Active"))
    .AddFilter(new FilterDescriptor("Price", FilterOperator.LessThan, 100))

    // String operations
    .AddFilter(new FilterDescriptor("Email", FilterOperator.Contains, "@company.com"))
    .AddFilter(new FilterDescriptor("Name", FilterOperator.StartsWith, "John"))

    // Null checks
    .AddFilter(new FilterDescriptor("DeletedAt", FilterOperator.IsNull, null))

    // Collection operations
    .AddFilter(new FilterDescriptor("Category", FilterOperator.In, new[] { "Electronics", "Books" }));

// Filter groups for complex logic (AND/OR)
var filterGroup = new FilterGroup(CompositeOperator.Or)
    .AddFilter(new FilterDescriptor("Role", FilterOperator.Equals, "Admin"))
    .AddFilter(new FilterDescriptor("Role", FilterOperator.Equals, "Manager"));

filters.AddGroup(filterGroup);

var query = new QueryBuilder()
    .WithFilters(filters)
    .Build();
```

**Supported operators:**
- `Equals`, `NotEquals`
- `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`
- `Contains`, `StartsWith`, `EndsWith`
- `In`, `NotIn`
- `IsNull`, `IsNotNull`

### Multi-column sorting

Specify multiple sort columns:

```csharp
var sort = new SortConfiguration()
    .AddSort(new SortDescriptor("Department", SortDirection.Ascending))
    .AddSort(new SortDescriptor("LastName", SortDirection.Ascending))
    .AddSort(new SortDescriptor("Salary", SortDirection.Descending));

var query = new QueryBuilder()
    .WithSort(sort)
    .Build();
```

### Offset pagination

Traditional page-based pagination:

```csharp
var query = new QueryBuilder()
    .UseOffsetPaging(limit: 20, offset: 40)  // Page 3 (0, 20, 40...)
    .Build();

var result = await executor.ExecuteAsync(query, source);

Console.WriteLine($"Total items: {result.Total}");
Console.WriteLine($"Showing items {result.PageInfo.Offset + 1} - {result.PageInfo.Offset + result.Data.Count}");
```

### Cursor pagination

Efficient cursor-based pagination for large datasets:

```csharp
var query = new QueryBuilder()
    .UseCursorPaging(
        cursorField: "Id",           // Field to use as cursor
        cursor: previousEndCursor,    // null for first page
        limit: 20,
        direction: CursorDirection.Forward)
    .Build();

var result = await executor.ExecuteAsync(query, source);

if (result.PageInfo.HasNextPage)
{
    var nextQuery = new QueryBuilder()
        .UseCursorPaging(
            cursorField: "Id",
            cursor: result.PageInfo.EndCursor,  // Use end cursor for next page
            limit: 20,
            direction: CursorDirection.Forward)
        .Build();
}
```

**Cursor pagination benefits:**
- No performance degradation with large offsets
- Consistent results even when data changes
- Ideal for infinite scroll, activity feeds, and real-time data

### Extension methods

Convenient extension methods in `Sumapap.Queries.Extensions`:

```csharp
// Execute on IQueryable
IQueryResult<Order> result = await dbContext.Orders
    .Where(o => o.CustomerId == customerId)
    .ExecuteQueryAsync(query, cancellationToken);

// Execute on IEnumerable
IQueryResult<Order> result = ordersList
    .Where(o => o.Status == "Pending")
    .ExecuteQuery(query);
```

### Expression caching

The library includes built-in expression caching for performance:

- **ExpressionCache** — caches compiled filter expressions
- **ReflectionCache** — caches property info and member access
- Automatic cache invalidation and type-safe key generation

## ⚠️ Notes & best practices

### Executor selection
- Use `QueryableQueryExecutor` for database queries to generate efficient SQL
- Use `EnumerableQueryExecutor` for in-memory collections or already-loaded data
- Executors are thread-safe and can be reused

### Async usage
- Prefer `ExecuteAsync` in server applications to avoid thread pool starvation
- `QueryableQueryExecutor` benefits more from async due to I/O operations
- `EnumerableQueryExecutor` executes synchronously even with `ExecuteAsync`

### Performance
- Filter field names are case-sensitive and must match property names exactly
- Expression caching significantly improves repeated query execution
- Pre-filter large in-memory collections before using `EnumerableQueryExecutor`

### Security
- Validate and sanitize filter field names from external input
- Use strongly-typed filter configuration builders to prevent injection
- Whitelist allowed filter fields in public APIs

### Pagination strategy
- Use offset pagination for:
  - Small to medium datasets
  - When users need to jump to specific pages
  - When total count is required

- Use cursor pagination for:
  - Large datasets (millions of rows)
  - Real-time data feeds
  - Infinite scroll interfaces
  - When data changes frequently

### Filter group logic
- Default group operator is `And`
- Nested groups allow complex boolean logic: `(A AND B) OR (C AND D)`
- Keep filter groups shallow for better readability

### Example: Repository integration

```csharp
public class OrderRepository
{
    private readonly ApplicationDbContext _context;
    private readonly QueryableQueryExecutor<Order> _executor;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
        _executor = new QueryableQueryExecutor<Order>();
    }

    public async Task<IQueryResult<Order>> GetOrdersAsync(
        IQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .ExecuteQueryAsync(query, cancellationToken);
    }

    public async Task<IQueryResult<Order>> SearchOrdersAsync(
        string? searchTerm,
        string? status,
        int limit = 20,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        var filters = new FilterConfiguration();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            filters.AddFilter(new FilterDescriptor("OrderNumber", FilterOperator.Contains, searchTerm));
        }

        if (!string.IsNullOrEmpty(status))
        {
            filters.AddFilter(new FilterDescriptor("Status", FilterOperator.Equals, status));
        }

        var query = new QueryBuilder()
            .WithOptionalFilter(filters.Filters.Any() ? filters : null)
            .WithSort(new SortConfiguration()
                .AddSort(new SortDescriptor("CreatedAt", SortDirection.Descending)))
            .UseCursorPaging("Id", cursor, limit)
            .Build();

        return await _executor.ExecuteAsync(query, _context.Orders, cancellationToken);
    }
}
```

# ⭐ License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

# 🚩 Contact

- **GitHub**: [muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)
- **Project URL**: [https://github.com/muhammadirwanto-dev/sumapap](https://github.com/muhammadirwanto-dev/sumapap)

# ☕ Support

If you find this project helpful, consider supporting the developer:

<a href="https://www.buymeacoffee.com/muhammadirwanto" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>

</p>