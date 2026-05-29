using Sumapap.Reporting.Abstractions;

namespace Sumapap.Reporting
{
    public sealed class Report(
        Exception exception,
        string? message = null,
        ReportSeverity severity = ReportSeverity.Error,
        IReportMetadata? metadata = null)
    {
        public Exception Exception { get; } = exception;

        public string Message { get; } = message ?? exception.Message;

        public ReportSeverity Severity { get; } = severity;

        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

        public IReportMetadata? Metadata { get; } = metadata;
    }
}
