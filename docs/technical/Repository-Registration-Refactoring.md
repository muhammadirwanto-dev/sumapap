# Repository Registration Refactoring

## Overview
This document describes the refactoring that eliminated boilerplate code by merging generic and non-generic repository registration implementations.

## Changes Made

### 1. Unified Registration Entry

**Before:**
- `RepositoryRegistrationEntry` - for non-generic repositories
- `GenericRepositoryRegistrationEntry` - for generic repositories

**After:**
- Single `RepositoryRegistrationEntry` with `IsGeneric` flag and nullable `EntityType`

```csharp
internal sealed record RepositoryRegistrationEntry(
	ServiceLifetime ServiceLifetime,
	Type AbstractType,
	Type ImplType,
	Type? EntityType,           // Nullable for generic repositories
	bool IsGeneric,             // Distinguishes generic from non-generic
	bool AllowCaching,
	RepositoryCacheConfiguration? CachingConfiguration = null);
```

### 2. Unified Configurator

**Before:**
- `RepositoryConfigurator` - for non-generic repositories
- `GenericRepositoryConfigurator` - for generic repositories with duplicated AllowCaching logic

**After:**
- Single `RepositoryConfigurator` that handles both cases

```csharp
public class RepositoryConfigurator
{
	// Works for both generic and non-generic registrations
	public RepositoryConfigurator AllowCaching(Action<RepositoryCacheConfiguration> configure)
	{
		// Unified implementation
	}

	public RepositoryConfigurator AllowCaching()
	{
		// Unified implementation
	}
}
```

### 3. Simplified Builder

**Before:**
- Separate lists: `_registrations` and `_genericRegistraions`
- Duplicate loops in `Build()` method
- Separate methods: `GetServiceTypes()` and `GetGenericServiceTypes()`

**After:**
- Single list: `_registrations`
- Single unified loop in `Build()` method
- Conditional logic based on `IsGeneric` flag

```csharp
public class RepositoryRegistrationBuilder
{
	internal readonly List<RepositoryRegistrationEntry> _registrations = [];

	public SumapapServiceBuilder Build()
	{
		var cacheRegistry = GetOrCreateCacheRegistry(_services);

		foreach (var registration in _registrations)
		{
			// Unified registration logic
			if (registration.IsGeneric)
			{
				RegisterGenericRepository(registration);
			}
			else
			{
				RegisterRepository(registration);
			}

			// Unified caching logic
			if (registration.AllowCaching && registration.CachingConfiguration != null)
			{
				var serviceTypes = registration.IsGeneric 
					? GetGenericServiceTypes(registration) 
					: GetServiceTypes(registration);

				// Register in cache registry...
			}
		}

		return _builder;
	}
}
```

## Benefits

### 1. Reduced Code Duplication
- **Before:** ~300 lines across multiple files with duplicated logic
- **After:** ~150 lines with unified implementation
- **Reduction:** ~50% less code

### 2. Easier Maintenance
- Changes to caching logic now only need to be made in one place
- Single source of truth for registration behavior
- Reduced chance of divergence between generic and non-generic paths

### 3. Improved Type Safety
- Nullable `EntityType` clearly indicates when it's not applicable (generic repositories)
- `IsGeneric` flag makes intent explicit
- Validation logic prevents incorrect usage

### 4. Consistent API
- Both generic and non-generic repositories use the same `RepositoryConfigurator`
- Same `AllowCaching()` methods work for both cases
- Same `.Builder` property for chaining

## Usage Remains Unchanged

The public API remains exactly the same:

```csharp
services.AddSumapap()
	.WithRepositories(builder => builder
		// Non-generic repository
		.AddScopedRepository<UserRepository, User>()
		.AllowCaching(config => config.Duration = TimeSpan.FromMinutes(10))
		.Builder

		// Generic repository
		.AddGenericRepository(typeof(IRepository<>), typeof(Repository<>), ServiceLifetime.Scoped)
		.AllowCaching()
	);
```

## Migration Guide

No migration needed! The refactoring is fully backward compatible. All existing code continues to work without changes.

## Testing Checklist

- [x] Non-generic repository registration works
- [x] Generic repository registration works
- [x] Non-generic repository caching works
- [x] Generic repository caching works
- [x] Builder chaining works for both cases
- [x] Cache registry properly records both types
- [x] Build compiles successfully

## Future Improvements

With this unified architecture, future enhancements become easier:

1. **Additional registration types** can be added with a single flag
2. **Caching enhancements** automatically apply to all repository types
3. **Validation logic** can be centralized
4. **Testing** is simplified with fewer code paths
