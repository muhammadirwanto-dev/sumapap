# Sumapap.Reporting

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Reporting.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Reporting.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Reporting` provides a centralized error and exception reporting infrastructure with a flexible sink-based architecture. This library enables you to capture exceptions, categorize them by severity, attach metadata, and route reports to multiple destinations (logging systems, UI dialogs, monitoring services, etc.) based on configurable reporting modes.

The package includes:
- **IReportingService** — Centralized service for reporting exceptions
- **Report** — Immutable report object with exception, message, severity, and metadata
- **IReportSink** — Interface for implementing custom report destinations
- **ReportingModes** — Flags for controlling report behavior (Silent, Background, StackTrace, etc.)
- **ReportSeverity** — Severity levels from Trace to Critical
- **Fluent Configuration API** — Type-safe builder pattern for registering sinks

## ✨ Why use `Sumapap.Reporting`?

- **Centralized Error Handling** — Single point for capturing and routing all exceptions
- **Sink-Based Architecture** — Route reports to multiple destinations (logs, UI, monitoring)
- **Flexible Severity Levels** — Categorize reports from Trace to Critical
- **Rich Metadata** — Attach contextual information to reports
- **Reporting Modes** — Control behavior with flags (Silent, Background, StackTrace, UserActionRequired)
- **Async-First** — Non-blocking report processing with cancellation support
- **Extensible** — Create custom sinks for any reporting destination
- **Framework Integration** — Built-in sinks for popular frameworks (see companion packages)

## 🚀 Quick start

1. Add the package to your project:

```bash
dotnet add package Sumapap.Reporting
```

2. Configure reporting in your service registration:

```csharp
using Sumapap.DependencyInjection;
using Sumapap.Reporting.DependencyInjection;

var builder = SumapapServiceBuilder.Create(services);

builder.WithReporting(reporting =>
{
	reporting.ConfigureSinks(sinks =>
	{
		// Register your sinks here (see sink packages)
	});
});
```

3. Inject and use the reporting service:

```csharp
using Sumapap.Reporting.Abstractions;

public class OrderService
{
	private readonly IReportingService _reportingService;

	public OrderService(IReportingService reportingService)
	{
		_reportingService = reportingService;
	}

	public async Task ProcessOrderAsync(Order order)
	{
		try
		{
			// Process order
			await SaveOrderAsync(order);
		}
		catch (Exception ex)
		{
			// Report the exception
			await _reportingService.ReportAsync(ex);
			throw;
		}
	}
}
```

4. Report with additional context:

```csharp
try
{
	await ProcessPaymentAsync(payment);
}
catch (PaymentException ex)
{
	var metadata = new ReportMetadata
	{
		["PaymentId"] = payment.Id,
		["Amount"] = payment.Amount,
		["Provider"] = payment.Provider,
		["UserId"] = payment.UserId
	};

	await _reportingService.ReportAsync(
		ex,
		ReportSeverity.Error,
		metadata);

	throw;
}
```

## 🛠 Features and usage

### IReportingService

The core reporting service interface with multiple overloads:

```csharp
public interface IReportingService
{
	// Synchronous reporting
	void Report(Exception exception);
	void Report(Exception exception, ReportSeverity severity, IReportMetadata? metadata = null);
	void Report(Report report);

	// Asynchronous reporting (preferred)
	Task ReportAsync(Exception exception, CancellationToken cancellationToken = default);
	Task ReportAsync(Exception exception, ReportSeverity severity, IReportMetadata? metadata = null, CancellationToken cancellationToken = default);
	Task ReportAsync(Report report, CancellationToken cancellationToken = default);
}
```

**Basic usage:**
```csharp
// Simple error reporting
await _reportingService.ReportAsync(exception);

// With severity
await _reportingService.ReportAsync(exception, ReportSeverity.Critical);

// With metadata
var metadata = new ReportMetadata { ["Context"] = "Checkout" };
await _reportingService.ReportAsync(exception, ReportSeverity.Error, metadata);

// Pre-constructed report
var report = new Report(exception, "Payment processing failed", ReportSeverity.Critical);
await _reportingService.ReportAsync(report);
```

### Report

Immutable report object containing exception details:

```csharp
public sealed class Report
{
	public Exception Exception { get; }
	public string Message { get; }
	public ReportSeverity Severity { get; }
	public DateTimeOffset Timestamp { get; }
	public IReportMetadata? Metadata { get; }
}
```

**Creating reports:**
```csharp
// Basic report
var report = new Report(
	exception: ex,
	message: "User authentication failed",
	severity: ReportSeverity.Warning);

// Report with metadata
var metadata = new ReportMetadata
{
	["Username"] = username,
	["IPAddress"] = ipAddress,
	["Attempt"] = attemptCount
};

