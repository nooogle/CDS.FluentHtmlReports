using System.Text;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Fluent HTML report generator. Use <see cref="Create"/> to start building a report,
/// chain content methods, and call <see cref="Generate"/> to produce the final HTML string.
/// </summary>
public class Generator
{
    private readonly StringBuilder _html = new(16384);
    private ReportOptions _options = new();

    private TextRenderer _textRenderer;
    private TableRenderer _tableRenderer;
    private ChartRenderer _chartRenderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Generator"/> class.
    /// </summary>
    public Generator()
    {
        _textRenderer = new(_html);
        _tableRenderer = new(_html);
        _chartRenderer = new(_html, _options);
    }

    /// <summary>
    /// Creates a new report with the specified title rendered as an &lt;h1&gt; heading.
    /// </summary>
    /// <param name="title">The report title.</param>
    /// <returns>A new <see cref="Generator"/> instance ready for fluent chaining.</returns>
    public static Generator Create(string title)
    {
        var generator = new Generator();
        AppendDocumentStart(generator._html);
        generator._html.AppendLine($"<h1>{title}</h1>");
        return generator;
    }

    /// <summary>
    /// Replaces the current report options with the specified instance.
    /// </summary>
    /// <param name="options">The options to apply. If <c>null</c>, defaults are used.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator WithOptions(ReportOptions options)
    {
        _options = options ?? new ReportOptions();
        _chartRenderer = new(_html, _options);
        return this;
    }

    /// <summary>
    /// Configures the current report options via a delegate.
    /// </summary>
    /// <param name="configure">An action that mutates the current <see cref="ReportOptions"/>.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator WithOptions(Action<ReportOptions> configure)
    {
        configure?.Invoke(_options);
        return this;
    }

    /// <summary>
    /// Finalizes the report and returns the complete HTML document as a string.
    /// </summary>
    /// <returns>The generated HTML string.</returns>
    public string Generate()
    {
        AppendDocumentEnd(_html);
        return _html.ToString();
    }

    // Text

    /// <summary>
    /// Adds an HTML heading at the specified level.
    /// </summary>
    /// <param name="title">The heading text.</param>
    /// <param name="level">The heading level (defaults to <see cref="HeadingLevel.H2"/>).</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddHeading(string title, HeadingLevel level = HeadingLevel.H2)
    {
        _textRenderer.AddHeading(title, level);
        return this;
    }

    /// <summary>
    /// Adds a paragraph of text.
    /// </summary>
    /// <param name="text">The paragraph content.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddParagraph(string text)
    {
        _textRenderer.AddParagraph(text);
        return this;
    }

    /// <summary>
    /// Adds a single label/value metadata line.
    /// </summary>
    /// <param name="label">The bold label text.</param>
    /// <param name="value">The value text.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddMetadata(string label, string value)
    {
        _textRenderer.AddMetadata(label, value);
        return this;
    }

    /// <summary>
    /// Adds raw HTML content to the report.
    /// </summary>
    /// <param name="htmlContent">The raw HTML string to insert.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddHtml(string htmlContent)
    {
        _textRenderer.AddHtml(htmlContent);
        return this;
    }

    /// <summary>
    /// Adds a row of inline label/value pairs.
    /// </summary>
    /// <param name="values">The label/value pairs to render.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddLabelValueRow(IEnumerable<(string, string)> values)
    {
        _textRenderer.AddLabelValueRow(values);
        return this;
    }

    /// <summary>
    /// Adds a visual separator line.
    /// </summary>
    /// <param name="lineType">The line style (defaults to <see cref="LineType.Solid"/>).</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddLine(LineType lineType = LineType.Solid)
    {
        _textRenderer.AddLine(lineType);
        return this;
    }

    // Tables

    /// <summary>
    /// Adds a table generated from the properties of the supplied objects.
    /// </summary>
    /// <param name="header">Controls which parts of the table receive fixed formatting.</param>
    /// <param name="values">The row data; property names become column headers.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddTable(TableFixedHeader header, object[] values)
    {
        _tableRenderer.AddTable(header, values);
        return this;
    }

    // Charts

    /// <summary>
    /// Adds a vertical bar chart with explicit bar colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value, color) tuples for each bar.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddVerticalBarChart(string title, (string label, int value, string color)[] data)
    {
        _chartRenderer.AddVerticalBarChart(title, data);
        return this;
    }

    /// <summary>
    /// Adds a vertical bar chart with automatically assigned colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value) tuples for each bar.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddVerticalBarChart(string title, (string label, int value)[] data)
    {
        _chartRenderer.AddVerticalBarChart(title, data);
        return this;
    }

    /// <summary>
    /// Adds a horizontal bar chart with explicit bar colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value, color) tuples for each bar.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddHorizontalBarChart(string title, (string label, int value, string color)[] data)
    {
        _chartRenderer.AddHorizontalBarChart(title, data);
        return this;
    }

    /// <summary>
    /// Adds a horizontal bar chart with automatically assigned colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value) tuples for each bar.</param>
    /// <returns>The current <see cref="Generator"/> for fluent chaining.</returns>
    public Generator AddHorizontalBarChart(string title, (string label, int value)[] data)
    {
        _chartRenderer.AddHorizontalBarChart(title, data);
        return this;
    }

    // Document structure

    private static void AppendDocumentStart(StringBuilder html)
    {
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"UTF-8\">");
        html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("<title>Reprocessing Summary Report</title>");
        html.AppendLine("<style>");
        AppendStyles(html);
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("<div class=\"container\">");
    }

    private static void AppendDocumentEnd(StringBuilder html)
    {
        html.AppendLine("</div>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");
    }

    private static void AppendStyles(StringBuilder html)
    {
        html.AppendLine("""
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body {
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                background: #f8f9fa;
                color: #333;
                line-height: 1.6;
            }
            .container {
                max-width: 900px;
                margin: 0 auto;
                padding: 30px;
                background: #fff;
            }
            h1 {
                font-size: 24px;
                margin-bottom: 8px;
                color: #1a1a1a;
                border-bottom: 3px solid #1976D2;
                padding-bottom: 8px;
            }
            h2 {
                font-size: 18px;
                margin: 30px 0 12px;
                color: #1976D2;
            }
            .metadata {
                font-size: 13px;
                color: #666;
                margin-bottom: 6px;
            }
            .metadata span {
                display: inline-block;
                margin-right: 24px;
            }
            .metadata-section {
                margin-bottom: 20px;
            }
            table {
                width: 100%;
                border-collapse: collapse;
                margin-bottom: 20px;
                font-size: 14px;
            }
            th, td {
                padding: 8px 12px;
                border: 1px solid #dee2e6;
                text-align: center;
            }
            th {
                background: #f1f3f5;
                font-weight: 600;
            }
            .row-header {
                background: #f1f3f5;
                font-weight: 600;
                text-align: left;
            }
            .confusion-match {
                background: #C8E6C9;
                font-weight: 600;
            }
            .confusion-mismatch {
                background: #FFCDD2;
            }
            .confusion-zero {
                background: #fff;
                color: #ccc;
            }
            .total-row td {
                background: #f8f9fa;
                font-weight: 600;
            }
            .total-col {
                background: #f8f9fa;
                font-weight: 600;
            }
            .correctness-rates {
                margin-top: 8px;
                margin-bottom: 24px;
            }
            .chart-container {
                margin: 12px 0 30px;
            }
            svg text {
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            }
            @media print {
                body { background: #fff; }
                .container { padding: 0; max-width: 100%; }
            }
        """);
    }
}
