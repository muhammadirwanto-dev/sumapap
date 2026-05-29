# Sumapap.Navigations

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Navigations.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Navigations/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Navigations.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Navigations/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Navigations` provides a clean abstraction layer for navigation functionality in your applications. This lightweight library defines interfaces and contracts for navigating between pages or views, passing parameters, and managing navigation stacks, enabling you to build navigation solutions that are framework-agnostic and testable.

The package includes:
- **INavigationService** — Core interface for navigation operations
- **INavigationParams** — Marker interface for type-safe parameter passing
- **NavigationPresentation** — Enum for specifying presentation modes (Normal/Modal)

## ✨ Why use `Sumapap.Navigations`?

- **Framework-Agnostic** — Abstractions that work across different UI frameworks (MAUI, WPF, Avalonia, etc.)
- **Testable** — Mock navigation behavior easily in unit tests
- **Type-Safe** — Strongly-typed parameter objects prevent runtime errors
- **Clean Architecture** — Decouple navigation logic from view implementation
- **Async-First** — All navigation operations support async/await with cancellation tokens
- **Presentation Control** — Specify how views should be presented (normal push or modal)

## 🚀 Quick start

1. Add the package to your project:

```bash
dotnet add package Sumapap.Navigations
```

2. Define a parameter object for navigation:

```csharp
using Sumapap.Navigations.Abstractions;

public record UserDetailParams(int UserId) : INavigationParams;
```

3. Inject and use the navigation service:

```csharp
using Sumapap.Navigations.Abstractions;

public class MainViewModel
{
	private readonly INavigationService _navigationService;

	public MainViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public async Task NavigateToUserDetailAsync(int userId)
	{
		var parameters = new UserDetailParams(userId);
		await _navigationService.NavigateToAsync<UserDetailView>(parameters);
	}
}
```

4. Implement the navigation service in your UI layer:

```csharp
// Example implementation for MAUI
public class MauiNavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;

	public MauiNavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task NavigateToAsync<TView>(CancellationToken cancellationToken = default)
	{
		var view = _serviceProvider.GetRequiredService<TView>();
		await Shell.Current.Navigation.PushAsync((Page)(object)view!);
	}

	public async Task NavigateToAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
	{
		var view = _serviceProvider.GetRequiredService<TView>();

		// Pass parameters to the view or view model
		if (view is Page page && page.BindingContext is INavigationAware aware)
		{
			aware.OnNavigatedTo(param);
		}

		await Shell.Current.Navigation.PushAsync(page);
	}

	public async Task BackAsync(CancellationToken cancellationToken = default)
	{
		await Shell.Current.Navigation.PopAsync();
	}

	public async Task BackAsync(INavigationParams param, CancellationToken cancellationToken = default)
	{
		await Shell.Current.Navigation.PopAsync();
	}

	public async Task NavigateToRootAsync<TView>(CancellationToken cancellationToken = default)
	{
		await Shell.Current.Navigation.PopToRootAsync();
		await NavigateToAsync<TView>(cancellationToken);
	}

	public async Task NavigateToRootAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
	{
		await Shell.Current.Navigation.PopToRootAsync();
		await NavigateToAsync<TView>(param, cancellationToken);
	}
}
```

5. Register the service in your DI container:

```csharp
services.AddSingleton<INavigationService, MauiNavigationService>();
```

## 🛠 Features and usage

### INavigationService

The core navigation interface with comprehensive navigation operations:

```csharp
public interface INavigationService
{
	// Navigate forward without parameters
	Task NavigateToAsync<TView>(CancellationToken cancellationToken = default);

	// Navigate forward with parameters
	Task NavigateToAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default);

	// Navigate backward without parameters
	Task BackAsync(CancellationToken cancellationToken = default);

	// Navigate backward with parameters
	Task BackAsync(INavigationParams param, CancellationToken cancellationToken = default);

	// Navigate to root and then to a specific view
	Task NavigateToRootAsync<TView>(CancellationToken cancellationToken = default);

	// Navigate to root and then to a specific view with parameters
	Task NavigateToRootAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default);
}
```

**Forward navigation:**
```csharp
// Simple navigation
await _navigationService.NavigateToAsync<SettingsView>();

// Navigation with parameters
var params = new ProductDetailParams(productId: 123);
await _navigationService.NavigateToAsync<ProductDetailView>(params);

