# 💡 Sumapap

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![GitHub Issues](https://img.shields.io/github/issues/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/issues)
[![GitHub Stars](https://img.shields.io/github/stars/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/stargazers)
[![GitHub Forks](https://img.shields.io/github/forks/muhammadirwanto-dev/sumapap?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/network/members)
[![Contributions Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/pulls)
[![Build & Publish](https://github.com/muhammadirwanto-dev/sumapap/actions/workflows/publish.yml/badge.svg?style=flat-square)](https://github.com/muhammadirwanto-dev/sumapap/actions/workflows/publish.yml)

`Sumapap` is an evolving ecosystem of .NET libraries born from real-world development challenges. It reflects my continuous journey as a software engineer — moving away from "perfect" abstractions toward practical, production-ready systems.

> [!IMPORTANT]
> `Sumapap` supersedes [`SingleScope`](https://github.com/muhammadirwanto-dev/singlescope-plugins/tree/main) as my primary project, incorporating years of real-world lessons into an evolving ecosystem of .NET libraries.

## 🤔 What is included in this repository?

This repository contains modular .NET libraries designed for `.NET 10`. These tools focus on decoupling domain logic from infrastructure while maintaining high performance and developer productivity.

| Package | Description | Latest Version | Download |
| --- | --- | --- | --- |
| [Sumapap.Ddd](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Ddd.md) | Lightweight DDD abstractions (Aggregates, Value Objects, Domain Events). | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Ddd)](https://www.nuget.org/packages/Sumapap.Ddd/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd) |
| [Sumapap.Ddd.Dispatcher](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Ddd.Dispatcher.md) | In-memory batch publishing for `IDomainEvent` instances. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Ddd.Dispatcher)](https://www.nuget.org/packages/Sumapap.Ddd.Dispatcher/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.Dispatcher) |
| [Sumapap.Ddd.Mediator](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Ddd.Mediator.md) | Adapts domain events to [Mediator](https://github.com/martinothamar/Mediator) pipelines. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Ddd.Mediator)](https://www.nuget.org/packages/Sumapap.Ddd.Mediator/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Ddd.Mediator) | 
| [Sumapap.Persistence](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Persistence.md) | Abstractions for Repositories, Specifications, and Unit of Work. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence)](https://www.nuget.org/packages/Sumapap.Persistence/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence) | 
| [Sumapap.Persistence.EfCore](https://github.com/muhammadirwanto-dev/sumapap/blob/main/docs/Sumapap.Persistence.EfCore.md) | Entity Framework Core implementation of persistence patterns defined in `Sumapap.Persistence`. | [![NuGet](https://img.shields.io/nuget/v/Sumapap.Persistence.EfCore)](https://www.nuget.org/packages/Sumapap.Persistence.EfCore/) | ![NuGet Downloads](https://img.shields.io/nuget/dt/Sumapap.Persistence.EfCore) | 

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
