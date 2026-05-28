namespace Sumapap.Reporting.Abstractions
{
    public interface IReportSink
    {
        bool CanHandle(ReportingModes modes, Report report);

        Task HandleAsync(ReportingModes modes, Report report, CancellationToken cancellationToken = default);
    }
}
