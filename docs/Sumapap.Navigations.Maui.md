# Sumapap.Navigations.Maui

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Navigations.Maui.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Navigations.Maui/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Navigations.Maui.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Navigations.Maui/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Navigations.Maui` is a concrete implementation of the `Sumapap.Navigations` abstractions specifically designed for .NET MAUI applications. It provides two powerful navigation adapters that work seamlessly with MAUI's navigation systems:

- **PageNavigationAdapter** — Traditional page-based navigation with modal support
- **ShellNavigationAdapter** — Modern Shell-based navigation with route patterns

This package eliminates boilerplate navigation code, provides type-safe parameter passing, and supports both animated and non-animated transitions across all MAUI platforms (Android, iOS, macOS, Windows).

## ✨ Why use `Sumapap.Navigations.Maui`?

- **Zero Boilerplate** — No need to manually implement `INavigationService`
- **Dual Navigation Modes** — Choose between Page-based or Shell-based navigation
- **Type-Safe Parameters** — Strongly-typed parameter objects with compile-time safety
- **Modal Support** — Built-in support for modal presentations
- **Animation Control** — Enable or disable navigation animations per operation
- **Query Parameters** — Pass complex data via Shell query parameters or Page binding context
- **Route Management** — Support for relative, absolute, and stack-clearing Shell routes
- **Cross-Platform** — Works identically across Android, iOS, macOS, and Windows

## 🚀 Quick start

### 1. Installation

```bash
dotnet add package Sumapap.Navigations.Maui
```

### 2. Register navigation services

Choose between Page-based or Shell-based navigation:

```csharp
// In MauiProgram.cs
using Sumapap.Navigations.Maui.DependencyInjection;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.Services
            .AddSumapap()
            .AddNavigations(nav =>
            {
                // Option 1: Use Page-based navigation
                nav.UsePageNavigation();

                // Option 2: Use Shell-based navigation
                // nav.UseShellNavigation();
            });

        // Register your pages/views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DetailPage>();

        return builder.Build();
    }
}
```

### 3. Use navigation in your ViewModels

```csharp
using Sumapap.Navigations.Abstractions;
using Sumapap.Navigations.Maui.Models;

public class MainViewModel
{
    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    // Simple navigation
    public async Task NavigateToDetailAsync()
    {
        await _navigationService.NavigateToAsync<DetailPage>();
    }

    // Navigation with parameters
    public async Task NavigateWithDataAsync(int itemId)
    {
        var parameters = PageNavigationParams.Create(
            ("ItemId", itemId),
            ("Title", "Item Details")
        );

        await _navigationService.NavigateToAsync<DetailPage>(parameters);
    }

    // Modal navigation
    public async Task ShowModalAsync()
    {
        var parameters = PageNavigationParams.Create(
            PageNavigationMode.Modal,
            ("ShowCloseButton", true)
        );

        await _navigationService.NavigateToAsync<ModalPage>(parameters);
    }
}
```

## 🛠 Features and usage

### PageNavigationAdapter

Traditional page-based navigation using MAUI's `INavigation` interface. Supports both normal (push/pop) and modal presentations.

#### Basic navigation

```csharp
using Sumapap.Navigations.Abstractions;
using Sumapap.Navigations.Maui.Models;

// Navigate forward
await _navigationService.NavigateToAsync<SettingsPage>();

// Navigate back
await _navigationService.BackAsync();

// Pop to root
await _navigationService.NavigateToRootAsync<HomePage>();
```

#### Modal navigation

```csharp
// Show modal page
var modalParams = PageNavigationParams.Create(
    PageNavigationMode.Modal,
    ("Data", someData)
);
await _navigationService.NavigateToAsync<ModalPage>(modalParams);

// Dismiss modal
var resultParams = PageNavigationParams.Create(
    PageNavigationMode.Modal,
    ("Result", "Saved")
);
await _navigationService.BackAsync(resultParams);
```

#### Passing parameters

```csharp
// Create parameters with query data
var parameters = PageNavigationParams.Create(
    ("UserId", 123),
    ("UserName", "John Doe"),
    ("IsAdmin", true)
);

await _navigationService.NavigateToAsync<UserDetailPage>(parameters);

// The parameters are automatically bound to the target page's BindingContext properties
// UserDetailViewModel.UserId will be set to 123
// UserDetailViewModel.UserName will be set to "John Doe"
// UserDetailViewModel.IsAdmin will be set to true
```

#### Animation control

```csharp
// Disable animation for specific navigation
var params = new PageNavigationParams
{
    Animated = false,
    Query = new NavigationQuery
    {
        ["ItemId"] = 456
    }
};

