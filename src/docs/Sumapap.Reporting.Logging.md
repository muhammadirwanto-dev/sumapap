# Sumapap.Reporting.Logging

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Reporting.Logging.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting.Logging/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Reporting.Logging.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting.Logging/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Reporting.Logging` provides a report sink that forwards error reports to the Microsoft.Extensions.Logging infrastructure. This package bridges `Sumapap.Reporting` with the standard .NET logging system, enabling reports to be automatically logged through configured logging providers (Console, File, Application Insights, Serilog, etc.).

The package includes:
- **LoggerReportSink** — Forwards reports to ILogger with severity mapping
- **Extension methods** — Fluent configuration API for adding the logger sink
- **Automatic severity mapping** — Translates ReportSeverity to LogLevel
- **Stack trace support** — Includes exception details based on reporting mode

## ✨ Why use `Sumapap.Reporting.Logging`?

- **Standard Logging Integration** — Uses Microsoft.Extensions.Logging for consistency
- **Provider-Agnostic** — Works with any logging provider (Console, File, Seq, Application Insights, etc.)
- **Automatic Severity Mapping** — Maps report severity to appropriate log levels
- **Stack Trace Control** — Conditionally logs exception details based on mode
- **Zero Configuration** — Works with existing logging setup
- **Performance Optimized** — Checks log level before formatting to avoid overhead
- **Universal Support** — Works in ASP.NET Core, MAUI, Console apps, etc.

## 🚀 Quick start

1. Add the package to your project:

```bash
dotnet add package Sumapap.Reporting.Logging
```

2. Configure the logger sink in your service registration:

```csharp
using Sumapap.DependencyInjection;
using Sumapap.Reporting.DependencyInjection;
using Sumapap.Reporting.Logging.DependencyInjection;

// ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

sumapapBuilder.WithReporting(reporting =>
{
	reporting.ConfigureSinks(sinks =>
	{
		sinks.AddLogger();  // Add logging sink
	});
});
```

3. Configure standard logging providers:

```csharp
// ASP.NET Core (appsettings.json)
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  }
}

// Or programmatically
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddApplicationInsights();
```

4. Use the reporting service:

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
			await SaveOrderAsync(order);
		}
		catch (Exception ex)
		{
			// Automatically logged via LoggerReportSink
			await _reportingService.ReportAsync(ex, ReportSeverity.Error);
			throw;
		}
	}
}
```

5. Reports are automatically logged:

```
[2024-05-24 10:30:45] [Error] OrderService: Failed to save order
System.InvalidOperationException: Database connection failed
   at OrderService.SaveOrderAsync(Order order)
   at OrderService.ProcessOrderAsync(Order order)
```

## 🛠 Features and usage

### LoggerReportSink

The core sink that forwards reports to ILogger:

**Severity mapping:**
- `ReportSeverity.Trace` → `LogLevel.Trace`
- `ReportSeverity.Information` → `LogLevel.Information`
- `ReportSeverity.Warning` → `LogLevel.Warning`
- `ReportSeverity.Error` → `LogLevel.Error`
- `ReportSeverity.Critical` → `LogLevel.Critical`

**Implementation details:**
```csharp
public class LoggerReportSink : IReportSink
{
	private readonly ILogger<LoggerReportSink> _logger;

	public bool CanHandle(ReportingModes modes, Report report)
	{
		// Only log if the logger level is enabled
		return _logger.IsEnabled(MapSeverity(report.Severity));
	}

	public Task HandleAsync(ReportingModes modes, Report report, CancellationToken cancellationToken)
	{
		var logLevel = MapSeverity(report.Severity);

		if (modes.HasFlag(ReportingModes.IncludeStackTrace))
		{
			_logger.Log(logLevel, report.Exception, report.Message);
		}
		else
		{
			_logger.Log(logLevel, report.Message);
		}

		return Task.CompletedTask;
	}
}
```

### AddLogger extension method

Fluent API for registering the logger sink:

```csharp
reporting.ConfigureSinks(sinks =>
{
	sinks.AddLogger();
});
```

### Combining with other sinks

Use multiple sinks for comprehensive error handling:

```csharp
using Sumapap.Reporting.Logging.DependencyInjection;
using Sumapap.Reporting.Maui.DependencyInjection;

sumapapBuilder.WithReporting(reporting =>
{
	reporting.ConfigureSinks(sinks =>
	{
		sinks.AddLogger();  // Log all reports
		sinks.AddDialog();  // Show UI for user-facing errors (MAUI)
	});
});
```

**Behavior:**
- Logger sink: Logs all reports based on configured log levels
- Dialog sink: Shows UI only for Error and Critical in MAUI apps

### Log level filtering

Control which reports are logged using standard logging configuration:

**appsettings.json:**
```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Sumapap.Reporting.Logging.Sinks.LoggerReportSink": "Warning"
	}
  }
}
```

**Result:**
- Reports with Trace and Information severity are not logged
- Reports with Warning, Error, and Critical severity are logged

### Stack trace control

Control whether stack traces are logged:

