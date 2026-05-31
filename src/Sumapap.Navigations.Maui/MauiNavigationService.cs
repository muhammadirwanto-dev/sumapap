using Sumapap.Navigations.Abstractions;

namespace Sumapap.Navigations.Maui
{
    internal sealed class MauiNavigationService(
        INavigationAdapter _adapter
        ) : INavigationService
    {
        public Task BackAsync(CancellationToken cancellationToken = default)
            => _adapter.BackAsync(cancellationToken)
                ?? Task.CompletedTask;

        public Task BackAsync(INavigationParams param, CancellationToken cancellationToken = default)
            => _adapter.BackAsync(param, cancellationToken)
                ?? Task.CompletedTask;

        public Task NavigateToAsync<TView>(CancellationToken cancellationToken = default)
            => _adapter.NavigateToAsync<TView>(cancellationToken)
                ?? Task.CompletedTask;

        public Task NavigateToAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
            => _adapter.NavigateToAsync<TView>(param, cancellationToken)
                ?? Task.CompletedTask;

        public Task NavigateToRootAsync<TView>(CancellationToken cancellationToken = default)
            => _adapter.NavigateToRootAsync<TView>(cancellationToken)
                ?? Task.CompletedTask;

        public Task NavigateToRootAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
            => _adapter.NavigateToRootAsync<TView>(param, cancellationToken)
                ?? Task.CompletedTask;
    }
}
