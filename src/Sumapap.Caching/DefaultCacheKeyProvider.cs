using System.Text;
using Microsoft.Extensions.Options;
using Sumapap.Caching.Abstractions;
using Sumapap.Caching.DependencyInjection.Options;
using Sumapap.Common.Extensions;

namespace Sumapap.Caching
{
    internal class DefaultCacheKeyProvider(
        IOptions<CacheKeyProviderOptions> options
        ) : ICacheKeyProvider
    {
        private readonly CacheKeyProviderOptions _options = options.Value;

        public string CreateKey<TObject>(TObject @object, params object[] parameters)
            where TObject : class
        {
            var sb = new StringBuilder();
            var sp = _options.Separator;

            if (!string.IsNullOrWhiteSpace(_options.Tenant))
            {
                sb.Append($"{_options.Tenant}{sp}");
            }

            var objString = @object is string str
                ? str
                : @object.GetContentHash();

            sb.Append($"{objString}{sp}{string.Join(sp, ToContentHashList(parameters))}");

            return sb.ToString().ToKebabCase();
        }

        public string CreateKey<TObject>(params object[] parameters)
            where TObject : class
            => CreateKey(typeof(TObject).Name, parameters);

        private static string[] ToContentHashList(object[] objects)
            => [.. objects.Select(o => o is string str ? str : o.GetContentHash())];
    }
}
