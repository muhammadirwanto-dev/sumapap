using CommunityToolkit.Mvvm.ComponentModel;
using Sumapap.Mvvm.Abstractions;

namespace Sumapap.Mvvm.ViewModels
{
    /// <summary>
    /// Base ViewModel built on CommunityToolkit.Mvvm.
    /// Provides property change notification and core MVVM behavior.
    /// </summary>
    public abstract class ViewModelBase
        : ObservableObject, IViewModel
    {
    }
}
