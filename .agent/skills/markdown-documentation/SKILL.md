---
name: markdown-documentation
description: 'Create or Update existing Markdown documentations. Use when asked to create or update Markdown documentation for specified modules.'
---

# Markdown Documentation

This skill helps you create or update Markdown documentation for the project. It ensures that documentation is clear, concise, and follows the project's guidelines.

## When Not to Use

- When the documentation is already up-to-date and accurate
- When the request is for a different format (e.g., HTML, PDF)
- When the request is for a non-documentation task (e.g., code implementation, bug fixing)
- When the request is for a task that requires real-time information or data that may change frequently
- When the request is for a task that requires subjective judgment or creative writing beyond technical documentation
- When the request is for a task that can be easily accomplished with existing documentation or resources
- When the request is for a task that requires access to private or sensitive information that should not be included in documentation
- When the request is for a task that requires a level of detail or specificity that is not appropriate for general documentation (e.g., internal implementation details, specific code examples that may become outdated)

### Key Principles

- Focus on clarity and conciseness: Use clear language and avoid unnecessary jargon or technical terms. Keep sentences and paragraphs short and to the point.
- Use consistent formatting: Follow the project's guidelines for headings, lists, code blocks, and other Markdown elements. Use consistent formatting throughout the documentation.
- Include relevant examples: Provide examples that illustrate key concepts or usage scenarios. Ensure that examples are accurate and up-to-date.
- Organize content logically: Structure the documentation in a way that makes it easy to navigate and understand. Use headings and subheadings to break up content into sections.
- Review and update regularly: Documentation should be reviewed and updated regularly to ensure that it remains accurate and relevant. Remove outdated information and add new information as needed.
- Filename same as the module `/src/Sumapap.{Capability}.{Technology}` → `/docs/Sumapap.{Capability}.{Technology}.md`
- All modules should have documentation, even if it's just a brief overview. If a module is deprecated or not intended for public use, this should be clearly stated in the documentation.

### Don'ts

- Don't include sensitive information: Avoid including any secrets, tokens, internal URLs, or other sensitive information in the documentation.
- Don't rewrite class or record definitions in the documentation. Instead, provide a high-level overview and link to the relevant code files for details.
- Don't rewrite method signatures in the documentation. Instead, provide a high-level overview of the method's purpose and link to the relevant code files for details.

## Workflow

### Step 1: Read and understand the module and its context

Build a mental model of the module's purpose, functionality, and how it fits into the larger project. Identify key concepts, usage scenarios, and any existing documentation or resources related to the module.

After investigating, verify:
- [ ] Can summarize the module's purpose and functionality in one paragraph
- [ ] Can identify 3-5 specific scenarios where the module is applicable
- [ ] Can identify common pitfalls or misconceptions about the module
- [ ] Can identify any relevant examples or code snippets that should be included in the documentation
- [ ] Verify if there are obsolete information or patterns that should be removed or updated in the documentation

If there are any ambiguities, gaps in understanding, or multiple valid approaches, ask the user for clarification before proceeding to document creation.

### Step 2: Verify or create the documentation file in the correct location

```
docs/
├── <mmodule-name>.md
```

### Step 3: Generate or update <module-name>.md

Create or update the documentation file with the following sections:
1. **Title** (# Package Name)
2. **Badges** - NuGet version, downloads, license, GitHub issues/stars/forks, contributions welcome
3. **Overview** (## 💡 Overview) - Brief description of what the module does and its core concept
4. **Why?** (## ✨ Why use `Package.Name`?) - Value proposition and benefits
5. **Quick Start** (## 🚀 Quick start) - Step-by-step installation and dependency injection setup (numbered list)
6. **Features and Usage** (## 🛠 Features and usage) - Detailed feature documentation with code examples
7. **Notes & Best Practices** (## ⚠️ Notes & best practices) - Important considerations, gotchas, and recommendations
8. **License** (# ⭐ License) - License reference mention with link to the repository's LICENSE file
9. **Contact** (# 🚩 Contact) - GitHub profile and project URL
10. **Support** (# ☕ Support) - Buy me a coffee section with button

The icon should exactly same as the mentioned section titles above, and the structure should be consistent across all documentation files.
If there's a missing icon's encoding, please use the unicode character directly (e.g., `\u{1F680}` for 🚀) to ensure it renders correctly.

### Step 4: Validate the documentation

Ensure the name:
- Document filename matches the module name (e.g., `Sumapap.Persistence.md` for `Sumapap.Persistence` module)

After creating a documentation, verify:
- [ ] No secrets, tokens, or internal URLs included
- [ ] All code examples are accurate, up-to-date, and optionally tested
- [ ] Documentation is clear, concise, and free of jargon or technical terms that may not be widely understood
- [ ] Documentation follows the project's formatting guidelines and is consistent with other documentation files
- [ ] Documentation includes relevant examples and usage scenarios that illustrate key concepts
- [ ] Documentation is organized logically with appropriate headings and subheadings for easy navigation
- [ ] Add new tests project to `tests/Sumapap.Tests.slnx`

## References

- [Copilot Instructions](../../../.github/copilot-instructions.md)