await _navigationService.NavigateToAsync<DetailPage>(params);
```

#### PageNavigationParams API

```csharp
// Empty parameters (animated by default)
var empty = PageNavigationParams.Empty;

// With query parameters
var withQuery = PageNavigationParams.Create(
    ("Key1", "Value1"),
    ("Key2", 42)
);

// With navigation mode
var modal = PageNavigationParams.Create(
    PageNavigationMode.Modal,
    ("Data", someObject)
);

// Full control
var custom = new PageNavigationParams
{
    Mode = PageNavigationMode.Normal,
    Animated = false,
    Query = new NavigationQuery
    {
        ["Param1"] = value1,
        ["Param2"] = value2
    }
};
```

### ShellNavigationAdapter

Modern Shell-based navigation using MAUI's `Shell.GoToAsync` routing. Supports relative, absolute, and hierarchical route patterns.

#### Basic navigation

```csharp
using Sumapap.Navigations.Abstractions;
using Sumapap.Navigations.Maui.Models;

// Navigate forward (relative)
await _navigationService.NavigateToAsync<DetailPage>();

// Navigate back
await _navigationService.BackAsync();

// Navigate to root
await _navigationService.NavigateToRootAsync<MainPage>();
```

#### Route patterns

```csharp
// Relative navigation (push onto current stack)
var relative = ShellNavigationParams.Create(
    ShellNavigationParams.Relative,  // "" - appends to current route
    ("Id", 123)
);
await _navigationService.NavigateToAsync<DetailPage>(relative);

// Absolute navigation (from app root)
var absolute = ShellNavigationParams.Create(
    ShellNavigationParams.Absolute,  // "//" - starts from root
    ("Tab", "Settings")
);
await _navigationService.NavigateToAsync<SettingsPage>(absolute);

// Absolute with stack clear
var clearStack = new ShellNavigationParams
{
    ShellRoute = ShellNavigationParams.AbsoluteClearStack,  // "///" - clears entire stack
    Query = new NavigationQuery { ["Reset"] = true }
};
await _navigationService.NavigateToAsync<LoginPage>(clearStack);
```

#### Query parameters

```csharp
// Shell automatically serializes query parameters to the URL
var parameters = ShellNavigationParams.Create(
    ("ProductId", 789),
    ("Category", "Electronics"),
    ("ShowReviews", true)
);

await _navigationService.NavigateToAsync<ProductPage>(parameters);
// Results in route: ProductPage?ProductId=789&Category=Electronics&ShowReviews=true
```

#### NavigateToRoot behavior

```csharp
// When using NavigateToRootAsync with ShellNavigationAdapter,
// the route is automatically converted to absolute if not already
await _navigationService.NavigateToRootAsync<HomePage>();
// Equivalent to: Shell.GoToAsync("//HomePage")
```

#### ShellNavigationParams API

```csharp
// Empty parameters (relative, animated)
var empty = ShellNavigationParams.Empty;

// With query parameters (relative by default)
var withQuery = ShellNavigationParams.Create(
    ("Key1", "Value1"),
    ("Key2", 42)
);

// With specific route pattern
var absolute = ShellNavigationParams.Create(
    ShellNavigationParams.Absolute,
    ("Data", someValue)
);

// Full control
var custom = new ShellNavigationParams
{
    ShellRoute = ShellNavigationParams.RelativeTo,  // "/"
    Animated = false,
    Query = new NavigationQuery
    {
        ["Param1"] = value1,
        ["Param2"] = value2
    }
};

// Route constants
ShellNavigationParams.Relative              // "" - relative to current
ShellNavigationParams.RelativeTo            // "/" - relative to parent
ShellNavigationParams.Absolute              // "//" - absolute from root
ShellNavigationParams.AbsoluteClearStack    // "///" - absolute + clear stack
```

### MauiNavigationParams

Base class for all MAUI navigation parameters, providing common animation control:

```csharp
public abstract record MauiNavigationParams : INavigationParams
{
    public bool Animated { get; init; } = true;
}
```

Both `PageNavigationParams` and `ShellNavigationParams` inherit from this base class.

### NavigationQuery

Dictionary-based container for navigation parameters:

```csharp
var query = new NavigationQuery
{
    ["UserId"] = 123,
    ["UserName"] = "Alice",
    ["Timestamp"] = DateTime.UtcNow
};

