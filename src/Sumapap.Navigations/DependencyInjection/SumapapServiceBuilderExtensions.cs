using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Navigations.DependencyInjection.Abstractions;

namespace Sumapap.Navigations.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        extension(ISumapapServiceBuilder builder)
        {
            public ISumapapServiceBuilder WithNavigations(Action<INavigationBuilder> configuration)
            {
                ArgumentNullException.ThrowIfNull(configuration);

                var navigationBuilder = new NavigationBuilder(builder);

                configuration(navigationBuilder);

                return navigationBuilder.Build();
            }
        }
    }
}
