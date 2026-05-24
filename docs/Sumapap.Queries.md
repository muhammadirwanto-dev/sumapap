# Sumapap.Queries

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Queries.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Queries.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Queries` provides powerful query execution infrastructure for the Sumapap query abstraction layer. This library implements the execution engine that transforms `IQuery` objects into actual data operations, supporting both in-memory (`IEnumerable<T>`) and database (`IQueryable<T>`) data sources with optimized filtering, sorting, paging, and cursor-based pagination.

## ✨ Why use `Sumapap.Queries`?

- Seamlessly execute queries against both database (`IQueryable<T>`) and in-memory (`IEnumerable<T>`) sources with the same API.
- Benefit from expression caching and reflection optimization for high-performance query execution.
- Leverage dynamic query building that generates efficient SQL for databases and compiled delegates for in-memory collections.
- Eliminate boilerplate code with factory patterns and extension methods that integrate cleanly with repositories and services.

## 🚀 Quick start

1. Add the package to your project (when published on NuGet):

``bash
dotnet add package Sumapap.Queries
``

2. Create a query using the builder:

``csharp
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Sorting;

var query = QueryBuilder.Create()
    .Where(filter => filter
        .Add("Status", FilterOperator.Equals, "Active")
        .Add("Age", FilterOperator.GreaterThan, 18))
    .OrderBy("Name", SortDirection.Ascending)
    .Page(1, 20);
``

3. Execute the query against your data source:

``csharp
using Sumapap.Queries;
using Sumapap.Queries.Factories;

// For database queries (IQueryable)
var executor = ExecutorFactory.CreateQueryableExecutor<User, UserDto>();
var result = await executor.ExecuteAsync(query, dbContext.Users);

// For in-memory collections (IEnumerable)
var executor = ExecutorFactory.CreateEnumerableExecutor<User, UserDto>();
var result = executor.Execute(query, usersList);
``

4. Use extension methods for cleaner syntax:

``csharp
using Sumapap.Queries.Extensions;

var result = await dbContext.Users.ExecuteQueryAsync<User, UserDto>(query);
``

5. Access the paginated results:

``csharp
Console.WriteLine($"Total: {result.Total}, Page: {result.Page} of {result.TotalPages}");
foreach (var user in result.Items)
{
    Console.WriteLine($"{user.Name} - {user.Email}");
}
``

## 🛠 Features and usage

### Query executors

The library follows a factory pattern with specialized executors:

- **`IQueryExecutor<TSource, TResult>`** — Core abstraction for query execution with both sync and async methods.
- **`QueryableQueryExecutor<TSource, TResult>`** — Optimized for `IQueryable<T>` sources (EF Core, LINQ to SQL) with efficient SQL generation.
- **`EnumerableQueryExecutor<TSource, TResult>`** — Optimized for `IEnumerable<T>` sources (in-memory collections) with compiled expression caching.
- **`ExecutorFactory`** — Factory for creating appropriate executors based on source type.

Example usage:

``csharp
// Database execution
var queryableExecutor = ExecutorFactory.CreateQueryableExecutor<Order, OrderDto>();
var dbResult = await queryableExecutor.ExecuteAsync(query, dbContext.Orders);

// In-memory execution
var enumerableExecutor = ExecutorFactory.CreateEnumerableExecutor<Order, OrderDto>();
var memResult = enumerableExecutor.Execute(query, ordersList);
``

### Dynamic filtering

Build complex filter expressions dynamically:

``csharp
var query = QueryBuilder.Create()
    .Where(filter => filter
        // AND conditions
        .Add("Status", FilterOperator.Equals, "Active")
        .Add("CreatedDate", FilterOperator.GreaterThanOrEqual, DateTime.Now.AddMonths(-6))
        
        // OR group
        .BeginGroup(LogicalOperator.Or)
            .Add("Role", FilterOperator.Equals, "Admin")
            .Add("Role", FilterOperator.Equals, "Manager")
        .EndGroup()
        
        // String operations
        .Add("Email", FilterOperator.Contains, "@company.com")
        .Add("Name", FilterOperator.StartsWith, "John"));

var result = await dbContext.Users.ExecuteQueryAsync<User, UserDto>(query);
``

### Multi-column sorting

Specify multiple sort columns with independent directions:

``csharp
var query = QueryBuilder.Create()
    .OrderBy("Department", SortDirection.Ascending)
    .ThenBy("LastName", SortDirection.Ascending)
    .ThenBy("FirstName", SortDirection.Ascending);

var result = await executor.ExecuteAsync(query, source);
``

### Offset pagination

Traditional page-based pagination:

``csharp
var query = QueryBuilder.Create()
    .Page(pageNumber: 1, pageSize: 20);

var result = await executor.ExecuteAsync(query, source);

Console.WriteLine($"Page {result.Page} of {result.TotalPages}");
Console.WriteLine($"Showing items {result.From} - {result.To} of {result.Total}");
``

### Cursor pagination

Efficient cursor-based pagination for large datasets:

``csharp
var query = QueryBuilder.Create()
    .WithCursor(
        cursorField: "Id",
        cursor: lastSeenId,
        limit: 20,
        direction: CursorDirection.Forward);

var result = await executor.ExecuteAsync(query, source);

if (result.PageInfo.HasNextPage)
{
    var nextCursor = result.PageInfo.EndCursor;
    // Use nextCursor for next page request
}
``

### Extension methods

Convenient extension methods for common scenarios:

``csharp
// Direct execution on IQueryable
var result = await dbContext.Orders
    .Where(o => o.CustomerId == customerId)
    .ExecuteQueryAsync<Order, OrderDto>(query);

// Direct execution on IEnumerable
var result = ordersList
    .Where(o => o.Status == "Pending")
    .ExecuteQuery<Order, OrderDto>(query);
``

## ⚠️ Notes & best practices

- Use `QueryableQueryExecutor` for database queries to generate efficient SQL rather than loading all data into memory.
- Prefer async APIs (`ExecuteAsync`) in server applications to avoid thread pool starvation.
- Cache executor instances when executing multiple queries with the same source/result types to benefit from internal expression caching.
- Validate and sanitize filter field names and values from external input to prevent injection attacks or runtime errors.
- Use cursor pagination for continuously updated datasets (activity feeds, logs) to avoid page drift caused by inserts/deletes.
- When using `EnumerableQueryExecutor`, be aware that filtering and sorting happen in-memory; pre-filter large collections before execution.
- Keep `QueryBuilder` fluent chains readable by grouping related operations (filter → sort → page) and avoid excessive nesting in filter groups.

### Example integration with repository

``csharp
public class OrderRepository : IOrderRepository
{
    private readonly DbContext _context;
    private readonly IQueryExecutor<IQueryable<Order>, OrderDto> _executor;

    public OrderRepository(DbContext context)
    {
        _context = context;
        _executor = ExecutorFactory.CreateQueryableExecutor<Order, OrderDto>();
    }

    public async Task<IQueryResult<OrderDto>> GetOrdersAsync(
        IQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _executor.ExecuteAsync(query, _context.Orders, cancellationToken);
    }
}
``

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/src/Sumapap.Queries

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>