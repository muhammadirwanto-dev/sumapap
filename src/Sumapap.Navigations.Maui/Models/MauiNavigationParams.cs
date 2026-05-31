using Sumapap.Navigations.Abstractions;

namespace Sumapap.Navigations.Maui.Models
{
    public abstract record MauiNavigationParams : INavigationParams
    {
        public bool Animated { get; init; } = true;
    }
}
