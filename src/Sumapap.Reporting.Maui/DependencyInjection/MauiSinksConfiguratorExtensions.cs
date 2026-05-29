using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.DependencyInjection;
using Sumapap.Reporting.Maui.Sinks;

namespace Sumapap.Reporting.Maui.DependencyInjection
{
    public static class MauiSinksConfiguratorExtensions
    {
        extension(SinksConfigurator configurator)
        {
            /// <summary>
            /// Adds a report sink that displays reports in a Maui application using the built-in logging system.
            /// </summary>
            /// <returns>The same configurator for method chaining.</returns>
            public SinksConfigurator AddDialog()
            {
                configurator.Services.AddSingleton<IReportSink, MauiDialogReportSink>();
                return configurator;
            }
        }
    }
}