// Or use collection initializer in params
var parameters = PageNavigationParams.Create(
    ("Key1", value1),
    ("Key2", value2),
    ("Key3", value3)
);
```

### PageNavigationMode

Enum for specifying page presentation mode:

```csharp
public enum PageNavigationMode
{
    Normal,  // Standard push/pop navigation
    Modal    // Modal presentation (overlays current page)
}
```

## 📋 Complete examples

### Example 1: Master-Detail with Page navigation

```csharp
// MainViewModel.cs
public class MainViewModel
{
    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public async Task ViewItemDetailsAsync(int itemId)
    {
        var parameters = PageNavigationParams.Create(
            ("ItemId", itemId)
        );

        await _navigationService.NavigateToAsync<DetailPage>(parameters);
    }
}

// DetailViewModel.cs
public class DetailViewModel
{
    public int ItemId { get; set; }  // Automatically populated by NavigationQuery

    public async Task GoBackAsync()
    {
        await _navigationService.BackAsync();
    }
}
```

### Example 2: Shell navigation with tabs

```csharp
// AppShell.xaml.cs - Register routes
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
        Routing.RegisterRoute(nameof(EditPage), typeof(EditPage));
    }
}

// ViewModel
public async Task NavigateToEditAsync(int itemId)
{
    var parameters = ShellNavigationParams.Create(
        ShellNavigationParams.Relative,
        ("ItemId", itemId),
        ("Mode", "Edit")
    );

    await _navigationService.NavigateToAsync<EditPage>(parameters);
}

// Navigate to different tab (absolute)
public async Task SwitchToSettingsTabAsync()
{
    var parameters = new ShellNavigationParams
    {
        ShellRoute = ShellNavigationParams.Absolute
    };

    await _navigationService.NavigateToAsync<SettingsPage>(parameters);
}
```

### Example 3: Modal form with result

```csharp
// Show modal form
public async Task ShowAddItemModalAsync()
{
    var parameters = PageNavigationParams.Create(
        PageNavigationMode.Modal,
        ("Title", "Add New Item")
    );

    await _navigationService.NavigateToAsync<AddItemPage>(parameters);
}

// In AddItemViewModel - save and dismiss
public async Task SaveAndCloseAsync()
{
    // Save logic...

    var result = PageNavigationParams.Create(
        PageNavigationMode.Modal,
        ("ItemAdded", true),
        ("NewItemId", newItemId)
    );

    await _navigationService.BackAsync(result);
}
```

### Example 4: Multi-level navigation

```csharp
// Navigate deep into hierarchy
public async Task NavigateToCategoryProductAsync(string category, int productId)
{
    // First navigate to category
    var categoryParams = PageNavigationParams.Create(
        ("Category", category)
    );
    await _navigationService.NavigateToAsync<CategoryPage>(categoryParams);

    // Then navigate to product
    var productParams = PageNavigationParams.Create(
        ("ProductId", productId)
    );
    await _navigationService.NavigateToAsync<ProductPage>(productParams);
}

// Reset to home
public async Task ResetToHomeAsync()
{
    await _navigationService.NavigateToRootAsync<HomePage>();
}
```

## 🎯 Best practices

### Page/View registration

```csharp
// Register pages as Transient (new instance per navigation)
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<DetailPage>();

// Register ViewModels as Transient
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<DetailViewModel>();
```

### Shell route registration

```csharp
// In AppShell.xaml.cs
Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
Routing.RegisterRoute("products/detail", typeof(ProductDetailPage));
Routing.RegisterRoute("settings/profile", typeof(ProfilePage));
```

### ViewModel initialization

```csharp
// Use properties for navigation parameters (auto-populated)
public class DetailViewModel
{
    public int ItemId { get; set; }
    public string? Title { get; set; }

    // Perform initialization in constructor or property setters
    public DetailViewModel()
    {
        // Don't load data here - properties not set yet
    }

