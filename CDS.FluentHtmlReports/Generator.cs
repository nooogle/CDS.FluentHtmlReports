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

    //public Generator AddMetadataSection(Action<MetadataBuilder> configure)
    //{
    //    html.AppendLine("<div class=\"metadata-section\">");
    //    var builder = new MetadataBuilder(html);
    //    configure(builder);
    //    html.AppendLine("</div>");
    //    return this;
    //}

    //public Generator AddTable(Action<TableBuilder> configure)
    //{
    //    html.AppendLine("<table>");
    //    var builder = new TableBuilder(html);
    //    configure(builder);
    //    html.AppendLine("</table>");
    //    return this;
    //}

    public Generator AddChartContainer(string svgContent)
    {
        html.AppendLine("<div class=\"chart-container\">");
        html.AppendLine(svgContent);
        html.AppendLine("</div>");
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
}
