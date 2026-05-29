using Sumapap.Reporting.Abstractions;

namespace Sumapap.Reporting.Maui.Sinks
{
    /// <summary>
    /// MAUI report sink that displays an error dialog to the user.
    /// </summary>
    internal sealed class MauiDialogReportSink : IReportSink
    {
        public bool CanHandle(ReportingModes modes, Report report)
        {
            if (modes.HasFlag(ReportingModes.Silent))
                return false;

            if (modes.HasFlag(ReportingModes.Background))
                return false;

            return report.Severity >= ReportSeverity.Error;
        }

        public Task HandleAsync(
            ReportingModes modes,
            Report report,
            CancellationToken cancellationToken = default)
        {
            var page = Application.Current?.Windows[0].Page;
            var message = modes.HasFlag(ReportingModes.IncludeStackTrace) && report.Exception != null
                ? $"{report.Message}\n\n{report.Exception}"
                : report.Message;

#if NET10_0_OR_GREATER
            return MainThread.InvokeOnMainThreadAsync(() => page?.DisplayAlertAsync(
#else
            return MainThread.InvokeOnMainThreadAsync(() => page?.DisplayAlert(
#endif // NET10_0_OR_GREATER
                "Error",
                message,
                "Ok"
                ));
        }
    }
}
