# Sumapap.Mvvm.Maui

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Mvvm.Maui.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Mvvm.Maui/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Mvvm.Maui.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Mvvm.Maui/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Mvvm.Maui` provides MAUI-specific utilities and extensions for implementing the MVVM (Model-View-ViewModel) architectural pattern in .NET MAUI applications. Built on top of [Sumapap.Mvvm](https://www.nuget.org/packages/Sumapap.Mvvm/) and [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/), this package offers seamless integration with .NET MAUI's dependency injection system and provides convenient access to platform services.

The package includes:
- **MauiServiceProvider** — Static accessor for the current MAUI application's service provider
- Platform-specific initialization hooks for Android, iOS, macOS Catalyst, and Windows
- Full compatibility with all Sumapap.Mvvm base classes and features

## ✨ Why use `Sumapap.Mvvm.Maui`?

- **Seamless MAUI Integration** — Direct access to MAUI's `IServiceProvider` anywhere in your code
- **Cross-Platform** — Works identically across Android, iOS, macOS, and Windows
- **Dependency Injection Ready** — Easily resolve services from the MAUI service container
- **Built on Sumapap.Mvvm** — All base view model classes available (ViewModelBase, RecipientViewModelBase, InteractiveViewModelBase)
- **CommunityToolkit.Mvvm Support** — Full support for source generators and MVVM toolkit features
- **Zero Boilerplate** — Minimal configuration required to get started

## 🚀 Quick start

1. Add the package to your MAUI project:

```bash
dotnet add package Sumapap.Mvvm.Maui
```

2. No additional configuration needed! The package automatically integrates with MAUI's dependency injection system.

3. Access the service provider anywhere in your application:

```csharp
using Sumapap.Mvvm.Maui;

// Get the current service provider
var serviceProvider = MauiServiceProvider.Current;

// Resolve services
var navigationService = serviceProvider.GetRequiredService<INavigationService>();
var dataService = serviceProvider.GetService<IDataService>();
```

4. Create view models using Sumapap.Mvvm base classes:

```csharp
using Sumapap.Mvvm.ViewModels;
using CommunityToolkit.Mvvm.Input;

public partial class MainViewModel : InteractiveViewModelBase
{
	private readonly IProductService _productService;

	[ObservableProperty]
	private ObservableCollection<Product> _products = [];

	public MainViewModel(IProductService productService)
	{
		_productService = productService;
	}

	[RelayCommand]
	private async Task LoadProductsAsync()
	{
		using (StartScopedRefresh())
		{
			var products = await _productService.GetAllAsync();
			Products = new ObservableCollection<Product>(products);
		}
	}
}
```

5. Use view models in MAUI pages:

```csharp
using Sumapap.Mvvm.Abstractions;

public partial class MainPage : ContentPage, IViewModelOwner<MainViewModel>
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		BindingContext = viewModel;
	}

	public MainViewModel ViewModel { get; }
}
```

## 🛠 Features and usage

### MauiServiceProvider

The `MauiServiceProvider` class provides static access to the current MAUI application's service provider:

```csharp
namespace Sumapap.Mvvm.Maui
{
	public static class MauiServiceProvider
	{
		public static IServiceProvider Current => IPlatformApplication.Current?.Services!;
	}
}
```

#### Basic Usage

```csharp
using Sumapap.Mvvm.Maui;
using Microsoft.Extensions.DependencyInjection;

// Get the service provider
var services = MauiServiceProvider.Current;

// Resolve a required service (throws if not found)
var navigationService = services.GetRequiredService<INavigationService>();

// Resolve an optional service (returns null if not found)
var analyticsService = services.GetService<IAnalyticsService>();

// Resolve multiple services
var validators = services.GetServices<IValidator>();

