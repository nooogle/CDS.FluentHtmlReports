# Suggested Features for CDS.FluentHtmlReports

## Context

These suggestions target the library's core audience: developers who currently dump data to CSV or text files and want something better, but don't want the weight of DevExpress, SSRS, Crystal Reports, or JavaScript-heavy web frameworks. Every suggestion below follows the library's existing principles:

- **Pure fluent API** — every method returns `Generator` for chaining
- **Zero dependencies** — HTML, CSS, and SVG only; no JavaScript
- **Self-contained output** — single `.html` file, openable in any browser
- **Reflection-friendly** — tables work from anonymous objects or POCOs
- **Print-ready** — output should look good on paper via `Ctrl+P`

---

## Tier 1 — High-Value, Natural Extensions

These fill obvious gaps that the current CSV-to-HTML developer would hit immediately.

### 1. Bullet and Numbered Lists

Developers writing text reports use lists constantly. Currently they'd have to use `AddHtml` with raw markup.

```csharp
.AddUnorderedList(["Passed validation", "Exported 1,204 rows", "No duplicates found"])
.AddOrderedList(["Connect to database", "Run migration", "Verify schema"])
```

Implementation: trivial `<ul>/<ol>` generation in `TextRenderer`. Support `string[]` or `IEnumerable<string>`.

### 2. Pie / Donut Chart

The single most-requested chart type for proportion data. Developers reporting on pass/fail ratios, category breakdowns, or budget allocations need this.

```csharp
.AddPieChart("Test Results", [("Pass", 42, "green"), ("Fail", 8, "red"), ("Skip", 3, "gray")])
.AddDonutChart("Budget Allocation", [("Engineering", 60), ("Marketing", 25), ("Operations", 15)])
```

Implementation: SVG `<path>` arcs with the same pattern as the existing bar charts. No JavaScript needed — just compute arc segments from percentages.

### 3. Line Chart

Essential for any time-series or trend data — build output over time, error rates, performance metrics. This is the chart type that makes developers stop using CSV and start using a reporting library.

```csharp
.AddLineChart("Build Times (last 30 days)", [("Jan 1", 42), ("Jan 2", 38), ("Jan 3", 45), ...])
```

Implementation: SVG `<polyline>` or `<path>` with the same grid/axis pattern already used in bar charts. Support multiple series via an overload accepting named series:

```csharp
.AddLineChart("Error Rates", [
    ("API", [("Mon", 12), ("Tue", 8), ("Wed", 15)]),
    ("Web", [("Mon", 5), ("Tue", 3), ("Wed", 7)])
])
```

### 4. Alert / Callout Boxes

Every report needs to highlight warnings, errors, or important notes. Currently there's no styled way to do this without `AddHtml`.

```csharp
.AddAlert(AlertLevel.Success, "All 1,204 records processed successfully.")
.AddAlert(AlertLevel.Warning, "3 records had missing postal codes — defaults applied.")
.AddAlert(AlertLevel.Error, "Database connection failed after retry 5.")
.AddAlert(AlertLevel.Info, "Report generated from staging environment.")
```

Implementation: styled `<div>` with left border color and background tint (green/amber/red/blue). Pure CSS, similar to GitHub-flavored markdown alerts.

### 5. KPI / Summary Cards

Developers replacing CSV output often have a few headline numbers — total records, pass rate, elapsed time. A prominent visual treatment for these prevents them from getting buried in a table.

```csharp
.AddKpiCards([
    ("Total Records", "12,847"),
    ("Pass Rate", "96.4%"),
    ("Duration", "00:03:42"),
    ("Errors", "14")
])
```

Implementation: inline-block `<div>` cards with large value text and smaller label below. Uses CSS flexbox (no JS). Wraps naturally on smaller screens.

### 6. Table Summary Row

The most common thing developers do after generating a table is manually add a totals row. Auto-summarization would be a significant convenience.

```csharp
.AddTable(TableFixedHeader.Header, data, summary: TableSummary.Sum)
// or more granularly:
.AddTable(TableFixedHeader.Header, data, summary: new TableSummary
{
    { "Quantity", SummaryType.Sum },
    { "Price", SummaryType.Average }
})
```

Implementation: after reflection-based table generation, iterate numeric columns and append a styled `<tfoot>` row. The CSS classes `.total-row` and `.total-col` already exist in the stylesheet.

### 7. Page Breaks for Print

When someone prints a long report (or saves to PDF via the browser), content gets cut at arbitrary points. Explicit page break control is essential for professional output.

```csharp
.AddPageBreak()
```

Implementation: `<div style="page-break-before: always;"></div>`. One line of HTML, massive value for anyone printing reports.

---

## Tier 2 — Developer-Friendly Additions

These address common frustrations for the target audience and keep people from falling back to `AddHtml`.

### 8. Badge / Status Indicators

