namespace Sumapap.Reporting
{
    [Flags]
    public enum ReportingModes : uint
    {
        None = 0,

        /// <summary>
        /// Suppresses all user-facing reporting.
        /// Sinks may still process the report (e.g. logging).
        /// </summary>
        Silent = 1 << 0,

        /// <summary>
        /// Include full exception stack trace in the report.
        /// </summary>
        IncludeStackTrace = 1 << 1,

        /// <summary>
        /// Indicates the report originates from a background process.
        /// UI sinks should ignore this.
        /// </summary>
        Background = 1 << 2,

        /// <summary>
        /// Indicates the error is user-actionable and may require attention.
        /// UI sinks may emphasize this.
        /// </summary>
        UserActionRequired = 1 << 3,

        /// <summary>
        /// Default reporting behavior.
        /// </summary>
        Default = IncludeStackTrace
    }
}
