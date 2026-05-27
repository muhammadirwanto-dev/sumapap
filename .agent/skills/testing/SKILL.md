---
name: testing
description: 'Implementation details for Sumapap test infrastructure. Use when changing test fixtures, assertions, test helpers, the test class hierarchy, or when adding new tests.'
user-invocable: false
---

# Testing
It's skill to understand and modify the test infrastructure for Sumapap. This includes test fixtures, assertions, test helpers, the test class hierarchy, and adding new tests.

## Principles
- **DRY**: Avoid duplication by using base classes and shared test helpers.
- **Provider override pattern**: Define tests in abstract base classes, then override in provider-specific test classes to assert provider-specific behavior (e.g., SQL).
- **Test categorization**: Organize tests into unit, specification, and functional tests based on their scope and dependencies.
- **Fixture selection**: Choose between shared store fixtures for read-heavy tests and non-shared model fixtures for tests needing unique schemas.

## Ruleset
- When adding a new test, first add it to the appropriate specification test base class. Then override it in every provider-specific functional test class that inherits from that base.
- When asserting SQL, use `AssertSql("""...""")` in the provider override. If the SQL changes due to a code change, re-run with `EF_TEST_REWRITE_BASELINES=1` to update the baseline.
- When testing cross-platform code, verify that the test passes on both Windows and Linux/macOS to ensure there are no platform-specific issues (e.g., path separators).
- When modifying test fixtures or helpers, ensure that existing tests still pass and that new tests can be added without significant boilerplate.
- Use `Moq` for unit tests to mock dependencies and isolate logic without needing a database.
- Name the folder structure to reflect the test categorization and module organization (e.g., `tests/{Module}.Specification.Tests/`, `tests/{Module}.Relational.Specification.Tests/`).
- Name the test projects and classes according to their purpose and scope (e.g., `QueryTests`, `PaginationQueryTestBase`, `SqlServerTestHelpers`).

## Folder Structure
```
src/
  {Module}/
    ... (module code)
tests/
  {Module}.Tests/                           # Unit tests (no database)
  {Module}.Specification.Tests/             # Provider-agnostic specification tests
    TestHelpers.cs                          # Base test helpers
  {Module}.Relational.Specification.Tests/  # Relational specification tests
```

## Test Categories

### Unit Tests
Isolated logic tests. No database needed.

### Specification Tests (provider-agnostic abstract bases)
Define WHAT to test (LINQ queries, expected results). Can't be run directly — provider tests override to verify HOW.

## Test Class Hierarchy (Query Example)

```
QueryTestBase<TFixture>                                # Core
  └─ PaginationQueryTestBase<TFixture>                 # Specification
      └─ PaginationQueryRelationalTestBase<TFixture>   # Relational specification
          └─ PaginationQuerySqlServerTest              # Provider
```

## TestHelpers Hierarchy

```
TestHelpers (abstract)                  # {Module}.Specification.Tests
  ├─ InMemoryTestHelpers                # non-relational
  └─ RelationalTestHelpers (abstract)   # {Module}.Relational.Specification.Tests
      ├─ SqlServerTestHelpers
      └─ SqliteTestHelpers
```

## Workflow: Adding New Tests

1. **Specification test**: Add to `tests/{Module}.Specification.Tests/` (core) or `tests/{Module}.Relational.Specification.Tests/` (relational)
2. **Unit test**: Add to `tests/{Module}.Tests/`
3. **Provider overrides**: Override in **every** provider functional test class (`tests/{Module}.Tests/{Module}.{Provider}.Tests.cs`) that inherits from the base with provider-appropriate assertions.
4. Run tests with project rebuilding enabled (don't use `--no-build`) to ensure code changes are picked up
5. When testing cross-platform code (e.g., file paths, path separators), verify the fix works on both Windows and Linux/macOS

## References

[EF Core Testing Skill](https://github.com/dotnet/efcore/blob/main/.agents/skills/testing)