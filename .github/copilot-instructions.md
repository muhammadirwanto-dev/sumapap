# Sumapap Project Development Guidelines

> "READMEs for AI" — Instructions that guide both humans and GitHub Copilot

## 🏗️ Technology Stack & Architecture

- **Target Framework**: .NET 10 (C# 14.0)
- **Language**: C# with nullable reference types enabled
- **Project Type**: Class libraries (modular ecosystem)
- **Architecture**: Clean Architecture / DDD (Domain-Driven Design)

## 📝 Naming Conventions

### Strict Hierarchy
Follow the **capability-technology** naming pattern:
```
Sumapap.<Capability>.<Technology>
```

**Examples:**
- ✅ `Sumapap.Persistence.EfCore`
- ✅ `Sumapap.Ddd.Mediator`
- ❌ `Sumapap.EfCore` (missing capability)
- ❌ `Sumapap.DatabaseAccess` (not following pattern)

### Code Naming Rules
- **Classes/Interfaces**: PascalCase (`UserRepository`, `IRepository<T>`)
- **Methods/Properties**: PascalCase (`GetByIdAsync`, `FirstName`)
- **Local variables/parameters**: camelCase (`userId`, `entityName`)
- **Private fields**: camelCase with underscore prefix (`_dbContext`, `_logger`)
- **Constants**: PascalCase or UPPER_SNAKE_CASE for magic numbers
- **Async methods**: Must end with `Async` suffix

Any other naming rules not covered above should comply with the Microsoft C# identifier naming conventions:
https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names

### File Organization
- One public type per file
- Abstractions in `Abstractions/` folder
- Service injections and it's related classes are grouped under `DependencyInjection/` folder
- File name matches primary type name
- Files belonging to the same module are grouped under a same folder
- Tests in separate test projects with `.Tests` suffix

## 🎯 Coding Standards

### C# Style
- **Indent**: 4 spaces (no tabs)
- **Line length**: Soft limit at 120 characters
- **Null handling**: Use nullable reference types (`?`), avoid `null!` suppression without justification
- **String literals**: Prefer double quotes (`"text"`)
- **Usings**: Place inside namespace declaration
- **Explicit typing**: Use `var` only when type is obvious from right-hand side
- **Extension Methods**: **ALWAYS** use modern C# 14 extension syntax (`extension(Type variable) { }`) instead of classic static extensions (`this Type variable`)

Any other coding standards not specified above should adhere to the Microsoft C# coding conventions:
https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

### Modern C# 14 Extension Syntax ⚡
**CRITICAL**: This project uses C# 14 modern extension syntax exclusively.

```csharp
// ✅ CORRECT: Modern C# 14 extension syntax
public static class MyExtensions
{
    extension(MyType instance)
    {
        public MyType DoSomething()
        {
            // implementation
            return instance;
        }

        public MyType ChainAnother()
        {
            // multiple methods in one extension block
            return instance;
        }
    }
}

// ❌ WRONG: Classic static extension (DO NOT USE)
public static class MyExtensions
{
    public static MyType DoSomething(this MyType instance)
    {
        return instance;
    }
}
```

**Rules for Extension Methods:**
1. **Always** use `extension(Type variable) { }` syntax
2. **Never** use `public static ReturnType MethodName(this Type variable)` syntax
3. Extension methods are declared inside `extension(...)` blocks
4. The extended type is the parameter to `extension()`, method definitions go inside
5. Multiple extension methods for the same type can be in one `extension()` block
6. Use `ArgumentNullException.ThrowIfNull(parameter)` for null checks

### Preferred Patterns
```csharp
// ✅ Good: Explicit type when not obvious
IEnumerable<User> users = await _repository.GetAllAsync();

// ✅ Good: var when obvious
var user = new User("John", "Doe");

// ❌ Avoid: var when type unclear
var result = SomeMethod(); // What is result?
```

### Required Practices
- **Always** include XML documentation for public APIs
- **Always** implement proper `IDisposable` pattern for unmanaged resources
- **Always** use `ConfigureAwait(false)` in library code (not UI)
- **Always** validate public method parameters (throw `ArgumentNullException`, `ArgumentException`)
- **Never** use `async void` except for event handlers
- **Never** catch exceptions without handling or logging


## 🔌 Dependency Injection Philosophy

### Extension Point Architecture
Sumapap uses the **SumapapServiceBuilder** pattern for fluent DI configuration:

```csharp
// Entry point
services.AddSumapap(builder => 
{
	// Each library extends SumapapServiceBuilder
	builder.AddScopedRepository<UserRepository, User>();
	builder.AddScopedRepository<ProductRepository, Product>()
		.UseCache();
});
```

### Key Principles
- **No circular dependencies**: DI project depends on nothing except abstractions
- **Each library registers itself**: `Sumapap.Persistence` extends `SumapapServiceBuilder`, not the other way around
- **Fluent API**: Chain methods for readability

### Builder Pattern
- Core abstraction: `SumapapServiceBuilder` in `Sumapap.DependencyInjection`
- Extension methods: Each library provides its own registration methods
- No direct `IServiceCollection` exposure in fluent API (accessed via `builder.Services` when needed)

## 📚 Documentation Standards

### XML Documentation
Required for all public APIs:
```csharp
/// <summary>
/// Retrieves a user by unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the user.</param>
/// <param name="cancellationToken">Token to cancel the operation.</param>
/// <returns>The user if found; otherwise <see langword="null"/>.</returns>
/// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is empty.</exception>
public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
```

### Markdown Documentation
Each package requires:
- All projects should be mentioned under **## 🤔 What is included in this repository?** section in `/README.md` file
- All projects should have it's project-specific documentation in `/docs/` folder with name `<Project.Namespace>.md`
- All raw images used in the documentations should be located in `/assets/` (use Mermaid or PNG)
- No documentation files in `/src/` directory

#### Documentation File Structure
Each library **must** have a corresponding documentation file in `/docs/` folder following this structure:

**Naming Convention:**
- File name must match project name: `/src/Sumapap.{Capability}.{Technology}` → `/docs/Sumapap.{Capability}.{Technology}.md`
- Example: `/src/Sumapap.Ddd.Dispatcher` → `/docs/Sumapap.Ddd.Dispatcher.md`

**Required Sections (in order):**
1. **Title** (# Package Name)
2. **Badges** - NuGet version, downloads, license, GitHub issues/stars/forks, contributions welcome
3. **Overview** (## 💡 Overview) - Brief description of what the package does and its core focus areas
4. **Why?** (## ✨ Why use `Package.Name`?) - Value proposition and benefits
5. **Quick Start** (## 🚀 Quick start) - Step-by-step installation and dependency injection setup (numbered list)
6. **Features and Usage** (## 🛠 Features and usage) - Detailed feature documentation with code examples
7. **Notes & Best Practices** (## ⚠️ Notes & best practices) - Important considerations, gotchas, and recommendations
8. **License** (# ⭐ License) - MIT License reference
9. **Contact** (# 🚩 Contact) - GitHub profile and project URL
10. **Support** (# ☕ Support) - Buy me a coffee section with button

**Content Guidelines:**
- Use emojis for section headers to match existing docs
- Include practical code examples in Features and Usage section
- Keep Quick Start concise and actionable (5 steps or less)
- Reference other Sumapap packages using relative links when applicable
- Use `<see langword="null"/>` style formatting for technical terms when appropriate

**References**
- /docs/Sumapap.Persistence.md as the template example

### Code Comments
- Don't add any code comments, except for something should be adjusted later/ future work
- Avoid obvious comments (`// Set name` is useless)
- Use `TODO:`, `FIXME:`, `NOTE:` markers for future work

### Don'ts
- Don't add any documentations in /src, put documentations in /docs instead

## 🧪 Testing Standards

### Test Project Naming
- Unit tests: `<ProjectName>.Tests`
- Integration tests: `<ProjectName>.Integration.Tests`

### Test Method Naming
Use the **Given-When-Then** or **MethodName_Scenario_ExpectedResult** pattern:
```csharp
[Fact]
public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
{
	// Arrange
	var userId = Guid.NewGuid();
	var expected = new User(userId, "John", "Doe");

	// Act
	var actual = await _repository.GetByIdAsync(userId);

	// Assert
	Assert.NotNull(actual);
	Assert.Equal(expected.Id, actual.Id);
}
```

### Test Coverage
- Aim for **80%+ coverage** for domain logic
- **100% coverage** for critical paths (authentication, authorization, payment)
- Mock external dependencies (databases, APIs)
- Use `ITestOutputHelper` for test logging

## 🚨 Error Handling

### Exception Guidelines
- Use built-in exceptions when appropriate (`ArgumentException`, `InvalidOperationException`)
- Create custom exceptions for domain-specific errors
- Always include meaningful error messages
- Never swallow exceptions silently

```csharp
// ✅ Good: Informative exception
if (string.IsNullOrWhiteSpace(email))
	throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));

// ❌ Bad: Generic exception
if (string.IsNullOrWhiteSpace(email))
	throw new Exception("Invalid input");
```

### Async Exception Handling
```csharp
try
{
	await SomeAsyncOperation();
}
catch (DbUpdateException ex)
{
	_logger.LogError(ex, "Failed to save entity {EntityType}", typeof(TEntity).Name);
	throw new RepositoryException("Database update failed", ex);
}
```

## 🎓 Philosophy & Mindset

### Core Values
1. **Pragmatism over Perfection**: Ship working code, iterate based on feedback
2. **Clarity over Cleverness**: Readable code > clever tricks
3. **Explicitness over Magic**: Prefer obvious implementations
4. **Evolvability**: Design for change, not eternal stability

### When Generating Code
- **Ask clarifying questions** instead of guessing when requirements are unclear
- **Follow existing patterns** in the codebase rather than inventing new ones
- **Reference related files** using `#file:path/to/file.cs` syntax when suggesting changes
- **Specify target file names** when outputting code suggestions
- **Validate assumptions** by checking existing implementations before proposing alternatives

### Code Review Mindset
When reviewing or generating code:
1. Does it follow the **Dependency Rule**?
2. Does it maintain **separation of concerns**?
3. Is it **testable** without mocking the entire world?
4. Does it **handle edge cases** (null, empty, errors)?
5. Is it **performant** for expected scale?

## 📦 Package Management

### Versioning
- Follow [Semantic Versioning 2.0.0](https://semver.org/)
- Update `AssemblyVersion` and `FileVersion` in `.csproj`
- Document breaking changes in release notes

### NuGet Properties
Each project must define:
```xml
<PropertyGroup>
  <Title>Sumapap.ProjectName</Title>
  <PackageId>Sumapap.ProjectName</PackageId>
  <Description>Clear, concise description</Description>
  <PackageTags>Sumapap;DDD;Persistence</PackageTags>
  <PackageReadmeFile>docs\Sumapap.ProjectName.md</PackageReadmeFile>
</PropertyGroup>
```

## 🚀 Performance Considerations

### Memory Management
- Dispose `DbContext` properly (use `using` statements)
- Avoid holding large collections in memory
- Use `IAsyncEnumerable<T>` for streaming scenarios

## 🛠️ Development Workflow

### Before Committing
1. Run `dotnet build` (ensure no warnings)
2. Run `dotnet test` (all tests passing)
3. Review changes for sensitive data (secrets, connection strings)
4. Update documentation if public API changed

### Pull Request Guidelines
- Title: `[Category] Brief description` (e.g., `[Persistence] Add caching decorator`)
- Description: Explain **why** the change is needed
- Link related issues
- Request review from relevant maintainers

---

**Remember**: This is a living document. When patterns emerge that improve code quality, propose updates to these guidelines.

**Last Updated**: March 2026 (Aligned with .NET 10 and GitHub Copilot best practices)