**With stack trace (default):**
```csharp
await _reportingService.ReportAsync(exception, ReportSeverity.Error);
```

**Log output:**
```
[Error] Failed to process payment
System.InvalidOperationException: Payment gateway timeout
   at PaymentService.ProcessAsync(Payment payment)
   at OrderService.CompleteOrderAsync(Order order)
```

**Without stack trace:**
```csharp
services.Configure<ReportingOptions>(options =>
{
	options.DefaultMode = ReportingModes.None; // No stack trace
});

await _reportingService.ReportAsync(exception, ReportSeverity.Error);
```

**Log output:**
```
[Error] Failed to process payment
```

### Structured logging with metadata

Include structured data in logs:

```csharp
var metadata = new ReportMetadata
{
	["OrderId"] = order.Id,
	["CustomerId"] = order.CustomerId,
	["Amount"] = order.TotalAmount,
	["PaymentMethod"] = order.PaymentMethod
};

await _reportingService.ReportAsync(
	exception,
	ReportSeverity.Error,
	metadata);
```

**Note:** The metadata is stored in the Report object but not automatically included in the log message by LoggerReportSink. To include metadata in logs, create a custom sink or use a structured logging provider that captures exception data.

### ASP.NET Core integration

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddApplicationInsights();

// Configure Sumapap reporting
var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

sumapapBuilder.WithReporting(reporting =>
{
	reporting.ConfigureSinks(sinks =>
	{
		sinks.AddLogger();
	});
});

var app = builder.Build();

// Use reporting in middleware
app.Use(async (context, next) =>
{
	try
	{
		await next(context);
	}
	catch (Exception ex)
	{
		var reportingService = context.RequestServices.GetRequiredService<IReportingService>();

		var metadata = new ReportMetadata
		{
			["RequestPath"] = context.Request.Path,
			["Method"] = context.Request.Method,
			["StatusCode"] = context.Response.StatusCode,
			["TraceId"] = context.TraceIdentifier
		};

		await reportingService.ReportAsync(ex, ReportSeverity.Error, metadata);
		throw;
	}
});

app.Run();
```

### MAUI integration

```csharp
// MauiProgram.cs
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		// Configure logging
		builder.Logging.AddDebug();

#if DEBUG
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
		builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

		// Configure Sumapap reporting
		var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

		sumapapBuilder.WithReporting(reporting =>
		{
			reporting.ConfigureSinks(sinks =>
			{
				sinks.AddLogger();  // Log to debug output
				sinks.AddDialog();  // Show dialogs for errors
			});
		});

		return builder.Build();
	}
}
```

### Console application integration

```csharp
// Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure Sumapap reporting
var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

sumapapBuilder.WithReporting(reporting =>
{
	reporting.ConfigureSinks(sinks =>
	{
		sinks.AddLogger();
	});
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
```

### Serilog integration

```csharp
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
	.WriteTo.Seq("http://localhost:5341")
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Configure Sumapap reporting
var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

sumapapBuilder.WithReporting(reporting =>
{
	reporting.ConfigureSinks(sinks =>
	{
		sinks.AddLogger();  // Reports are logged via Serilog
	});
});

var app = builder.Build();
app.Run();
```

**Log output (Serilog):**
```json
{
  "@t": "2024-05-24T10:30:45.123Z",
  "@l": "Error",
  "@m": "Failed to process payment",
  "@x": "System.InvalidOperationException: Payment gateway timeout\n   at PaymentService.ProcessAsync(Payment payment)",
  "SourceContext": "Sumapap.Reporting.Logging.Sinks.LoggerReportSink"
}
```

### Custom logger sink with metadata

Create a custom sink to include metadata in logs:

```csharp
public class StructuredLoggerReportSink : IReportSink
{
	private readonly ILogger<StructuredLoggerReportSink> _logger;

	public StructuredLoggerReportSink(ILogger<StructuredLoggerReportSink> logger)
	{
		_logger = logger;
	}

	public bool CanHandle(ReportingModes modes, Report report)
	{
		return _logger.IsEnabled(MapSeverity(report.Severity));
	}

	public Task HandleAsync(
		ReportingModes modes,
		Report report,
		CancellationToken cancellationToken = default)
	{
		var logLevel = MapSeverity(report.Severity);

		// Build structured log with metadata
		using (_logger.BeginScope(new Dictionary<string, object>
		{
			["ReportSeverity"] = report.Severity.ToString(),
			["Timestamp"] = report.Timestamp,
			["ExceptionType"] = report.Exception.GetType().Name
		}))
		{
			// Add metadata to scope
			if (report.Metadata is not null)
			{
				using (_logger.BeginScope(report.Metadata))
				{
					if (modes.HasFlag(ReportingModes.IncludeStackTrace))
					{
						_logger.Log(logLevel, report.Exception, report.Message);
					}
					else
					{
						_logger.Log(logLevel, report.Message);
					}
				}
			}
			else
			{
				if (modes.HasFlag(ReportingModes.IncludeStackTrace))
				{
					_logger.Log(logLevel, report.Exception, report.Message);
				}
				else
				{
					_logger.Log(logLevel, report.Message);
				}
			}
		}

		return Task.CompletedTask;
	}

	private static LogLevel MapSeverity(ReportSeverity severity) =>
		severity switch
		{
			ReportSeverity.Trace => LogLevel.Trace,
			ReportSeverity.Information => LogLevel.Information,
			ReportSeverity.Warning => LogLevel.Warning,
			ReportSeverity.Error => LogLevel.Error,
			ReportSeverity.Critical => LogLevel.Critical,
			_ => LogLevel.Error
		};
}

// Register custom sink
reporting.ConfigureSinks(sinks =>
{
	sinks.Services.AddSingleton<IReportSink, StructuredLoggerReportSink>();
});
```

### Background task logging

```csharp
public class BackgroundSyncService : BackgroundService
{
	private readonly IReportingService _reportingService;
	private readonly ILogger<BackgroundSyncService> _logger;

	public BackgroundSyncService(
		IReportingService reportingService,
		ILogger<BackgroundSyncService> logger)
	{
		_reportingService = reportingService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await SyncDataAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				// Logged via LoggerReportSink
				await _reportingService.ReportAsync(ex, ReportSeverity.Error);
			}

			await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
		}
	}
}
```

**Log output:**
```
[2024-05-24 10:30:45] [Error] BackgroundSyncService: Sync failed
System.Net.Http.HttpRequestException: Connection timeout
   at BackgroundSyncService.SyncDataAsync(CancellationToken cancellationToken)
