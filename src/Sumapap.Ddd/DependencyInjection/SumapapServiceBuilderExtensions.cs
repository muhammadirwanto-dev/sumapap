using Sumapap.Ddd.DependencyInjection.Abstractions;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Ddd.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        public static ISumapapServiceBuilder WithDdd(this ISumapapServiceBuilder builder, Action<IDddBuilder> configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var dddBuilder = new DddBuilder(builder);

            configuration(dddBuilder);

            return dddBuilder.Build();
        }
    }
}
