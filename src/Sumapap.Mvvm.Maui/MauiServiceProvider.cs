namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static IServiceProvider Current => IPlatformApplication.Current?.Services!;
    }
}