```

### Testing

Mock the reporting service and verify logging behavior:

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class OrderServiceTests
{
	[Fact]
	public async Task ProcessOrder_LogsError_WhenExceptionThrown()
	{
		// Arrange
		var mockReporting = new Mock<IReportingService>();
		var service = new OrderService(mockReporting.Object);
		var order = new Order { Id = 1 };

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(
			() => service.ProcessOrderAsync(order));

		mockReporting.Verify(
			x => x.ReportAsync(
				It.IsAny<Exception>(),
				ReportSeverity.Error,
				It.IsAny<IReportMetadata>(),
				It.IsAny<CancellationToken>()),
			Times.Once);
	}
}
```

## ⚠️ Notes & best practices

### Log level configuration
- Configure log levels via `appsettings.json` or programmatically
- Use appropriate severity levels: Trace for debugging, Error for production issues
- Consider different log levels for Development vs Production

### Performance
- LoggerReportSink checks `IsEnabled` before logging to avoid overhead
- Stack trace formatting is minimal when not needed
- Synchronous logging minimizes async overhead

### Structured logging
- Use Serilog or other structured logging providers for rich log data
- Include metadata in reports for contextual information
- Consider creating a custom sink to include metadata in structured logs

### Log aggregation
- Use centralized logging (Application Insights, Seq, Elasticsearch)
- Configure appropriate log retention policies
- Monitor log volume and adjust levels as needed

### Silent mode
- LoggerReportSink respects `ReportingModes.Silent`
- Use Silent mode to suppress logging when not needed
- Combine with other sinks for selective reporting

### Stack traces
- Include stack traces in non-production for debugging
- Consider excluding stack traces in production for sensitive data
- Use `ReportingModes.IncludeStackTrace` to control behavior

### Integration patterns
- Combine with Application Insights for cloud monitoring
- Use with Serilog for flexible log enrichment
- Pair with console logging for development
- Add file logging for offline scenarios

### Security
- Avoid logging sensitive data (passwords, tokens, PII)
- Sanitize exception messages before logging
- Configure log access controls appropriately
- Review logs regularly for security events

### Example: Production configuration

```csharp
// appsettings.Production.json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Warning",
	  "Microsoft": "Warning",
	  "Sumapap.Reporting.Logging.Sinks.LoggerReportSink": "Error"
	},
	"ApplicationInsights": {
	  "LogLevel": {
		"Default": "Information"
	  }
	}
  }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure logging
if (builder.Environment.IsProduction())
{
	builder.Logging.AddApplicationInsights();
	builder.Services.Configure<ReportingOptions>(options =>
	{
		// Don't include stack traces in production logs
		options.DefaultMode = ReportingModes.None;
	});
}
else
{
	builder.Logging.AddConsole();
	builder.Logging.AddDebug();
	builder.Services.Configure<ReportingOptions>(options =>
	{
		options.DefaultMode = ReportingModes.IncludeStackTrace;
	});
}

// Configure reporting
var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

sumapapBuilder.WithReporting(reporting =>
{
	reporting.ConfigureSinks(sinks =>
	{
		sinks.AddLogger();
	});
});

var app = builder.Build();
app.Run();
```

# ⭐ License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

# 🚩 Contact

- **GitHub**: [muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)
- **Project URL**: [https://github.com/muhammadirwanto-dev/sumapap](https://github.com/muhammadirwanto-dev/sumapap)

# ☕ Support

If you find this project helpful, consider supporting the developer:

<a href="https://www.buymeacoffee.com/muhammadirwanto" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
