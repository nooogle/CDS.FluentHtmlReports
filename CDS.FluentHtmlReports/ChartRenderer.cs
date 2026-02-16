using System.Globalization;
using System.Text;
using static CDS.FluentHtmlReports.HtmlHelpers;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Renders SVG-based charts (vertical/horizontal bar, pie, donut, and line).
/// </summary>
internal class ChartRenderer(StringBuilder _html, ReportOptions _options)
{
    private static readonly string[] s_defaultColors =
    [
        "#4CAF50", "#2196F3", "#FF9800", "#F44336", "#9C27B0",
        "#00BCD4", "#FFEB3B", "#795548", "#607D8B", "#E91E63"
    ];

    // ── Vertical Bar Chart ──────────────────────────────────────────────

    /// <summary>
    /// Appends a vertical bar chart with explicit bar colors.
    /// </summary>
    internal void AddVerticalBarChart(string title, (string label, int value, string color)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        int effectiveWidthPercent = _options.ChartWidthPercent;

        _html.AppendLine($"<h2>{Enc(title)}</h2>");

        var alignment = GetAlignmentStyle();

        const int marginLeft = 60;
        const int marginTop = 30;
        const int marginBottom = 40;
        const int marginRight = 20;
        int svgHeight = _options.ChartHeight;
        int chartHeight = svgHeight - marginTop - marginBottom;
        const int minBarWidth = 20;

        const int containerWidth = 900;
        int svgWidth = containerWidth * effectiveWidthPercent / 100;
        int chartWidth = svgWidth - marginLeft - marginRight;

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

        int totalBarArea = data.Length * barWidth + (data.Length - 1) * gap;
        int startX = marginLeft + (chartWidth - totalBarArea) / 2;

        _html.AppendLine($"<svg viewBox=\"0 0 {svgWidth} {svgHeight}\" " +
                         $"width=\"100%\" height=\"{svgHeight}\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

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

        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop}\" " +
                         $"x2=\"{marginLeft}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");
        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop + chartHeight}\" " +
                         $"x2=\"{svgWidth - marginRight}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");

        for (int i = 0; i < data.Length; i++)
        {
            int barX = startX + i * (barWidth + gap);
            int barHeight = maxValue > 0 ? data[i].value * chartHeight / maxValue : 0;
            int barY = marginTop + chartHeight - barHeight;
            int centerX = barX + barWidth / 2;

            _html.AppendLine($"<rect x=\"{barX}\" y=\"{barY}\" " +
                             $"width=\"{barWidth}\" height=\"{barHeight}\" " +
                             $"fill=\"{Enc(data[i].color)}\" rx=\"3\" />");

            _html.AppendLine($"<text x=\"{centerX}\" y=\"{barY - 6}\" " +
                             "text-anchor=\"middle\" font-size=\"13\" " +
                             $"font-weight=\"600\" fill=\"#333\">{data[i].value}</text>");

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
    internal void AddVerticalBarChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        var dataWithColors = data.Select((d, i) =>
            (d.label, d.value, s_defaultColors[i % s_defaultColors.Length]))
            .ToArray();

        AddVerticalBarChart(title, dataWithColors);
    }

    // ── Horizontal Bar Chart ────────────────────────────────────────────

    /// <summary>
    /// Appends a horizontal bar chart with explicit bar colors.
    /// </summary>
    internal void AddHorizontalBarChart(string title, (string label, int value, string color)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        int effectiveWidthPercent = _options.ChartWidthPercent;

        _html.AppendLine($"<h2>{Enc(title)}</h2>");

        var alignment = GetAlignmentStyle();

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

        int totalBarArea = data.Length * barHeight + (data.Length - 1) * gap;
        int startY = marginTop + (chartHeight - totalBarArea) / 2;

        _html.AppendLine($"<svg viewBox=\"0 0 {svgWidth} {svgHeight}\" " +
                         $"width=\"100%\" height=\"{svgHeight}\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

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

        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop}\" " +
                         $"x2=\"{marginLeft}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");
        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop + chartHeight}\" " +
                         $"x2=\"{svgWidth - marginRight}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");

        for (int i = 0; i < data.Length; i++)
        {
            int barY = startY + i * (barHeight + gap);
            int barW = maxValue > 0 ? data[i].value * chartWidth / maxValue : 0;
            int centerY = barY + barHeight / 2;

            _html.AppendLine($"<rect x=\"{marginLeft}\" y=\"{barY}\" " +
                             $"width=\"{barW}\" height=\"{barHeight}\" " +
                             $"fill=\"{Enc(data[i].color)}\" rx=\"3\" />");

            _html.AppendLine($"<text x=\"{marginLeft + barW + 6}\" y=\"{centerY + 4}\" " +
                             "text-anchor=\"start\" font-size=\"13\" " +
                             $"font-weight=\"600\" fill=\"#333\">{data[i].value}</text>");

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
    internal void AddHorizontalBarChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return; }

        var dataWithColors = data.Select((d, i) =>
            (d.label, d.value, s_defaultColors[i % s_defaultColors.Length]))
            .ToArray();

        AddHorizontalBarChart(title, dataWithColors);
    }

    // ── Pie Chart ───────────────────────────────────────────────────────

    /// <summary>
    /// Appends a pie chart with explicit slice colors.
    /// </summary>
    internal void AddPieChart(string title, (string label, int value, string color)[] data)
    {
        RenderPieOrDonut(title, data, innerRadiusRatio: 0);
    }

    /// <summary>
    /// Appends a pie chart with automatically assigned colors.
    /// </summary>
    internal void AddPieChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return; }
        var dataWithColors = data.Select((d, i) =>
            (d.label, d.value, s_defaultColors[i % s_defaultColors.Length])).ToArray();
        AddPieChart(title, dataWithColors);
    }

    /// <summary>
    /// Appends a donut chart with explicit slice colors.
    /// </summary>
    internal void AddDonutChart(string title, (string label, int value, string color)[] data)
    {
        RenderPieOrDonut(title, data, innerRadiusRatio: 0.55);
    }

    /// <summary>
    /// Appends a donut chart with automatically assigned colors.
    /// </summary>
    internal void AddDonutChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return; }
        var dataWithColors = data.Select((d, i) =>
            (d.label, d.value, s_defaultColors[i % s_defaultColors.Length])).ToArray();
        AddDonutChart(title, dataWithColors);
    }

    private void RenderPieOrDonut(string title, (string label, int value, string color)[] data, double innerRadiusRatio)
    {
        if (data == null || data.Length == 0) { return; }

        _html.AppendLine($"<h2>{Enc(title)}</h2>");

        var alignment = GetAlignmentStyle();
        int svgSize = Math.Min(_options.ChartHeight, 400);
        int effectiveWidthPercent = _options.ChartWidthPercent;
        const int containerWidth = 900;
        int maxWidth = containerWidth * effectiveWidthPercent / 100;

        int chartSvgWidth = maxWidth;
        double cx = svgSize / 2.0;
        double cy = svgSize / 2.0 + 10;
        double radius = svgSize / 2.0 - 20;
        double innerRadius = radius * innerRadiusRatio;

        int total = Math.Max(1, data.Sum(d => d.value));
        int legendHeight = data.Length * 22 + 10;
        int totalHeight = svgSize + 20 + legendHeight;

        _html.AppendLine($"<div class=\"chart-container\" style=\"max-width:{maxWidth}px; {alignment}\">");
        _html.AppendLine($"<svg viewBox=\"0 0 {chartSvgWidth} {totalHeight}\" " +
                         $"width=\"100%\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

        double currentAngle = -Math.PI / 2;

        for (int i = 0; i < data.Length; i++)
        {
            double sliceAngle = 2 * Math.PI * data[i].value / total;

            if (data.Length == 1)
            {
                _html.AppendLine($"<circle cx=\"{F(cx)}\" cy=\"{F(cy)}\" r=\"{F(radius)}\" " +
                                 $"fill=\"{Enc(data[i].color)}\" />");
                if (innerRadiusRatio > 0)
                {
                    _html.AppendLine($"<circle cx=\"{F(cx)}\" cy=\"{F(cy)}\" r=\"{F(innerRadius)}\" " +
                                     "fill=\"#fff\" />");
                }
            }
            else
            {
                double endAngle = currentAngle + sliceAngle;

                double x1Outer = cx + radius * Math.Cos(currentAngle);
                double y1Outer = cy + radius * Math.Sin(currentAngle);
                double x2Outer = cx + radius * Math.Cos(endAngle);
                double y2Outer = cy + radius * Math.Sin(endAngle);

                int largeArc = sliceAngle > Math.PI ? 1 : 0;

                string path;
                if (innerRadiusRatio > 0)
                {
                    double x1Inner = cx + innerRadius * Math.Cos(endAngle);
                    double y1Inner = cy + innerRadius * Math.Sin(endAngle);
                    double x2Inner = cx + innerRadius * Math.Cos(currentAngle);
                    double y2Inner = cy + innerRadius * Math.Sin(currentAngle);

                    path = $"M {F(x1Outer)} {F(y1Outer)} " +
                           $"A {F(radius)} {F(radius)} 0 {largeArc} 1 {F(x2Outer)} {F(y2Outer)} " +
                           $"L {F(x1Inner)} {F(y1Inner)} " +
                           $"A {F(innerRadius)} {F(innerRadius)} 0 {largeArc} 0 {F(x2Inner)} {F(y2Inner)} Z";
                }
                else
                {
                    path = $"M {F(cx)} {F(cy)} " +
                           $"L {F(x1Outer)} {F(y1Outer)} " +
                           $"A {F(radius)} {F(radius)} 0 {largeArc} 1 {F(x2Outer)} {F(y2Outer)} Z";
                }

                _html.AppendLine($"<path d=\"{path}\" fill=\"{Enc(data[i].color)}\" " +
                                 "stroke=\"#fff\" stroke-width=\"2\" />");

                currentAngle = endAngle;
            }
        }

        // Legend below the chart
        double legendY = svgSize + 20;
        for (int i = 0; i < data.Length; i++)
        {
            double ly = legendY + i * 22;
            int percent = (int)Math.Round(100.0 * data[i].value / total);
            _html.AppendLine($"<rect x=\"10\" y=\"{F(ly)}\" width=\"14\" height=\"14\" " +
                             $"fill=\"{Enc(data[i].color)}\" rx=\"2\" />");
            _html.AppendLine($"<text x=\"30\" y=\"{F(ly + 12)}\" font-size=\"13\" fill=\"#333\">" +
                             $"{Enc(data[i].label)} ({data[i].value} \u2014 {percent}%)</text>");
        }

        _html.AppendLine("</svg>");
        _html.AppendLine("</div>");
    }

    // ── Line Chart ──────────────────────────────────────────────────────

    /// <summary>
    /// Appends a single-series line chart.
    /// </summary>
    internal void AddLineChart(string title, (string label, int value)[] data)
    {
        if (data == null || data.Length == 0) { return; }
        AddLineChart(title, [("", data)]);
    }

    /// <summary>
    /// Appends a multi-series line chart.
    /// </summary>
    internal void AddLineChart(string title, (string seriesName, (string label, int value)[] points)[] series)
    {
        if (series == null || series.Length == 0) { return; }

        _html.AppendLine($"<h2>{Enc(title)}</h2>");

        var alignment = GetAlignmentStyle();
        int effectiveWidthPercent = _options.ChartWidthPercent;

        const int marginLeft = 60;
        const int marginTop = 30;
        const int marginBottom = 50;
        const int marginRight = 20;
        int svgHeight = _options.ChartHeight;
        int chartHeight = svgHeight - marginTop - marginBottom;

        const int containerWidth = 900;
        int svgWidth = containerWidth * effectiveWidthPercent / 100;
        int chartWidth = svgWidth - marginLeft - marginRight;
        int maxWidth = svgWidth;

        int maxValue = 1;
        int maxPoints = 0;
        string[] xLabels = [];
        foreach (var s in series)
        {
            if (s.points.Length > maxPoints)
            {
                maxPoints = s.points.Length;
                xLabels = s.points.Select(p => p.label).ToArray();
            }
            if (s.points.Length > 0)
            {
                maxValue = Math.Max(maxValue, s.points.Max(p => p.value));
            }
        }

        bool hasLegend = series.Length > 1 || !string.IsNullOrEmpty(series[0].seriesName);
        int legendHeight = hasLegend ? series.Length * 22 + 10 : 0;

        _html.AppendLine($"<div class=\"chart-container\" style=\"max-width:{maxWidth}px; {alignment}\">");
        _html.AppendLine($"<svg viewBox=\"0 0 {svgWidth} {svgHeight + legendHeight}\" " +
                         $"width=\"100%\" height=\"{svgHeight + legendHeight}\" preserveAspectRatio=\"xMinYMin meet\" " +
                         "xmlns=\"http://www.w3.org/2000/svg\">");

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

        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop}\" " +
                         $"x2=\"{marginLeft}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");
        _html.AppendLine($"<line x1=\"{marginLeft}\" y1=\"{marginTop + chartHeight}\" " +
                         $"x2=\"{svgWidth - marginRight}\" y2=\"{marginTop + chartHeight}\" " +
                         "stroke=\"#999\" stroke-width=\"1\" />");

        if (maxPoints > 0)
        {
            int labelStep = Math.Max(1, maxPoints / 10);
            for (int i = 0; i < maxPoints; i += labelStep)
            {
                int x = marginLeft + (maxPoints > 1 ? i * chartWidth / (maxPoints - 1) : chartWidth / 2);
                _html.AppendLine($"<text x=\"{x}\" y=\"{marginTop + chartHeight + 22}\" " +
                                 $"text-anchor=\"middle\" font-size=\"11\" fill=\"#888\">" +
                                 $"{Enc(xLabels[i])}</text>");
            }
        }

        for (int s = 0; s < series.Length; s++)
        {
            var points = series[s].points;
            if (points.Length == 0) { continue; }

            string color = s_defaultColors[s % s_defaultColors.Length];
            var pointCoords = new List<(int x, int y)>();

            for (int i = 0; i < points.Length; i++)
            {
                int x = marginLeft + (points.Length > 1 ? i * chartWidth / (points.Length - 1) : chartWidth / 2);
                int y = marginTop + chartHeight - (maxValue > 0 ? points[i].value * chartHeight / maxValue : 0);
                pointCoords.Add((x, y));
            }

            var polylinePoints = string.Join(" ", pointCoords.Select(p => $"{p.x},{p.y}"));
            _html.AppendLine($"<polyline points=\"{polylinePoints}\" " +
                             $"fill=\"none\" stroke=\"{color}\" stroke-width=\"2.5\" " +
                             "stroke-linejoin=\"round\" stroke-linecap=\"round\" />");

            foreach (var (x, y) in pointCoords)
            {
                _html.AppendLine($"<circle cx=\"{x}\" cy=\"{y}\" r=\"3.5\" " +
                                 $"fill=\"{color}\" stroke=\"#fff\" stroke-width=\"1.5\" />");
            }
        }

        if (hasLegend)
        {
            double legendY = svgHeight + 5;
            for (int s = 0; s < series.Length; s++)
            {
                if (string.IsNullOrEmpty(series[s].seriesName)) { continue; }
                double ly = legendY + s * 22;
                string color = s_defaultColors[s % s_defaultColors.Length];
                _html.AppendLine($"<line x1=\"10\" y1=\"{F(ly + 7)}\" x2=\"24\" y2=\"{F(ly + 7)}\" " +
                                 $"stroke=\"{color}\" stroke-width=\"3\" />");
                _html.AppendLine($"<circle cx=\"17\" cy=\"{F(ly + 7)}\" r=\"3.5\" fill=\"{color}\" />");
                _html.AppendLine($"<text x=\"30\" y=\"{F(ly + 12)}\" font-size=\"13\" fill=\"#333\">" +
                                 $"{Enc(series[s].seriesName)}</text>");
            }
        }

        _html.AppendLine("</svg>");
        _html.AppendLine("</div>");
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private string GetAlignmentStyle() => _options.ChartAlignment switch
    {
        ChartAlignment.Left => "text-align: left;",
        ChartAlignment.Center => "text-align: center;",
        ChartAlignment.Right => "text-align: right;",
        _ => "text-align: left;"
    };

    private static string F(double value) => value.ToString("F1", CultureInfo.InvariantCulture);
}
