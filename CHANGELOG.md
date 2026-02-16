# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Comprehensive unit test suite with 64 tests covering all features
- GitHub Actions CI/CD pipeline for automated builds and releases
- Code security scanning with CodeQL

### Fixed
- HTML encoding in `AddParagraph` to prevent XSS vulnerabilities
- Alert components now include level-specific CSS classes (alert-info, alert-success, etc.)
- HTML5 standard compliance with lowercase charset attribute

## [1.0.0] - Initial Release

### Added
- Fluent API for building HTML reports
- Text components: headings, paragraphs, lists, alerts, code blocks
- Table rendering with automatic property reflection
- SVG charts: vertical bar, horizontal bar, pie, donut, line charts
- Layout features: columns, collapsible sections, KPI cards
- Progress bars with automatic color coding
- HTML encoding for security
- Zero external dependencies
- Support for .NET 8 and .NET 10
- Self-contained HTML output (inline CSS, SVG charts)
- Print-friendly styling with media queries

[Unreleased]: https://github.com/nooogle/CDS.FluentHtmlReports/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/nooogle/CDS.FluentHtmlReports/releases/tag/v1.0.0
