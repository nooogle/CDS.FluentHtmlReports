using System.Text;
using static CDS.FluentHtmlReports.HtmlHelpers;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Renders text-based HTML elements (headings, paragraphs, metadata, lines,
/// lists, alerts, code blocks, badges, links, definitions, progress bars, KPI cards, and footers).
/// </summary>
internal class TextRenderer(StringBuilder _html)
{
    /// <summary>
    /// Appends an HTML heading element at the specified level.
    /// </summary>
    internal void AddHeading(string title, HeadingLevel level)
    {
        int tag = (int)level;
        _html.AppendLine($"<h{tag}>{title}</h{tag}>");
    }

    /// <summary>
    /// Appends a paragraph element.
    /// </summary>
    internal void AddParagraph(string text)
    {
        _html.AppendLine($"<p>{Enc(text)}</p>");
    }

    /// <summary>
    /// Appends a single label/value metadata line.
    /// </summary>
    internal void AddMetadata(string label, string value)
    {
        _html.AppendLine($"<div class=\"metadata\"><span><strong>{label}:</strong> {value}</span></div>");
    }

    /// <summary>
    /// Appends raw HTML content.
    /// </summary>
    internal void AddHtml(string htmlContent)
    {
        _html.AppendLine(htmlContent);
    }

    /// <summary>
    /// Appends a row of inline label/value pairs.
    /// </summary>
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

    /// <summary>
    /// Appends an unordered (bullet) list.
    /// </summary>
    internal void AddUnorderedList(IEnumerable<string> items)
    {
        if (items == null) { return; }
        _html.AppendLine("<ul>");
        foreach (var item in items)
        {
            _html.AppendLine($"<li>{Enc(item)}</li>");
        }
        _html.AppendLine("</ul>");
    }

    /// <summary>
    /// Appends an ordered (numbered) list.
    /// </summary>
    internal void AddOrderedList(IEnumerable<string> items)
    {
        if (items == null) { return; }
        _html.AppendLine("<ol>");
        foreach (var item in items)
        {
            _html.AppendLine($"<li>{Enc(item)}</li>");
        }
        _html.AppendLine("</ol>");
    }

    /// <summary>
    /// Appends a styled alert/callout box.
    /// </summary>
    internal void AddAlert(AlertLevel level, string message)
    {
        var (bg, border, icon, cssClass) = level switch
        {
            AlertLevel.Info    => ("#e3f2fd", "#1976D2", "\u2139\uFE0F", "alert-info"),
            AlertLevel.Success => ("#e8f5e9", "#4CAF50", "\u2705", "alert-success"),
            AlertLevel.Warning => ("#fff8e1", "#FF9800", "\u26A0\uFE0F", "alert-warning"),
            AlertLevel.Error   => ("#ffebee", "#F44336", "\u274C", "alert-error"),
            _                  => ("#e3f2fd", "#1976D2", "\u2139\uFE0F", "alert-info")
        };

        _html.AppendLine($"<div class=\"alert {cssClass}\" style=\"background:{bg}; border-left:4px solid {border};\">");
        _html.AppendLine($"<span class=\"alert-icon\">{icon}</span> {Enc(message)}");
        _html.AppendLine("</div>");
    }

    /// <summary>
    /// Appends a page break marker for print output.
    /// </summary>
    internal void AddPageBreak()
    {
        _html.AppendLine("<div style=\"page-break-before: always;\"></div>");
    }

    /// <summary>
    /// Appends a preformatted code block.
    /// </summary>
    internal void AddCodeBlock(string code, string? language = null)
    {
        var langClass = string.IsNullOrEmpty(language) ? "" : $" class=\"language-{Enc(language)}\"";
        _html.AppendLine($"<pre class=\"code-block\"><code{langClass}>{Enc(code)}</code></pre>");
    }

    /// <summary>
    /// Appends a small colored badge/label.
    /// </summary>
    internal void AddBadge(string text, string color)
    {
        _html.AppendLine($"<span class=\"badge\" style=\"background:{Enc(color)};\">{Enc(text)}</span>");
    }

    /// <summary>
    /// Appends a hyperlink.
    /// </summary>
    internal void AddLink(string text, string url)
    {
        _html.AppendLine($"<p><a class=\"report-link\" href=\"{Enc(url)}\">{Enc(text)}</a></p>");
    }

    /// <summary>
    /// Appends a definition list of term/definition pairs.
    /// </summary>
    internal void AddDefinitionList(IEnumerable<(string term, string definition)> items)
    {
        if (items == null) { return; }
        _html.AppendLine("<dl class=\"definition-list\">");
        foreach (var (term, definition) in items)
        {
            _html.AppendLine($"<dt>{Enc(term)}</dt>");
            _html.AppendLine($"<dd>{Enc(definition)}</dd>");
        }
        _html.AppendLine("</dl>");
    }

    /// <summary>
    /// Appends a progress bar.
    /// </summary>
    internal void AddProgressBar(string label, int value, int max = 100, string? suffix = null)
    {
        int percent = max > 0 ? Math.Clamp(value * 100 / max, 0, 100) : 0;
        string barColor = percent >= 80 ? "#4CAF50" : percent >= 50 ? "#FF9800" : "#F44336";
        string displayValue = suffix != null ? $"{value}{suffix}" : $"{value}/{max}";

        _html.AppendLine("<div class=\"progress-container\">");
        _html.AppendLine($"<div class=\"progress-label\"><strong>{Enc(label)}</strong> <span>{Enc(displayValue)}</span></div>");
        _html.AppendLine("<div class=\"progress-bar-bg\">");
        _html.AppendLine($"<div class=\"progress-bar-fill\" style=\"width:{percent}%; background:{barColor};\"></div>");
        _html.AppendLine("</div>");
        _html.AppendLine("</div>");
    }

    /// <summary>
    /// Appends a row of KPI summary cards.
    /// </summary>
    internal void AddKpiCards(IEnumerable<(string label, string value)> cards)
    {
        if (cards == null) { return; }
        _html.AppendLine("<div class=\"kpi-row\">");
        foreach (var (label, value) in cards)
        {
            _html.AppendLine("<div class=\"kpi-card\">");
            _html.AppendLine($"<div class=\"kpi-value\">{Enc(value)}</div>");
            _html.AppendLine($"<div class=\"kpi-label\">{Enc(label)}</div>");
            _html.AppendLine("</div>");
        }
        _html.AppendLine("</div>");
    }

    /// <summary>
    /// Appends a footer with optional custom text. Uses a timestamp if no text is provided.
    /// </summary>
    internal void AddFooter(string? text = null)
    {
        var content = text ?? $"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        // Support {timestamp} token replacement
        content = content.Replace("{timestamp}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        _html.AppendLine($"<footer class=\"report-footer\">{Enc(content)}</footer>");
    }

    /// <summary>
    /// Appends a base64-encoded image from raw bytes.
    /// </summary>
    internal void AddImage(byte[] imageData, string mimeType, string? alt = null,
        int maxWidthPercent = 100, ImageAlignment alignment = ImageAlignment.Center, string? caption = null)
    {
        var base64 = Convert.ToBase64String(imageData);
        AppendImageElement($"data:{Enc(mimeType)};base64,{base64}", alt, maxWidthPercent, alignment, caption);
    }

    /// <summary>
    /// Appends a base64-encoded image read from a stream.
    /// </summary>
    internal void AddImage(Stream imageData, string mimeType, string? alt = null,
        int maxWidthPercent = 100, ImageAlignment alignment = ImageAlignment.Center, string? caption = null)
    {
        using var ms = new MemoryStream();
        imageData.CopyTo(ms);
        AddImage(ms.ToArray(), mimeType, alt, maxWidthPercent, alignment, caption);
    }

    /// <summary>
    /// Appends an image referenced by an external URL.
    /// </summary>
    internal void AddImageFromUrl(string url, string? alt = null,
        int maxWidthPercent = 100, ImageAlignment alignment = ImageAlignment.Center, string? caption = null)
    {
        AppendImageElement(Enc(url), alt, maxWidthPercent, alignment, caption);
    }

    /// <summary>
    /// Appends an inline SVG fragment.
    /// </summary>
    internal void AddSvg(string svgMarkup, int maxWidthPercent = 100,
        ImageAlignment alignment = ImageAlignment.Center, string? caption = null)
    {
        var alignClass = AlignmentClass(alignment);
        var containerStyle = maxWidthPercent is > 0 and < 100
            ? $" style=\"max-width:{maxWidthPercent}%; {AlignmentMargin(alignment)}\""
            : "";

        _html.AppendLine($"<figure class=\"image-container {alignClass}\"{containerStyle}>");
        _html.AppendLine(svgMarkup);
        if (caption != null)
        {
            _html.AppendLine($"<figcaption class=\"image-caption\">{Enc(caption)}</figcaption>");
        }
        _html.AppendLine("</figure>");
    }

    // ── Private helpers ─────────────────────────────────────────────────

    private static string AlignmentClass(ImageAlignment alignment) => alignment switch
    {
        ImageAlignment.Left => "image-container--left",
        ImageAlignment.Right => "image-container--right",
        _ => "image-container--center"
    };

    // Returns the margin shorthand that positions a constrained block at the desired edge.
    // text-align only affects inline content; margin is required to align the block itself.
    private static string AlignmentMargin(ImageAlignment alignment) => alignment switch
    {
        ImageAlignment.Left  => "margin-right:auto;",
        ImageAlignment.Right => "margin-left:auto;",
        _                    => "margin-left:auto; margin-right:auto;"
    };

    private void AppendImageElement(string src, string? alt, int maxWidthPercent,
        ImageAlignment alignment, string? caption)
    {
        var alignClass = AlignmentClass(alignment);
        var altAttr = alt != null ? $" alt=\"{Enc(alt)}\"" : "";
        // max-width constrains the img; text-align on the figure (via CSS class) positions it.
        var widthStyle = maxWidthPercent is > 0 and < 100 ? $" style=\"max-width:{maxWidthPercent}%;\"" : "";

        _html.AppendLine($"<figure class=\"image-container {alignClass}\">");
        _html.AppendLine($"<img src=\"{src}\"{altAttr}{widthStyle} />");
        if (caption != null)
        {
            _html.AppendLine($"<figcaption class=\"image-caption\">{Enc(caption)}</figcaption>");
        }
        _html.AppendLine("</figure>");
    }

    /// <summary>
    /// Opens a collapsible section.
    /// </summary>
    internal void BeginCollapsible(string title, bool expanded)
    {
        var openAttr = expanded ? " open" : "";
        _html.AppendLine($"<details class=\"collapsible\"{openAttr}>");
        _html.AppendLine($"<summary>{Enc(title)}</summary>");
        _html.AppendLine("<div class=\"collapsible-content\">");
    }

    /// <summary>
    /// Closes a collapsible section.
    /// </summary>
    internal void EndCollapsible()
    {
        _html.AppendLine("</div>");
        _html.AppendLine("</details>");
    }

    /// <summary>
    /// Opens a multi-column flex layout.
    /// </summary>
    internal void BeginColumns()
    {
        _html.AppendLine("<div class=\"columns-row\">");
    }

    /// <summary>
    /// Opens an individual column within a columns layout.
    /// </summary>
    internal void BeginColumn()
    {
        _html.AppendLine("<div class=\"column\">");
    }

    /// <summary>
    /// Closes an individual column.
    /// </summary>
    internal void EndColumn()
    {
        _html.AppendLine("</div>");
    }

    /// <summary>
    /// Closes a multi-column flex layout.
    /// </summary>
    internal void EndColumns()
    {
        _html.AppendLine("</div>");
    }
}
