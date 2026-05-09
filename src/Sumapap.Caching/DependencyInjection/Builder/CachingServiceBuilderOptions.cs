namespace Sumapap.Caching.DependencyInjection.Builder
{
    public class CachingServiceBuilderOptions
    {
        public string? Tenant { get; set; }

        public string Separator { get; set; } = ":";
    }
}
