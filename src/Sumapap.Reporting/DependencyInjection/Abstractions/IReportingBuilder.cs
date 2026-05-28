using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Reporting.Options;

namespace Sumapap.Reporting.DependencyInjection.Abstractions
{
    public interface IReportingBuilder : IBuilder<ISumapapServiceBuilder>
    {
        SinksConfigurator Sinks { get; }

        IReportingBuilder AddReporting();

        IReportingBuilder AddReporting(Action<ReportingOptions> configuration);
    }
}
