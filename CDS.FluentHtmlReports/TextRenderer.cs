using System.Text;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Renders text-based HTML elements (headings, paragraphs, metadata, lines).
/// </summary>
internal class TextRenderer(StringBuilder _html)
{
    /// <summary>
    /// Appends an HTML heading element at the specified level.
    /// </summary>
    /// <param name="title">The heading text.</param>
    /// <param name="level">The heading level.</param>
    internal void AddHeading(string title, HeadingLevel level)
    {
        int tag = (int)level;
        _html.AppendLine($"<h{tag}>{title}</h{tag}>");
    }

    /// <summary>
    /// Appends a paragraph element.
    /// </summary>
    /// <param name="text">The paragraph content.</param>
    internal void AddParagraph(string text)
    {
        _html.AppendLine($"<p>{text}</p>");
    }

    /// <summary>
    /// Appends a single label/value metadata line.
    /// </summary>
    /// <param name="label">The bold label text.</param>
    /// <param name="value">The value text.</param>
    internal void AddMetadata(string label, string value)
    {
        _html.AppendLine($"<div class=\"metadata\"><span><strong>{label}:</strong> {value}</span></div>");
    }

    /// <summary>
    /// Appends raw HTML content.
    /// </summary>
    /// <param name="htmlContent">The raw HTML string to insert.</param>
    internal void AddHtml(string htmlContent)
    {
        _html.AppendLine(htmlContent);
    }

    /// <summary>
    /// Appends a row of inline label/value pairs.
    /// </summary>
    /// <param name="values">The label/value pairs to render.</param>
    internal void AddLabelValueRow(IEnumerable<(string, string)> values)
    {
        if (values == null) { return; }

        _html.AppendLine("<div class=\"metadata\">");
        foreach (var value in values)
        {
            _html.AppendLine($"<span><strong>{value.Item1}:</strong> {value.Item2}</span>");
        }
        _html.AppendLine("</div>");
    }

    /// <summary>
    /// Appends a visual separator line of the specified style.
    /// </summary>
    /// <param name="lineType">The line style.</param>
    internal void AddLine(LineType lineType)
    {
        switch (lineType)
        {
            case LineType.Blank:
                _html.AppendLine("<div style=\"height: 20px;\"></div>");
                break;
            case LineType.Solid:
                _html.AppendLine("<hr style=\"border: none; border-top: 1px solid #dee2e6; margin: 20px 0;\" />");
                break;
            case LineType.Dashed:
                _html.AppendLine("<hr style=\"border: none; border-top: 1px dashed #dee2e6; margin: 20px 0;\" />");
                break;
            case LineType.Dotted:
                _html.AppendLine("<hr style=\"border: none; border-top: 1px dotted #dee2e6; margin: 20px 0;\" />");
                break;
        }
    }
}
