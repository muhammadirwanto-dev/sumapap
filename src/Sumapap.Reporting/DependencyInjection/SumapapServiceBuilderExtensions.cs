using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Reporting.DependencyInjection.Abstractions;

namespace Sumapap.Reporting.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        extension(ISumapapServiceBuilder builder)
        {
            public ISumapapServiceBuilder WithReporting(Action<IReportingBuilder> configuration)
            {
                ArgumentNullException.ThrowIfNull(configuration);

                var reportingBuilder = new ReportingBuilder(builder);

                configuration(reportingBuilder);

                return reportingBuilder.Build();
            }
        }
    }
}
