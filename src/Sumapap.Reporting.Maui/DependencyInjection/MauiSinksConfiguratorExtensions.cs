using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.DependencyInjection;
using Sumapap.Reporting.Maui.Sinks;

namespace Sumapap.Reporting.Maui.DependencyInjection
{
    public static class MauiSinksConfiguratorExtensions
    {
        /// <summary>
        /// Adds a report sink that displays reports in a Maui application using the built-in logging system.
        /// </summary>
        /// <param name="configurator">The sinks configurator.</param>
        /// <returns>The same configurator for method chaining.</returns>
        public static SinksConfigurator AddDialog(this SinksConfigurator configurator)
        {
            configurator.Services.AddSingleton<IReportSink, MauiDialogReportSink>();
            return configurator;
        }
    }
}
