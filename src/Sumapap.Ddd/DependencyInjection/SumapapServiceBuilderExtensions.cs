using Sumapap.Ddd.DependencyInjection.Abstractions;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Ddd.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        extension(ISumapapServiceBuilder builder)
        {
            public ISumapapServiceBuilder WithDdd(Action<IDddBuilder> configuration)
            {
                ArgumentNullException.ThrowIfNull(configuration);

                var dddBuilder = new DddBuilder(builder);

                configuration(dddBuilder);

                return dddBuilder.Build();
            }
        }
    }
}