// Create a scope for scoped services
using (var scope = services.CreateScope())
{
	var scopedService = scope.ServiceProvider.GetRequiredService<IScopedService>();
	// Use scoped service
}
```

#### Use Cases

**1. Service Resolution in Static Contexts:**

```csharp
public static class AppSettings
{
	public static string GetApiUrl()
	{
		var config = MauiServiceProvider.Current
			.GetRequiredService<IConfiguration>();
		return config["ApiUrl"] ?? "https://api.example.com";
	}
}
```

**2. Service Resolution in Extension Methods:**

```csharp
public static class NavigationExtensions
{
	public static Task NavigateToDetailAsync<TDetail>(this ContentPage page, int id)
		where TDetail : ContentPage
	{
		var navigationService = MauiServiceProvider.Current
			.GetRequiredService<INavigationService>();

		return navigationService.NavigateToAsync<TDetail>(
			new DetailParams(id)
		);
	}
}
```

**3. Service Resolution in Converters:**

```csharp
using Sumapap.Mvvm.Maui;
using System.Globalization;

public class ImageUrlConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is string imagePath)
		{
			var imageService = MauiServiceProvider.Current
				.GetRequiredService<IImageService>();
			return imageService.GetFullImageUrl(imagePath);
		}
		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
```

**4. Service Resolution in Custom Controls:**

```csharp
public class CustomButton : Button
{
	private readonly IThemeService _themeService;

	public CustomButton()
	{
		_themeService = MauiServiceProvider.Current
			.GetRequiredService<IThemeService>();

		_themeService.ThemeChanged += OnThemeChanged;
		ApplyTheme();
	}

	private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
	{
		ApplyTheme();
	}

	private void ApplyTheme()
	{
		var theme = _themeService.CurrentTheme;
		BackgroundColor = theme.ButtonBackground;
		TextColor = theme.ButtonText;
	}
}
```

**5. Service Resolution in Behaviors:**

```csharp
using Microsoft.Maui.Controls;
using Sumapap.Mvvm.Maui;

public class ValidationBehavior : Behavior<Entry>
{
	private readonly IValidationService _validationService;

	public ValidationBehavior()
	{
		_validationService = MauiServiceProvider.Current
			.GetRequiredService<IValidationService>();
	}

	protected override void OnAttachedTo(Entry entry)
	{
		base.OnAttachedTo(entry);
		entry.TextChanged += OnTextChanged;
	}

	protected override void OnDetachingFrom(Entry entry)
	{
		entry.TextChanged -= OnTextChanged;
		base.OnDetachingFrom(entry);
	}

	private void OnTextChanged(object? sender, TextChangedEventArgs e)
	{
		if (sender is Entry entry)
		{
			var isValid = _validationService.ValidateEmail(e.NewTextValue);
			entry.TextColor = isValid ? Colors.Black : Colors.Red;
		}
	}
}
```

### Platform-Specific Support

The package includes platform-specific initialization files for all supported MAUI platforms:

- **Android** — `Platforms/Android/PlatformClass1.cs`
- **iOS** — `Platforms/iOS/PlatformClass1.cs`
- **macOS Catalyst** — `Platforms/MacCatalyst/PlatformClass1.cs`
- **Windows** — `Platforms/Windows/PlatformClass1.cs`

These files provide hooks for platform-specific initialization if needed in the future.

### Integration with Sumapap.Mvvm

All Sumapap.Mvvm base classes are available for use in MAUI applications:

#### ViewModelBase Example

```csharp
using Sumapap.Mvvm.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class AboutViewModel : ViewModelBase
{
	[ObservableProperty]
	private string _appVersion = AppInfo.VersionString;

	[ObservableProperty]
	private string _appName = AppInfo.Name;

	[RelayCommand]
	private async Task OpenWebsiteAsync()
	{
		await Browser.OpenAsync("https://example.com", BrowserLaunchMode.SystemPreferred);
	}

	[RelayCommand]
	private async Task ShareAppAsync()
	{
		await Share.RequestAsync(new ShareTextRequest
		{
			Text = $"Check out {AppName}!",
			Title = "Share App"
		});
	}
}
```

#### RecipientViewModelBase Example

```csharp
using Sumapap.Mvvm.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;

public record LocationChangedMessage(Location NewLocation);

public partial class MapViewModel : RecipientViewModelBase
{
	[ObservableProperty]
	private Location? _currentLocation;

	[ObservableProperty]
	private ObservableCollection<MapPin> _pins = [];

