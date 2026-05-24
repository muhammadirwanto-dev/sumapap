# Visitor Pattern Refactoring: Repository Caching Separation

**Date**: 2025  
**Status**: Implemented  
**Impact**: Breaking Change (Requires API Update)

## Problem Statement

The original `RepositoryRegistrationBuilder` had tight coupling between repository registration logic and caching concerns:

```csharp
public SumapapServiceBuilder Build()
{
    var cacheRegistry = GetOrCreateCacheRegistry(_services);
    
    foreach (var registration in _registrations)
    {
        RegisterRepository(registration);
        
        // Tight coupling: Builder directly handles cache logic
        if (registration.AllowCaching)
        {
            var cacheEntry = new RepositoryCacheEntry { /* ... */ };
            cacheRegistry.Register(cacheEntry);
        }
    }
}
```

**Issues**:
1. **Single Responsibility Violation**: Builder responsible for both registration AND caching
2. **Closed for Extension**: Adding logging, validation, or other cross-cutting concerns requires modifying the builder
3. **Package Boundary Blur**: Core DI logic tightly coupled to caching infrastructure
4. **Testing Complexity**: Cannot test registration and caching concerns in isolation

## Solution: Visitor Pattern

Applied the Visitor pattern to separate cross-cutting concerns from core registration logic:

```csharp
// Core builder (open for extension, closed for modification)
public SumapapServiceBuilder Build()
{
    // Phase 1: Register services
    foreach (var registration in _registrations)
    {
        RegisterRepository(registration);
    }
    
    // Phase 2: Apply visitors for cross-cutting concerns
    foreach (var visitor in _visitors)
    {
        foreach (var registration in _registrations)
        {
            visitor.Visit(registration, _services);
        }
    }
    
    return _builder;
}
```

### Visitor Interface

```csharp
public interface IRepositoryRegistrationVisitor
{
    void Visit(RepositoryRegistrationEntry entry, IServiceCollection services);
}
```

### Caching Visitor Implementation

```csharp
public class CachingRepositoryVisitor : IRepositoryRegistrationVisitor
{
    public void Visit(RepositoryRegistrationEntry entry, IServiceCollection services)
    {
        if (!entry.AllowCaching || entry.CachingConfiguration is null)
            return;
            
        var cacheRegistry = GetOrCreateCacheRegistry(services);
        var cacheEntry = new RepositoryCacheEntry { /* ... */ };
        cacheRegistry.Register(cacheEntry);
    }
}
```

## Architecture Benefits

### Before (Tight Coupling)

```
┌─────────────────────────────────────┐
│  RepositoryRegistrationBuilder      │
│  ┌────────────────────────────────┐ │
│  │  Registration Logic            │ │
│  │  + Caching Logic               │ │
│  │  + (Future: Logging?)          │ │
│  │  + (Future: Validation?)       │ │
│  └────────────────────────────────┘ │
└─────────────────────────────────────┘
```

### After (Visitor Pattern)

```
┌─────────────────────────────────────┐
│  RepositoryRegistrationBuilder      │
│  ┌────────────────────────────────┐ │
│  │  Registration Logic (Core)     │ │
│  │  + Visitor Orchestration       │ │
│  └────────────────────────────────┘ │
└─────────────────────────────────────┘
           │
           │ delegates to
           ▼
┌──────────────────────────────────────┐
│  Visitors (Pluggable)                │
│  ┌─────────────────────────────────┐ │
│  │ CachingRepositoryVisitor        │ │ ← Sumapap.Persistence.Caching
│  └─────────────────────────────────┘ │
│  ┌─────────────────────────────────┐ │
│  │ LoggingRepositoryVisitor        │ │ ← Future extension
│  └─────────────────────────────────┘ │
│  ┌─────────────────────────────────┐ │
│  │ ValidationRepositoryVisitor     │ │ ← Future extension
│  └─────────────────────────────────┘ │
└──────────────────────────────────────┘
```

## API Changes

### Before

```csharp
// Caching was automatically applied if AllowCaching() was called
builder.Services.AddSumapap()
    .WithRepositories(repos =>
    {
        repos.AddScopedRepository<UserRepository, User>()
            .AllowCaching(); // Cache registry populated implicitly
    });
```

### After (Breaking Change)

```csharp
// Must explicitly register the caching visitor
builder.Services.AddSumapap()
    .WithRepositories(repos =>
    {
        repos.AddScopedRepository<UserRepository, User>()
            .AllowCaching() // Only marks intent
            .Builder
            .UseRepositoryCaching(); // Registers visitor to process caching
    });
```

**Migration Path**:
- Add `.UseRepositoryCaching()` call after repository registrations
- Caching visitor will process all repositories marked with `AllowCaching()`

## Implementation Details

