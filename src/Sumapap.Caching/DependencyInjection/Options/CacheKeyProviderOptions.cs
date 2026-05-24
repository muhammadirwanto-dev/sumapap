namespace Sumapap.Caching.DependencyInjection.Options
{
    public class CacheKeyProviderOptions
    {
        public string? Tenant { get; set; }

        public string Separator { get; set; } = ":";
    }
}