	public MapViewModel()
	{
		IsActive = true;
	}

	protected override void OnActivated()
	{
		Messenger.Register<LocationChangedMessage>(this, (r, m) =>
		{
			CurrentLocation = m.NewLocation;
			UpdateMapCenter(m.NewLocation);
		});
	}

	private void UpdateMapCenter(Location location)
	{
		// Update map center logic
	}
}

// Location tracking service sends updates
public class LocationService : ILocationService
{
	private readonly IMessenger _messenger;

	public LocationService(IMessenger messenger)
	{
		_messenger = messenger;
	}

	public void OnLocationChanged(Location newLocation)
	{
		_messenger.Send(new LocationChangedMessage(newLocation));
	}
}
```

#### InteractiveViewModelBase Example

```csharp
using Sumapap.Mvvm.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sumapap.Navigations.Abstractions;

public partial class ProductListViewModel : InteractiveViewModelBase
{
	private readonly IProductService _productService;
	private readonly INavigationService _navigationService;

	[ObservableProperty]
	private ObservableCollection<Product> _products = [];

	[ObservableProperty]
	private Product? _selectedProduct;

	[ObservableProperty]
	private bool _isSearching;

	public ProductListViewModel(
		IProductService productService,
		INavigationService navigationService)
	{
		_productService = productService;
		_navigationService = navigationService;
	}

	[RelayCommand]
	private async Task InitializeAsync()
	{
		await LoadProductsAsync();
	}

	[RelayCommand(CanExecute = nameof(CanRefresh))]
	private async Task RefreshProductsAsync()
	{
		using (StartScopedRefresh())
		{
			var products = await _productService.GetAllAsync();
			Products = new ObservableCollection<Product>(products);
		}
	}

	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task ViewProductDetailAsync(int productId)
	{
		using (StartScopedNavigation())
		{
			await _navigationService.NavigateToAsync<ProductDetailPage>(
				new ProductDetailParams(productId)
			);
		}
	}

	[RelayCommand(CanExecute = nameof(CanUserInteraction))]
	private async Task AddToCartAsync(Product product)
	{
		using (StartScopedUserInteraction())
		{
			await _productService.AddToCartAsync(product);
			await DisplayToastAsync($"{product.Name} added to cart");
		}
	}

	[RelayCommand]
	private async Task SearchProductsAsync(string searchTerm)
	{
		IsSearching = true;
		try
		{
			var results = await _productService.SearchAsync(searchTerm);
			Products = new ObservableCollection<Product>(results);
		}
		finally
		{
			IsSearching = false;
		}
	}

	private async Task LoadProductsAsync()
	{
		using (StartScopedRefresh())
		{
			var products = await _productService.GetAllAsync();
			Products = new ObservableCollection<Product>(products);
		}
	}

	private async Task DisplayToastAsync(string message)
	{
		// Display toast using MAUI CommunityToolkit or platform-specific API
		await Task.CompletedTask;
	}
}
```

## 📋 MAUI application setup

### 1. MauiProgram.cs Configuration

```csharp
using Sumapap.DependencyInjection;
using Sumapap.Navigations.Maui.DependencyInjection;

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

		// Register Sumapap services
		builder.Services
			.AddSumapap()
			.AddNavigations(nav =>
			{
				nav.UsePageNavigation(); // or nav.UseShellNavigation()
			});

		// Register view models
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<ProductListViewModel>();
		builder.Services.AddTransient<ProductDetailViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		// Register pages
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<ProductListPage>();
		builder.Services.AddTransient<ProductDetailPage>();
		builder.Services.AddTransient<SettingsPage>();

		// Register services
		builder.Services.AddSingleton<IProductService, ProductService>();
		builder.Services.AddSingleton<ICartService, CartService>();
		builder.Services.AddSingleton<ISettingsService, SettingsService>();

		return builder.Build();
	}
}
```

### 2. App.xaml.cs

```csharp
using Sumapap.Mvvm.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Resolve the main page from DI
		MainPage = MauiServiceProvider.Current
			.GetRequiredService<AppShell>();
	}
}
```

### 3. Page Implementation

```csharp
using Sumapap.Mvvm.Abstractions;

