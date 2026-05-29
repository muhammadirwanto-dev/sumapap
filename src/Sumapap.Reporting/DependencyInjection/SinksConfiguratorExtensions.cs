using Microsoft.Extensions.DependencyInjection;
using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.Sinks;

namespace Sumapap.Reporting.DependencyInjection
{
    public static class SinksConfiguratorExtensions
    {
        extension(SinksConfigurator configurator)
        {
            public SinksConfigurator AddLogger()
            {
                configurator.Services.AddSingleton<IReportSink, LoggerReportSink>();
                return configurator;
            }
        }
    }
}
