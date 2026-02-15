using System.Net;
using System.Text;

namespace CDS.FluentHtmlReports;

public class Generator
{
    private StringBuilder html = new(16384);
    private ReportOptions options = new();

    public static Generator Create(string title)
    {
        var generator = new Generator();
        AppendDocumentStart(generator.html);
        generator.html.AppendLine($"<h1>{title}</h1>");
        return generator;
    }

    public Generator WithOptions(ReportOptions options)
    {
        this.options = options ?? new ReportOptions();
        return this;
    }

    public Generator WithOptions(Action<ReportOptions> configure)
    {
        configure?.Invoke(options);
        return this;
    }


    public Generator AddSection(string title)
    {
        html.AppendLine($"<h2>{title}</h2>");
        return this;
    }

    public Generator AddParagraph(string text)
    {
        html.AppendLine($"<p>{text}</p>");
        return this;
    }

    public Generator AddMetadata(string label, string value)
    {
        html.AppendLine($"<div class=\"metadata\"><span><strong>{label}:</strong> {value}</span></div>");
        return this;
    }

    public Generator AddHtml(string htmlContent)
    {
        html.AppendLine(htmlContent);
        return this;
    }

    public string Generate()
    {
        AppendDocumentEnd(html);
        return html.ToString();
    }

    public Generator()
    {
    }


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

    public Generator AddLabelValueRow(IEnumerable<(string, string)> values)
    {
        if (values == null) { return this; }

        html.AppendLine("<div class=\"metadata\">");
        foreach (var value in values)
        {
            html.AppendLine($"<span><strong>{value.Item1}:</strong> {value.Item2}</span>");
        }
        html.AppendLine("</div>");

        return this;
    }


    public Generator AddTable(
        TableFixedHeader header,
        object[] values)
    {
        if (values == null) { return this; }

        var properties = values[0].GetType().GetProperties();
        var hasHeaderRow = header == TableFixedHeader.Header || header == TableFixedHeader.Both;
        var hasFirstColumnFixed = header == TableFixedHeader.FirstColumn || header == TableFixedHeader.Both;

        html.AppendLine("<table>");

        // Add header row - with or without special formatting based on enum
        html.AppendLine("<thead><tr>");
        var isFirstHeader = true;
        foreach (var prop in properties)
        {
            var shouldFormat = hasHeaderRow || (isFirstHeader && hasFirstColumnFixed);

            if (shouldFormat)
            {
                // Apply background and bold formatting to header
                html.AppendLine($"<th>{prop.Name}</th>");
            }
            else
            {
                // No special formatting when None
                html.AppendLine($"<th style=\"background: transparent; font-weight: normal;\">{prop.Name}</th>");
            }
            isFirstHeader = false;
        }
        html.AppendLine("</tr></thead>");

        // Add body rows
        html.AppendLine("<tbody>");
        foreach (var value in values)
        {
            html.AppendLine("<tr>");
            var isFirstCell = true;
            foreach (var prop in properties)
            {
                var cellValue = prop.GetValue(value)?.ToString() ?? "";
                if (isFirstCell && hasFirstColumnFixed)
                {
                    // Apply background and bold to first column (no alignment change)
                    html.AppendLine($"<td style=\"background: #f1f3f5; font-weight: 600;\">{cellValue}</td>");
                }
                else
                {
                    html.AppendLine($"<td>{cellValue}</td>");
                }
                isFirstCell = false;
            }
            html.AppendLine("</tr>");
        }
        html.AppendLine("</tbody>");

        html.AppendLine("</table>");
        return this;
    }

    public Generator AddLine(LineType lineType = LineType.Solid)
    {
        switch (lineType)
        {
            case LineType.Blank:
                html.AppendLine("<div style=\"height: 20px;\"></div>");
                break;
            case LineType.Solid:
                html.AppendLine("<hr style=\"border: none; border-top: 1px solid #dee2e6; margin: 20px 0;\" />");
                break;
            case LineType.Dashed:
                html.AppendLine("<hr style=\"border: none; border-top: 1px dashed #dee2e6; margin: 20px 0;\" />");
                break;
            case LineType.Dotted:
                html.AppendLine("<hr style=\"border: none; border-top: 1px dotted #dee2e6; margin: 20px 0;\" />");
                break;
        }
        return this;
    }