namespace MyMauiApp.Pages;

[ViewModelOwner<MainViewModel>]
public partial class MainPage : ContentPage, IViewModelOwner<MainViewModel>
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		BindingContext = viewModel;
	}

	public MainViewModel ViewModel { get; }

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Initialize data when page appears
		if (ViewModel.InitializeCommand.CanExecute(null))
		{
			await ViewModel.InitializeCommand.ExecuteAsync(null);
		}
	}
}
```

### 4. XAML Binding

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
			 xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
			 x:Class="MyMauiApp.Pages.MainPage"
			 Title="Products">

	<RefreshView IsRefreshing="{Binding IsRefreshing}"
				 Command="{Binding RefreshProductsCommand}">
		<CollectionView ItemsSource="{Binding Products}"
						SelectionMode="Single"
						SelectedItem="{Binding SelectedProduct}">
			<CollectionView.ItemTemplate>
				<DataTemplate>
					<Grid Padding="10">
						<Grid.RowDefinitions>
							<RowDefinition Height="Auto" />
							<RowDefinition Height="Auto" />
						</Grid.RowDefinitions>
						<Grid.ColumnDefinitions>
							<ColumnDefinition Width="*" />
							<ColumnDefinition Width="Auto" />
						</Grid.ColumnDefinitions>

						<Label Text="{Binding Name}"
							   FontSize="18"
							   FontAttributes="Bold"
							   Grid.Row="0"
							   Grid.Column="0" />

						<Label Text="{Binding Price, StringFormat='{0:C}'}"
							   FontSize="14"
							   TextColor="Gray"
							   Grid.Row="1"
							   Grid.Column="0" />

						<Button Text="Add to Cart"
								Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=ViewModel.AddToCartCommand}"
								CommandParameter="{Binding .}"
								IsEnabled="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=ViewModel.CanUserInteraction}"
								Grid.Row="0"
								Grid.RowSpan="2"
								Grid.Column="1"
								VerticalOptions="Center" />
					</Grid>
				</DataTemplate>
			</CollectionView.ItemTemplate>
		</CollectionView>
	</RefreshView>

</ContentPage>
```

## 🔗 Integration with other Sumapap packages

### With Sumapap.Navigations.Maui

```csharp
using Sumapap.Mvvm.ViewModels;
using Sumapap.Navigations.Abstractions;
using Sumapap.Navigations.Maui.Models;

public partial class HomeViewModel : InteractiveViewModelBase
{
	private readonly INavigationService _navigationService;

	public HomeViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task NavigateToProductsAsync()
	{
		using (StartScopedNavigation())
		{
			await _navigationService.NavigateToAsync<ProductListPage>();
		}
	}

	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task OpenProductDetailAsync(int productId)
	{
		using (StartScopedNavigation())
		{
			var parameters = PageNavigationParams.Create(
				("ProductId", productId)
			);
			await _navigationService.NavigateToAsync<ProductDetailPage>(parameters);
		}
	}
}
```

### With Sumapap.Ddd.Mediator

```csharp
using Sumapap.Mvvm.ViewModels;
using Sumapap.Ddd.Mediator.Abstractions;

public record GetProductsQuery : IQuery<List<Product>>;

public partial class ProductCatalogViewModel : InteractiveViewModelBase
{
	private readonly IMediator _mediator;

	[ObservableProperty]
	private ObservableCollection<Product> _products = [];

	public ProductCatalogViewModel(IMediator mediator)
	{
		_mediator = mediator;
	}

	[RelayCommand]
	private async Task LoadCatalogAsync()
	{
		using (StartScopedRefresh())
		{
			var query = new GetProductsQuery();
			var products = await _mediator.SendAsync(query);
			Products = new ObservableCollection<Product>(products);
		}
	}
}
```

### With Sumapap.Reporting.Maui