Pass/fail, status codes, severity levels — developers constantly need small colored labels. Without this, they're concatenating raw HTML spans.

```csharp
.AddBadge("PASS", "green")
.AddBadge("FAIL", "red")
.AddBadge("PENDING", "orange")
// Or inline within a paragraph:
.AddParagraph("The deployment was " + Badge.Render("successful", "green"))
```

Implementation: styled `<span>` with border-radius, background color, and white text. Could also work as a static helper for inline use.

### 9. Progress Bars

Build pipelines, data processing jobs, test coverage — a simple visual bar that shows completion. Much more communicative than a raw percentage in a table cell.

```csharp
.AddProgressBar("Test Coverage", 87, suffix: "%")
.AddProgressBar("Migration Progress", 1204, max: 1500)
```

Implementation: nested `<div>` elements with percentage width and background color. Pure CSS. Could auto-color (green > 80%, amber > 50%, red below).

### 10. Definition Lists

For configuration dumps, environment details, or parameter listings, definition lists are semantically correct and visually cleaner than tables.

```csharp
.AddDefinitionList([
    ("Environment", "Production"),
    ("Server", "app-server-03"),
    ("Version", "2.4.1"),
    ("Last Deploy", "2024-06-15 14:32 UTC")
])
```

Implementation: `<dl>/<dt>/<dd>` elements. Very similar to the existing `AddLabelValueRow` but with vertical stacking and proper semantic HTML. Useful when there are many pairs or values are long.

### 11. Code Blocks

Developers are the primary audience. When reporting on build output, SQL queries, config files, or error stack traces, a monospaced formatted block is essential.

```csharp
.AddCodeBlock("SELECT * FROM Orders WHERE Status = 'Pending'", language: "sql")
.AddCodeBlock(exceptionMessage) // no language = plain preformatted text
```

Implementation: `<pre><code>` block with light gray background, monospace font. No syntax highlighting (that would require JS) — just proper formatting and HTML encoding. The `language` parameter is for future use or CSS class hooks.

### 12. Two-Column Layout

Side-by-side content is common: a chart next to a summary table, two tables for comparison, metadata alongside a chart. Currently everything stacks vertically.

```csharp
.BeginColumns()
    .BeginColumn()
        .AddTable(...)
    .EndColumn()
    .BeginColumn()
        .AddVerticalBarChart(...)
    .EndColumn()
.EndColumns()
```

Implementation: CSS flexbox wrapper divs. The fluent API push/pop pattern is slightly different from the current flat chain, but the `Begin`/`End` naming makes the nesting clear. Could also support a simpler flat version:

```csharp
.AddTwoColumnRow(
    left: g => g.AddTable(...),
    right: g => g.AddVerticalBarChart(...)
)
```

### 13. Conditional Table Cell Formatting

The most requested table feature after basic generation. Developers want to highlight cells based on thresholds — red for failures, green for passes, amber for warnings.

```csharp
.AddTable(TableFixedHeader.Header, data, cellFormat: (columnName, value) =>
{
    if (columnName == "Status" && value == "FAIL") return CellStyle.Red;
    if (columnName == "Score" && int.Parse(value) > 90) return CellStyle.Green;
    return CellStyle.Default;
})
```

Implementation: a `Func<string, string, CellStyle>` callback invoked per cell during table rendering. `CellStyle` is an enum or small struct mapping to background/text color CSS.

### 14. Image Embedding (Base64)

For reports that include screenshots, diagrams, or generated plots from other tools, base64 embedding keeps the output self-contained — no external file references.

```csharp
.AddImage(File.ReadAllBytes("chart.png"), "image/png", alt: "Monthly trend")
.AddImageFromFile("screenshot.jpg", alt: "Dashboard screenshot")
```

Implementation: `<img src="data:{mimeType};base64,{Convert.ToBase64String(bytes)}" />`. The file grows, but it stays self-contained — which is the entire point of this library.

### 15. Footer with Generation Metadata

Almost every report needs a "Generated at" timestamp and optionally machine/user info. Currently developers manually add this with `AddParagraph` at the end.

```csharp
.AddFooter() // auto-generates: "Generated on 2024-06-15 14:32:00 UTC"
.AddFooter("Generated by Build Pipeline v2.4 on {timestamp}")
```

Implementation: styled `<footer>` element with light text, top border, inserted before the closing `</div>`. The `{timestamp}` token is replaced with `DateTime.UtcNow`.

---

## Tier 3 — Nice-to-Have Enhancements

Lower priority, but would differentiate the library from "just another HTML generator."

### 16. Stacked Bar Charts

For comparing compositions across categories (e.g., pass/fail/skip per test suite, or budget by department per quarter).

```csharp
.AddStackedBarChart("Results by Suite", [
    ("Unit Tests", [("Pass", 120, "green"), ("Fail", 5, "red"), ("Skip", 3, "gray")]),
    ("Integration", [("Pass", 45, "green"), ("Fail", 12, "red"), ("Skip", 1, "gray")])
])
```

