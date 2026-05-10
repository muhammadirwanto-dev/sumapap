# Modern C# 14 Extension Syntax - Code Standard

## Rule

**ALL extension methods in Sumapap MUST use modern C# 14 extension syntax.**

## Syntax Comparison

### ✅ CORRECT: Modern C# 14 Extension Syntax

```csharp
public static class SumapapServiceBuilderExtensions
{
	extension(SumapapServiceBuilder builder)
	{
		public SumapapServiceBuilder WithRepositories(Action<RepositoryRegistrationBuilder> configuration)
		{
			ArgumentNullException.ThrowIfNull(configuration);

			var repositoryBuilder = new RepositoryRegistrationBuilder(builder);
			configuration(repositoryBuilder);

			return repositoryBuilder.Build();
		}

		public SumapapServiceBuilder WithCaching(Action<CacheOptions> options)
		{
			// Multiple methods in one extension block
			return builder;
		}
	}
}
```

### ❌ WRONG: Classic Static Extension (DO NOT USE)

```csharp
public static class SumapapServiceBuilderExtensions
{
	public static SumapapServiceBuilder WithRepositories(
		this SumapapServiceBuilder builder,
		Action<RepositoryRegistrationBuilder> configuration)
	{
		if (builder == null)
			throw new ArgumentNullException(nameof(builder));
		if (configuration == null)
			throw new ArgumentNullException(nameof(configuration));

		var repositoryBuilder = new RepositoryRegistrationBuilder(builder);
		configuration(repositoryBuilder);

		return repositoryBuilder.Build();
	}
}
```

## Key Differences

| Aspect | Modern Syntax | Classic Syntax |
|--------|---------------|----------------|
| Declaration | `extension(Type variable) { }` | `public static ReturnType Method(this Type variable)` |
| Method Location | Inside `extension()` block | Top-level in static class |
| Null Checks | `ArgumentNullException.ThrowIfNull(param)` | Manual `if (param == null) throw` |
| Grouping | Multiple methods in one block | One method per declaration |
| Readability | Cleaner, more organized | More verbose |

## Benefits of Modern Syntax

1. **Cleaner Code**: No `this` keyword cluttering method signatures
2. **Better Organization**: Multiple related extensions grouped in one block
3. **Reduced Boilerplate**: Shorter, more readable declarations
4. **Modern Best Practice**: Aligns with C# 14 standards
5. **Consistency**: All extensions follow the same pattern

## Migration Examples

### Example 1: Single Extension

```csharp
// Before (Classic)
public static class StringExtensions
{
	public static bool IsNullOrEmpty(this string? value)
		=> string.IsNullOrEmpty(value);
}

// After (Modern)
public static class StringExtensions
{
	extension(string? value)
	{
		public bool IsNullOrEmpty()
			=> string.IsNullOrEmpty(value);
	}
}
```

### Example 2: Multiple Extensions

```csharp
// Before (Classic)
public static class CollectionExtensions
{
	public static bool IsEmpty<T>(this IEnumerable<T> source)
		=> !source.Any();

	public static bool IsNotEmpty<T>(this IEnumerable<T> source)
		=> source.Any();
}

// After (Modern)
public static class CollectionExtensions
{
	extension<T>(IEnumerable<T> source)
	{
		public bool IsEmpty()
			=> !source.Any();

		public bool IsNotEmpty()
			=> source.Any();
	}
}
```

### Example 3: Builder Pattern

```csharp
// Before (Classic)
public static class RepositoryBuilderExtensions
{
	public static RepositoryConfigurator<T, E> AllowCaching<T, E>(
		this RepositoryConfigurator<T, E> configurator,
		Action<CacheConfig> configure)
		where T : class
		where E : class, IEntity
	{
		ArgumentNullException.ThrowIfNull(configure);
		// implementation
		return configurator;
	}
}

// After (Modern)
public static class RepositoryBuilderExtensions
{
	extension<T, E>(RepositoryConfigurator<T, E> configurator)
		where T : class
		where E : class, IEntity
	{
		public RepositoryConfigurator<T, E> AllowCaching(Action<CacheConfig> configure)
		{
			ArgumentNullException.ThrowIfNull(configure);
			// implementation
			return configurator;
		}
	}
}
```

## Updated Files

The following files have been migrated to modern extension syntax:

1. ✅ `Sumapap.Persistence\DependencyInjection\SumapapServiceBuilderExtensions.cs`
2. ✅ `Sumapap.Persistence.EfCore\DependencyInjection\RepositoryServiceBuilderExtensions.cs`

## Rule Enforcement

This rule has been added to:
- ✅ `.github/copilot-instructions.md` (repository-wide AI guidance)
- ✅ GitHub Copilot memory (personal instruction)

## For Future Development

When creating **any** extension method in Sumapap:

1. **Always** use `extension(Type variable) { }` syntax
2. **Never** use `public static Method(this Type variable)` syntax
3. Group related extensions in the same `extension()` block
4. Use `ArgumentNullException.ThrowIfNull()` for null validation
5. Follow existing patterns in migrated files

## References

- [C# 14 Extension Syntax Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)
- Project Standard: `.github/copilot-instructions.md`
