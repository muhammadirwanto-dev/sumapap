using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.DependencyInjection
{
    public sealed class SumapapBuilder(IServiceCollection services) : ISumapapBuilder
    {
        public IServiceCollection Services => services;
    }
}

