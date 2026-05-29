# Sumapap.Reporting.Maui

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Reporting.Maui.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting.Maui/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Reporting.Maui.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting.Maui/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Reporting.Maui` provides a MAUI-specific report sink that displays error reports to users via native alert dialogs. This package extends `Sumapap.Reporting` with mobile-optimized UI error presentation, automatically handling main thread marshalling and respecting reporting modes like Silent and Background.

The package includes:
- **MauiDialogReportSink** — Displays error dialogs using MAUI's `DisplayAlert`
- **Extension methods** — Fluent configuration API for adding the dialog sink
- **Cross-platform support** — Works on iOS, Android, macOS Catalyst, and Windows

## ✨ Why use `Sumapap.Reporting.Maui`?

- **Native Error Dialogs** — Present errors to users with platform-native alert dialogs
- **Smart Filtering** — Automatically respects Silent and Background reporting modes
- **Main Thread Safe** — Handles UI thread marshalling automatically
- **Severity-Based** — Only shows dialogs for Error and Critical severity levels
- **Stack Trace Support** — Optionally includes exception details based on reporting mode
- **Zero Configuration** — Works out of the box with sensible defaults
- **MAUI Native** — Uses MAUI's built-in `DisplayAlert` for consistent UX

## 🚀 Quick start

1. Add the package to your MAUI project:

```bash
dotnet add package Sumapap.Reporting.Maui
```

2. Configure the dialog sink in your MauiProgram.cs:

```csharp
using Sumapap.DependencyInjection;
using Sumapap.Reporting.DependencyInjection;
using Sumapap.Reporting.Maui.DependencyInjection;

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

		// Configure Sumapap reporting
		var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

		sumapapBuilder.WithReporting(reporting =>
		{
			reporting.ConfigureSinks(sinks =>
			{
				sinks.AddDialog();  // Add MAUI dialog sink
			});
		});

		return builder.Build();
	}
}
```

3. Use the reporting service in your view models or services:

```csharp
using Sumapap.Reporting.Abstractions;

public class MainViewModel
{
	private readonly IReportingService _reportingService;

	public MainViewModel(IReportingService reportingService)
	{
		_reportingService = reportingService;
	}

	public async Task LoadDataAsync()
	{
		try
		{
			await FetchDataFromApiAsync();
		}
		catch (Exception ex)
		{
			// Show error dialog to user
			await _reportingService.ReportAsync(ex, ReportSeverity.Error);
		}
	}
}
```

4. The dialog will automatically appear when errors occur:

```
┌────────────────────────────────────┐
│             Error                  │
├────────────────────────────────────┤
│ Failed to load data from server    │
│                                    │
│ [Stack trace if enabled]           │
├────────────────────────────────────┤
│                [OK]                │
└────────────────────────────────────┘
```

## 🛠 Features and usage

### MauiDialogReportSink

The core sink that displays error dialogs:

**Automatic filtering:**
- Only handles reports with severity >= Error
- Skips Silent mode reports (no UI shown)
- Skips Background mode reports (background tasks)
- Includes stack traces when IncludeStackTrace mode is enabled

**Main thread marshalling:**
```csharp
// Sink automatically marshals to main thread
public async Task HandleAsync(
	ReportingModes modes,
	Report report,
	CancellationToken cancellationToken = default)
{
	var page = Application.Current?.Windows[0].Page;
	var message = modes.HasFlag(ReportingModes.IncludeStackTrace) && report.Exception != null
		? $"{report.Message}\n\n{report.Exception}"
		: report.Message;

	await MainThread.InvokeOnMainThreadAsync(() => page?.DisplayAlert(
		"Error",
		message,
		"Ok"));
}
```

### AddDialog extension method

Fluent API for registering the dialog sink:

```csharp
reporting.ConfigureSinks(sinks =>
{
	sinks.AddDialog();
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
		sinks.AddLogger();  // Log all errors
		sinks.AddDialog();  // Show UI for user-facing errors
	});
});
```

**Behavior:**
- Logger sink: Logs all reports (Trace to Critical)
- Dialog sink: Shows UI only for Error and Critical

### Controlling dialog visibility

Use reporting modes to control when dialogs appear:

**Show dialog (default):**
```csharp
// Error with dialog
await _reportingService.ReportAsync(exception, ReportSeverity.Error);
```

**Suppress dialog (silent mode):**
```csharp
// Log but don't show dialog
var report = new Report(exception, severity: ReportSeverity.Error);
await _reportingService.ReportAsync(report); // Internal: uses Silent mode
```

**Background tasks:**
```csharp
// From background task - no dialog
try
{
	await SyncDataAsync();
}
catch (Exception ex)
{
	// Dialog sink automatically skips Background mode reports
	await _reportingService.ReportAsync(ex, ReportSeverity.Error);
}
```

### Stack trace display

Control whether stack traces are shown:

**With stack trace (default):**
```csharp
await _reportingService.ReportAsync(exception);
```

Dialog shows:
```
Error

