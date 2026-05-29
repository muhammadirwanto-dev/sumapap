namespace Sumapap.Reporting
{
    /// <summary>
    /// Represents the severity level of a report.
    /// Used to indicate impact, urgency, and handling strategy.
    /// </summary>
    public enum ReportSeverity
    {
        /// <summary>
        /// Diagnostic information useful for debugging.
        /// Should never interrupt user flow.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Non-critical informational message.
        /// </summary>
        Information = 1,

        /// <summary>
        /// Something unexpected happened, but the application can continue.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// A recoverable error that should be reported.
        /// </summary>
        Error = 3,

        /// <summary>
        /// A critical failure that may leave the application in an unstable state.
        /// </summary>
        Critical = 4
    }
}