// With cancellation support
await _navigationService.NavigateToAsync<OrderView>(cancellationToken);
```

**Backward navigation:**
```csharp
// Go back to previous view
await _navigationService.BackAsync();

// Go back with result parameters
var result = new OrderCompletedParams(orderId: 456, success: true);
await _navigationService.BackAsync(result);
```

**Root navigation:**
```csharp
// Clear stack and navigate to home
await _navigationService.NavigateToRootAsync<HomeView>();

// Clear stack and navigate with parameters
var params = new ResetParams(reason: "Logout");
await _navigationService.NavigateToRootAsync<LoginView>(params);
```

### INavigationParams

Marker interface for creating type-safe parameter objects:

```csharp
// Simple parameter object
public record UserDetailParams(int UserId, string? Tab = null) : INavigationParams;

// Complex parameter object
public record EditProductParams : INavigationParams
{
	public required int ProductId { get; init; }
	public required string Mode { get; init; } // "edit" or "view"
	public bool ShowComments { get; init; }
	public Dictionary<string, object>? AdditionalData { get; init; }
}

// Usage
var editParams = new EditProductParams
{
	ProductId = 789,
	Mode = "edit",
	ShowComments = true,
	AdditionalData = new Dictionary<string, object>
	{
		["source"] = "search",
		["highlight"] = true
	}
};

await _navigationService.NavigateToAsync<ProductEditView>(editParams);
```

### NavigationPresentation

Enum for specifying how the destination should be presented:

```csharp
public enum NavigationPresentation
{
	Normal,  // Standard navigation (push onto stack)
	Modal    // Modal presentation (overlay current view)
}
```

**Usage in custom implementations:**
```csharp
// Example: Extended navigation service with presentation mode
public interface IExtendedNavigationService : INavigationService
{
	Task NavigateToAsync<TView>(
		INavigationParams param,
		NavigationPresentation presentation,
		CancellationToken cancellationToken = default);
}

public class MauiExtendedNavigationService : IExtendedNavigationService
{
	public async Task NavigateToAsync<TView>(
		INavigationParams param,
		NavigationPresentation presentation,
		CancellationToken cancellationToken = default)
	{
		var view = GetView<TView>();

		if (presentation == NavigationPresentation.Modal)
		{
			await Shell.Current.Navigation.PushModalAsync(view);
		}
		else
		{
			await Shell.Current.Navigation.PushAsync(view);
		}
	}
}
```

### Navigation lifecycle integration

Create an interface for views that need navigation lifecycle notifications:

```csharp
public interface INavigationAware
{
	void OnNavigatedTo(INavigationParams? parameters);
	void OnNavigatedFrom();
}

// ViewModel implementation
public class ProductDetailViewModel : INavigationAware
{
	public void OnNavigatedTo(INavigationParams? parameters)
	{
		if (parameters is ProductDetailParams productParams)
		{
			LoadProduct(productParams.ProductId);
		}
	}

	public void OnNavigatedFrom()
	{
		// Cleanup resources
	}
}
```

### Testing navigation

Mock the navigation service for unit tests:

```csharp
using Moq;
using Xunit;

