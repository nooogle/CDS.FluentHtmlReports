namespace CDS.FluentHtmlReports;

/// <summary>
/// Configuration options that control chart rendering behavior.
/// </summary>
public class ReportOptions
{
    /// <summary>
    /// Gets or sets the chart width as a percentage of the container width (1–100).
    /// </summary>
    public int ChartWidthPercent { get; set; } = 100;

    /// <summary>
    /// Gets or sets the chart height in SVG units.
    /// </summary>
    public int ChartHeight { get; set; } = 280;

    /// <summary>
    /// Gets or sets the horizontal alignment of charts within the container.
    /// </summary>
    public ChartAlignment ChartAlignment { get; set; } = ChartAlignment.Left;
}
