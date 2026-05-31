using Microsoft.Extensions.DependencyInjection.Extensions;
using Sumapap.Navigations.Abstractions;
using Sumapap.Navigations.DependencyInjection.Abstractions;
using Sumapap.Navigations.Maui.Adapters;

namespace Sumapap.Navigations.Maui.DependencyInjection
{
    public static class NavigationBuilderExtensions
    {
        extension(INavigationBuilder builder)
        {
            public INavigationBuilder UsePageNavigation()
            {
                builder.Services.TryAddSingleton<INavigationService, MauiNavigationService>();
                builder.Services
                    .RemoveAll<INavigationAdapter>()
                    .AddSingleton<INavigationAdapter, PageNavigationAdapter>();

                return builder;
            }

            public INavigationBuilder UseShellNavigation()
            {
                builder.Services.TryAddSingleton<INavigationService, MauiNavigationService>();
                builder.Services
                    .RemoveAll<INavigationAdapter>()
                    .AddSingleton<INavigationAdapter, ShellNavigationAdapter>();

                return builder;
            }
        }
    }
}
