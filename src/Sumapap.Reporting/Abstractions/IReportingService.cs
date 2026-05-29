namespace Sumapap.Reporting.Abstractions
{
    /// <summary>
    /// Provides a centralized service for reporting errors and exceptions.
    /// The service orchestrates report creation and dispatches it to configured sinks.
    /// </summary>
    public interface IReportingService
    {
        void Report(Exception exception);

        void Report(Exception exception, ReportSeverity severity, IReportMetadata? metadata = null);

        void Report(Report report);

        /// <summary>
        /// Reports an exception using the default reporting configuration.
        /// </summary>
        /// <param name="exception">The exception to report.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ReportAsync(Exception exception, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reports an exception with additional context and severity.
        /// </summary>
        /// <param name="exception">The exception to report.</param>
        /// <param name="severity">Severity level of the report.</param>
        /// <param name="metadata">Optional metadata information.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ReportAsync(Exception exception, ReportSeverity severity, IReportMetadata? metadata = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reports a pre-constructed report.
        /// Intended for advanced scenarios or custom pipelines.
        /// </summary>
        /// <param name="report">The report to dispatch.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ReportAsync(Report report, CancellationToken cancellationToken = default);
    }
}