public class MainViewModelTests
{
	[Fact]
	public async Task NavigateToUserDetail_CallsNavigationService()
	{
		// Arrange
		var mockNavigation = new Mock<INavigationService>();
		var viewModel = new MainViewModel(mockNavigation.Object);

		// Act
		await viewModel.NavigateToUserDetailAsync(userId: 42);

		// Assert
		mockNavigation.Verify(
			x => x.NavigateToAsync<UserDetailView>(
				It.Is<UserDetailParams>(p => p.UserId == 42),
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Fact]
	public async Task GoBack_CallsBackAsync()
	{
		// Arrange
		var mockNavigation = new Mock<INavigationService>();
		var viewModel = new DetailViewModel(mockNavigation.Object);

		// Act
		await viewModel.GoBackAsync();

		// Assert
		mockNavigation.Verify(
			x => x.BackAsync(It.IsAny<CancellationToken>()),
			Times.Once);
	}
}
```

### Dependency injection patterns

**Register views for navigation:**
```csharp
// Register all views as transient (new instance per navigation)
services.AddTransient<HomeView>();
services.AddTransient<UserDetailView>();
services.AddTransient<ProductListView>();
services.AddTransient<SettingsView>();

// Register navigation service
services.AddSingleton<INavigationService, MauiNavigationService>();
```

**Advanced registration with factory:**
```csharp
services.AddSingleton<INavigationService>(sp =>
{
	var logger = sp.GetRequiredService<ILogger<MauiNavigationService>>();
	return new MauiNavigationService(sp, logger);
});
```

## ⚠️ Notes & best practices

### Implementation requirements
- Implementations must handle thread safety for navigation operations
- Use `ConfigureAwait(false)` in library code to avoid deadlocks
- Ensure UI operations are executed on the main thread (use framework-specific dispatchers)

### Parameter design
- Use records for immutable parameter objects
- Keep parameter objects focused and cohesive
- Consider using optional parameters with default values for flexibility
- Avoid putting business logic in parameter objects

### Navigation patterns
- Prefer parameterless overloads when no data needs to be passed
- Use `NavigateToRootAsync` for clearing navigation history (logout, reset scenarios)
- Return navigation results using `BackAsync(INavigationParams)` for callback scenarios
- Consider implementing a navigation history service for complex navigation flows

### Cancellation support
- Always pass cancellation tokens to navigation methods in long-running operations
- Implementations should respect cancellation tokens and cancel animations/transitions
- Test cancellation scenarios in your implementations

### Cross-platform considerations
- Different UI frameworks handle navigation differently (stack, modal, tabs)
- Implementations should adapt to platform-specific navigation patterns
- Consider creating framework-specific implementations:
  - `Sumapap.Navigations.Maui`
  - `Sumapap.Navigations.Wpf`
  - `Sumapap.Navigations.Avalonia`

### Security
- Validate navigation parameters in receiving views/view models
- Don't pass sensitive data (passwords, tokens) through navigation parameters
- Consider encrypting sensitive navigation state if needed

### Performance
- Register views with appropriate lifetime (typically Transient)
- Cache navigation routes/paths in implementations
- Avoid heavy initialization in view constructors (use `OnNavigatedTo` instead)

### Example: Complete MAUI implementation

```csharp
public class MauiNavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<MauiNavigationService> _logger;

	public MauiNavigationService(
		IServiceProvider serviceProvider,
		ILogger<MauiNavigationService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	public async Task NavigateToAsync<TView>(CancellationToken cancellationToken = default)
	{
		await NavigateToAsync<TView>(null!, cancellationToken);
	}

	public async Task NavigateToAsync<TView>(
		INavigationParams param,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var view = _serviceProvider.GetRequiredService<TView>() as Page;

			if (view is null)
			{
				throw new InvalidOperationException($"{typeof(TView).Name} must inherit from Page");
			}

			NotifyNavigationLifecycle(view, param);

			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				await Shell.Current.Navigation.PushAsync(view, animated: true);
			});

			_logger.LogInformation("Navigated to {ViewName}", typeof(TView).Name);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Navigation to {ViewName} failed", typeof(TView).Name);
			throw;
		}
	}

	public async Task BackAsync(CancellationToken cancellationToken = default)
	{
		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			await Shell.Current.Navigation.PopAsync(animated: true);
		});
	}

	public async Task BackAsync(INavigationParams param, CancellationToken cancellationToken = default)
	{
		var previousView = Shell.Current.Navigation.NavigationStack[^2];
		NotifyNavigationLifecycle(previousView, param);
		await BackAsync(cancellationToken);
	}

	public async Task NavigateToRootAsync<TView>(CancellationToken cancellationToken = default)
	{
		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			await Shell.Current.Navigation.PopToRootAsync(animated: true);
		});
		await NavigateToAsync<TView>(cancellationToken);
	}

	public async Task NavigateToRootAsync<TView>(
		INavigationParams param,
		CancellationToken cancellationToken = default)
	{
		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			await Shell.Current.Navigation.PopToRootAsync(animated: true);
		});
		await NavigateToAsync<TView>(param, cancellationToken);
	}

	private void NotifyNavigationLifecycle(Page page, INavigationParams? parameters)
	{
		if (page.BindingContext is INavigationAware aware)
		{
			aware.OnNavigatedTo(parameters);
		}
	}
}

// Optional: Navigation aware interface
public interface INavigationAware
{
	void OnNavigatedTo(INavigationParams? parameters);
	void OnNavigatedFrom();
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
