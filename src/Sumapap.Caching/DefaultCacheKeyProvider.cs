using System.Text;
using Microsoft.Extensions.Options;
using Sumapap.Caching.Abstractions;
using Sumapap.Caching.DependencyInjection.Builder;

namespace Sumapap.Caching
{
    internal class DefaultCacheKeyProvider(
        IOptions<CachingServiceBuilderOptions> options
        ) : ICacheKeyProvider
    {
        private readonly CachingServiceBuilderOptions _options = options.Value;

        public string CreateKey(string @object, params object[] parameters)
        {
            var sb = new StringBuilder();
            var sp = _options.Separator;

            if (!string.IsNullOrWhiteSpace(_options.Tenant))
            {
                sb.Append($"{_options.Tenant}{sp}");
            }

            sb.Append($"{@object}{sp}{string.Join(sp, parameters)}");

            return sb.ToString().ToLowerInvariant();
        }

        public string CreateKey<TObject>(params object[] parameters)
            => CreateKey(typeof(TObject).Name, parameters);
    }
}
