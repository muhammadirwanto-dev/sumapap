# Sumapap.Reporting

[![NuGet Version](https://img.shields.io/nuget/v/Sumapap.Reporting.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Reporting.svg?style=flat-square)](https://www.nuget.org/packages/Sumapap.Reporting/)
[![License](https://img.shields.io/github/license/muhirwanto-dev/sumapap?style=flat-square)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/muhirwanto-dev/sumapap?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhirwanto-dev/sumapap?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhirwanto-dev/sumapap?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhirwanto-dev/sumapap/pulls)

## Overview

A lightweight, extensible **error and exception reporting pipeline** for .NET applications.

`Sumapap.Reporting` captures runtime errors and forwards them to configurable reporting targets (called *sinks*), such as logging systems, UI dialogs, databases, or external services.

## ✨ Why Sumapap.Reporting?

Most applications handle errors in scattered places:
- some log to files
- some show dialogs
- some swallow exceptions
- some crash the app

`Sumapap.Reporting` centralizes this responsibility into a **single reporting pipeline**.

You decide:
- **what** to report
- **where** it goes
- **how much detail** is included

## 🎯 Design Goals

- Cross-platform (MAUI, Web, Console, Worker)
- No UI or platform dependencies in the core
- Pluggable output destinations (sinks)
- Async-friendly and safe
- Clear separation of concerns

## 🚦 Reporting Modes

Reporting behavior is controlled via `ReportingMode` flags:

```csharp
    [Flags]
    public enum ReportingMode : uint
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
```

## 🔧 Basic Usage

```csharp
    // inject to container
    builder.Services.AddSumapapReporting()
        .AddLogReporting()      // report as logging
        .AddDialogReporting();  // report as dialog

    // use somewhere in your code
    try
    {
        // application code
    }
    catch (Exception ex)
    {
        await reportingService.ReportAsync(ex);
    }
```

## Contributions

Contributions are welcome! If you encounter a bug, have a suggestion, or want to contribute code, please follow these steps:

1.  Check the [GitHub Issues](https://github.com/muhirwanto-dev/sumapap/issues) to see if your issue or idea has already been reported.
2.  If not, open a new issue to describe the bug or feature request.
3.  **For code contributions:**
    * Fork the Project repository.
    * Create your Feature Branch (`git checkout -b feature/YourAmazingFeature`).
    * Commit your Changes (`git commit -m 'Add YourAmazingFeature'`). Adhere to conventional commit messages if possible.
    * Push to the Branch (`git push origin feature/YourAmazingFeature`).
    * Open a Pull Request against the `main` branch of the original repository.
4.  Please try to follow the existing coding style and include unit tests for new or modified functionality.

## License

Distributed under the [MIT License](https://github.com/muhirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

## Contact

[@muhirwanto-dev](https://github.com/muhirwanto-dev)

Project link: [https://github.com/muhirwanto-dev/sumapap/tree/main/source/Sumapap.Reporting](https://github.com/muhirwanto-dev/sumapap/tree/main/source/Sumapap.Reporting)
