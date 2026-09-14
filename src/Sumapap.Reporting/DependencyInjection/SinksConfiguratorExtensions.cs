using Microsoft.Extensions.DependencyInjection;
using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.Sinks;

namespace Sumapap.Reporting.DependencyInjection
{
    public static class SinksConfiguratorExtensions
    {
        public static SinksConfigurator AddLogger(this SinksConfigurator configurator)
        {
            configurator.Services.AddSingleton<IReportSink, LoggerReportSink>();
            return configurator;
        }
    }
}
