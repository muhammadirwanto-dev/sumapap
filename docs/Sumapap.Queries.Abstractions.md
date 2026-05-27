# Sumapap.Queries.Abstractions

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Queries.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries.Abstractions/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Queries.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries.Abstractions/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## ?? Overview

`Sumapap.Queries.Abstractions` provides a composable query metadata model that unifies filtering, sorting, and pagination concerns for repositories, APIs, and UI layers. This package contains only the core abstractions and data structures—no execution logic—making it perfect for sharing between domain, application, and infrastructure layers without introducing dependencies.

The package includes:
- **Query Abstractions** — `IQuery`, `IQueryExecutor`, `IQueryResult` interfaces
- **Filtering** — `FilterConfiguration`, `FilterDescriptor`, `FilterGroup`, `FilterOperator`
- **Sorting** — `SortConfiguration`, `SortDescriptor`, `SortDirection`
- **Pagination** — `OffsettPaginationConfiguration`, `CursorPaginationConfiguration`, `PageInfo`

## ? Why use `Sumapap.Queries.Abstractions`?

- **Zero Dependencies** — Pure abstractions with no implementation or external packages
- **Transport Agnostic** — Normalize query requests across REST, gRPC, or messaging endpoints
- **Separation of Concerns** — Keep persistence logic focused on translating queries, not parsing requests
- **Dual Pagination** — Support both offset and cursor pagination without code duplication
- **Type-Safe** — Strongly-typed query components with enumerated operators and directions

## ?? Quick start

1. Add the package:

```bash
dotnet add package Sumapap.Queries.Abstractions
```

2. Define query components using the abstractions:

```csharp
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Sorting;
using Sumapap.Queries.Abstractions.Paging;

// Build filter configuration
var filters = new FilterConfiguration()
    .WithFilters([
        new FilterDescriptor("Status", FilterOperator.Equals, "Active"),
        new FilterDescriptor("Amount", FilterOperator.GreaterThan, 1000)
    ]);

// Build sort configuration
var sort = new SortConfiguration()
    .By("CreatedAt", SortDirection.Desc)
    .ThenBy("Id", SortDirection.Asc);

// Create query using QueryBuilder from Sumapap.Queries package
var queryBuilder = new QueryBuilder()
    .WithFilters(filters)
    .WithSort(sort)
    .UseOffsetPaging(20, 0);

IQuery query = queryBuilder.Build();
```

3. Pass to your repository or executor:

```csharp
// Repository receives IQuery abstraction
public interface IOrderRepository
{
    Task<IQueryResult<Order>> GetOrdersAsync(IQuery query, CancellationToken cancellationToken = default);
}
```

4. Work with results using IQueryResult:

```csharp
IQueryResult<Order> result = await repository.GetOrdersAsync(query);

Console.WriteLine($"Total: {result.Total}");
foreach (var order in result.Data)
{
    Console.WriteLine($"Order {order.Id}: {order.Total}");
}

if (result.PageInfo.HasNextPage)
{
    // Load next page
}
```

## ?? Features and usage

### IQuery

Core abstraction representing a complete query with filtering, sorting, and pagination. This interface is the primary contract between query consumers (controllers, services) and query executors (repositories, data access layers).

**Key Properties:**
- `Filters` — Never null; defaults to `FilterConfiguration.Empty`
- `Sort` — Never null; defaults to empty configuration
- `OffsetPaging` / `CursorPaging` — Mutually exclusive; one may be set
- `UsesCursorPaging` / `UsesOffsetPaging` — Convenience flags for pagination strategy detection

### IQueryExecutor

Abstraction for query execution engines that transform `IQuery` objects into actual data operations. Takes a source collection and applies filtering, sorting, and pagination.

**Implementations provided by [`Sumapap.Queries`](Sumapap.Queries.md):**
- `QueryableQueryExecutor<T>` — For database queries (IQueryable sources)
- `EnumerableQueryExecutor<T>` — For in-memory collections (IEnumerable sources)

### IQueryResult

Result container returned by query executors, containing the paginated data and metadata.

**Key Components:**
- `Data` — Result items for current page
- `Total` — Total count of items matching query (before pagination)
- `PageInfo` — Pagination metadata (cursors, next/previous indicators)