var report = new Report(
	exception: ex,
	message: "Login attempt failed",
	severity: ReportSeverity.Warning,
	metadata: metadata);

// Message defaults to exception.Message if not provided
var report = new Report(ex); // Uses ex.Message and ReportSeverity.Error
```

### ReportSeverity

Severity levels for categorizing reports:

```csharp
public enum ReportSeverity
{
	Trace = 0,        // Diagnostic information for debugging
	Information = 1,  // Non-critical informational message
	Warning = 2,      // Unexpected but recoverable situation
	Error = 3,        // Recoverable error (default)
	Critical = 4      // Critical failure, application unstable
}
```

**Usage guidelines:**
```csharp
// Trace: Diagnostic details
await _reportingService.ReportAsync(ex, ReportSeverity.Trace);

// Information: Expected exceptions (e.g., validation)
await _reportingService.ReportAsync(ex, ReportSeverity.Information);

// Warning: Something unexpected but recoverable
await _reportingService.ReportAsync(ex, ReportSeverity.Warning);

// Error: Standard error reporting (default)
await _reportingService.ReportAsync(ex, ReportSeverity.Error);

// Critical: System-level failures
await _reportingService.ReportAsync(ex, ReportSeverity.Critical);
```

### ReportingModes

Flags for controlling report processing behavior:

```csharp
[Flags]
public enum ReportingModes : uint
{
	None = 0,
	Silent = 1 << 0,                  // Suppress user-facing reporting
	IncludeStackTrace = 1 << 1,       // Include full stack trace
	Background = 1 << 2,              // From background process
	UserActionRequired = 1 << 3,      // Requires user attention
	Default = IncludeStackTrace       // Default behavior
}
```

**Mode usage:**
```csharp
// Silent mode: Log but don't show UI
var modes = ReportingModes.Silent | ReportingModes.IncludeStackTrace;

// Background process: Skip UI sinks
var modes = ReportingModes.Background;

// Critical user issue: Show prominent UI
var modes = ReportingModes.UserActionRequired | ReportingModes.IncludeStackTrace;

// No stack trace: Brief user message
var modes = ReportingModes.None;
```

### IReportSink

Interface for implementing custom report destinations:

```csharp
public interface IReportSink
{
	bool CanHandle(ReportingModes modes, Report report);
	Task HandleAsync(ReportingModes modes, Report report, CancellationToken cancellationToken = default);
}
```

**Custom sink example:**
```csharp
public class EmailReportSink : IReportSink
{
	private readonly IEmailService _emailService;

	public EmailReportSink(IEmailService emailService)
	{
		_emailService = emailService;
	}

	public bool CanHandle(ReportingModes modes, Report report)
	{
		// Only handle critical errors, not background tasks
		return report.Severity == ReportSeverity.Critical
			&& !modes.HasFlag(ReportingModes.Background);
	}

	public async Task HandleAsync(
		ReportingModes modes,
		Report report,
		CancellationToken cancellationToken = default)
	{
		var subject = $"Critical Error: {report.Message}";
		var body = BuildEmailBody(report, modes);

		await _emailService.SendAsync(
			to: "admin@example.com",
			subject: subject,
			body: body,
			cancellationToken: cancellationToken);
	}

	private string BuildEmailBody(Report report, ReportingModes modes)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Severity: {report.Severity}");
		sb.AppendLine($"Timestamp: {report.Timestamp}");
		sb.AppendLine($"Message: {report.Message}");

		if (modes.HasFlag(ReportingModes.IncludeStackTrace))
		{
			sb.AppendLine();
			sb.AppendLine("Stack Trace:");
			sb.AppendLine(report.Exception.ToString());
		}

		if (report.Metadata is not null)
		{
			sb.AppendLine();
			sb.AppendLine("Metadata:");
			foreach (var (key, value) in report.Metadata)
			{
				sb.AppendLine($"  {key}: {value}");
			}
		}

		return sb.ToString();
	}
}
```

### IReportMetadata

Dictionary-based metadata for attaching context to reports:

```csharp
public interface IReportMetadata : IDictionary<string, object>
```

**Usage:**
```csharp
public class ReportMetadata : Dictionary<string, object>, IReportMetadata
{
}

// Create and populate metadata
var metadata = new ReportMetadata
{
	["UserId"] = currentUser.Id,
	["Operation"] = "UpdateProfile",
	["RequestId"] = httpContext.TraceIdentifier,
	["Timestamp"] = DateTimeOffset.UtcNow,
	["Version"] = Assembly.GetExecutingAssembly().GetName().Version?.ToString()
};

