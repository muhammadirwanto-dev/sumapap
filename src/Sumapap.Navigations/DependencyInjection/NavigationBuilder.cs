using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Navigations.DependencyInjection.Abstractions;

namespace Sumapap.Navigations.DependencyInjection
{
    internal class NavigationBuilder(ISumapapServiceBuilder _builder) : INavigationBuilder
    {
        private readonly IServiceCollection _services = _builder.Services;

        IServiceCollection IBuilder<ISumapapServiceBuilder>.Services => _services;

        public ISumapapServiceBuilder Build()
        {
            return _builder;
        }
    }
}
