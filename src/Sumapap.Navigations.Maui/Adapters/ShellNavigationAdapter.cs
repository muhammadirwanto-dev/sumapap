using Sumapap.Navigations.Abstractions;
using Sumapap.Navigations.Maui.Models;

namespace Sumapap.Navigations.Maui.Adapters
{
    internal sealed class ShellNavigationAdapter(
        ) : INavigationAdapter
    {
        private static readonly Shell _navigation = Shell.Current;

        public bool CanHandle() => Shell.Current is not null;

        public Task BackAsync(CancellationToken cancellationToken = default)
            => BackAsync(ShellNavigationParams.Empty, cancellationToken);

        public Task BackAsync(INavigationParams param, CancellationToken cancellationToken = default)
        {
            var shellParam = GetConcrete(param);

            return _navigation.GoToAsync(shellParam.CombinePath(".."), shellParam.Animated, shellParam.Query);
        }

        public Task NavigateToAsync<TView>(CancellationToken cancellationToken = default)
            => NavigateToAsync<TView>(ShellNavigationParams.Empty, cancellationToken);

        public Task NavigateToAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
        {
            var shellParam = GetConcrete(param);

            return _navigation.GoToAsync(shellParam.CombinePath(typeof(TView).Name), shellParam.Animated, shellParam.Query);
        }

        public Task NavigateToRootAsync<TView>(CancellationToken cancellationToken = default)
            => NavigateToRootAsync<TView>(ShellNavigationParams.Empty, cancellationToken);

        public Task NavigateToRootAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
        {
            var shellParam = GetConcrete(param);

            if (shellParam.ShellRoute is not ShellNavigationParams.Absolute &&
                shellParam.ShellRoute is not ShellNavigationParams.AbsoluteClearStack)
            {
                shellParam.ShellRoute = ShellNavigationParams.Absolute;
            }

            return _navigation.GoToAsync(shellParam.CombinePath(typeof(TView).Name), shellParam.Animated, shellParam.Query);
        }

        private static ShellNavigationParams GetConcrete(INavigationParams param)
            => param is ShellNavigationParams concrete
             ? concrete
             : throw new InvalidCastException();
    }
}