```csharp
using Sumapap.Mvvm.ViewModels;
using Sumapap.Reporting.Abstractions;
using Sumapap.Mvvm.Maui;

public partial class ReportsViewModel : InteractiveViewModelBase
{
	[RelayCommand]
	private async Task GenerateReportAsync()
	{
		using (StartScopedUserInteraction())
		{
			var reportService = MauiServiceProvider.Current
				.GetRequiredService<IReportService>();

			var report = await reportService.GenerateSalesReportAsync(
				startDate: DateTime.Now.AddMonths(-1),
				endDate: DateTime.Now
			);

			var pdfService = MauiServiceProvider.Current
				.GetRequiredService<IPdfService>();

			var pdfPath = await pdfService.SaveAsync(report);
			await Share.RequestAsync(new ShareFileRequest
			{
				Title = "Sales Report",
				File = new ShareFile(pdfPath)
			});
		}
	}
}
```

## 🔧 Advanced scenarios

### Custom Service Provider Usage

```csharp
using Sumapap.Mvvm.Maui;
using Microsoft.Extensions.DependencyInjection;

public class ServiceLocator
{
	private static IServiceProvider? _serviceProvider;

	public static void Initialize(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public static T GetService<T>() where T : notnull
	{
		return (_serviceProvider ?? MauiServiceProvider.Current)
			.GetRequiredService<T>();
	}

	public static T? GetOptionalService<T>() where T : class
	{
		return (_serviceProvider ?? MauiServiceProvider.Current)
			.GetService<T>();
	}
}
```

### Lazy Service Resolution in View Models

```csharp
using Sumapap.Mvvm.ViewModels;
using Sumapap.Mvvm.Maui;

public partial class LazyLoadViewModel : ViewModelBase
{
	private IExpensiveService? _expensiveService;

	private IExpensiveService ExpensiveService =>
		_expensiveService ??= MauiServiceProvider.Current
			.GetRequiredService<IExpensiveService>();

	[RelayCommand]
	private async Task PerformOperationAsync()
	{
		// Service is only resolved when first accessed
		await ExpensiveService.DoWorkAsync();
	}
}
```

### Factory Pattern with Service Provider

```csharp
using Sumapap.Mvvm.Maui;

public interface IViewModelFactory
{
	TViewModel Create<TViewModel>() where TViewModel : IViewModel;
}

public class ViewModelFactory : IViewModelFactory
{
	public TViewModel Create<TViewModel>() where TViewModel : IViewModel
	{
		return MauiServiceProvider.Current
			.GetRequiredService<TViewModel>();
	}
}

// Usage in view model
public partial class MasterViewModel : ViewModelBase
{
	private readonly IViewModelFactory _factory;

	public MasterViewModel(IViewModelFactory factory)
	{
		_factory = factory;
	}

	[RelayCommand]
	private void CreateDetailViewModel()
	{
		var detailViewModel = _factory.Create<DetailViewModel>();
		// Use detail view model
	}
}
```

### Platform-Specific Service Resolution

```csharp
using Sumapap.Mvvm.Maui;

public partial class CameraViewModel : InteractiveViewModelBase
{
	[RelayCommand]
	private async Task TakePictureAsync()
	{
		using (StartScopedUserInteraction())
		{
			// Use platform-specific camera service
			var cameraService = MauiServiceProvider.Current
				.GetRequiredService<ICameraService>();

			var photo = await cameraService.TakePhotoAsync();

			if (photo != null)
			{
				await SavePhotoAsync(photo);
			}
		}
	}

	private async Task SavePhotoAsync(Stream photo)
	{
		// Save photo logic
		await Task.CompletedTask;
	}
}

// Platform-specific registration in MauiProgram.cs
#if ANDROID
builder.Services.AddSingleton<ICameraService, AndroidCameraService>();
#elif IOS
builder.Services.AddSingleton<ICameraService, iOSCameraService>();
#elif WINDOWS
builder.Services.AddSingleton<ICameraService, WindowsCameraService>();
#endif
```

## 📋 Best practices

### 1. Use Constructor Injection for View Models

