# Sumapap.Persistence.EFCore

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Persistence.EFCore.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.EFCore/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.EFCore.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Persistence.EFCore/)
[![License](https://img.shields.io/github/license/muhirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhirwanto-dev/sumapap?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhirwanto-dev/sumapap?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhirwanto-dev/sumapap?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/pulls)

## Overview

This library provides concrete implementations of the generic `IReadWriteRepository<TEntity>`, `IReadRepository<TEntity>`, `IWriteRepository<TEntity>` and `IUnitOfWork` interfaces from the [Sumapap.Persistence](https://github.com/muhirwanto-dev/sumapap/tree/main/source/Sumapap.Persistence) package, leveraging Entity Framework Core for data persistence.

## Features

* Generic ReadWriteRepository implementation (`ReadWriteRepository`, `ReadOnlyRepository`) for `EntityFrameworkCore`.
* Unit of Work implementation (`UnitOfWork`) to manage transactions across multiple repositories.
* Easy integration with .NET Dependency Injection.

## Installation

You can install the package via NuGet Package Manager or the .NET CLI:

**Package Manager Console:**

```powershell
Install-Package Sumapap.Persistence.EFCore
```

**.NET CLI**
```bash
dotnet add package Sumapap.Persistence.EFCore
```

## Usage

**Configure Your `DbContext`**

Ensure you have an `DbContext` set up for your application.

```csharp
public class YourDbContext : DbContext
{
    public YourDbContext(DbContextOptions<YourDbContext> options)
        : base(options)
    {
    }
}
```

**Implement `*ReadWriteRepository<TEntity>` and `UnitOfWork`**

```csharp
using Sumapap.Persistence.EFCore.Repositories;
using Sumapap.Persistence.EFCore.UnitOfWork;

// Read-Write repository
public class YourRwRepository<TEntity, TContext> : ReadWriteRepository<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext
{   
}

// Read-Only repository
public class YourRoRepository<TEntity, TContext> : ReadOnlyRepository<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext
{
}

// Unit of work
public class YourUnitOfWork<TContext> : UnitOfWork<TContext>
    where TContext : DbContext
{
    public YourUnitOfWork(TContext dbContext,
        YourRoRepository roRepository,
        YourRwRepository rwRepository)
        : base(dbContext)
    {
        AddRepository<YourRoRepository>(roRepository);
        AddRepository<YourRwRepository>(rwRepository);
    }
}
```

**Configure Dependency Injection**

```csharp
// Inject the services in Program.cs

// Option 1: inject generic ReadWriteRepository & UnitOfWork with db context at once
services.AddEfCorePersistence<YourDbContext>(builder => builder.UseSqlServer("your connection string"));

// Option 2: inject generic ReadWriteRepository & UnitOfWork and db context separately
services.AddEfCorePersistence();
services.AddDbContext<YourDbContext>(builder => builder.UseSqlServer("your connection string"));

// Inject specific ReadWriteRepository & UnitOfWork
services.AddScoped<IReadWriteRepository<YourEntity, YourDbContext>, YourRwRepository<YourEntity, YourDbContext>>();
services.AddScoped<IReadRepository<YourEntity, YourDbContext>, YourRoRepository<YourEntity, YourDbContext>>();
services.AddScoped<IUnitOfWork<YourDbContext>, YourUnitOfWork<YourDbContext>>();

// Set Service Locator after build the app
app.Services.UseSumapapPersistence();
```

## Contributions

Contributions are welcome! If you encounter a bug, have a suggestion, or want to contribute code, please follow these steps:

1.  Check the [GitHub Issues](https://github.com/muhirwanto-dev/sumapap/issues) to see if your issue or idea has already been reported.
2.  If not, open a new issue to describe the bug or feature request.
3.  **For code contributions:**
    * Fork the Project repository.
    * Create your Feature Branch (`git checkout -b feature/YourAmazingFeature`).
    * Commit your Changes (`git commit -m 'Add YourAmazingFeature'`). Adhere to conventional commit messages if possible.
    * Push to the Branch (`git push origin feature/YourAmazingFeature`).
    * Open a Pull Request against the `main` branch of the original repository.
4.  Please try to follow the existing coding style and include unit tests for new or modified functionality.

## License

Distributed under the [MIT License](https://github.com/muhirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

## Contact

[@muhirwanto-dev](https://github.com/muhirwanto-dev)

Project link: [https://github.com/muhirwanto-dev/sumapap/tree/main/source/Sumapap.Persistence.EFCore](https://github.com/muhirwanto-dev/sumapap/tree/main/source/Sumapap.Persistence.EFCore)
