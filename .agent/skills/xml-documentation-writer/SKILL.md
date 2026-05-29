---
name: xml-documentation-writer
description: Create and update C# XML documentation comments using interface-first docs, inheritdoc for implementations, and full docs for non-interface public/internal members. Use when asked to write or update XML documentation comments in a C# project.
---

# XML Documentation Writer

This skill focuses on creating and updating XML documentation comments in C# code. It emphasizes an interface-first approach, using `<inheritdoc />` for implementations, and ensuring that all relevant public and internal members are properly documented.

## When to Use
Apply this skill whenever a request mentions any of the following:
- Writing XML documentation comments
- Updating existing XML docs
- Improving code comments for C# APIs
- Ensuring IntelliSense-friendly API documentation

## Scope
This skill governs XML documentation comments for:
- Interfaces
- Interface members (properties and methods)
- Classes implementing interfaces
- Non-interface `public` and `internal` properties and methods

## Core Principles

### 1) Interface-first documentation
Write XML documentation comments on all interface declarations and all declared interface members.

Required targets:
- `interface` declarations
- Interface `property` declarations
- Interface `method` declarations

### 2) Use `<inheritdoc />` on implementations
For classes and class members that implement interface contracts, use `<inheritdoc />` rather than duplicating text.

Required targets:
- Class declaration implementing one or more interfaces
- Implementing properties
- Implementing methods

Use inherited docs for:
- Explicit interface implementations
- Implicit interface implementations

### 3) Document non-interface API surface
For `public` and `internal` properties and methods that are **not** interface implementations, write full XML documentation comments.

Required targets:
- `public` properties and methods not implementing interface members
- `internal` properties and methods not implementing interface members

## Authoring Rules

### XML tags
Prefer the following standard tags:
- `<summary>`: concise behavior description
- `<param>`: for each method parameter
- `<returns>`: for non-`void` methods
- `<exception>`: for meaningful thrown exceptions (when relevant)
- `<remarks>`: optional, for important usage notes
- `<inheritdoc />`: for interface implementations

### Style
- Use clear, imperative, and domain-accurate wording.
- Keep `<summary>` concise (typically 1–2 sentences).
- Ensure parameter docs match parameter names exactly.
- Avoid copy-pasting duplicate summaries where inheritance applies.
- Do not add XML docs to private members unless explicitly requested.

## Decision Workflow
For each C# type/member encountered during documentation work:

1. Is it an interface declaration?
   - Yes → write full XML docs.
2. Is it an interface property/method declaration?
   - Yes → write full XML docs.
3. Is it a class/type that implements an interface?
   - Yes → use `<inheritdoc />` on the type.
4. Is it a property/method implementing an interface member?
   - Yes → use `<inheritdoc />`.
5. Is it `public` or `internal` and not implementing an interface member?
   - Yes → write full XML docs.
6. Otherwise:
   - Leave undocumented unless the request explicitly expands scope.

## Quality Checklist
Before finishing:
- [ ] All interfaces and interface members are documented.
- [ ] All implementations of interface members use `<inheritdoc />`.
- [ ] All non-interface `public`/`internal` properties and methods are documented.
- [ ] XML is valid and tag structure is complete.
- [ ] Documentation text reflects actual behavior/signature.

## Output Expectations
When applying this skill:
- Modify only relevant C# files.
- Preserve existing coding style and formatting.
- Keep comments precise and non-redundant.
- Report which files were updated and what rule(s) were applied.

## Notes
- If project conventions differ (for example, mandatory `<remarks>` everywhere), follow repository conventions.
- If documentation source-of-truth exists elsewhere, align wording to that source.