await _reportingService.ReportAsync(ex, ReportSeverity.Error, metadata);
```

### Fluent configuration API

Configure reporting using the builder pattern:

```csharp
using Sumapap.DependencyInjection;
using Sumapap.Reporting.DependencyInjection;

var builder = SumapapServiceBuilder.Create(services);

builder.WithReporting(reporting =>
{
	// Configure options
	reporting.Services.Configure<ReportingOptions>(options =>
	{
		options.DefaultMode = ReportingModes.IncludeStackTrace;
		options.MaxReportsPerMinute = 60;
	});

	// Configure sinks
	reporting.ConfigureSinks(sinks =>
	{
		sinks.AddLogger();  // From Sumapap.Reporting.Logging
		sinks.AddDialog();  // From Sumapap.Reporting.Maui

		// Custom sink
		sinks.Services.AddSingleton<IReportSink, EmailReportSink>();
	});
});
```

### Multiple sink coordination

Reports are dispatched to all sinks that can handle them:

```csharp
// Example: Log everything, show UI for errors
public class LoggerReportSink : IReportSink
{
	public bool CanHandle(ReportingModes modes, Report report)
	{
		// Log everything except silent reports
		return !modes.HasFlag(ReportingModes.Silent);
	}

	public Task HandleAsync(ReportingModes modes, Report report, CancellationToken cancellationToken)
	{
		_logger.Log(MapSeverity(report.Severity), report.Exception, report.Message);
		return Task.CompletedTask;
	}
}

public class DialogReportSink : IReportSink
{
	public bool CanHandle(ReportingModes modes, Report report)
	{
		// Show UI only for errors, not background or silent
		return report.Severity >= ReportSeverity.Error
			&& !modes.HasFlag(ReportingModes.Silent)
			&& !modes.HasFlag(ReportingModes.Background);
	}

	public async Task HandleAsync(ReportingModes modes, Report report, CancellationToken cancellationToken)
	{
		await ShowErrorDialogAsync(report.Message);
	}
}
```

### Application insights integration example

```csharp
public class ApplicationInsightsReportSink : IReportSink
{
	private readonly TelemetryClient _telemetryClient;

	public ApplicationInsightsReportSink(TelemetryClient telemetryClient)
	{
		_telemetryClient = telemetryClient;
	}

	public bool CanHandle(ReportingModes modes, Report report)
	{
		// Track all reports
		return true;
	}

	public Task HandleAsync(
		ReportingModes modes,
		Report report,
		CancellationToken cancellationToken = default)
	{
		var exceptionTelemetry = new ExceptionTelemetry(report.Exception)
		{
			Message = report.Message,
			SeverityLevel = MapSeverity(report.Severity),
			Timestamp = report.Timestamp
		};

		// Add metadata as custom properties
		if (report.Metadata is not null)
		{
			foreach (var (key, value) in report.Metadata)
			{
				exceptionTelemetry.Properties[key] = value?.ToString() ?? "";
			}
		}

		// Add reporting modes
		exceptionTelemetry.Properties["ReportingModes"] = modes.ToString();

		_telemetryClient.TrackException(exceptionTelemetry);

		return Task.CompletedTask;
	}

	private static SeverityLevel MapSeverity(ReportSeverity severity) =>
		severity switch
		{
			ReportSeverity.Trace => SeverityLevel.Verbose,
			ReportSeverity.Information => SeverityLevel.Information,
			ReportSeverity.Warning => SeverityLevel.Warning,
			ReportSeverity.Error => SeverityLevel.Error,
			ReportSeverity.Critical => SeverityLevel.Critical,
			_ => SeverityLevel.Error
		};
}
```

### Global exception handler integration

```csharp
public class GlobalExceptionHandler
{
	private readonly IReportingService _reportingService;

	public GlobalExceptionHandler(IReportingService reportingService)
	{
		_reportingService = reportingService;
	}

	public async Task HandleExceptionAsync(Exception exception)
	{
		try
		{
			var metadata = new ReportMetadata
			{
				["Source"] = "GlobalExceptionHandler",
				["MachineName"] = Environment.MachineName,
				["OSVersion"] = Environment.OSVersion.ToString()
			};

			await _reportingService.ReportAsync(
				exception,
				ReportSeverity.Critical,
				metadata);
		}
		catch (Exception reportingEx)
		{
			// Fallback: Don't let reporting failures crash the app
			Console.Error.WriteLine($"Failed to report exception: {reportingEx}");
		}
	}
}

