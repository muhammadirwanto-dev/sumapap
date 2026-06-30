# Sumapap.Mvvm

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Mvvm.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Mvvm/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Mvvm.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Mvvm/)
[![License](https://img.shields.io/github/license/muhammadirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)

## 💡 Overview

`Sumapap.Mvvm` provides MVVM infrastructure and base classes built on top of [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) for building modern .NET applications with the Model-View-ViewModel pattern. This package extends CommunityToolkit.Mvvm with additional features for managing user interactions, navigation states, and view-viewmodel relationships.

The package includes:
- **IViewModel** — Marker interface for view model identification
- **IViewModelOwner** — Contract for views that own view models
- **ViewModelOwnerAttribute** — Attribute for declaring view-viewmodel relationships
- **ViewModelBase** — Base class for simple view models with property change notification
- **RecipientViewModelBase** — Base class for view models that participate in messaging
- **InteractiveViewModelBase** — Advanced base class for managing user interaction states

> **📦 For MAUI applications:** Use [Sumapap.Mvvm.Maui](https://www.nuget.org/packages/Sumapap.Mvvm.Maui/) for MAUI-specific utilities and integration.

## ✨ Why use `Sumapap.Mvvm`?

- **Built on CommunityToolkit.Mvvm** — Leverage the power of the official Microsoft toolkit with source generators
- **Framework-Agnostic** — Works with any .NET UI framework (MAUI, WPF, Avalonia, Uno Platform, etc.)
- **Interaction State Management** — Track refreshing, navigating, and user interaction states automatically
- **Command Coordination** — Automatically disable commands during long-running operations
- **Scoped Operations** — Use `IDisposableScope` for clean state management with automatic cleanup
- **Messaging Support** — Built-in support for CommunityToolkit.Mvvm's messenger pattern
- **Type-Safe View-ViewModel Binding** — Strongly-typed relationships between views and view models
- **Source Generator Ready** — Fully compatible with `[ObservableProperty]` and `[RelayCommand]` attributes

## 🚀 Quick start

1. Add the package to your project:

```bash
dotnet add package Sumapap.Mvvm
```

2. Create a view model by inheriting from one of the base classes:

```csharp
using Sumapap.Mvvm.ViewModels;
using CommunityToolkit.Mvvm.Input;

public partial class MainViewModel : ViewModelBase
{
	[ObservableProperty]
	private string _title = "Hello World";

	[ObservableProperty]
	private int _counter;

	[RelayCommand]
	private void IncrementCounter()
	{
		Counter++;
	}
}
```

3. Use the view model in your view:

```csharp
// Example for MAUI
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

4. For advanced scenarios with interaction states:

```csharp
using Sumapap.Mvvm.ViewModels;
using CommunityToolkit.Mvvm.Input;

public partial class ProductListViewModel : InteractiveViewModelBase
{
	private readonly IProductService _productService;

	[ObservableProperty]
	private ObservableCollection<Product> _products = [];

	public ProductListViewModel(IProductService productService)
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

	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task NavigateToDetailAsync(int productId)
	{
		using (StartScopedNavigation())
		{
			// Navigation logic here
			await Task.Delay(100); // Simulate navigation
		}
	}
}
```

## 🛠 Features and usage

### IViewModel Interface

A marker interface that identifies a class as a view model:

```csharp
namespace Sumapap.Mvvm.Abstractions
{
	public interface IViewModel;
}
```

**Use Cases:**
- Generic constraints for view model parameters
- Service registration and resolution
- Type checking and reflection scenarios

**Example:**
```csharp
// Generic constraint
public class ViewFactory<TViewModel> where TViewModel : IViewModel
{
	public TViewModel CreateViewModel()
	{
		// Factory logic
	}
}

// Service registration
services.AddTransient<IViewModel, MainViewModel>();

// Type checking
if (dataContext is IViewModel viewModel)
{
	// Work with view model
}
```

### IViewModelOwner Interface

Defines the contract for views that own view models:

```csharp
// Non-generic marker interface
public interface IViewModelOwner;

// Strongly-typed interface
public interface IViewModelOwner<TViewModel> : IViewModelOwner
	where TViewModel : IViewModel
{
	TViewModel ViewModel { get; }
}
```

**Implementation Example:**
```csharp
// MAUI Page
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

// WPF Window
public partial class MainWindow : Window, IViewModelOwner<MainViewModel>
{
	public MainWindow(MainViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		DataContext = viewModel;
	}

	public MainViewModel ViewModel { get; }
}
```

**Benefits:**
- Type-safe access to the view model
- Clear contract between view and view model
- Enables framework-agnostic view model discovery
- Simplifies testing and mocking

### ViewModelOwnerAttribute

Specifies the view model type associated with a view class:

```csharp
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ViewModelOwnerAttribute(Type viewModelType) : Attribute
{
	public Type ViewModelType { get; }
	public bool IsDefaultConstructor { get; init; } = false;
}

// Generic version for type safety
public class ViewModelOwnerAttribute<TViewModel> : ViewModelOwnerAttribute
{
	public ViewModelOwnerAttribute() : base(typeof(TViewModel))
	{
	}
}
```

**Usage:**
```csharp
// Using generic attribute (recommended)
[ViewModelOwner<MainViewModel>]
public partial class MainPage : ContentPage
{
	// ...
}

// Using non-generic attribute
[ViewModelOwner(typeof(SettingsViewModel))]
public partial class SettingsPage : ContentPage
{
	// ...
}

// With default constructor flag
[ViewModelOwner<LoginViewModel>(IsDefaultConstructor = true)]
public partial class LoginPage : ContentPage
{
	// View model will be created with parameterless constructor
}
```

**Use Cases:**
- Dependency injection container configuration
- View-ViewModel mapping for navigation systems
- Design-time view model instantiation
- Code generation and tooling support

### ViewModelBase

Basic view model class providing property change notification:

```csharp
public abstract class ViewModelBase : ObservableObject, IViewModel
{
}
```

Inherits all functionality from CommunityToolkit.Mvvm's `ObservableObject`:
- `INotifyPropertyChanged` implementation
- `SetProperty` methods for property change notification
- Source generator support for `[ObservableProperty]`
- `OnPropertyChanged` virtual method

**Example:**
```csharp
public partial class CounterViewModel : ViewModelBase
{
	[ObservableProperty]
	private int _count;

	[ObservableProperty]
	private string _message = string.Empty;

	partial void OnCountChanged(int value)
	{
		Message = $"Count is now {value}";
	}

	[RelayCommand]
	private void Increment()
	{
		Count++;
	}

	[RelayCommand]
	private void Reset()
	{
		Count = 0;
	}
}
```

**Best For:**
- Simple view models without messaging
- Lightweight scenarios
- View models that don't need interaction state management
- Read-only or display-only views

### RecipientViewModelBase

View model base class with support for CommunityToolkit.Mvvm's messenger pattern:

```csharp
public abstract class RecipientViewModelBase : ObservableRecipient, IViewModel
{
}
```

Inherits all functionality from CommunityToolkit.Mvvm's `ObservableRecipient`:
- All `ObservableObject` features
- `IRecipient` interface implementation
- `IsActive` property for message subscription management
- Automatic message registration/unregistration
- Broadcast methods for sending messages

**Example:**
```csharp
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

public record UserLoggedInMessage(string UserName);

public partial class HeaderViewModel : RecipientViewModelBase
{
	[ObservableProperty]
	private string _userName = "Guest";

	public HeaderViewModel()
	{
		// Activate to start receiving messages
		IsActive = true;
	}

	// Register message handler
	protected override void OnActivated()
	{
		Messenger.Register<UserLoggedInMessage>(this, (r, m) =>
		{
			UserName = m.UserName;
		});
	}
}

public partial class LoginViewModel : RecipientViewModelBase
{
	[RelayCommand]
	private async Task LoginAsync(string userName)
	{
		// Perform login...
		await Task.Delay(1000);

		// Broadcast message to all active recipients
		Messenger.Send(new UserLoggedInMessage(userName));
	}
}
```

**Message Broadcasting:**
```csharp
public partial class CartViewModel : RecipientViewModelBase
{
	[RelayCommand]
	private void AddToCart(Product product)
	{
		// Add product to cart...

		// Broadcast property change message
		Messenger.Send(new PropertyChangedMessage<int>(
			this,
			nameof(CartItemCount),
			CartItemCount - 1,
			CartItemCount
		));
	}
}
```

**Best For:**
- View models that need to communicate across the application
- Decoupled component communication
- Event aggregation scenarios
- Cross-cutting concerns (notifications, state synchronization)

### InteractiveViewModelBase

Advanced view model base class for managing user interaction states:

```csharp
public partial class InteractiveViewModelBase : RecipientViewModelBase
{
	[ObservableProperty]
	private bool _isRefreshing;

	[ObservableProperty]
	[NotifyPropertyChangedRecipients]
	private bool _isNavigating;

	[ObservableProperty]
	[NotifyPropertyChangedRecipients]
	private bool _isUserInteraction;

	// ... methods and features
}
```

#### Interaction State Properties

**IsRefreshing** — Indicates swipe-to-refresh or data reload operations:
```csharp
public async Task RefreshDataAsync()
{
	IsRefreshing = true;
	try
	{
		await _dataService.LoadDataAsync();
	}
	finally
	{
		IsRefreshing = false;
	}
}
```

**IsNavigating** — Indicates navigation in progress:
```csharp
[RelayCommand]
private async Task NavigateToDetailAsync()
{
	IsNavigating = true;
	try
	{
		await _navigationService.NavigateToAsync<DetailPage>();
	}
	finally
	{
		IsNavigating = false;
	}
}
```

**IsUserInteraction** — Indicates any UI interaction that should not be interrupted:
```csharp
[RelayCommand]
private async Task SaveAsync()
{
	IsUserInteraction = true;
	try
	{
		await _repository.SaveAsync(CurrentItem);
	}
	finally
	{
		IsUserInteraction = false;
	}
}
```

#### Helper Methods

**State Checking:**
```csharp
protected bool CanRefresh() => !IsRefreshing;
protected bool CanNavigate() => !IsNavigating;
protected bool CanUserInteraction() => !IsUserInteraction;
```

**State Setting:**
```csharp
protected void Refreshing() => IsRefreshing = true;
protected void Refreshed() => IsRefreshing = false;
protected void Navigating() => IsNavigating = true;
protected void Navigated() => IsNavigating = false;
protected void UserInteracting() => IsUserInteraction = true;
protected void UserInteracted() => IsUserInteraction = false;
```

**Usage with Commands:**
```csharp
public partial class ProductListViewModel : InteractiveViewModelBase
{
	[RelayCommand(CanExecute = nameof(CanRefresh))]
	private async Task RefreshAsync()
	{
		Refreshing();
		try
		{
			await LoadProductsAsync();
		}
		finally
		{
			Refreshed();
		}
	}

	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task OpenProductAsync(int productId)
	{
		Navigating();
		try
		{
			await _navigationService.NavigateToAsync<ProductDetailPage>(
				new ProductDetailParams(productId)
			);
		}
		finally
		{
			Navigated();
		}
	}
}
```

#### Scoped Operations

Automatically manage state with disposable scopes:

```csharp
public IDisposableScope StartScopedRefresh();
public IDisposableScope StartScopedNavigation();
public IDisposableScope StartScopedUserInteraction();
```

**Example:**
```csharp
public partial class OrderViewModel : InteractiveViewModelBase
{
	[RelayCommand]
	private async Task LoadOrdersAsync()
	{
		using (StartScopedRefresh())
		{
			var orders = await _orderService.GetAllAsync();
			Orders = new ObservableCollection<Order>(orders);
		}
		// IsRefreshing automatically set to false when scope disposes
	}

	[RelayCommand]
	private async Task CheckoutAsync()
	{
		using (StartScopedUserInteraction())
		{
			await _paymentService.ProcessPaymentAsync();
			await _orderService.CreateOrderAsync();
		}
		// IsUserInteraction automatically set to false
	}

	[RelayCommand]
	private async Task NavigateToPaymentAsync()
	{
		using (StartScopedNavigation())
		{
			await _navigationService.NavigateToAsync<PaymentPage>();
		}
		// IsNavigating automatically set to false
	}
}
```

**Nested Scopes:**
```csharp
public async Task ComplexOperationAsync()
{
	using (StartScopedUserInteraction())
	{
		// Process payment
		await ProcessPaymentAsync();

		using (StartScopedNavigation())
		{
			// Navigate to confirmation
			await _navigationService.NavigateToAsync<ConfirmationPage>();
		}

		// Continue with post-navigation work
		await SendConfirmationEmailAsync();
	}
}
```

#### Interaction Command Registration

Automatically coordinate command availability with interaction states:

```csharp
protected void RegisterInteractionCommand(string property, IRelayCommand command);
protected void UnRegisterInteractionCommand(string property, IRelayCommand command);
```

**Example:**
```csharp
public partial class ShoppingCartViewModel : InteractiveViewModelBase
{
	[ObservableProperty]
	private bool _isProcessingPayment;

	private IRelayCommand? _checkoutCommand;

	public ShoppingCartViewModel()
	{
		// Register command to be notified when IsProcessingPayment changes
		_checkoutCommand = new RelayCommand(Checkout);
		RegisterInteractionCommand(nameof(IsProcessingPayment), _checkoutCommand);
	}

	private void Checkout()
	{
		if (!IsProcessingPayment)
		{
			// Process checkout
		}
	}
}
```

**Automatic Command Updates:**
When a property changes that has registered commands, all registered commands automatically call `NotifyCanExecuteChanged()`:

```csharp
protected override void Broadcast<T>(T oldValue, T newValue, string? propertyName)
{
	base.Broadcast(oldValue, newValue, propertyName);

	if (propertyName != null)
	{
		if (_interactionCommands.TryGetValue(propertyName, out var commands))
		{
			foreach (var command in commands)
			{
				command.NotifyCanExecuteChanged();
			}
		}
	}
}
```

#### Complete Example

```csharp
using Sumapap.Mvvm.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

public partial class CustomerListViewModel : InteractiveViewModelBase
{
	private readonly ICustomerService _customerService;
	private readonly INavigationService _navigationService;

	[ObservableProperty]
	private ObservableCollection<Customer> _customers = [];

	[ObservableProperty]
	private Customer? _selectedCustomer;

	[ObservableProperty]
	private string _searchText = string.Empty;

	public CustomerListViewModel(
		ICustomerService customerService,
		INavigationService navigationService)
	{
		_customerService = customerService;
		_navigationService = navigationService;
	}

	[RelayCommand]
	private async Task LoadCustomersAsync()
	{
		using (StartScopedRefresh())
		{
			var customers = await _customerService.GetAllAsync();
			Customers = new ObservableCollection<Customer>(customers);
		}
	}

	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task ViewCustomerDetailAsync(int customerId)
	{
		using (StartScopedNavigation())
		{
			await _navigationService.NavigateToAsync<CustomerDetailPage>(
				new CustomerDetailParams(customerId)
			);
		}
	}

	[RelayCommand(CanExecute = nameof(CanUserInteraction))]
	private async Task DeleteCustomerAsync(int customerId)
	{
		using (StartScopedUserInteraction())
		{
			var confirmed = await ShowConfirmationAsync(
				"Are you sure you want to delete this customer?"
			);

			if (confirmed)
			{
				await _customerService.DeleteAsync(customerId);
				await LoadCustomersAsync();
			}
		}
	}

	[RelayCommand]
	private async Task SearchAsync()
	{
		using (StartScopedRefresh())
		{
			var results = await _customerService.SearchAsync(SearchText);
			Customers = new ObservableCollection<Customer>(results);
		}
	}

	partial void OnSearchTextChanged(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			_ = LoadCustomersAsync();
		}
	}
}
```

**Best For:**
- Interactive views with loading states
- Navigation scenarios
- Views with swipe-to-refresh
- Long-running operations that need visual feedback
- Complex user interactions requiring state coordination

## 📋 Best practices

### 1. Choose the Right Base Class

```csharp
// Simple view models without messaging
public partial class ReadOnlyViewModel : ViewModelBase
{
	[ObservableProperty]
	private string _displayText = string.Empty;
}

// View models that need messaging
public partial class NotificationViewModel : RecipientViewModelBase
{
	protected override void OnActivated()
	{
		Messenger.Register<NotificationMessage>(this, OnNotification);
	}
}

// Interactive view models with state management
public partial class DataEntryViewModel : InteractiveViewModelBase
{
	[RelayCommand(CanExecute = nameof(CanUserInteraction))]
	private async Task SaveAsync()
	{
		using (StartScopedUserInteraction())
		{
			await SaveDataAsync();
		}
	}
}
```

### 2. Use Source Generators

Leverage CommunityToolkit.Mvvm source generators for cleaner code:

```csharp
// ✅ Good: Use [ObservableProperty]
public partial class UserViewModel : ViewModelBase
{
	[ObservableProperty]
	private string _firstName = string.Empty;

	[ObservableProperty]
	private string _lastName = string.Empty;
}

// ❌ Avoid: Manual property implementation
public class UserViewModel : ViewModelBase
{
	private string _firstName = string.Empty;
	public string FirstName
	{
		get => _firstName;
		set => SetProperty(ref _firstName, value);
	}
}
```

### 3. Use Scoped Operations

```csharp
// ✅ Good: Automatic state cleanup
[RelayCommand]
private async Task LoadDataAsync()
{
	using (StartScopedRefresh())
	{
		await FetchDataAsync();
	}
}

// ❌ Avoid: Manual state management
[RelayCommand]
private async Task LoadDataAsync()
{
	IsRefreshing = true;
	try
	{
		await FetchDataAsync();
	}
	finally
	{
		IsRefreshing = false;
	}
}
```

### 4. Implement IViewModelOwner

```csharp
// ✅ Good: Type-safe view model access
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

// ❌ Avoid: Weak-typed BindingContext casting
public partial class ProductPage : ContentPage
{
	public ProductPage(ProductViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	private ProductViewModel ViewModel => (ProductViewModel)BindingContext;
}
```

### 5. Use CanExecute with Interaction States

```csharp
// ✅ Good: Built-in interaction checks
public partial class OrderViewModel : InteractiveViewModelBase
{
	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task CheckoutAsync()
	{
		using (StartScopedNavigation())
		{
			await NavigateToCheckoutAsync();
		}
	}
}

// ❌ Avoid: Manual state checking
public partial class OrderViewModel : InteractiveViewModelBase
{
	[RelayCommand(CanExecute = nameof(CanCheckout))]
	private async Task CheckoutAsync()
	{
		if (!IsNavigating)
		{
			IsNavigating = true;
			try
			{
				await NavigateToCheckoutAsync();
			}
			finally
			{
				IsNavigating = false;
			}
		}
	}

	private bool CanCheckout() => !IsNavigating;
}
```

### 6. Activate Recipients When Needed

```csharp
// ✅ Good: Activate in constructor or explicitly
public partial class NotificationHubViewModel : RecipientViewModelBase
{
	public NotificationHubViewModel()
	{
		IsActive = true; // Start receiving messages immediately
	}

	protected override void OnActivated()
	{
		Messenger.Register<NotificationMessage>(this, OnNotification);
	}
}

// ❌ Avoid: Forgetting to activate
public partial class NotificationHubViewModel : RecipientViewModelBase
{
	// Messages won't be received until IsActive = true
	protected override void OnActivated()
	{
		Messenger.Register<NotificationMessage>(this, OnNotification);
	}
}
```

## 🔗 Integration with other Sumapap packages

### With Sumapap.Navigations

```csharp
using Sumapap.Mvvm.ViewModels;
using Sumapap.Navigations.Abstractions;

public partial class MainViewModel : InteractiveViewModelBase
{
	private readonly INavigationService _navigationService;

	public MainViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	[RelayCommand(CanExecute = nameof(CanNavigate))]
	private async Task NavigateToSettingsAsync()
	{
		using (StartScopedNavigation())
		{
			await _navigationService.NavigateToAsync<SettingsPage>();
		}
	}
}
```

### With Sumapap.Ddd.Mediator

```csharp
using Sumapap.Mvvm.ViewModels;
using Sumapap.Ddd.Mediator.Abstractions;

public partial class ProductListViewModel : InteractiveViewModelBase
{
	private readonly IMediator _mediator;

	[ObservableProperty]
	private ObservableCollection<Product> _products = [];

	public ProductListViewModel(IMediator mediator)
	{
		_mediator = mediator;
	}

	[RelayCommand]
	private async Task LoadProductsAsync()
	{
		using (StartScopedRefresh())
		{
			var query = new GetAllProductsQuery();
			var result = await _mediator.SendAsync(query);
			Products = new ObservableCollection<Product>(result);
		}
	}
}
```

## 🔧 Advanced scenarios

### Custom Interaction State

```csharp
public partial class CustomViewModel : InteractiveViewModelBase
{
	[ObservableProperty]
	private bool _isExporting;

	private bool CanExport() => !IsExporting;

	[RelayCommand(CanExecute = nameof(CanExport))]
	private async Task ExportAsync()
	{
		IsExporting = true;
		try
		{
			await PerformExportAsync();
		}
		finally
		{
			IsExporting = false;
		}
	}

	// Create custom scope helper
	public IDisposableScope StartScopedExport()
	{
		IsExporting = true;
		return DisposableScope.Create(() => IsExporting = false);
	}
}
```

### Property Change Validation

```csharp
public partial class FormViewModel : ViewModelBase
{
	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
	private string _email = string.Empty;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
	private string _password = string.Empty;

	[ObservableProperty]
	private string? _emailError;

	[ObservableProperty]
	private string? _passwordError;

	partial void OnEmailChanged(string value)
	{
		EmailError = IsValidEmail(value) ? null : "Invalid email address";
	}

	partial void OnPasswordChanged(string value)
	{
		PasswordError = value.Length >= 8 ? null : "Password must be at least 8 characters";
	}

	private bool CanSubmit() =>
		!string.IsNullOrWhiteSpace(Email) &&
		!string.IsNullOrWhiteSpace(Password) &&
		EmailError == null &&
		PasswordError == null;

	[RelayCommand(CanExecute = nameof(CanSubmit))]
	private async Task SubmitAsync()
	{
		await SubmitFormAsync();
	}

	private bool IsValidEmail(string email)
	{
		return email.Contains('@') && email.Contains('.');
	}
}
```

### Cross-ViewModel Communication

```csharp
// Message definition
public record CartUpdatedMessage(int ItemCount, decimal TotalPrice);

// Sender view model
public partial class ProductDetailViewModel : RecipientViewModelBase
{
	[RelayCommand]
	private void AddToCart(Product product)
	{
		// Add to cart logic...

		Messenger.Send(new CartUpdatedMessage(
			ItemCount: _cart.Count,
			TotalPrice: _cart.TotalPrice
		));
	}
}

// Receiver view model
public partial class HeaderViewModel : RecipientViewModelBase
{
	[ObservableProperty]
	private int _cartItemCount;

	[ObservableProperty]
	private decimal _cartTotal;

	public HeaderViewModel()
	{
		IsActive = true;
	}

	protected override void OnActivated()
	{
		Messenger.Register<CartUpdatedMessage>(this, (r, m) =>
		{
			CartItemCount = m.ItemCount;
			CartTotal = m.TotalPrice;
		});
	}
}
```

## 📚 API reference

### Namespaces

- `Sumapap.Mvvm.Abstractions` — Core interfaces
- `Sumapap.Mvvm.Attributes` — Attribute definitions
- `Sumapap.Mvvm.ViewModels` — Base view model classes

### Core Types

| Type | Description |
|------|-------------|
| `IViewModel` | Marker interface for view models |
| `IViewModelOwner` | Non-generic view model owner marker |
| `IViewModelOwner<TViewModel>` | Strongly-typed view model owner contract |
| `ViewModelOwnerAttribute` | Declares view-viewmodel relationship |
| `ViewModelOwnerAttribute<TViewModel>` | Generic version of ViewModelOwnerAttribute |
| `ViewModelBase` | Basic view model with property change notification |
| `RecipientViewModelBase` | View model with messaging support |
| `InteractiveViewModelBase` | Advanced view model with interaction state management |

### InteractiveViewModelBase Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsRefreshing` | `bool` | Indicates swipe-to-refresh or data reload |
| `IsNavigating` | `bool` | Indicates navigation in progress |
| `IsUserInteraction` | `bool` | Indicates UI interaction in progress |

### InteractiveViewModelBase Methods

| Method | Description |
|--------|-------------|
| `CanRefresh()` | Returns `true` if not currently refreshing |
| `CanNavigate()` | Returns `true` if not currently navigating |
| `CanUserInteraction()` | Returns `true` if no user interaction in progress |
| `Refreshing()` | Sets `IsRefreshing` to `true` |
| `Refreshed()` | Sets `IsRefreshing` to `false` |
| `Navigating()` | Sets `IsNavigating` to `true` |
| `Navigated()` | Sets `IsNavigating` to `false` |
| `UserInteracting()` | Sets `IsUserInteraction` to `true` |
| `UserInteracted()` | Sets `IsUserInteraction` to `false` |
| `StartScopedRefresh()` | Returns a disposable scope for refresh operations |
| `StartScopedNavigation()` | Returns a disposable scope for navigation operations |
| `StartScopedUserInteraction()` | Returns a disposable scope for user interactions |
| `RegisterInteractionCommand(string, IRelayCommand)` | Register command for property change notifications |
| `UnRegisterInteractionCommand(string, IRelayCommand)` | Unregister command from property notifications |

## 🤝 Related packages

- [Sumapap.Mvvm.Maui](https://www.nuget.org/packages/Sumapap.Mvvm.Maui/) — MAUI-specific MVVM utilities
- [Sumapap.Navigations](https://www.nuget.org/packages/Sumapap.Navigations/) — Navigation abstractions
- [Sumapap.Ddd.Mediator](https://www.nuget.org/packages/Sumapap.Ddd.Mediator/) — Mediator pattern for CQRS
- [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/) — Microsoft's official MVVM toolkit

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/muhammadirwanto-dev/sumapap/blob/main/LICENSE) file for details.

## 🙌 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📬 Support

- 📝 [Report Issues](https://github.com/muhammadirwanto-dev/sumapap/issues)
- 💬 [Discussions](https://github.com/muhammadirwanto-dev/sumapap/discussions)
- 📖 [Documentation](https://github.com/muhammadirwanto-dev/sumapap)
