using Sumapap.Navigations.Abstractions;
using Sumapap.Navigations.Maui.Models;

namespace Sumapap.Navigations.Maui.Adapters
{
    internal sealed class PageNavigationAdapter(
        IServiceProvider _serviceProvider
        ) : INavigationAdapter
    {
        private static readonly Page? _page = Application.Current!.Windows[0].Page;
        private readonly INavigation _navigation = _page!.Navigation;

        public bool CanHandle() => _page is not null;

        public Task BackAsync(CancellationToken cancellationToken = default)
            => BackAsync(PageNavigationParams.Empty, cancellationToken);

        public async Task BackAsync(INavigationParams param, CancellationToken cancellationToken = default)
        {
            var pageParam = GetConcrete(param);
            var page = pageParam.Mode == PageNavigationMode.Normal
                ? await _navigation.PopAsync(pageParam.Animated)
                : await _navigation.PopModalAsync(pageParam.Animated);
            var top = _navigation.NavigationStack.LastOrDefault();

            if (top != null && top != page)
            {
                ReadQuery(top, pageParam.Query);
            }
        }

        public Task NavigateToAsync<TView>(CancellationToken cancellationToken = default)
            => NavigateToAsync<TView>(PageNavigationParams.Empty, cancellationToken);

        public Task NavigateToAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
        {
            var view = _serviceProvider.GetService<TView>();
            if (view is not Page page)
            {
                throw new NullReferenceException($"Page {typeof(TView).Name} not registered");
            }

            var pageParam = GetConcrete(param);

            ReadQuery(page, pageParam.Query);

            return pageParam.Mode == PageNavigationMode.Normal
                ? _navigation.PushAsync(page, pageParam.Animated)
                : _navigation.PushModalAsync(page, pageParam.Animated);
        }

        public Task NavigateToRootAsync<TView>(CancellationToken cancellationToken = default)
            => NavigateToRootAsync<TView>(PageNavigationParams.Empty, cancellationToken);

        public async Task NavigateToRootAsync<TView>(INavigationParams param, CancellationToken cancellationToken = default)
        {
            var pageParam = GetConcrete(param);

            await _navigation.PopToRootAsync(pageParam.Animated);

            var top = _navigation.NavigationStack.LastOrDefault();
            if (top != null)
            {
                ReadQuery(top, pageParam.Query);
            }
        }

        private static void ReadQuery(Page page, NavigationQuery query)
        {
            foreach (var item in query)
            {
                page.BindingContext?.GetType().GetProperty(item.Key)?.SetValue(page.BindingContext, item.Value);
            }
        }

        private static PageNavigationParams GetConcrete(INavigationParams param)
            => param is PageNavigationParams concrete
             ? concrete
             : throw new InvalidCastException();
    }
}
