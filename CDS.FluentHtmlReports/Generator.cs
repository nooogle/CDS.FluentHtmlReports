using System.Text;

namespace CDS.FluentHtmlReports;

public class Generator
{
    private StringBuilder html = new(16384);

    public static Generator Create(string title)
    {
        var generator = new Generator();
        AppendDocumentStart(generator.html);
        generator.html.AppendLine($"<h1>{title}</h1>");
        return generator;
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

