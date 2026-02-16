# Contributing to CDS.FluentHtmlReports

Thank you for considering contributing to CDS.FluentHtmlReports! This document outlines the process for contributing to this project.

## Code of Conduct

Be respectful, constructive, and professional in all interactions.

## How Can I Contribute?

### Reporting Bugs

- Use the GitHub issue tracker
- Check if the issue already exists
- Include a clear title and description
- Provide code samples and expected vs. actual behavior
- Mention your .NET version and OS

### Suggesting Features

- Use the GitHub issue tracker with the "enhancement" label
- Explain the use case and why it would be valuable
- Keep the scope focused on HTML report generation

### Pull Requests

1. **Fork the repository** and create your branch from `master`
2. **Follow the coding guidelines** (see below)
3. **Add tests** for any new functionality
4. **Ensure all tests pass**: `dotnet test`
5. **Update documentation** in README.md if needed
6. **Update CHANGELOG.md** under the `[Unreleased]` section
7. **Submit a pull request**

## Development Setup

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/CDS.FluentHtmlReports.git
cd CDS.FluentHtmlReports

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Run console test app
dotnet run --project ConsoleTest
```

## Coding Guidelines

This project follows the guidelines in `.github/copilot-instructions.md`:

### Style
- Use Allman braces (opening brace on new line)
- File-scoped namespaces (`namespace X;`)
- Use `var` when type is obvious
- Enable nullable reference types
- PascalCase for public members
- `_camelCase` for private fields
- XML documentation for all public APIs

### Quality
- Write meaningful XML documentation comments
- Add inline comments for complex logic
- Validate inputs with guard clauses
- Use `async`/`await` for I/O operations
- HTML-encode all user content to prevent XSS

### Testing
- Use MSTest with FluentAssertions
- Follow Arrange-Act-Assert pattern
- Use descriptive test names (e.g., `AddParagraph_EncodesLessThan`)
- Use `[TestCategory]` to organize tests
- Test both positive and negative scenarios

## Versioning

This project uses [MinVer](https://github.com/adamralph/minver) for automatic semantic versioning based on git tags:

- Version is derived from git tags (e.g., `v1.2.3`)
- Tag format: `v{major}.{minor}.{patch}`
- Maintainers create tags for releases

## Release Process

1. Update `CHANGELOG.md` with release notes
2. Create and push a version tag: `git tag v1.2.3 && git push origin v1.2.3`
3. GitHub Actions automatically:
   - Builds and tests the project
   - Creates a GitHub release with generated notes
   - Publishes the NuGet package

## Questions?

Open an issue with the "question" label or start a discussion.
