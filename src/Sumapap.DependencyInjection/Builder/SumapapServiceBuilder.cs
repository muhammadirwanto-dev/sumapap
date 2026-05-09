using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.DependencyInjection.Builder
{
    public sealed class SumapapServiceBuilder(IServiceCollection _services) : IBuilder<IServiceCollection>
    {
        public IServiceCollection Build() => _services;
    }
}