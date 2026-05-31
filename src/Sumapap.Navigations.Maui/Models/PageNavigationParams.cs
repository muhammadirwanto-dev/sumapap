namespace Sumapap.Navigations.Maui.Models
{
    public sealed record PageNavigationParams : MauiNavigationParams
    {
        public static PageNavigationParams Empty => new();

        public PageNavigationMode Mode { get; init; } = PageNavigationMode.Normal;

        public NavigationQuery Query { get; init; } = [];

        public PageNavigationParams() { }

        private PageNavigationParams(NavigationQuery query)
            => Query = query;

        private PageNavigationParams(NavigationQuery query, PageNavigationMode mode)
            : this(query)
            => Mode = mode;

        public static PageNavigationParams Create(NavigationQuery query)
            => new(query);

        public static PageNavigationParams Create(NavigationQuery query, PageNavigationMode mode)
            => new(query, mode);

        public static PageNavigationParams Create(params (string key, object value)[] queries)
            => Create(PageNavigationMode.Normal, queries);

        public static PageNavigationParams Create(PageNavigationMode mode, params (string key, object value)[] queries)
        {
            var q = new NavigationQuery();

            foreach (var (key, value) in queries)
            {
                q[key] = value;
            }

            return new(q, mode);
        }
    }
}
