namespace Sumapap.Navigations.Maui.Models
{
    public sealed record ShellNavigationParams : MauiNavigationParams
    {
        public const string Relative = "";
        public const string RelativeTo = "/";
        public const string Absolute = "//";
        public const string AbsoluteClearStack = "///";

        public static ShellNavigationParams Empty => new();

        public NavigationQuery Query { get; init; } = [];

        public string ShellRoute { get; set; } = Relative;

        public string CombinePath(string path)
            => $"{ShellRoute}{path}";

        private ShellNavigationParams() { }

        private ShellNavigationParams(NavigationQuery query)
            => Query = query;

        private ShellNavigationParams(NavigationQuery query, string route)
            : this(query)
            => ShellRoute = route;

        public static ShellNavigationParams Create(NavigationQuery query)
            => new(query);

        public static ShellNavigationParams Create(NavigationQuery query, string route)
            => new(query, route);

        public static ShellNavigationParams Create(params (string key, object value)[] queries)
            => Create(Relative, queries);

        public static ShellNavigationParams Create(string route, params (string key, object value)[] queries)
        {
            var q = new NavigationQuery();

            foreach (var (key, value) in queries)
            {
                q[key] = value;
            }

            return new(q, route);
        }
    }
}