Network connection failed

System.Net.Http.HttpRequestException: Connection timeout
   at HttpClient.SendAsync(...)
   at DataService.FetchAsync(...)
```

**Without stack trace:**
```csharp
// Configure reporting options
services.Configure<ReportingOptions>(options =>
{
	options.DefaultMode = ReportingModes.None; // No stack trace
});
```

Dialog shows:
```
Error

Network connection failed
```

### Custom error messages

Provide user-friendly messages:

```csharp
try
{
	await SaveProfileAsync(profile);
}
catch (ValidationException ex)
{
	// User-friendly message
	var metadata = new ReportMetadata
	{
		["UserId"] = profile.UserId
	};

	await _reportingService.ReportAsync(
		ex,
		ReportSeverity.Warning,
		metadata);
}
catch (Exception ex)
{
	// Generic error message
	var report = new Report(
		exception: ex,
		message: "Unable to save profile. Please try again.",
		severity: ReportSeverity.Error);

	await _reportingService.ReportAsync(report);
}
```

### Severity-based UI

Only Error and Critical reports show dialogs:

```csharp
// No dialog - Trace level
await _reportingService.ReportAsync(ex, ReportSeverity.Trace);

// No dialog - Information level
await _reportingService.ReportAsync(ex, ReportSeverity.Information);

// No dialog - Warning level
await _reportingService.ReportAsync(ex, ReportSeverity.Warning);

// ✓ Shows dialog - Error level
await _reportingService.ReportAsync(ex, ReportSeverity.Error);

// ✓ Shows dialog - Critical level
await _reportingService.ReportAsync(ex, ReportSeverity.Critical);
```

### Network error handling example

```csharp
public class DataService
{
	private readonly IReportingService _reportingService;
	private readonly HttpClient _httpClient;

	public DataService(
		IReportingService reportingService,
		HttpClient httpClient)
	{
		_reportingService = reportingService;
		_httpClient = httpClient;
	}

	public async Task<List<Item>> GetItemsAsync()
	{
		try
		{
			var response = await _httpClient.GetAsync("api/items");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<List<Item>>();
		}
		catch (HttpRequestException ex)
		{
			// User-friendly network error
			var report = new Report(
				exception: ex,
				message: "Unable to connect to the server. Please check your internet connection.",
				severity: ReportSeverity.Error);

			await _reportingService.ReportAsync(report);

			return new List<Item>(); // Return empty list
		}
		catch (TaskCanceledException ex)
		{
			var report = new Report(
				exception: ex,
				message: "The request timed out. Please try again.",
				severity: ReportSeverity.Warning);

			await _reportingService.ReportAsync(report);

			return new List<Item>();
		}
	}
}
```

### Form validation example

```csharp
public class RegistrationViewModel
{
	private readonly IReportingService _reportingService;
	private readonly IUserService _userService;

	public async Task RegisterAsync(RegistrationModel model)
	{
		try
		{
			ValidateModel(model);
			await _userService.RegisterAsync(model);
			await _navigationService.NavigateToAsync<SuccessView>();
		}
		catch (ValidationException ex)
		{
			// Show validation errors in dialog
			var report = new Report(
				exception: ex,
				message: $"Please correct the following:\n\n{ex.Message}",
				severity: ReportSeverity.Warning); // Warning - won't show dialog by default

			await _reportingService.ReportAsync(report);
		}
		catch (DuplicateEmailException ex)
		{
			var report = new Report(
				exception: ex,
				message: "This email address is already registered. Please use a different email or try logging in.",
				severity: ReportSeverity.Error);

			await _reportingService.ReportAsync(report);
		}
		catch (Exception ex)
		{
			var report = new Report(
				exception: ex,
				message: "Registration failed. Please try again later.",
				severity: ReportSeverity.Error);

			await _reportingService.ReportAsync(report);
		}
	}
}
```

### Background task error handling

```csharp
public class SyncService
{
	private readonly IReportingService _reportingService;

	public async Task SyncInBackgroundAsync()
	{
		try
		{
			await PerformSyncAsync();
		}
		catch (Exception ex)
		{
			// Background mode - dialog sink will skip this
			var metadata = new ReportMetadata
			{
				["SyncType"] = "Full",
				["LastSuccessfulSync"] = _lastSyncTime.ToString()
			};

			// Still logged by logger sink, but no dialog shown
			await _reportingService.ReportAsync(ex, ReportSeverity.Error, metadata);
		}
	}

