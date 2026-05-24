# Sumapap.Queries.Abstractions

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Queries.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries.Abstractions/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Queries.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Queries.Abstractions/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Queries.Abstractions` delivers a composable query metadata model that unifies filtering, sorting, and pagination concerns for repositories, APIs, and UI layers. By centralizing query intent into transport-friendly objects, it keeps persistence logic focused on translating declarative inputs instead of parsing ad-hoc request contracts.

## ✨ Why use `Sumapap.Queries.Abstractions`?

- Normalize query requests across REST, gRPC, or messaging endpoints using the same abstraction.
- Avoid leaking ORM-specific semantics by mapping generic descriptors to provider-specific implementations.
- Support both offset and cursor pagination strategies without duplicating state handling.
- Compose, reuse, and test filter logic independently from persistence execution code.

## 🚀 Quick start

1. Add the package (once published on NuGet):

    ``bash
    dotnet add package Sumapap.Queries.Abstractions
    ``

2. Build a declarative query from incoming DTOs:

    ``csharp
    using Sumapap.Queries.Abstractions;
    using Sumapap.Queries.Filtering;
    using Sumapap.Queries.Paging;
    using Sumapap.Queries.Sorting;

    var filters = new FilterOptions(
        new FilterGroup()
            .WithFilters([
                new FilterDescriptor("Status", FilterOperator.Equals, "Pending"),
                new FilterDescriptor("Total", FilterOperator.GreaterThanOrEqual, 1000M)
            ]));

    var sort = new SortOptions()
        .By("CreatedAt", SortDirection.Desc)
        .ThenBy("Id");

    var query = new Query(filters, sort, new OffsetPaginationOptions(page: 1, pageSize: 25));
    ``

3. Pass the query to your repository/service and wrap the response:

    ``csharp
    var data = await orderRepository.ExecuteAsync(query, cancellationToken);

    return new QueryResult<OrderDto>(
        data.Items,
        data.TotalDataCount,
        data.PageInfo);
    ``

## 🛠 Features and usage

### Query abstractions
- `IQuery` exposes normalized access to filters, sort descriptors, and pagination payloads with helper flags (`UsesOffsetPaging`, `UsesCursorPaging`) so repositories can branch logic safely.
- `Query` provides multiple constructor overloads to support common scenarios (filters-only, sort-only, pagination-only) while defaulting to `FilterOptions.Empty` and `SortOptions.Empty` when not provided.
- `IQueryResult<T>` and `QueryResult<T>` hold the actual data items, total counts, and optional cursor metadata so callers can return rich responses without bespoke DTOs.

### Filtering
- `FilterDescriptor` pairs a field name, operator, and optional value; use enums in your domain to avoid magic strings.
- `FilterGroup` composes descriptors with `CompositeOperator` (`And`/`Or`) and supports nested groups via `HasSubGroups`, enabling complex boolean expressions.
- `FilterOptions` wraps the root group and offers the `Empty` singleton for cases with no filters.

### Sorting
- `SortDescriptor` keeps a field + direction (`Asc`/`Desc`); use multiple descriptors for deterministic ordering.
- `SortOptions` exposes fluent helpers `By` and `ThenBy` to build sorting chains, and its default constructor starts with an empty list to avoid accidental null checks.

### Pagination
- `OffsetPaginationOptions` covers traditional page/pageSize workflows and exposes a computed `Offset` for SQL `OFFSET` queries.
- `CursorPaginationOptions` supports cursor-based pagination with explicit cursor field, opaque token, limit, and direction (`Forward`/`Backward`).
- Repositories can branch on `UsesCursorPaging` vs `UsesOffsetPaging` to decide which strategy to execute while sharing the same query pipeline.

### Result packaging
- `PageInfo` stores `HasNextPage`, `HasPreviousPage`, and cursor boundaries (`StartCursor`, `EndCursor`) so UI layers can continue pagination confidently.
- Overloaded `QueryResult<T>` constructors make it easy to return empty results or total-count-only payloads without allocating collections.

## ⚠️ Notes & best practices

- Align `FilterDescriptor.Field` values with the names your persistence translator understands (columns, properties, or aliases).
- Validate and sanitize external inputs before turning them into descriptors to prevent injection or unexpected operator usage.
- Prefer cursor pagination for continuously mutating datasets (activity feeds, infinite scroll) to reduce duplicate or missing records.
- Reuse `FilterOptions.Empty`, `SortOptions.Empty`, and lightweight constructors to avoid unnecessary allocations in hot paths.
- Keep mapping between application-specific query DTOs and `Sumapap.Queries.Abstractions` types in a dedicated mapper to ensure consistency across endpoints.

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/src/Sumapap.Queries.Abstractions

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>