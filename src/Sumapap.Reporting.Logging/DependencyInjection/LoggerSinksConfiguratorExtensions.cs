using Microsoft.Extensions.DependencyInjection;
using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.DependencyInjection;
using Sumapap.Reporting.Logging.Sinks;

namespace Sumapap.Reporting.Logging.DependencyInjection
{
    public static class LoggerSinksConfiguratorExtensions
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