    // Option 1: React to property changes
    private int _itemId;
    public int ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            LoadItemAsync(value);  // Load when parameter is set
        }
    }

    // Option 2: Use INavigationAware (custom interface)
    public void OnNavigatedTo(INavigationParams? parameters)
    {
        LoadItemAsync(ItemId);
    }
}
```

### Error handling

```csharp
public async Task NavigateWithErrorHandlingAsync()
{
    try
    {
        var parameters = PageNavigationParams.Create(("Id", 123));
        await _navigationService.NavigateToAsync<DetailPage>(parameters);
    }
    catch (InvalidOperationException ex)
    {
        // Page not registered
        await ShowErrorAsync("Navigation failed: " + ex.Message);
    }
    catch (InvalidCastException ex)
    {
        // Wrong parameter type
        await ShowErrorAsync("Invalid parameters: " + ex.Message);
    }
}
```

### Thread safety

```csharp
// MAUI navigation must run on main thread
// MauiNavigationService handles this automatically
await _navigationService.NavigateToAsync<DetailPage>();

// If implementing custom adapter, use:
await MainThread.InvokeOnMainThreadAsync(async () =>
{
    await Shell.Current.GoToAsync("DetailPage");
});
```

## 🔍 Advanced scenarios

### Custom navigation adapter

```csharp
public class CustomNavigationAdapter : INavigationAdapter
{
    public bool CanHandle() => /* your condition */;

    public Task NavigateToAsync<TView>(CancellationToken cancellationToken = default)
    {
        // Your custom implementation
        return Task.CompletedTask;
    }

    // Implement other INavigationService methods...
}

// Register
builder.Services.AddSumapap()
    .AddNavigations(nav =>
    {
        nav.Services.AddSingleton<INavigationAdapter, CustomNavigationAdapter>();
    });
```

### Mixing navigation modes

```csharp
// Use different adapters in different parts of your app
// Register multiple adapters and choose at runtime

public class HybridNavigationService : INavigationService
{
    private readonly IEnumerable<INavigationAdapter> _adapters;

    public HybridNavigationService(IEnumerable<INavigationAdapter> adapters)
    {
        _adapters = adapters;
    }

    public Task NavigateToAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
    {
        var adapter = _adapters.FirstOrDefault(a => a.CanHandle())
            ?? throw new InvalidOperationException("No suitable adapter found");

        return adapter.NavigateToAsync<TView>(param, cancellationToken);
    }
}
```

## 🔧 Troubleshooting

### Page not found exception

```
NullReferenceException: Page DetailPage not registered
```

**Solution:** Register the page in DI container:
```csharp
builder.Services.AddTransient<DetailPage>();
```

### InvalidCastException

```
InvalidCastException: Cannot cast INavigationParams to PageNavigationParams
```

**Solution:** Use correct parameter type for the adapter:
```csharp
// For PageNavigationAdapter, use PageNavigationParams
var pageParams = PageNavigationParams.Create(("Id", 123));

// For ShellNavigationAdapter, use ShellNavigationParams
var shellParams = ShellNavigationParams.Create(("Id", 123));
```

### Shell route not found

```
Shell: route not found: DetailPage
```

**Solution:** Register the route in AppShell:
```csharp
Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
```

### Parameters not binding

```
Properties in ViewModel remain null after navigation
```

**Solution:** Ensure property names match exactly:
```csharp
// Navigation
var params = PageNavigationParams.Create(("UserId", 123));

// ViewModel - property name must match "UserId"
public int UserId { get; set; }  // ✓ Correct
public int userid { get; set; }  // ✗ Wrong case
public int ID { get; set; }      // ✗ Wrong name
```

## ⚙️ Configuration

### Choose navigation adapter

```csharp
// Page-based navigation (traditional)
builder.Services.AddSumapap()
    .AddNavigations(nav => nav.UsePageNavigation());

// Shell-based navigation (modern)
builder.Services.AddSumapap()
    .AddNavigations(nav => nav.UseShellNavigation());
```

### Default animation setting

Animation is enabled by default. Disable per-navigation:

```csharp
var params = new PageNavigationParams { Animated = false };
await _navigationService.NavigateToAsync<DetailPage>(params);
```

## 📚 Related packages

- [Sumapap.Navigations](https://www.nuget.org/packages/Sumapap.Navigations/) — Core navigation abstractions
- [Sumapap.DependencyInjection](https://www.nuget.org/packages/Sumapap.DependencyInjection/) — DI utilities and builders

## ⭐ License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

## 🚩 Contact

- **GitHub**: [muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)
- **Project URL**: [https://github.com/muhammadirwanto-dev/sumapap](https://github.com/muhammadirwanto-dev/sumapap)

## ☕ Support

If you find this project helpful, consider supporting the developer:

<a href="https://www.buymeacoffee.com/muhammadirwanto" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