### FilterConfiguration

Root container for filter logic:

```csharp
// Create filter configuration with list of filters
var filters = new FilterConfiguration()
    .WithFilters([
        new FilterDescriptor("Status", FilterOperator.Equals, "Active"),
        new FilterDescriptor("Price", FilterOperator.LessThan, 100)
    ]);

// Add filter groups for OR logic
var roleGroup = new FilterConfiguration()
    .WithOperator(CompositeOperator.Or)
    .WithFilters([
        new FilterDescriptor("Role", FilterOperator.Equals, "Admin"),
        new FilterDescriptor("Role", FilterOperator.Equals, "Manager")
    ]);

var filters = new FilterConfiguration()
    .WithFilters([
        new FilterDescriptor("Status", FilterOperator.Equals, "Active")
    ])
    .HavingSubGroups([roleGroup]);

// Empty singleton for queries without filters
var noFilters = FilterConfiguration.Empty;
```

### FilterDescriptor

Represents a single filter condition with a field name, operator, and value.

**Example:**
```csharp
new FilterDescriptor("Email", FilterOperator.Contains, "@example.com")
new FilterDescriptor("Age", FilterOperator.GreaterThanOrEqual, 18)
new FilterDescriptor("DeletedAt", FilterOperator.IsNull, null)
```

### FilterOperator

Defines the comparison operations available for filtering.

**Available Operators:**
- Equality: `Equals`, `NotEquals`
- Comparison: `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`
- String: `Contains`, `StartsWith`, `EndsWith`
- Collection: `In`, `NotIn`
- Nullability: `IsNull`, `IsNotNull`

### FilterGroup

Groups filters with AND/OR logic:

```csharp
// Create a group with OR operator
var group = new FilterConfiguration()
    .WithOperator(CompositeOperator.Or)
    .WithFilters([
        new FilterDescriptor("Category", FilterOperator.Equals, "Electronics"),
        new FilterDescriptor("Category", FilterOperator.Equals, "Books")
    ]);

// Nested groups
var outerGroup = new FilterConfiguration()
    .WithOperator(CompositeOperator.And)
    .WithFilters([
        new FilterDescriptor("InStock", FilterOperator.Equals, true)
    ])
    .HavingSubGroups([group]);  // (Category = Electronics OR Category = Books) AND InStock = true
```

### CompositeOperator

Defines how filters within a group are combined: `And` or `Or`.

### SortConfiguration

Container for sort descriptors:

```csharp
// Create sort with primary and secondary fields
var sort = new SortConfiguration()
    .By("Department", SortDirection.Asc)
    .ThenBy("LastName", SortDirection.Asc)
    .ThenBy("FirstName", SortDirection.Asc);

// Check if sorting is configured
if (sort.Sorts.Any())
{
    // Apply sorting
}

// Empty singleton
var noSort = SortConfiguration.Empty;
```

### SortDescriptor

Represents a single sort criterion with a field name and direction (Asc or Desc).

### SortDirection

Defines sort order: `Asc` (ascending) or `Desc` (descending).

### OffsettPaginationConfiguration

Traditional page-based pagination using limit and offset.

**Example:**
```csharp
// Page 1 (first 20 items)
new OffsettPaginationConfiguration(limit: 20, offset: 0)

// Page 3 (items 41-60)
new OffsettPaginationConfiguration(limit: 20, offset: 40)
```

### CursorPaginationConfiguration

Cursor-based pagination for efficient traversal of large datasets without offset performance issues.

**Example:**
```csharp
// First page
new CursorPaginationConfiguration(
    cursorField: "Id",
    cursor: null,
    limit: 20,
    direction: CursorDirection.Forward)

// Next page
new CursorPaginationConfiguration(
    cursorField: "Id",
    cursor: result.PageInfo.EndCursor,
    limit: 20,
    direction: CursorDirection.Forward)
```

### CursorDirection

Defines traversal direction: `Forward` or `Backward`.

### PageInfo

Contains pagination metadata returned with query results, including cursor positions and page availability flags.

**Usage:**
```csharp
if (result.PageInfo.HasNextPage)
{
    var nextQuery = new QueryBuilder()
        .WithFilters(filters)
        .WithSort(sort)
        .UseCursorPaging("Id", result.PageInfo.EndCursor, 20, CursorDirection.Forward)
        .Build();
}
```

