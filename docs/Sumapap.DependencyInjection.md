# Sumapap.DependencyInjection

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.DependencyInjection/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.DependencyInjection/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

Sumapap.DependencyInjection provides a fluent builder pattern for configuring Sumapap services with Microsoft Dependency Injection. It serves as the foundation for the Sumapap ecosystem's extension point architecture, enabling each library to contribute its own registration methods through a consistent, chainable API.

Core abstractions included:
- IBuilder<T> generic builder interface for fluent configuration patterns.
- SumapapServiceBuilder concrete builder implementation wrapping IServiceCollection.
- ServiceExtensions.AddSumapap() entry point for starting fluent configuration.

## ✨ Why use Sumapap.DependencyInjection?

- Provides a consistent, fluent API for configuring all Sumapap libraries in your application.
- Enables each Sumapap package to extend the builder with its own registration methods without circular dependencies.
- Promotes discoverability IDE IntelliSense guides you through available configuration options as you chain methods.
- Separates DI orchestration from individual library implementations, keeping concerns focused.
- Supports clean, readable service registration in Program.cs or Startup.cs.

## 🚀 Quick start

1. Add the package to your project (when published on NuGet):

   ``bash
   dotnet add package Sumapap.DependencyInjection
   ``

2. Use the fluent builder in your application startup:

   ``csharp
   using Sumapap.DependencyInjection;

   var builder = WebApplication.CreateBuilder(args);

   // Start Sumapap configuration
   var sumapapBuilder = builder.Services.AddSumapap();

   // Each Sumapap library extends the builder
   // (Examples require respective packages to be installed)
   // sumapapBuilder.AddPersistence(...)
   // sumapapBuilder.AddDomainEvents(...)
   // sumapapBuilder.AddQueries(...)

   var app = builder.Build();
   ``

3. Access the underlying IServiceCollection if needed:

   ``csharp
   var services = sumapapBuilder.Build();
   // Continue with manual service registrations if needed
   ``

4. Extension pattern individual libraries extend SumapapServiceBuilder:

   ``csharp
   public static class PersistenceExtensions
   {
       public static SumapapServiceBuilder AddPersistence(
           this SumapapServiceBuilder builder,
           Action<PersistenceOptions> configure)
       {
           // Configure persistence services
           var services = builder.Build();
           services.AddScoped<IUnitOfWork, UnitOfWork>();
           
           return builder;
       }
   }
   ``
# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/src/Sumapap.Ddd.Dispatcher

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>
## 🛠 Features and usage

### IBuilder<T>
- Generic interface defining the builder pattern contract.
- Single method: T Build() returns the built instance.
- Can be used for any fluent configuration scenario beyond Sumapap.

### SumapapServiceBuilder
- Concrete builder wrapping IServiceCollection.
- Primary entry point for Sumapap configuration.
- Implements IBuilder<IServiceCollection>.
- Enables method chaining by returning itself from extension methods.
- Access underlying services via Build() method.

### ServiceExtensions
- Entry point: AddSumapap() extension method on IServiceCollection.
- Returns SumapapServiceBuilder to start fluent configuration chain.
- Validates input (throws ArgumentNullException if services is null).

### Extension Point Architecture
Each Sumapap library extends SumapapServiceBuilder with its own methods:
- AddPersistence(...) from Sumapap.Persistence
- AddDomainEvents(...) from Sumapap.Ddd.Dispatcher or Sumapap.Ddd.Mediator
- AddQueries(...) from Sumapap.Queries
- Libraries access IServiceCollection via builder.Build() internally
- Return the builder to enable continued chaining

## ⚠️ Notes & best practices

- **No circular dependencies** This package depends only on Microsoft.Extensions.DependencyInjection.Abstractions. Individual Sumapap libraries reference this package and extend it, not vice versa.
- **Extension methods** Keep extension methods in dedicated static classes with clear naming (e.g., PersistenceExtensions, DomainEventsExtensions).
- **Return the builder** Always return SumapapServiceBuilder from extension methods to enable chaining.
- **Access services internally** Extension methods should call builder.Build() to get IServiceCollection for registration, but never expose it directly in fluent API.
- **Validation** Use ArgumentNullException.ThrowIfNull() to validate builder parameter in extension methods.
- **Configuration delegates** Prefer Action<TOptions> parameters for complex configuration scenarios to keep APIs clean.
- **Discoverability** Use XML documentation on all extension methods to provide IntelliSense guidance.

## ✅ Example

``csharp
// Program.cs
using Sumapap.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Fluent Sumapap configuration
builder.Services
    .AddSumapap()
    // Extensions from other Sumapap packages
    .AddPersistence(opts => opts.UseEfCore<AppDbContext>())
    .AddDomainEvents()
    .AddQueries();

var app = builder.Build();
app.Run();
``

``csharp
// Custom extension (in your library)
public static class MyLibraryExtensions
{
    /// <summary>
    /// Adds My Library services to the Sumapap builder.
    /// </summary>
    public static SumapapServiceBuilder AddMyLibrary(
        this SumapapServiceBuilder builder,
        Action<MyLibraryOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var services = builder.Build();
        
        // Register services
        services.AddScoped<IMyService, MyService>();
        
        // Apply configuration
        if (configure != null)
        {
            services.Configure(configure);
        }

        return builder;
    }
}
``

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the LICENSE file in the repository for more information.

# 🚩 Contact

GitHub [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
Project Url https://github.com/muhammadirwanto-dev/sumapap/tree/main/src/Sumapap.DependencyInjection

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>
