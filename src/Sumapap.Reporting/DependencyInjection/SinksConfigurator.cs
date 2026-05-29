using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.Reporting.DependencyInjection
{
    public class SinksConfigurator(IServiceCollection services)
    {
        public IServiceCollection Services => services;
    }
}