```csharp
// ✅ Good: Constructor injection
public partial class OrderViewModel : InteractiveViewModelBase
{
	private readonly IOrderService _orderService;
	private readonly IPaymentService _paymentService;

	public OrderViewModel(
		IOrderService orderService,
		IPaymentService paymentService)
	{
		_orderService = orderService;
		_paymentService = paymentService;
	}
}

// ❌ Avoid: Direct service provider access in constructor
public partial class OrderViewModel : InteractiveViewModelBase
{
	private readonly IOrderService _orderService;

	public OrderViewModel()
	{
		_orderService = MauiServiceProvider.Current
			.GetRequiredService<IOrderService>();
	}
}
```

### 2. Use MauiServiceProvider for Special Cases Only

Use `MauiServiceProvider.Current` only when constructor injection is not possible:

```csharp
// ✅ Good: Use in static methods, converters, behaviors
public class CurrencyConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		var currencyService = MauiServiceProvider.Current
			.GetRequiredService<ICurrencyService>();
		return currencyService.Format(value);
	}
}

// ✅ Good: Use for lazy initialization
public partial class HeavyViewModel : ViewModelBase
{
	private IHeavyService? _heavyService;

	private IHeavyService HeavyService =>
		_heavyService ??= MauiServiceProvider.Current
			.GetRequiredService<IHeavyService>();
}
```

### 3. Register All Views and View Models

```csharp
// In MauiProgram.cs
builder.Services

	// Register view models
	.AddTransient<MainViewModel>()
	.AddTransient<ProductListViewModel>()
	.AddTransient<ProductDetailViewModel>()

	// Register pages
	.AddTransient<MainPage>()
	.AddTransient<ProductListPage>()
	.AddTransient<ProductDetailPage>();
```

### 4. Use IViewModelOwner for Type-Safe Binding

```csharp
// ✅ Good: Implement IViewModelOwner<TViewModel>
public partial class ProductPage : ContentPage, IViewModelOwner<ProductViewModel>
{
	public ProductPage(ProductViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		BindingContext = viewModel;
	}

	public ProductViewModel ViewModel { get; }
}

// ❌ Avoid: Weak-typed BindingContext
public partial class ProductPage : ContentPage
{
	public ProductPage(object viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
```

### 5. Initialize View Models Properly

```csharp
public partial class MainPage : ContentPage, IViewModelOwner<MainViewModel>
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		BindingContext = viewModel;
	}

	public MainViewModel ViewModel { get; }

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Load data when page appears
		if (ViewModel.LoadDataCommand.CanExecute(null))
		{
			await ViewModel.LoadDataCommand.ExecuteAsync(null);
		}
	}
}
```

## 📚 API reference

### Namespaces

- `Sumapap.Mvvm.Maui` — MAUI-specific utilities

### Core Types

| Type | Description |
|------|-------------|
| `MauiServiceProvider` | Static accessor for MAUI's service provider |

### MauiServiceProvider Members

| Member | Type | Description |
|--------|------|-------------|
| `Current` | `IServiceProvider` | Gets the current MAUI application's service provider |

### Platform Support

| Platform | Supported | Minimum Version |
|----------|-----------|----------------|
| Android | ✅ | API 21 (Android 5.0) |
| iOS | ✅ | iOS 15.0+ |
| macOS Catalyst | ✅ | macOS 15.0+ |
| Windows | ✅ | Windows 10.0.17763.0+ |

## 🤝 Related packages

- [Sumapap.Mvvm](https://www.nuget.org/packages/Sumapap.Mvvm/) — Framework-agnostic MVVM infrastructure
- [Sumapap.Navigations.Maui](https://www.nuget.org/packages/Sumapap.Navigations.Maui/) — MAUI navigation implementation
- [Sumapap.Reporting.Maui](https://www.nuget.org/packages/Sumapap.Reporting.Maui/) — Report generation for MAUI
- [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/) — Microsoft's official MVVM toolkit
- [Microsoft.Maui.Controls](https://www.nuget.org/packages/Microsoft.Maui.Controls/) — .NET MAUI controls library

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

## 🙌 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📬 Support

- 📝 [Report Issues](https://github.com/muhammadirwanto-dev/sumapap/issues)
- 💬 [Discussions](https://github.com/muhammadirwanto-dev/sumapap/discussions)
- 📖 [Documentation](https://github.com/muhammadirwanto-dev/sumapap)
