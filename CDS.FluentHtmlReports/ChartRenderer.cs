using System.Text;
using static CDS.FluentHtmlReports.HtmlHelpers;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Renders SVG-based bar charts (vertical and horizontal).
/// </summary>
internal class ChartRenderer(StringBuilder _html, ReportOptions _options)
{
    private static readonly string[] s_defaultColors =
    [
        "#4CAF50", "#2196F3", "#FF9800", "#F44336", "#9C27B0",
        "#00BCD4", "#FFEB3B", "#795548", "#607D8B", "#E91E63"
    ];

    /// <summary>
    /// Appends a vertical bar chart with explicit bar colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value, color) tuples for each bar.</param>
    internal void AddVerticalBarChart(string title, (string label, int value, string color)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        int effectiveWidthPercent = _options.ChartWidthPercent;

        _html.AppendLine($"<h2>{Enc(title)}</h2>");

        var alignment = GetAlignmentStyle();

        // Layout constants
        const int marginLeft = 60;
        const int marginTop = 30;
        const int marginBottom = 40;
        const int marginRight = 20;
        int svgHeight = _options.ChartHeight;
        int chartHeight = svgHeight - marginTop - marginBottom;
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

        _html.AppendLine($"<div class=\"chart-container\" style=\"max-width:{maxWidth}px; {alignment}\">");

        int maxValue = Math.Max(1, data.Max(d => d.value));

        // Position the bars within the chart area
        int totalBarArea = data.Length * barWidth + (data.Length - 1) * gap;
        int startX = marginLeft + (chartWidth - totalBarArea) / 2;

        _html.AppendLine($"<svg viewBox=\"0 0 {svgWidth} {svgHeight}\" " +
                         $"width=\"100%\" height=\"{svgHeight}\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

        // Horizontal grid lines and Y-axis labels
        const int gridLines = 4;
        for (int i = 0; i <= gridLines; i++)
        {
            int y = marginTop + chartHeight - (i * chartHeight / gridLines);
            int gridValue = maxValue * i / gridLines;

            _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{y}\" " +
                             $"x2=\"{svgWidth - marginRight}\" y2=\"{y}\" " +
                             "stroke=\"#e0e0e0\" stroke-width=\"1\" />");

            _html.AppendLine($"<text x=\"{marginLeft - 8}\" y=\"{y + 4}\" " +
                             "text-anchor=\"end\" font-size=\"11\" fill=\"#888\">" +
                             $"{gridValue}</text>");
        }

        // Axes
        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop}\" " +
                         $"x2=\"{marginLeft}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");
        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop + chartHeight}\" " +
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
            _html.AppendLine($"<rect x=\"{barX}\" y=\"{barY}\" " +
                             $"width=\"{barWidth}\" height=\"{barHeight}\" " +
                             $"fill=\"{Enc(data[i].color)}\" rx=\"3\" />");

            // Count value above the bar
            _html.AppendLine($"<text x=\"{centerX}\" y=\"{barY - 6}\" " +
                             "text-anchor=\"middle\" font-size=\"13\" " +
                             $"font-weight=\"600\" fill=\"#333\">{data[i].value}</text>");

            // Category label below the X-axis
            _html.AppendLine($"<text x=\"{centerX}\" y=\"{marginTop + chartHeight + 22}\" " +
                             $"text-anchor=\"middle\" font-size=\"13\" fill=\"#333\">" +
                             $"{Enc(data[i].label)}</text>");
        }

        _html.AppendLine("</svg>");
        _html.AppendLine("</div>");
    }

    /// <summary>
    /// Appends a vertical bar chart with automatically assigned colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value) tuples for each bar.</param>
    internal void AddVerticalBarChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        var dataWithColors = data.Select((d, i) =>
            (d.label, d.value, s_defaultColors[i % s_defaultColors.Length]))
            .ToArray();

        AddVerticalBarChart(title, dataWithColors);
    }

    /// <summary>
    /// Appends a horizontal bar chart with explicit bar colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value, color) tuples for each bar.</param>
    internal void AddHorizontalBarChart(string title, (string label, int value, string color)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        int effectiveWidthPercent = _options.ChartWidthPercent;

        _html.AppendLine($"<h2>{Enc(title)}</h2>");

        var alignment = GetAlignmentStyle();

        // Layout constants
        const int marginLeft = 120;
        const int marginTop = 30;
        const int marginBottom = 30;
        const int marginRight = 50;
        int svgHeight = _options.ChartHeight;
        int chartHeight = svgHeight - marginTop - marginBottom;
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

        _html.AppendLine($"<div class=\"chart-container\" style=\"max-width:{maxWidth}px; {alignment}\">");

        int maxValue = Math.Max(1, data.Max(d => d.value));

        // Position the bars within the chart area
        int totalBarArea = data.Length * barHeight + (data.Length - 1) * gap;
        int startY = marginTop + (chartHeight - totalBarArea) / 2;

        _html.AppendLine($"<svg viewBox=\"0 0 {svgWidth} {svgHeight}\" " +
                         $"width=\"100%\" height=\"{svgHeight}\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

        // Vertical grid lines and X-axis labels
        const int gridLines = 4;
        for (int i = 0; i <= gridLines; i++)
        {
            int x = marginLeft + (i * chartWidth / gridLines);
            int gridValue = maxValue * i / gridLines;

            _html.AppendLine($"<line x1=\"{x}\" y1=\"{marginTop}\" " +
                             $"x2=\"{x}\" y2=\"{marginTop + chartHeight}\" " +
                             "stroke=\"#e0e0e0\" stroke-width=\"1\" />");

            _html.AppendLine($"<text x=\"{x}\" y=\"{marginTop + chartHeight + 18}\" " +
                             "text-anchor=\"middle\" font-size=\"11\" fill=\"#888\">" +
                             $"{gridValue}</text>");
        }

        // Axes
        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop}\" " +
                         $"x2=\"{marginLeft}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");
        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop + chartHeight}\" " +
                         $"x2=\"{svgWidth - marginRight}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");

        // Bars, value labels, and category labels
        for (int i = 0; i < data.Length; i++)
        {
            int barY = startY + i * (barHeight + gap);
            int barW = maxValue > 0 ? data[i].value * chartWidth / maxValue : 0;
            int centerY = barY + barHeight / 2;

            // Bar rectangle
            _html.AppendLine($"<rect x=\"{marginLeft}\" y=\"{barY}\" " +
                             $"width=\"{barW}\" height=\"{barHeight}\" " +
                             $"fill=\"{Enc(data[i].color)}\" rx=\"3\" />");

            // Value label to the right of the bar
            _html.AppendLine($"<text x=\"{marginLeft + barW + 6}\" y=\"{centerY + 4}\" " +
                             "text-anchor=\"start\" font-size=\"13\" " +
                             $"font-weight=\"600\" fill=\"#333\">{data[i].value}</text>");

            // Category label to the left of the Y-axis
            _html.AppendLine($"<text x=\"{marginLeft - 8}\" y=\"{centerY + 4}\" " +
                             $"text-anchor=\"end\" font-size=\"13\" fill=\"#333\">" +
                             $"{Enc(data[i].label)}</text>");
        }

        _html.AppendLine("</svg>");
        _html.AppendLine("</div>");
    }

    /// <summary>
    /// Appends a horizontal bar chart with automatically assigned colors.
    /// </summary>
    /// <param name="title">The chart title.</param>
    /// <param name="data">An array of (label, value) tuples for each bar.</param>
    internal void AddHorizontalBarChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        var dataWithColors = data.Select((d, i) =>
            (d.label, d.value, s_defaultColors[i % s_defaultColors.Length]))
            .ToArray();

        AddHorizontalBarChart(title, dataWithColors);
    }

    /// <summary>
    /// Returns the CSS text-align value for the configured chart alignment.
    /// </summary>
    private string GetAlignmentStyle() => _options.ChartAlignment switch
    {
        ChartAlignment.Left => "text-align: left;",
        ChartAlignment.Center => "text-align: center;",
        ChartAlignment.Right => "text-align: right;",
        _ => "text-align: left;"
    };
}
