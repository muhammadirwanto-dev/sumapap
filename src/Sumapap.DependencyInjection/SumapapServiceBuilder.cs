using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.DependencyInjection
{
    internal sealed class SumapapServiceBuilder(IServiceCollection _services) : ISumapapServiceBuilder
    {
        IServiceCollection IBuilder<IServiceCollection>.Services => _services;

        public IServiceCollection Build() => _services;
    }
}