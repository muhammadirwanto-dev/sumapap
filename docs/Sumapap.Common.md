# Sumapap.Common

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Common.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Common/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Common.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Common/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Common` provides shared utilities and extension methods for common programming tasks across Sumapap applications. The package focuses on:

- String case conversions (kebab-case, snake_case, UPPER_SNAKE_CASE)
- String sanitization and security (HTML encoding, SecureString conversion)
- Object hashing for content comparison (SHA256-based)
- Exception message formatting (deep message extraction with inner exceptions)
- Compiled regex patterns for performance-critical operations

The goal is to reduce boilerplate code and provide consistent, well-tested utility methods that work across all Sumapap packages.

## ✨ Why use `Sumapap.Common`?

- **Modern C# 14 Extensions**: Uses modern `extension(Type variable) { }` syntax for better IntelliSense and discoverability
- **Performance-Optimized**: Compiled regex patterns via `[GeneratedRegex]` for zero-allocation matching
- **Null-Safe**: All extension methods handle `null` gracefully with `[NotNullIfNotNull]` attributes
- **Security-Aware**: Provides sanitization methods to prevent log injection and HTML encoding utilities
- **Type-Safe Hashing**: SHA256-based content hashing for cache keys and change detection
- **Exception Diagnostics**: Deep exception message extraction for better error logging

## 🚀 Quick start

1. Add the package to your project:

``bash
dotnet add package Sumapap.Common
``

2. Import the extensions namespace:

``csharp
using Sumapap.Common.Extensions;
``

3. Use string case conversions:

``csharp
var kebab = "MyVariableName".ToKebabCase();
// Result: "my-variable-name"

var snake = "MyVariableName".ToSnakeCase();
// Result: "my_variable_name"

var upperSnake = "MyVariableName".ToUpperSnakeCase();
// Result: "MY_VARIABLE_NAME"
``

4. Hash objects for content comparison:

``csharp
var user = new User { Id = 1, Name = "John" };
var hash = user.GetContentHash();
// Result: "A1B2C3D4..." (SHA256 hex string)
``

5. Extract deep exception messages:

``csharp
try
{
	// code that throws nested exceptions
}
catch (Exception ex)
{
	var message = ex.GetDeepMessage();
	// Result: Multi-line formatted message with all inner exceptions
}
``

## 🛠 Features and usage

### String Case Conversions

**ToKebabCase()** - Convert to kebab-case (lowercase with hyphens):

``csharp
extension(string? origin)
{
	public string? ToKebabCase();
}
``

**Examples:**
``csharp
"MyVariableName".ToKebabCase();          // "my-variable-name"
"HTTPResponse".ToKebabCase();            // "httpresponse"
"UserId".ToKebabCase();                  // "user-id"
"ProductCatalogService".ToKebabCase();   // "product-catalog-service"

// Null-safe
string? nullValue = null;
nullValue.ToKebabCase();                 // null
"".ToKebabCase();                        // ""
``

**ToSnakeCase()** - Convert to snake_case (lowercase with underscores):

``csharp
extension(string? origin)
{
	public string? ToSnakeCase();
}
``

**Examples:**
``csharp
"MyVariableName".ToSnakeCase();          // "my_variable_name"
"HTTPResponse".ToSnakeCase();            // "httpresponse"
"UserId".ToSnakeCase();                  // "user_id"
"ProductCatalogService".ToSnakeCase();   // "product_catalog_service"
``

**ToUpperSnakeCase()** - Convert to UPPER_SNAKE_CASE (uppercase with underscores):

``csharp
extension(string? origin)
{
	public string? ToUpperSnakeCase();
}
``

**Examples:**
``csharp
"MyVariableName".ToUpperSnakeCase();          // "MY_VARIABLE_NAME"
"HTTPResponse".ToUpperSnakeCase();            // "HTTPRESPONSE"
"UserId".ToUpperSnakeCase();                  // "USER_ID"
"ProductCatalogService".ToUpperSnakeCase();   // "PRODUCT_CATALOG_SERVICE"
``

**Use Cases:**
- **API Endpoints**: Convert C# PascalCase to kebab-case URLs (`/api/user-profile`)
- **JSON Property Names**: Convert to snake_case for Python/Ruby API compatibility
- **Environment Variables**: Convert to UPPER_SNAKE_CASE for standard env var naming
- **Database Columns**: Convert to snake_case for PostgreSQL/MySQL conventions

### String Sanitization and Security

**Sanitize()** - Sanitize strings for safe logging:

``csharp
extension(string? origin)
{
	public string? Sanitize();
}
``

**Examples:**
``csharp
"User input\nwith newlines".Sanitize();
// Result: "User input\\nwith newlines" (escaped)

"<script>alert('XSS')</script>".Sanitize();
// Result: "&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;" (HTML encoded)

"Name: John\r\nPassword: secret".Sanitize();
// Result: "Name: John\\nPassword: secret" (safe for logs)
``

**Use Cases:**
- **Log Injection Prevention**: Escape newlines to prevent log forging
- **HTML Output**: Encode HTML to prevent XSS attacks
- **Safe Logging**: Sanitize user input before writing to logs

**ToSecureString()** - Convert string to SecureString for sensitive data:

``csharp
extension(string? origin)
{
	public SecureString? ToSecureString();
}
``

**Examples:**
``csharp
var password = "MySecretPassword123";
var secured = password.ToSecureString();
// Result: SecureString instance (encrypted in memory)

string? emptyPassword = "";
emptyPassword.ToSecureString();          // null

string? nullPassword = null;
nullPassword.ToSecureString();           // null
``

**Use Cases:**
- **Password Handling**: Store passwords in memory securely
- **API Keys**: Protect sensitive keys from memory dumps
- **Credentials**: Secure user credentials before passing to APIs

### Object Content Hashing

**GetContentHash()** - Generate SHA256 hash of object content:

``csharp
extension(object @object)
{
	public string GetContentHash();
}
``

**Examples:**
``csharp
var user = new User { Id = 1, Name = "John", Email = "john@example.com" };
var hash1 = user.GetContentHash();
// Result: "A1B2C3D4E5F6..." (64-character hex string)

user.Email = "john.doe@example.com";
var hash2 = user.GetContentHash();
// Result: Different hash (content changed)

object? nullObject = null;
nullObject.GetContentHash();             // "" (empty string)
``

**How It Works:**
1. Serializes object to JSON using `System.Text.Json`
2. Computes SHA256 hash of JSON bytes
3. Returns uppercase hex string representation

**Use Cases:**
- **Cache Keys**: Generate deterministic cache keys from object content
- **Change Detection**: Detect if object state has changed
- **Entity Versioning**: Track entity modifications via content hash
- **Idempotency Keys**: Generate unique keys for idempotent operations

**Example - Cache Key Generation:**
``csharp
public class ProductService
{
	private readonly ICacheKeyProvider _cacheKeyProvider;
	private readonly IDistributedCache _cache;

	public async Task<Product?> GetProductAsync(int id, ProductFilter filter)
	{
		// Generate cache key using content hash
		var filterHash = filter.GetContentHash();
		var cacheKey = $"product:{id}:{filterHash}";

		var cached = await _cache.GetStringAsync(cacheKey);
		if (cached != null)
		{
			return JsonSerializer.Deserialize<Product>(cached);
		}

		// Fetch and cache...
	}
}
``

### Exception Message Formatting

**GetDeepMessage()** - Extract full exception message including all inner exceptions:

``csharp
extension(Exception exception)
{
	public string GetDeepMessage(bool writeNewLine = true);
}
``

**Examples:**
``csharp
try
{
	// Throws nested exceptions
	throw new InvalidOperationException(
		"Operation failed",
		new ArgumentException(
			"Invalid argument",
			new NullReferenceException("Object is null")
		)
	);
}
catch (Exception ex)
{
	// Multi-line format (default)
	var multiLine = ex.GetDeepMessage();
	// Result:
	// Operation failed
	//   Invalid argument
	//     Object is null

	// Single-line format
	var singleLine = ex.GetDeepMessage(writeNewLine: false);
	// Result: Operation failed > Invalid argument > Object is null
}
``

**Special Handling:**
- **TaskCanceledException**: Returns only the cancellation message (stops traversing inner exceptions)
- **HttpRequestException**: Extracts HTTP-specific error details

**Use Cases:**
- **Structured Logging**: Log complete exception chain for diagnostics
- **Error Messages**: Display user-friendly error messages with context
- **Exception Monitoring**: Send detailed error info to monitoring systems (Sentry, Application Insights)

**Example - Logging:**
``csharp
public class ErrorHandler
{
	private readonly ILogger<ErrorHandler> _logger;

	public void HandleError(Exception ex)
	{
		_logger.LogError(
			"Request failed: {ErrorMessage}", 
			ex.GetDeepMessage(writeNewLine: false)
		);
		// Logs: Request failed: Operation failed > Invalid argument > Object is null
	}
}
``

### Compiled Regex Patterns

**ComputedRegex** - Pre-compiled regex patterns for performance:

``csharp
public partial class ComputedRegex
{
	[GeneratedRegex(@"[a-zA-Z]+(?=(?:[^{}]*{[^{}]*})*[^{}]*$)")]
	public static partial Regex OutsideBracesRegex();

	[GeneratedRegex("([a-z])([A-Z])")]
	public static partial Regex CaseBoundaryRegex();
}
``

**OutsideBracesRegex()** - Matches text outside curly braces:
``csharp
var input = "Hello {world} foo {bar}";
var matches = ComputedRegex.OutsideBracesRegex().Matches(input);
// Matches: "Hello ", " foo "
``

**CaseBoundaryRegex()** - Matches case boundaries (lowercase to uppercase):
``csharp
var input = "myVariableName";
var result = ComputedRegex.CaseBoundaryRegex().Replace(input, "$1-$2");
// Result: "my-Variable-Name"
``

**Performance Benefits:**
- **Zero Allocation**: `[GeneratedRegex]` generates optimized code at compile-time
- **No Regex Compilation Cost**: Pattern compiled during build, not runtime
- **Reusable**: Shared across all case conversion methods

## ⚠️ Notes & best practices

### ✅ Do

- **Use case conversions for API contracts** - standardize naming conventions between systems
- **Sanitize user input before logging** - prevent log injection attacks
- **Use `GetContentHash()` for cache keys** - deterministic keys based on object state
- **Use `GetDeepMessage()` for error logging** - capture complete exception context
- **Use `ToSecureString()` for sensitive data** - protect passwords and API keys in memory
- **Leverage compiled regex** - `ComputedRegex` provides zero-allocation pattern matching

### ❌ Don't

- **Don't use `ToSecureString()` for non-sensitive data** - adds overhead without benefit
- **Don't rely on `GetContentHash()` for security** - it's for change detection, not cryptographic signatures
- **Avoid excessive sanitization** - only sanitize data going to logs or external systems
- **Don't use case conversions on already-formatted strings** - idempotency not guaranteed for all inputs
- **Don't cache exception messages** - format them on-demand for accurate stack traces

### Case Conversion Edge Cases

**Acronyms and Consecutive Capitals:**
``csharp
"HTTPRequest".ToKebabCase();     // "httprequest" (no separator)
"XMLParser".ToSnakeCase();       // "xmlparser"
"IOError".ToUpperSnakeCase();    // "IOERROR"
``

**Preserve Text in Braces:**
``csharp
"My{Template}Name".ToKebabCase(); // "my-{Template}-name"
// Text inside braces is not converted
``

### Content Hashing Considerations

- **JSON Serialization**: Hash is based on JSON output; property order matters if using custom serializers
- **Deterministic**: Same object state always produces the same hash
- **Collision Resistant**: SHA256 provides strong collision resistance
- **Not Cryptographic**: Suitable for cache keys, not for digital signatures or authentication

### Exception Message Performance

- **Recursive Traversal**: `GetDeepMessage()` traverses all inner exceptions recursively
- **String Building**: Uses `StringBuilder` for efficient string concatenation
- **Early Exit**: Stops at `TaskCanceledException` to avoid verbose cancellation stacks

### Testing Recommendations

When testing code using these extensions:
1. **Test null safety**: Verify behavior with `null` inputs
2. **Test edge cases**: Empty strings, whitespace, special characters
3. **Test case conversion idempotency**: Ensure repeated calls don't corrupt data
4. **Test hash stability**: Verify same object produces same hash
5. **Mock SecureString usage**: Use test doubles for `SecureString` in unit tests

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
	<img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>