Implementation: extends the existing bar chart SVG rendering with stacked rect segments.

### 17. Sparklines (Inline Mini Charts)

Tiny inline trend indicators inside table cells or next to KPI values — common in dashboards and executive summaries.

```csharp
// As a standalone element
.AddSparkline([3, 7, 2, 9, 5, 8, 4])
// Ideally usable inside table data via a helper
new { Metric = "CPU", Current = "72%", Trend = Sparkline.Render([65, 70, 68, 72]) }
```

Implementation: tiny inline SVG `<polyline>`. Fixed height (~20px), no axes, no labels — just the shape. Would need a static render helper for use inside table cell values.

### 18. Color Themes

A lightweight way to switch between visual styles without the user defining all CSS manually.

```csharp
.WithOptions(o => o.Theme = ReportTheme.Dark)    // dark background, light text
.WithOptions(o => o.Theme = ReportTheme.Compact)  // tighter spacing, smaller fonts
.WithOptions(o => o.Theme = ReportTheme.Corporate) // subdued blue/gray palette
```

Implementation: swap the CSS block in `AppendStyles` based on the theme enum. 3-4 preset themes covers most needs.

### 19. Table of Contents

For longer reports, an auto-generated TOC from heading elements with anchor links. Pure HTML — no JavaScript needed.

```csharp
.AddTableOfContents() // auto-generates from all headings added so far (or all headings in the report)
```

Implementation: track headings in a list during `AddHeading`, then render `<ul>` with `<a href="#heading-id">` links. Requires adding `id` attributes to heading elements. Could be placed anywhere in the chain and rendered based on all headings in the document.

### 20. Grouped / Collapsible Sections

For large reports where sections can be expanded/collapsed. Uses the native HTML `<details>/<summary>` elements — no JavaScript required.

```csharp
.BeginCollapsible("Detailed Error Log", expanded: false)
    .AddTable(TableFixedHeader.Header, errorData)
.EndCollapsible()
```

Implementation: `<details><summary>Title</summary>...content...</details>`. This is pure HTML5, works in all modern browsers, and is genuinely useful for long reports with optional detail sections.

### 21. Horizontal Key-Value Table

A two-column table specifically for displaying configuration or summary data as key-value pairs. Sits between `AddMetadata` (too informal) and `AddTable` (too structured for simple pairs).

```csharp
.AddKeyValueTable([
    ("Database", "production-db-03"),
    ("Records Processed", "12,847"),
    ("Start Time", "2024-06-15 14:00:00"),
    ("End Time", "2024-06-15 14:03:42"),
    ("Status", "Complete")
])
```

Implementation: simple two-column `<table>` with the first column styled as headers. Different from `AddDefinitionList` in that it uses the existing table styling.

---

## Implementation Priority Recommendation

If implementing incrementally, this order maximizes value with minimal effort:

| Priority | Feature | Effort | Impact |
|----------|---------|--------|--------|
| 1 | Bullet/Numbered Lists | Very low | Fills basic gap |
| 2 | Alert/Callout Boxes | Low | High visual value |
| 3 | Page Breaks | Trivial | Essential for print |
| 4 | Code Blocks | Low | Core audience need |
| 5 | KPI/Summary Cards | Low | High visual impact |
| 6 | Pie/Donut Chart | Medium | Most-wanted chart |
| 7 | Progress Bars | Low | Good visual element |
| 8 | Definition Lists | Very low | Semantic improvement |
| 9 | Footer Metadata | Very low | Common need |
| 10 | Table Summary Row | Medium | High convenience |
| 11 | Line Chart | Medium | Essential chart type |
| 12 | Conditional Cell Formatting | Medium | High table value |
| 13 | Badge/Status Indicators | Low | Nice polish |
| 14 | Collapsible Sections | Low | Good for long reports |
| 15 | Two-Column Layout | Medium | Layout flexibility |
| 16 | Image Embedding | Low | Self-contained output |
| 17 | Key-Value Table | Very low | Convenience |
| 18 | Stacked Bar Charts | Medium | Niche but useful |
| 19 | Sparklines | Medium | Niche but impressive |
| 20 | Table of Contents | Medium | Long report need |
| 21 | Color Themes | Medium | Nice-to-have |

---

## What NOT to Add

To stay true to the library's philosophy, these should be explicitly avoided:

- **JavaScript of any kind** — the moment you add JS, you lose the "open in any browser, email as attachment, print directly" simplicity
- **Interactive filtering/sorting** — that's a web app, not a report
- **Data binding / live data** — the report is a snapshot, generated once
- **Templating engines** — the fluent API *is* the template
- **External CSS/font files** — breaks self-contained output
- **PDF generation** — browsers already do `Ctrl+P` to PDF; don't reinvent this
- **Database connectivity** — the developer provides the data, the library renders it