### Phase 1: Create Visitor Abstraction

```csharp
// Sumapap.Persistence\DependencyInjection\Abstractions\IRepositoryRegistrationVisitor.cs
public interface IRepositoryRegistrationVisitor
{
    void Visit(RepositoryRegistrationEntry entry, IServiceCollection services);
}
```

### Phase 2: Add Visitor Collection to Builder

```csharp
public class RepositoryRegistrationBuilder
{
    private readonly List<IRepositoryRegistrationVisitor> _visitors = [];
    
    public RepositoryRegistrationBuilder AddVisitor(IRepositoryRegistrationVisitor visitor)
    {
        _visitors.Add(visitor);
        return this;
    }
}
```

### Phase 3: Refactor Build() Method

```csharp
public SumapapServiceBuilder Build()
{
    // Register all repository services first
    foreach (var registration in _registrations)
    {
        if (registration.IsGeneric)
            RegisterGenericRepository(registration);
        else
            RegisterRepository(registration);
    }
    
    // Then apply all visitors for cross-cutting concerns
    foreach (var visitor in _visitors)
    {
        foreach (var registration in _registrations)
        {
            visitor.Visit(registration, _services);
        }
    }
    
    return _builder;
}
```

### Phase 4: Move Caching Logic to Visitor

```csharp
// Sumapap.Persistence\Caching\Visitors\CachingRepositoryVisitor.cs
public class CachingRepositoryVisitor : IRepositoryRegistrationVisitor
{
    public void Visit(RepositoryRegistrationEntry entry, IServiceCollection services)
    {
        if (!entry.AllowCaching || entry.CachingConfiguration is null)
            return;
            
        var cacheRegistry = GetOrCreateCacheRegistry(services);
        // ... populate cache registry
    }
}
```

### Phase 5: Create Extension Method

```csharp
// Sumapap.Persistence\DependencyInjection\Extensions\RepositoryCachingExtensions.cs
public static RepositoryRegistrationBuilder UseRepositoryCaching(
    this RepositoryRegistrationBuilder builder)
{
    builder.AddVisitor(new CachingRepositoryVisitor());
    return builder;
}
```

## Design Principles Applied

1. **Single Responsibility**: Builder handles registration, visitors handle decoration
2. **Open/Closed**: Builder open for extension (add visitors), closed for modification
3. **Dependency Inversion**: Builder depends on `IRepositoryRegistrationVisitor` abstraction
4. **Separation of Concerns**: Caching logic isolated in `Sumapap.Persistence.Caching` package

## Future Extensions

The visitor pattern enables easy addition of:

- **Logging Visitor**: Log all repository registrations
- **Validation Visitor**: Validate registration configurations
- **Metrics Visitor**: Collect registration metrics
- **Audit Visitor**: Track registration for compliance

Example future logging visitor:

```csharp
public class LoggingRepositoryVisitor : IRepositoryRegistrationVisitor
{
    private readonly ILogger _logger;
    
    public void Visit(RepositoryRegistrationEntry entry, IServiceCollection services)
    {
        _logger.LogInformation(
            "Registered repository {Type} for entity {Entity} with lifetime {Lifetime}",
            entry.ImplType, entry.EntityType, entry.ServiceLifetime);
    }
}
```

## Testing Impact

### Before (Coupled)

Testing caching required testing the entire builder:

```csharp
[Fact]
public void Build_WithCaching_PopulatesRegistry()
{
    // Must test registration AND caching together
    var builder = new RepositoryRegistrationBuilder(services);
    builder.AddScopedRepository<UserRepo, User>().AllowCaching();
    builder.Build();
    
    var registry = services.GetCacheRegistry();
    Assert.Single(registry.CachedRepositories);
}
```

### After (Decoupled)

Caching and registration can be tested independently:

```csharp
[Fact]
public void CachingVisitor_ProcessesAllowCachingEntries()
{
    // Test visitor in isolation
    var visitor = new CachingRepositoryVisitor();
    var entry = new RepositoryRegistrationEntry { AllowCaching = true, /* ... */ };
    
    visitor.Visit(entry, services);
    
    var registry = services.GetCacheRegistry();
    Assert.Single(registry.CachedRepositories);
}
```

## Related Documentation

- [Sumapap.Persistence.DependencyInjection](../Sumapap.Persistence.DependencyInjection.md) - Package documentation
- [Sumapap.Persistence.Caching](../Sumapap.Persistence.Caching.md) - Caching package documentation
- [Repository Registration Refactoring](Repository-Registration-Refactoring.md) - Previous refactoring notes

## References

- **Visitor Pattern**: [GoF Design Patterns](https://en.wikipedia.org/wiki/Visitor_pattern)
- **Open/Closed Principle**: [SOLID Principles](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle)
