using Microsoft.Extensions.Logging;
using Sumapap.Reporting.Abstractions;

namespace Sumapap.Reporting.Sinks
{
    /// <summary>
    /// Report sink that forwards reports to Microsoft.Extensions.Logging.
    /// </summary>
    internal sealed class LoggerReportSink(ILogger<LoggerReportSink> _logger) : IReportSink
    {
        public bool CanHandle(ReportingModes modes, Report report)
        {
            return _logger.IsEnabled(MapSeverity(report.Severity));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "<Pending>")]
        public Task HandleAsync(ReportingModes modes, Report report, CancellationToken cancellationToken = default)
        {
            var message = report.Message;
            var logSeverity = MapSeverity(report.Severity);

            if (modes.HasFlag(ReportingModes.IncludeStackTrace))
            {
                _logger.Log(
                    logSeverity,
                    report.Exception,
                    message);
            }
            else
            {
                _logger.Log(
                    logSeverity,
                    message);
            }

            return Task.CompletedTask;
        }

        private static LogLevel MapSeverity(ReportSeverity severity) =>
            severity switch
            {
                ReportSeverity.Trace => LogLevel.Trace,
                ReportSeverity.Information => LogLevel.Information,
                ReportSeverity.Warning => LogLevel.Warning,
                ReportSeverity.Error => LogLevel.Error,
                ReportSeverity.Critical => LogLevel.Critical,
                _ => LogLevel.Error
            };
    }
}
