using Microsoft.Extensions.DependencyInjection;
using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.DependencyInjection;
using Sumapap.Reporting.Logging.Sinks;

namespace Sumapap.Reporting.Logging.DependencyInjection
{
    public static class SinksConfiguratorExtensions
    {
        extension(SinksConfigurator configurator)
        {
            public SinksConfigurator Logger()
            {
                configurator.Builder.Services.AddSingleton<IReportSink, LoggerReportSink>();

                return configurator;
            }
        }
    }
}