	public async Task SyncOnUserRequestAsync()
	{
		try
		{
			await PerformSyncAsync();
		}
		catch (Exception ex)
		{
			// User-initiated - show dialog
			var report = new Report(
				exception: ex,
				message: "Sync failed. Please check your connection and try again.",
				severity: ReportSeverity.Error);

			await _reportingService.ReportAsync(report);
		}
	}
}
```

### Complete MAUI app example

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Configure services
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddSingleton<IDataService, DataService>();
		builder.Services.AddTransient<MainViewModel>();

		// Configure Sumapap reporting
		var sumapapBuilder = SumapapServiceBuilder.Create(builder.Services);

		sumapapBuilder.WithReporting(reporting =>
		{
			reporting.ConfigureSinks(sinks =>
			{
				sinks.AddLogger();  // Log all reports
				sinks.AddDialog();  // Show dialogs for errors
			});

			// Optional: Configure reporting options
			reporting.Services.Configure<ReportingOptions>(options =>
			{
				options.DefaultMode = ReportingModes.IncludeStackTrace;
			});
		});

		return builder.Build();
	}
}

// MainPage.xaml.cs
public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadDataAsync();
	}
}

// MainViewModel.cs
public class MainViewModel : ObservableObject
{
	private readonly IReportingService _reportingService;
	private readonly IDataService _dataService;

	public MainViewModel(
		IReportingService reportingService,
		IDataService dataService)
	{
		_reportingService = reportingService;
		_dataService = dataService;
	}

	public async Task LoadDataAsync()
	{
		try
		{
			var data = await _dataService.GetDataAsync();
			// Update UI
		}
		catch (Exception ex)
		{
			// Dialog automatically shown by MauiDialogReportSink
			await _reportingService.ReportAsync(
				ex,
				ReportSeverity.Error);
		}
	}
}
```

## ⚠️ Notes & best practices

### Platform support
- Supports Android, iOS, macOS Catalyst, and Windows
- Requires MAUI 8.0 or later
- Uses platform-native alert dialogs via MAUI

### Main thread requirements
- Dialog sink automatically marshals to main thread
- No manual thread handling needed
- Safe to call from any thread or async context

### Dialog behavior
- Only one dialog shown at a time (MAUI limitation)
- Dialogs are modal and block user interaction
- User must dismiss dialog before continuing
- Consider rate limiting for repeated errors

### Severity guidelines
- **Trace/Information/Warning**: No dialog (logged only)
- **Error**: Standard error dialog
- **Critical**: Critical error dialog (same appearance as Error in MAUI)

### User experience
- Keep error messages concise and actionable
- Avoid technical jargon in user-facing messages
- Include next steps or suggested actions
- Consider localization for multi-language apps

### Testing
- Mock `IReportingService` in unit tests
- Test error scenarios in UI tests
- Verify dialogs appear for user-initiated actions
- Verify dialogs are suppressed for background tasks

### Performance
- Dialogs are async and don't block background work
- Stack trace formatting is minimal overhead
- No caching or batching needed for typical usage

### Error message localization

```csharp
// Resources/AppResources.resx
public class AppResources
{
	public static string NetworkError => "Unable to connect to the server";
	public static string TimeoutError => "The request timed out";
	public static string GenericError => "An unexpected error occurred";
}

// Usage
var report = new Report(
	exception: ex,
	message: AppResources.NetworkError,
	severity: ReportSeverity.Error);

await _reportingService.ReportAsync(report);
```

### Custom dialog sink

Create a custom sink for more control:

```csharp
public class CustomDialogReportSink : IReportSink
{
	public bool CanHandle(ReportingModes modes, Report report)
	{
		return report.Severity >= ReportSeverity.Error
			&& !modes.HasFlag(ReportingModes.Silent)
			&& !modes.HasFlag(ReportingModes.Background);
	}

	public async Task HandleAsync(
		ReportingModes modes,
		Report report,
		CancellationToken cancellationToken = default)
	{
		var title = report.Severity == ReportSeverity.Critical
			? "Critical Error"
			: "Error";

		var message = modes.HasFlag(ReportingModes.IncludeStackTrace)
			? $"{report.Message}\n\nDetails:\n{report.Exception}"
			: report.Message;

		var action = modes.HasFlag(ReportingModes.UserActionRequired)
			? "Retry"
			: "OK";

		var page = Application.Current?.Windows[0].Page;

		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			if (modes.HasFlag(ReportingModes.UserActionRequired))
			{
				var retry = await page?.DisplayAlert(title, message, action, "Cancel");
				if (retry)
				{
					// Handle retry logic
				}
			}
			else
			{
				await page?.DisplayAlert(title, message, action);
			}
		});
	}
}

// Register custom sink
reporting.ConfigureSinks(sinks =>
{
	sinks.Services.AddSingleton<IReportSink, CustomDialogReportSink>();
});
```

### Integration with application lifecycle

```csharp
public partial class App : Application
{
	private readonly IReportingService _reportingService;

	public App(IReportingService reportingService)
	{
		InitializeComponent();
		_reportingService = reportingService;

		// Handle unhandled exceptions
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

		MainPage = new AppShell();
	}

	private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (e.ExceptionObject is Exception ex)
		{
			_reportingService.Report(ex, ReportSeverity.Critical);
		}
	}

	private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
	{
		_reportingService.Report(e.Exception, ReportSeverity.Critical);
		e.SetObserved();
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