// ASP.NET Core middleware
public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IReportingService _reportingService;

	public ExceptionHandlingMiddleware(
		RequestDelegate next,
		IReportingService reportingService)
	{
		_next = next;
		_reportingService = reportingService;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			var metadata = new ReportMetadata
			{
				["RequestPath"] = context.Request.Path,
				["Method"] = context.Request.Method,
				["StatusCode"] = context.Response.StatusCode,
				["TraceId"] = context.TraceIdentifier
			};

			await _reportingService.ReportAsync(ex, ReportSeverity.Error, metadata);
			throw;
		}
	}
}
```

## ⚠️ Notes & best practices

### Sink design
- Implement `CanHandle` to filter reports by mode and severity
- Keep `HandleAsync` fast and non-blocking (use async I/O)
- Handle exceptions within sinks to prevent cascading failures
- Consider rate limiting in sinks to prevent spam

### Metadata usage
- Include contextual information (user ID, operation, request ID)
- Avoid sensitive data (passwords, tokens, PII)
- Keep metadata values simple (strings, numbers, dates)
- Use consistent key naming across your application

### Severity guidelines
- **Trace**: Verbose debugging information, never shown to users
- **Information**: Expected exceptions (validation failures)
- **Warning**: Unusual but recoverable situations
- **Error**: Standard error reporting (default)
- **Critical**: System failures, data corruption, security issues

### Reporting modes
- Use `Silent` to suppress UI notifications while still logging
- Use `Background` for exceptions from background tasks/workers
- Use `UserActionRequired` for errors requiring immediate user attention
- Use `IncludeStackTrace` for detailed error information (default)

### Performance
- Prefer `ReportAsync` over synchronous `Report` methods
- Sinks should process reports in parallel when possible
- Consider buffering/batching for high-volume reporting scenarios
- Cache reflection/formatting operations in sinks

### Testing
- Mock `IReportingService` in unit tests
- Test that exceptions are reported at correct severity levels
- Verify metadata is populated correctly
- Test sink filtering logic (`CanHandle`)

### Error handling
- Never throw exceptions from `IReportingService` methods
- Sinks should catch and swallow their own exceptions
- Provide fallback logging when reporting fails
- Monitor reporting service health

### Companion packages
- **Sumapap.Reporting.Logging** — Microsoft.Extensions.Logging sink
- **Sumapap.Reporting.Maui** — MAUI dialog sink for mobile apps
- Create custom sinks for your specific needs (Slack, email, monitoring services)

### Example: Complete application setup

```csharp
// Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
	var sumapapBuilder = SumapapServiceBuilder.Create(services);

	sumapapBuilder.WithReporting(reporting =>
	{
		reporting.ConfigureSinks(sinks =>
		{
			// Add built-in sinks
			sinks.AddLogger();

			// Add custom sinks
			sinks.Services.AddSingleton<IReportSink, EmailReportSink>();
			sinks.Services.AddSingleton<IReportSink, ApplicationInsightsReportSink>();
		});

		// Configure options
		reporting.Services.Configure<ReportingOptions>(options =>
		{
			options.DefaultMode = ReportingModes.IncludeStackTrace;
		});
	});

	// Register email service for custom sink
	services.AddSingleton<IEmailService, SmtpEmailService>();

	// Register Application Insights
	services.AddApplicationInsightsTelemetry();
}

// Usage in services
public class OrderService
{
	private readonly IReportingService _reportingService;
	private readonly ILogger<OrderService> _logger;

	public OrderService(
		IReportingService reportingService,
		ILogger<OrderService> logger)
	{
		_reportingService = reportingService;
		_logger = logger;
	}

	public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
	{
		try
		{
			var order = await ProcessOrderAsync(request);
			return order;
		}
		catch (ValidationException ex)
		{
			// Expected error: Log but don't alert
			await _reportingService.ReportAsync(ex, ReportSeverity.Information);
			throw;
		}
		catch (PaymentException ex)
		{
			// Business error: Report with context
			var metadata = new ReportMetadata
			{
				["OrderId"] = request.OrderId,
				["Amount"] = request.Amount,
				["PaymentMethod"] = request.PaymentMethod
			};

			await _reportingService.ReportAsync(ex, ReportSeverity.Error, metadata);
			throw;
		}
		catch (Exception ex)
		{
			// Unexpected error: Critical alert
			var metadata = new ReportMetadata
			{
				["Service"] = nameof(OrderService),
				["Method"] = nameof(CreateOrderAsync),
				["RequestId"] = Activity.Current?.Id
			};

			await _reportingService.ReportAsync(ex, ReportSeverity.Critical, metadata);
			throw;
		}
	}
}
```

# ⭐ License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

# 🚩 Contact

- **GitHub**: [muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)
- **Project URL**: [https://github.com/muhammadirwanto-dev/sumapap](https://github.com/muhammadirwanto-dev/sumapap)

# ☕ Support

If you find this project helpful, consider supporting the developer:

<a href="https://www.buymeacoffee.com/muhammadirwanto" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
