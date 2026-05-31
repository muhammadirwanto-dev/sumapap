namespace Sumapap.Navigations.Abstractions
{
    public interface INavigationAdapter : INavigationService
    {
        bool CanHandle();
    }
}