## ?? Notes & best practices

### Field naming
- Filter and sort field names must match property names exactly (case-sensitive)
- Validate field names from external input to prevent runtime errors
- Consider using nameof() for compile-time safety: `new FilterDescriptor(nameof(Order.Status), ...)`

### Filter validation
- Sanitize values from external input before creating descriptors
- Validate operators are appropriate for field types
- Use strongly-typed enums for status fields instead of magic strings

### Empty configurations
- Use `FilterConfiguration.Empty` instead of `new FilterConfiguration()` when no filters needed
- Empty configurations avoid null checks and allocation

### Pagination strategy selection

**Use Offset Pagination when:**
- Dataset is small to medium (<100K rows)
- Users need to jump to specific pages
- Total count is required
- Data changes infrequently

**Use Cursor Pagination when:**
- Dataset is large (millions of rows)
- Implementing infinite scroll
- Data changes frequently
- Consistency is more important than random access

### Example: API controller integration

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _repository;

    [HttpGet]
    public async Task<ActionResult<OrderListResponse>> GetOrders(
        [FromQuery] string? status,
        [FromQuery] decimal? minAmount,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] string? sortDir = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filterList = new List<FilterDescriptor>();

        if (!string.IsNullOrEmpty(status))
            filterList.Add(new FilterDescriptor("Status", FilterOperator.Equals, status));

        if (minAmount.HasValue)
            filterList.Add(new FilterDescriptor("Amount", FilterOperator.GreaterThanOrEqual, minAmount.Value));

        var filters = new FilterConfiguration().WithFilters(filterList);

        var direction = sortDir.ToLower() == "asc" ? SortDirection.Asc : SortDirection.Desc;
        var sort = new SortConfiguration().By(sortBy, direction);

        var query = new QueryBuilder()
            .WithFilters(filters)
            .WithSort(sort)
            .UseOffsetPaging(pageSize, (page - 1) * pageSize)
            .Build();

        var result = await _repository.GetOrdersAsync(query);

        return Ok(new OrderListResponse
        {
            Orders = result.Data,
            Total = result.Total,
            Page = page,
            PageSize = pageSize,
            HasMore = result.PageInfo.HasNextPage
        });
    }
}
```

### Example: Mapping from DTOs

```csharp
public static class QueryMapper
{
    public static IQuery ToQuery(this OrderFilterDto dto)
    {
        var filterList = new List<FilterDescriptor>();

        if (dto.Statuses?.Any() == true)
        {
            var statusGroup = new FilterConfiguration()
                .WithOperator(CompositeOperator.Or)
                .WithFilters(dto.Statuses.Select(s => 
                    new FilterDescriptor("Status", FilterOperator.Equals, s)).ToList());

            filterList = [.. filterList];
        }

        if (dto.MinAmount.HasValue)
            filterList.Add(new FilterDescriptor("Amount", FilterOperator.GreaterThanOrEqual, dto.MinAmount));

        if (dto.MaxAmount.HasValue)
            filterList.Add(new FilterDescriptor("Amount", FilterOperator.LessThanOrEqual, dto.MaxAmount));

        var filters = new FilterConfiguration().WithFilters(filterList);

        var sort = new SortConfiguration();
        if (dto.Sorts?.Any() == true)
        {
            sort = sort.By(dto.Sorts[0].Field, dto.Sorts[0].Direction);
            foreach (var sortDto in dto.Sorts.Skip(1))
            {
                sort = sort.ThenBy(sortDto.Field, sortDto.Direction);
            }
        }

        return new QueryBuilder()
            .WithFilters(filters)
            .WithSort(sort)
            .UseOffsetPaging(dto.PageSize, dto.Offset)
            .Build();
    }
}
```

# ? License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

# ?? Contact

- **GitHub**: [muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)
- **Project URL**: [https://github.com/muhammadirwanto-dev/sumapap](https://github.com/muhammadirwanto-dev/sumapap)

# ? Support

If you find this project helpful, consider supporting the developer:

<a href="https://www.buymeacoffee.com/muhammadirwanto" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
