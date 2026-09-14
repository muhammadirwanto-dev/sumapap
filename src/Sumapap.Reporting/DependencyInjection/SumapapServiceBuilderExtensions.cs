using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Reporting.DependencyInjection.Abstractions;

namespace Sumapap.Reporting.DependencyInjection
{
    public static class SumapapServiceBuilderExtensions
    {
        public static ISumapapServiceBuilder WithReporting(this ISumapapServiceBuilder builder, Action<IReportingBuilder> configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var reportingBuilder = new ReportingBuilder(builder);

            configuration(reportingBuilder);

            return reportingBuilder.Build();
        }
    }
}
