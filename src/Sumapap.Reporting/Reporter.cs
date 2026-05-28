using Microsoft.Extensions.Options;
using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.Options;

namespace Sumapap.Reporting
{
    internal sealed class Reporter(
        IEnumerable<IReportSink> sinks,
        IOptions<ReportingOptions> options
        ) : IReporter
    {
        private readonly IEnumerable<IReportSink> _sinks = sinks;
        private readonly ReportingOptions _options = options.Value;

        /// <inheritdoc/>
        public void Report(Exception exception)
        {
            _ = ReportAsync(exception);
        }

        /// <inheritdoc/>
        public void Report(Exception exception, ReportSeverity severity, IReportMetadata? metadata = null)
        {
            _ = ReportAsync(exception, severity, metadata);
        }

        /// <inheritdoc/>
        public void Report(Report report)
        {
            _ = ReportAsync(report);
        }

        /// <inheritdoc/>
        public Task ReportAsync(Exception exception, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return ReportAsync(
                exception,
                ReportSeverity.Error,
                metadata: null,
                cancellationToken);
        }

        /// <inheritdoc/>
        public Task ReportAsync(Exception exception, ReportSeverity severity, IReportMetadata? metadata = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return ReportAsync(report: new Report(
                exception,
                severity: severity,
                metadata: metadata),
                cancellationToken);
        }

        /// <inheritdoc/>
        public async Task ReportAsync(Report report, CancellationToken cancellationToken = default)
        {
            foreach (var sink in _sinks)
            {
                if (!sink.CanHandle(_options.Modes, report))
                {
                    continue;
                }

                try
                {
                    await sink.HandleAsync(_options.Modes, report, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch
                {
                    // Swallow exceptions from sinks to prevent cascading failures.
                    // In a real implementation, consider logging this internally.
                }
            }
        }
    }
}
