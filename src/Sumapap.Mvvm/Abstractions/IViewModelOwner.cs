namespace Sumapap.Mvvm.Abstractions
{
    /// <summary>
    /// Marker interface for a View that owns a ViewModel.
    /// </summary>
    public interface IViewModelOwner;

    /// <summary>
    /// Strongly typed ViewModel owner contract.
    /// Implemented by Views (Pages, Windows, Controls).
    /// </summary>
    /// <typeparam name="TViewModel">The owned ViewModel type.</typeparam>
    public interface IViewModelOwner<TViewModel> : IViewModelOwner
        where TViewModel : IViewModel
    {
        TViewModel ViewModel { get; }
    }
}
