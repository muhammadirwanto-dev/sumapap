# 💡 Sumapap

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)
[![Build & Publish](https://github.com/muhammadirwanto-dev/sumapap/actions/workflows/cd-publish-nuget.yaml/badge.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/actions/workflows/cd-publish-nuget.yaml)
[![Code Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/muhammadirwanto-dev/41038afccedef8e2268247b2d75d71e0/raw/sumapap-coverage.json&style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/actions/workflows/ci-test-coverage.yaml)

`Sumapap` is an evolving ecosystem of .NET libraries born from real-world development challenges. It reflects my continuous journey as a software engineer — moving away from "perfect" abstractions toward practical, production-ready systems.

> [!IMPORTANT]
> `Sumapap` supersedes [`SingleScope`](https://github.com/muhammadirwanto-dev/singlescope-plugins/tree/main) as my primary project, incorporating years of real-world lessons into an evolving ecosystem of .NET libraries.

## 🤔 What is included in this repository?

This repository contains modular .NET libraries designed for `.NET 10`. These tools focus on decoupling domain logic from infrastructure while maintaining high performance and developer productivity.

### Domain-Driven Design (DDD)

| Package | Description | Latest Version | Download |
| --- | --- | --- | --- |
| [Sumapap.Ddd.Abstractions](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Ddd.Abstractions.md) | Core abstractions for DDD patterns (interfaces only, no implementations). | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Ddd.Abstractions)](https://www.nuget.org/packages/Sumapap.Ddd.Abstractions/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.Abstractions) |
| [Sumapap.Ddd](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Ddd.md) | Lightweight DDD implementations (Aggregates, Value Objects, Domain Events). | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Ddd)](https://www.nuget.org/packages/Sumapap.Ddd/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd) |
| [Sumapap.Ddd.Mediator](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Ddd.Mediator.md) | Adapts domain events to [Mediator](https://github.com/martinothamar/Mediator) pipelines. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Ddd.Mediator)](https://www.nuget.org/packages/Sumapap.Ddd.Mediator/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.Mediator) |

### Persistence & Data Access

| Package | Description | Latest Version | Download |
| --- | --- | --- | --- |
| [Sumapap.Persistence.Abstractions](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Persistence.Abstractions.md) | Core abstractions for repositories and persistence patterns. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence.Abstractions)](https://www.nuget.org/packages/Sumapap.Persistence.Abstractions/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.Abstractions) |
| [Sumapap.Persistence](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Persistence.md) | Repository patterns, Specifications, and Unit of Work implementations. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence)](https://www.nuget.org/packages/Sumapap.Persistence/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence) |
| [Sumapap.Persistence.EfCore](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Persistence.EfCore.md) | Entity Framework Core implementation of persistence patterns. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence.EfCore)](https://www.nuget.org/packages/Sumapap.Persistence.EfCore/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.EfCore) |
| [Sumapap.Persistence.Caching](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Persistence.Caching.md) | Decorator-based caching layer for repositories with tag-based invalidation. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence.Caching)](https://www.nuget.org/packages/Sumapap.Persistence.Caching/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.Caching) |
| [Sumapap.Persistence.Caching.FusionCache](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Persistence.Caching.FusionCache.md) | FusionCache-based implementation of persistence caching. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence.Caching.FusionCache)](https://www.nuget.org/packages/Sumapap.Persistence.Caching.FusionCache/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.Caching.FusionCache) |

### Queries & Filtering

| Package | Description | Latest Version | Download |
| --- | --- | --- | --- |
| [Sumapap.Queries.Abstractions](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Queries.Abstractions.md) | Core query abstractions for filtering, sorting, and pagination. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Queries.Abstractions)](https://www.nuget.org/packages/Sumapap.Queries.Abstractions/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Queries.Abstractions) |
| [Sumapap.Queries](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Queries.md) | Query execution infrastructure with support for IQueryable and IEnumerable. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Queries)](https://www.nuget.org/packages/Sumapap.Queries/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Queries) |

### Infrastructure & Utilities

| Package | Description | Latest Version | Download |
| --- | --- | --- | --- |
| [Sumapap.Common](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Common.md) | Shared utilities and extensions used across Sumapap libraries. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Common)](https://www.nuget.org/packages/Sumapap.Common/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Common) |
| [Sumapap.Caching](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Caching.md) | Cache key provider abstractions with consistent key generation. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Caching)](https://www.nuget.org/packages/Sumapap.Caching/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Caching) |
| [Sumapap.DependencyInjection](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.DependencyInjection.md) | Service registration helpers and DI utilities. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.DependencyInjection)](https://www.nuget.org/packages/Sumapap.DependencyInjection/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.DependencyInjection) |
| [Sumapap.Reporting](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Reporting.md) | Centralized error and exception reporting with sink-based architecture. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Reporting)](https://www.nuget.org/packages/Sumapap.Reporting/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Reporting) |
| [Sumapap.Reporting.Maui](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Reporting.Maui.md) | MAUI report sinks for displaying errors in mobile/desktop apps. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Reporting.Maui)](https://www.nuget.org/packages/Sumapap.Reporting.Maui/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Reporting.Maui) |

### UI & Navigation

| Package | Description | Latest Version | Download |
| --- | --- | --- | --- |
| [Sumapap.Navigations](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Navigations.md) | Navigation abstractions for MVVM applications. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Navigations)](https://www.nuget.org/packages/Sumapap.Navigations/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Navigations) |

# ⭐ Naming Convention

We follow a strict capability-technology hierarchy:
```jsx
Sumapap.<Capability>.<Technology>
```

# 🚩 Layer Diagram

![image.png](assets/layer-diagram.png)

The image is a **layered architecture diagram** showing how different libraries and modules are organized and layered to support a scalable system.

> [!TIP]
> **The Dependency Rule:** Dependencies always flow downward. Foundational abstractions (bottom) are technology-agnostic, while implementation layers (top) handle specific frameworks.

# 🛡️ Disclaimer
This project is an honest reflection of my professional journey. While it is used in production environments, I encourage users to **review the source code against their specific performance and security requirements**. `Sumapap` is not a "perfect" framework — it is a set of tools that continues to evolve as I learn and grow.

# 🚀 Getting Started

Please read the documentation for each respective library in the [/docs](https://github.com/muhammadirwanto-dev/sumapap/tree/main/docs) folder.

# 💪 Contributions

Contributions are welcome! If you encounter a bug, have a suggestion, or want to contribute code, please follow these steps:

1. Check the [GitHub Issues](https://github.com/muhammadirwanto-dev/sumapap/issues) to see if your issue or idea has already been reported.
2. If not, open a new issue to describe the bug or feature request.
3. For code contributions:
   * Fork the Project repository (`https://github.com/muhammadirwanto-dev/sumapap`).
   * Create your Feature Branch (`git checkout -b feature/YourAmazingFeature`).
   * Commit your Changes. Adhere to conventional commit messages if possible.
   * Push to the Branch and open a Pull Request against `main`.
4. Please try to follow the existing coding style and include unit tests for new or modified functionality.

# ⭐ License

Distributed under the [MIT License](https://github.com/muhammadirwanto-dev/sumapap/tree/main?tab=MIT-1-ov-file#readme). See the `LICENSE` file in the repository for more information.

# 🚩 Contact

`GitHub` [@muhammadirwanto-dev](https://github.com/muhammadirwanto-dev)  
`Project Url` https://github.com/muhammadirwanto-dev/sumapap/tree/main/source/Sumapap

# ☕ Support

If you like this project and want to support it, you can [buy me a coffee︎](https://buymeacoffee.com/muhirwanto.dev). Your coffee will keep me awake while developing this project ☕.

<p align="center">
  <a href="https://buymeacoffee.com/muhirwanto.dev">
    <img src="https://img.buymeacoffee.com/button-api/?text=Buy%20me%20a%20coffee&emoji=&slug=muhirwanto.dev&button_colour=FFDD00&font_colour=000000&font_family=Comic&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee">
  </a>
</p>
