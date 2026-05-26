---
name: xml-documentation
description: 'Create or Update XML documentations. Use when asked to create or update XML documentation for interfaces and their methods.'
---

# XML Documentation

This skill helps you create or update XML documentation for interfaces and their methods. It ensures that documentation is clear, concise, and follows the project's guidelines.

# When to Use This Skill

- When asked to create or update XML documentation for interfaces and their methods.
- When the existing XML documentation is outdated, incomplete, or missing for interfaces and their methods.

# Ruleset

- Focus on clarity and conciseness: Use clear language and avoid unnecessary jargon or technical terms. Keep sentences short and to the point.
- Use consistent formatting: Follow the project's guidelines for XML documentation, including the use of `<summary>`, `<param>`, `<returns>`, and other relevant tags. Use consistent formatting throughout the documentation.
- Add the documentation directly above the interface and method declaration in the code.
- Don't add the documentation above the class and its members and methods, unless it doesn't have any interfaces.
- If you need to add the documentation for classes which are not implementing interface, don't add the documentation for constructors, unless it is necessary for understanding the public API.
- If you need to add the documentation for classes which are not implementing interface, don't add the documentation for private members, unless it is necessary for understanding the public API.

# References

[XML Documentation Comments (C# Programming Guide)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)