    public Generator AddVerticalBarChart(string title, (string label, int value, string color)[] data)
    {
        if (data == null || data.Length == 0) { return this; }

        // Use widthPercent parameter if provided, otherwise fall back to options
        int effectiveWidthPercent = options.ChartWidthPercent;

        html.AppendLine($"<h2>{Enc(title)}</h2>");

        // Apply alignment and sizing to container div
        var alignment = options.ChartAlignment switch
        {
            ChartAlignment.Left => "text-align: left;",
            ChartAlignment.Center => "text-align: center;",
            ChartAlignment.Right => "text-align: right;",
            _ => "text-align: left;"
        };

        // Layout constants
        const int marginLeft = 60;
        const int marginTop = 30;
        const int marginBottom = 40;
        const int marginRight = 20;
        const int svgHeight = 480;
        const int chartHeight = svgHeight - marginTop - marginBottom;
        const int minBarWidth = 20;

        // Fixed viewBox width matching the container
        const int containerWidth = 900;
        int svgWidth = containerWidth * effectiveWidthPercent / 100;
        int chartWidth = svgWidth - marginLeft - marginRight;

        // Calculate bar width dynamically with a fixed gap
        // Bars fill all available space after accounting for gaps
        const int gap = 10;
        int barWidth;
        if (data.Length == 1)
        {
            barWidth = chartWidth;
        }
        else
        {
            barWidth = (chartWidth - gap * (data.Length - 1)) / data.Length;
            barWidth = Math.Max(minBarWidth, barWidth);
        }

        int maxWidth = svgWidth;

        html.AppendLine($"<div class=\"chart-container\" style=\"max-width:{maxWidth}px; {alignment}\">");

        int maxValue = Math.Max(1, data.Max(d => d.value));

        // Position the bars within the chart area
        int totalBarArea = data.Length * barWidth + (data.Length - 1) * gap;
        int startX = marginLeft + (chartWidth - totalBarArea) / 2;

        html.AppendLine($"<svg viewBox=\"0 0 {svgWidth} {svgHeight}\" " +
                         $"width=\"100%\" height=\"{svgHeight}\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

        // Horizontal grid lines and Y-axis labels
        const int gridLines = 4;
        for (int i = 0; i <= gridLines; i++)
        {
            int y = marginTop + chartHeight - (i * chartHeight / gridLines);
            int gridValue = maxValue * i / gridLines;

            html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{y}\" " +
                             $"x2=\"{svgWidth - marginRight}\" y2=\"{y}\" " +
                             "stroke=\"#e0e0e0\" stroke-width=\"1\" />");

            html.AppendLine($"<text x=\"{marginLeft - 8}\" y=\"{y + 4}\" " +
                             "text-anchor=\"end\" font-size=\"11\" fill=\"#888\">" +
                             $"{gridValue}</text>");
        }

        // Axes
        html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop}\" " +
                         $"x2=\"{marginLeft}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");
        html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop + chartHeight}\" " +
                         $"x2=\"{svgWidth - marginRight}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");

        // Bars, value labels, and category labels
        for (int i = 0; i < data.Length; i++)
        {
            int barX = startX + i * (barWidth + gap);
            int barHeight = maxValue > 0 ? data[i].value * chartHeight / maxValue : 0;
            int barY = marginTop + chartHeight - barHeight;
            int centerX = barX + barWidth / 2;

            // Bar rectangle
            html.AppendLine($"<rect x=\"{barX}\" y=\"{barY}\" " +
                             $"width=\"{barWidth}\" height=\"{barHeight}\" " +
                             $"fill=\"{Enc(data[i].color)}\" rx=\"3\" />");

            // Count value above the bar
            html.AppendLine($"<text x=\"{centerX}\" y=\"{barY - 6}\" " +
                             "text-anchor=\"middle\" font-size=\"13\" " +
                             $"font-weight=\"600\" fill=\"#333\">{data[i].value}</text>");

            // Category label below the X-axis
            html.AppendLine($"<text x=\"{centerX}\" y=\"{marginTop + chartHeight + 22}\" " +
                             $"text-anchor=\"middle\" font-size=\"13\" fill=\"#333\">" +
                             $"{Enc(data[i].label)}</text>");
        }

        html.AppendLine("</svg>");
        html.AppendLine("</div>");

        return this;
    }

    public Generator AddVerticalBarChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return this; }

        // Default color palette
        string[] defaultColors = 
        [
            "#4CAF50", "#2196F3", "#FF9800", "#F44336", "#9C27B0",
            "#00BCD4", "#FFEB3B", "#795548", "#607D8B", "#E91E63"
        ];

        var dataWithColors = data.Select((d, i) => 
            (d.label, d.value, defaultColors[i % defaultColors.Length]))
            .ToArray();

        return AddVerticalBarChart(title, dataWithColors);
    }


    public Generator AddHorizontalBarChart(string title, (string label, int value, string color)[] data)
    {
        if (data == null || data.Length == 0) { return this; }

        int effectiveWidthPercent = options.ChartWidthPercent;

        html.AppendLine($"<h2>{Enc(title)}</h2>");

        var alignment = options.ChartAlignment switch
        {
            ChartAlignment.Left => "text-align: left;",
            ChartAlignment.Center => "text-align: center;",
            ChartAlignment.Right => "text-align: right;",
            _ => "text-align: left;"
        };

        // Layout constants
        const int marginLeft = 120;
        const int marginTop = 30;
        const int marginBottom = 30;
        const int marginRight = 50;
        const int svgHeight = 480;
        const int chartHeight = svgHeight - marginTop - marginBottom;
        const int minBarHeight = 16;

        const int containerWidth = 900;
        int svgWidth = containerWidth * effectiveWidthPercent / 100;
        int chartWidth = svgWidth - marginLeft - marginRight;

        // Calculate bar height dynamically with a fixed gap
        const int gap = 8;
        int barHeight;
        if (data.Length == 1)
        {
            barHeight = chartHeight;
        }
        else
        {
            barHeight = (chartHeight - gap * (data.Length - 1)) / data.Length;
            barHeight = Math.Max(minBarHeight, barHeight);
        }

        int maxWidth = svgWidth;

        html.AppendLine($"<div class=\"chart-container\" style=\"max-width:{maxWidth}px; {alignment}\">");

        int maxValue = Math.Max(1, data.Max(d => d.value));

        // Position the bars within the chart area
        int totalBarArea = data.Length * barHeight + (data.Length - 1) * gap;
        int startY = marginTop + (chartHeight - totalBarArea) / 2;

        html.AppendLine($"<svg viewBox=\"0 0 {svgWidth} {svgHeight}\" " +
                         $"width=\"100%\" height=\"{svgHeight}\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

        // Vertical grid lines and X-axis labels
        const int gridLines = 4;
        for (int i = 0; i <= gridLines; i++)
        {
            int x = marginLeft + (i * chartWidth / gridLines);
            int gridValue = maxValue * i / gridLines;

            html.AppendLine($"<line x1=\"{x}\" y1=\"{marginTop}\" " +
                             $"x2=\"{x}\" y2=\"{marginTop + chartHeight}\" " +
                             "stroke=\"#e0e0e0\" stroke-width=\"1\" />");

            html.AppendLine($"<text x=\"{x}\" y=\"{marginTop + chartHeight + 18}\" " +
                             "text-anchor=\"middle\" font-size=\"11\" fill=\"#888\">" +
                             $"{gridValue}</text>");
        }

        // Axes
        html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop}\" " +
                         $"x2=\"{marginLeft}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");
        html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop + chartHeight}\" " +
                         $"x2=\"{svgWidth - marginRight}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");

        // Bars, value labels, and category labels
        for (int i = 0; i < data.Length; i++)
        {
            int barY = startY + i * (barHeight + gap);
            int barW = maxValue > 0 ? data[i].value * chartWidth / maxValue : 0;
            int centerY = barY + barHeight / 2;

            // Bar rectangle
            html.AppendLine($"<rect x=\"{marginLeft}\" y=\"{barY}\" " +
                             $"width=\"{barW}\" height=\"{barHeight}\" " +
                             $"fill=\"{Enc(data[i].color)}\" rx=\"3\" />");

            // Value label to the right of the bar
            html.AppendLine($"<text x=\"{marginLeft + barW + 6}\" y=\"{centerY + 4}\" " +
                             "text-anchor=\"start\" font-size=\"13\" " +
                             $"font-weight=\"600\" fill=\"#333\">{data[i].value}</text>");

            // Category label to the left of the Y-axis
            html.AppendLine($"<text x=\"{marginLeft - 8}\" y=\"{centerY + 4}\" " +
                             $"text-anchor=\"end\" font-size=\"13\" fill=\"#333\">" +
                             $"{Enc(data[i].label)}</text>");
        }

        html.AppendLine("</svg>");
        html.AppendLine("</div>");

        return this;
    }

    public Generator AddHorizontalBarChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return this; }

        string[] defaultColors =
        [
            "#4CAF50", "#2196F3", "#FF9800", "#F44336", "#9C27B0",
            "#00BCD4", "#FFEB3B", "#795548", "#607D8B", "#E91E63"
        ];

        var dataWithColors = data.Select((d, i) =>
            (d.label, d.value, defaultColors[i % defaultColors.Length]))
            .ToArray();

        return AddHorizontalBarChart(title, dataWithColors);
    }


    private static string Enc(string value) => WebUtility.HtmlEncode(value ?? string.Empty);
}

public class ReportOptions
{
    public int ChartWidthPercent { get; set; } = 100;
    public ChartAlignment ChartAlignment { get; set; } = ChartAlignment.Left;
}

public enum ChartAlignment
{
    Left,
    Center,
    Right
}

public enum TableFixedHeader
{
    None,
    Header,
    FirstColumn,
    Both
}

public enum LineType
{
    Blank,
    Solid,
    Dashed,
    Dotted
}

