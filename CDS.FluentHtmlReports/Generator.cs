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
    public static Generator Create(string title)
    {
        var generator = new Generator();
        AppendDocumentStart(generator._html, title);
        generator._html.AppendLine($"<h1>{title}</h1>");
        return generator;
    }

    /// <summary>
    /// Replaces the current report options with the specified instance.
    /// </summary>
    public Generator WithOptions(ReportOptions options)
    {
        _options = options ?? new ReportOptions();
        _chartRenderer = new(_html, _options);
        return this;
    }

    /// <summary>
    /// Configures the current report options via a delegate.
    /// </summary>
    public Generator WithOptions(Action<ReportOptions> configure)
    {
        configure?.Invoke(_options);
        return this;
    }

    /// <summary>
    /// Finalizes the report and returns the complete HTML document as a string.
    /// </summary>
    public string Generate()
    {
        AppendDocumentEnd(_html);
        return _html.ToString();
    }

    // ── Text ────────────────────────────────────────────────────────────

    /// <summary>Adds an HTML heading at the specified level.</summary>
    public Generator AddHeading(string title, HeadingLevel level = HeadingLevel.H2)
    {
        _textRenderer.AddHeading(title, level);
        return this;
    }

    /// <summary>Adds a paragraph of text.</summary>
    public Generator AddParagraph(string text)
    {
        _textRenderer.AddParagraph(text);
        return this;
    }

    /// <summary>Adds a single label/value metadata line.</summary>
    public Generator AddMetadata(string label, string value)
    {
        _textRenderer.AddMetadata(label, value);
        return this;
    }

    /// <summary>Adds raw HTML content to the report.</summary>
    public Generator AddHtml(string htmlContent)
    {
        _textRenderer.AddHtml(htmlContent);
        return this;
    }

    /// <summary>Adds a row of inline label/value pairs.</summary>
    public Generator AddLabelValueRow(IEnumerable<(string, string)> values)
    {
        _textRenderer.AddLabelValueRow(values);
        return this;
    }

    /// <summary>Adds a visual separator line.</summary>
    public Generator AddLine(LineType lineType = LineType.Solid)
    {
        _textRenderer.AddLine(lineType);
        return this;
    }

    /// <summary>Adds an unordered (bullet) list.</summary>
    public Generator AddUnorderedList(IEnumerable<string> items)
    {
        _textRenderer.AddUnorderedList(items);
        return this;
    }

    /// <summary>Adds an ordered (numbered) list.</summary>
    public Generator AddOrderedList(IEnumerable<string> items)
    {
        _textRenderer.AddOrderedList(items);
        return this;
    }

    /// <summary>Adds a styled alert/callout box.</summary>
    public Generator AddAlert(AlertLevel level, string message)
    {
        _textRenderer.AddAlert(level, message);
        return this;
    }

    /// <summary>Adds a page break marker for print output.</summary>
    public Generator AddPageBreak()
    {
        _textRenderer.AddPageBreak();
        return this;
    }

    /// <summary>Adds a preformatted code block.</summary>
    public Generator AddCodeBlock(string code, string? language = null)
    {
        _textRenderer.AddCodeBlock(code, language);
        return this;
    }

    /// <summary>Adds a small colored badge/label.</summary>
    public Generator AddBadge(string text, string color)
    {
        _textRenderer.AddBadge(text, color);
        return this;
    }

    /// <summary>Adds a hyperlink.</summary>
    public Generator AddLink(string text, string url)
    {
        _textRenderer.AddLink(text, url);
        return this;
    }

    /// <summary>Adds a definition list of term/definition pairs.</summary>
    public Generator AddDefinitionList(IEnumerable<(string term, string definition)> items)
    {
        _textRenderer.AddDefinitionList(items);
        return this;
    }

    /// <summary>Adds a progress bar.</summary>
    public Generator AddProgressBar(string label, int value, int max = 100, string? suffix = null)
    {
        _textRenderer.AddProgressBar(label, value, max, suffix);
        return this;
    }

    /// <summary>Adds a row of KPI summary cards.</summary>
    public Generator AddKpiCards(IEnumerable<(string label, string value)> cards)
    {
        _textRenderer.AddKpiCards(cards);
        return this;
    }

    /// <summary>Adds a footer with optional custom text. Uses a timestamp if no text is provided.</summary>
    public Generator AddFooter(string? text = null)
    {
        _textRenderer.AddFooter(text);
        return this;
    }

    /// <summary>Adds a base64-encoded image from raw bytes.</summary>
    public Generator AddImage(byte[] imageData, string mimeType, string? alt = null)
    {
        _textRenderer.AddImage(imageData, mimeType, alt);
        return this;
    }

    /// <summary>Adds a base64-encoded image from a file path.</summary>
    public Generator AddImageFromFile(string filePath, string? alt = null)
    {
        var bytes = File.ReadAllBytes(filePath);
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var mimeType = ext switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
        _textRenderer.AddImage(bytes, mimeType, alt);
        return this;
    }

    // ── Layout ──────────────────────────────────────────────────────────

    /// <summary>Opens a collapsible (details/summary) section.</summary>
    public Generator BeginCollapsible(string title, bool expanded = false)
    {
        _textRenderer.BeginCollapsible(title, expanded);
        return this;
    }

    /// <summary>Closes a collapsible section.</summary>
    public Generator EndCollapsible()
    {
        _textRenderer.EndCollapsible();
        return this;
    }

    /// <summary>Opens a multi-column flex layout row.</summary>
    public Generator BeginColumns()
    {
        _textRenderer.BeginColumns();
        return this;
    }

    /// <summary>Opens an individual column within a columns layout.</summary>
    public Generator BeginColumn()
    {
        _textRenderer.BeginColumn();
        return this;
    }

    /// <summary>Closes an individual column.</summary>
    public Generator EndColumn()
    {
        _textRenderer.EndColumn();
        return this;
    }

    /// <summary>Closes a multi-column flex layout row.</summary>
    public Generator EndColumns()
    {
        _textRenderer.EndColumns();
        return this;
    }

    // ── Tables ──────────────────────────────────────────────────────────

    /// <summary>Adds a table generated from the properties of the supplied objects.</summary>
    public Generator AddTable(TableFixedHeader header, object[] values)
    {
        _tableRenderer.AddTable(header, values);
        return this;
    }

    /// <summary>Adds a table with conditional cell formatting.</summary>
    public Generator AddTable(TableFixedHeader header, object[] values, Func<string, string, CellStyle> cellFormat)
    {
        _tableRenderer.AddTable(header, values, cellFormat, summaryColumns: null);
        return this;
    }

    /// <summary>Adds a table with column summary rows.</summary>
    public Generator AddTable(TableFixedHeader header, object[] values, Dictionary<string, SummaryType> summaryColumns)
    {
        _tableRenderer.AddTable(header, values, cellFormat: null, summaryColumns: summaryColumns);
        return this;
    }

    /// <summary>Adds a table with both conditional formatting and summary rows.</summary>
    public Generator AddTable(TableFixedHeader header, object[] values, Func<string, string, CellStyle> cellFormat, Dictionary<string, SummaryType> summaryColumns)
    {
        _tableRenderer.AddTable(header, values, cellFormat, summaryColumns);
        return this;
    }

    /// <summary>Adds a two-column key-value table.</summary>
    public Generator AddKeyValueTable(IEnumerable<(string key, string value)> items)
    {
        _tableRenderer.AddKeyValueTable(items);
        return this;
    }

    // ── Charts ──────────────────────────────────────────────────────────

    /// <summary>Adds a vertical bar chart with explicit bar colors.</summary>
    public Generator AddVerticalBarChart(string title, (string label, int value, string color)[] data)
    {
        _chartRenderer.AddVerticalBarChart(title, data);
        return this;
    }

    /// <summary>Adds a vertical bar chart with automatically assigned colors.</summary>
    public Generator AddVerticalBarChart(string title, (string label, int value)[] data)
    {
        _chartRenderer.AddVerticalBarChart(title, data);
        return this;
    }

    /// <summary>Adds a horizontal bar chart with explicit bar colors.</summary>
    public Generator AddHorizontalBarChart(string title, (string label, int value, string color)[] data)
    {
        _chartRenderer.AddHorizontalBarChart(title, data);
        return this;
    }

    /// <summary>Adds a horizontal bar chart with automatically assigned colors.</summary>
    public Generator AddHorizontalBarChart(string title, (string label, int value)[] data)
    {
        _chartRenderer.AddHorizontalBarChart(title, data);
        return this;
    }

    /// <summary>Adds a pie chart with explicit slice colors.</summary>
    public Generator AddPieChart(string title, (string label, int value, string color)[] data)
    {
        _chartRenderer.AddPieChart(title, data);
        return this;
    }

    /// <summary>Adds a pie chart with automatically assigned colors.</summary>
    public Generator AddPieChart(string title, (string label, int value)[] data)
    {
        _chartRenderer.AddPieChart(title, data);
        return this;
    }

    /// <summary>Adds a donut chart with explicit slice colors.</summary>
    public Generator AddDonutChart(string title, (string label, int value, string color)[] data)
    {
        _chartRenderer.AddDonutChart(title, data);
        return this;
    }

    /// <summary>Adds a donut chart with automatically assigned colors.</summary>
    public Generator AddDonutChart(string title, (string label, int value)[] data)
    {
        _chartRenderer.AddDonutChart(title, data);
        return this;
    }

    /// <summary>Adds a single-series line chart.</summary>
    public Generator AddLineChart(string title, (string label, int value)[] data)
    {
        _chartRenderer.AddLineChart(title, data);
        return this;
    }

    /// <summary>Adds a multi-series line chart.</summary>
    public Generator AddLineChart(string title, (string seriesName, (string label, int value)[] points)[] series)
    {
        _chartRenderer.AddLineChart(title, series);
        return this;
    }

    // ── Document structure ──────────────────────────────────────────────

    private static void AppendDocumentStart(StringBuilder html, string title)
    {
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"UTF-8\">");
        html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine($"<title>{title}</title>");
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
            h3 { font-size: 16px; margin: 20px 0 8px; color: #333; }
            h4 { font-size: 14px; margin: 16px 0 6px; color: #555; }
            h5 { font-size: 13px; margin: 14px 0 4px; color: #666; }
            h6 { font-size: 12px; margin: 12px 0 4px; color: #888; }
            p { margin-bottom: 12px; font-size: 14px; }
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
                border-top: 2px solid #999;
            }
            .total-col {
                background: #f8f9fa;
                font-weight: 600;
            }
            .kv-table td:first-child {
                text-align: left;
            }
            .kv-table td:last-child {
                text-align: left;
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

            /* Lists */
            ul, ol {
                margin: 10px 0 16px 28px;
                font-size: 14px;
            }
            li {
                margin-bottom: 4px;
            }

            /* Alerts */
            .alert {
                padding: 12px 16px;
                margin: 12px 0;
                border-radius: 4px;
                font-size: 14px;
                line-height: 1.5;
            }
            .alert-icon {
                margin-right: 6px;
            }

            /* Code blocks */
            .code-block {
                background: #f5f5f5;
                border: 1px solid #e0e0e0;
                border-radius: 4px;
                padding: 14px 16px;
                margin: 12px 0;
                font-family: 'Consolas', 'Courier New', monospace;
                font-size: 13px;
                line-height: 1.5;
                overflow-x: auto;
                white-space: pre;
            }

            /* Badges */
            .badge {
                display: inline-block;
                padding: 2px 10px;
                border-radius: 12px;
                color: #fff;
                font-size: 12px;
                font-weight: 600;
                margin: 2px 4px 2px 0;
            }

            /* Links */
            .report-link {
                color: #1976D2;
                text-decoration: none;
                font-size: 14px;
                border-bottom: 1px solid transparent;
            }
            .report-link:hover {
                border-bottom-color: #1976D2;
            }

            /* Definition lists */
            .definition-list {
                margin: 10px 0 16px;
                font-size: 14px;
            }
            .definition-list dt {
                font-weight: 600;
                color: #333;
                margin-top: 8px;
            }
            .definition-list dd {
                margin-left: 20px;
                color: #555;
            }

            /* Progress bars */
            .progress-container {
                margin: 10px 0;
            }
            .progress-label {
                display: flex;
                justify-content: space-between;
                font-size: 13px;
                margin-bottom: 4px;
            }
            .progress-bar-bg {
                background: #e9ecef;
                border-radius: 6px;
                height: 18px;
                overflow: hidden;
            }
            .progress-bar-fill {
                height: 100%;
                border-radius: 6px;
                transition: width 0.3s;
            }

            /* KPI cards */
            .kpi-row {
                display: flex;
                flex-wrap: wrap;
                gap: 16px;
                margin: 16px 0 20px;
            }
            .kpi-card {
                flex: 1;
                min-width: 140px;
                background: #f8f9fa;
                border: 1px solid #dee2e6;
                border-radius: 8px;
                padding: 16px 20px;
                text-align: center;
            }
            .kpi-value {
                font-size: 28px;
                font-weight: 700;
                color: #1976D2;
                line-height: 1.2;
            }
            .kpi-label {
                font-size: 12px;
                color: #666;
                margin-top: 4px;
                text-transform: uppercase;
                letter-spacing: 0.5px;
            }

            /* Footer */
            .report-footer {
                margin-top: 40px;
                padding-top: 12px;
                border-top: 1px solid #dee2e6;
                font-size: 12px;
                color: #999;
                text-align: center;
            }

            /* Collapsible sections */
            .collapsible {
                margin: 12px 0;
                border: 1px solid #dee2e6;
                border-radius: 4px;
            }
            .collapsible summary {
                padding: 10px 16px;
                background: #f8f9fa;
                cursor: pointer;
                font-weight: 600;
                font-size: 14px;
                border-radius: 4px;
            }
            .collapsible[open] summary {
                border-bottom: 1px solid #dee2e6;
                border-radius: 4px 4px 0 0;
            }
            .collapsible-content {
                padding: 16px;
            }

            /* Columns layout */
            .columns-row {
                display: flex;
                gap: 24px;
                margin: 12px 0;
            }
            .column {
                flex: 1;
                min-width: 0;
            }

            /* Image container */
            .image-container {
                margin: 12px 0;
                text-align: center;
            }
            .image-container img {
                max-width: 100%;
                height: auto;
            }

            @media print {
                body { background: #fff; }
                .container { padding: 0; max-width: 100%; }
                .collapsible { border: none; }
                .collapsible summary { display: none; }
                .collapsible-content { padding: 0; }
                .kpi-card { border: 1px solid #ccc; }
            }
        """);
    }
}
