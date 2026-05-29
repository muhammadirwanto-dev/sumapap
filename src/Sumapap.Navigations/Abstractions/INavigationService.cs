namespace Sumapap.Navigations.Abstractions
{
    /// <summary>
    /// Defines a service for navigating between pages or views within an application, supporting forward, backward, and
    /// root navigation with optional parameters and presentation styles.
    /// </summary>
    /// <remarks>The INavigationService interface provides asynchronous methods for navigation operations,
    /// allowing developers to move to specific pages, pass navigation queries, control animation, and specify
    /// presentation modes. It is commonly used in applications with a navigation stack, such as mobile or desktop UI
    /// frameworks. Implementations should ensure thread safety and handle navigation requests appropriately according
    /// to the application's navigation model.</remarks>
    public interface INavigationService
    {
        Task NavigateToAsync<TView>(CancellationToken cancellationToken = default);

        Task NavigateToAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default);

        Task BackAsync(CancellationToken cancellationToken = default);

        Task BackAsync(INavigationParams param, CancellationToken cancellationToken = default);

        Task NavigateToRootAsync<TView>(CancellationToken cancellationToken = default);

        Task NavigateToRootAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default);
    }
}
