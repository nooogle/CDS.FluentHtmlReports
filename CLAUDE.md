# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build
dotnet build --configuration Release

# Test
dotnet test
dotnet test --filter "FullyQualifiedName~GeneratorTests.AddParagraph"   # single test
dotnet test --filter "TestCategory=Chart"                                # by category

# Test with coverage (matches CI)
dotnet test --configuration Release --no-build --verbosity normal --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"

# Pack NuGet package
dotnet pack CDS.FluentHtmlReports/CDS.FluentHtmlReports.csproj --configuration Release

# Run demo application (generates HTML sample reports)
dotnet run --project ConsoleTest
```

Test categories: `Core`, `Text`, `Table`, `Chart`, `Layout`.

## Architecture

This is a zero-dependency .NET library (targets `net8.0` and `net10.0`) that generates self-contained HTML reports via a fluent API. Reports are single HTML strings with embedded CSS, inline SVG charts, and no JavaScript.

**Single public facade pattern.** `Generator` is the only public type users interact with. All renderers are `internal`.

```
Generator (public, fluent facade)
├── TextRenderer    — headings, paragraphs, alerts, badges, KPI cards, progress bars, images, layouts, etc.
├── TableRenderer   — reflection-based table generation; supports conditional formatting, summary rows, fixed headers
└── ChartRenderer   — inline SVG charts: bar (vertical/horizontal), pie/donut, line (single & multi-series)
```

**Data flow:**
1. `Generator.Create(title)` initialises a `StringBuilder` and writes the HTML document head/body open
2. `.Add*()` calls delegate to the appropriate renderer, which appends HTML fragments to the `StringBuilder`
3. `.Generate()` closes the document and returns the complete HTML string

**Key conventions to follow:**
- `HtmlHelpers.Enc()` must be used on all user-supplied strings to prevent XSS
- `ReportOptions` holds chart sizing/alignment config; set via `.WithOptions()`
- Every `.Add*()` method returns `this` for chaining
- Renderers are lazily instantiated and hold a reference to the same `StringBuilder`

## Coding Style

From `.github/copilot-instructions.md`:

- **Braces:** Allman style (opening brace on new line)
- **Namespaces:** File-scoped (`namespace CDS.FluentHtmlReports;`)
- **`var`:** Only when the type is obvious from the right-hand side
- **Nullable:** Enabled; annotate all types accordingly
- **Private fields:** `_camelCase`; static fields `s_camelCase`
- **XML docs:** Required on all public types, all public and internal members
- **Public APIs:** Prefer overloads over default parameters
- **No `async void`** except event handlers; use `ConfigureAwait(false)` in library code
- **One public type per file**; filename matches the type name

## Testing Conventions

- Framework: MSTest + FluentAssertions
- Pattern: Arrange-Act-Assert with descriptive test names, e.g. `AddParagraph_EncodesLessThan`
- Tag tests with `[TestCategory]` matching the feature area
- Test both positive and negative (encoding, edge cases, null inputs)

## Versioning & Release

Version is derived automatically from git tags via **MinVer**. A release is triggered by pushing a `v*.*.*` tag, which runs CI → pack → publish to NuGet.
