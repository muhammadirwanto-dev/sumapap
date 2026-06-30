using CommunityToolkit.Mvvm.ComponentModel;
using Sumapap.Mvvm.Abstractions;

namespace Sumapap.Mvvm.ViewModels
{
    /// <summary>
    /// Serves as a base class for view models that represent a recipient in the application.
    /// </summary>
    /// <remarks>Inherit from this class to implement recipient-specific view models that support property
    /// change notification and integrate with the application's MVVM infrastructure. This class provides common
    /// functionality for recipient view models and implements the IViewModel interface.</remarks>
    public abstract class RecipientViewModelBase
        : ObservableRecipient, IViewModel
    {
    }
}
