using System.Text;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Renders HTML table elements from object arrays using reflection.
/// </summary>
internal class TableRenderer(StringBuilder _html)
{
    /// <summary>
    /// Appends an HTML table with formatting controlled by the specified header mode.
    /// </summary>
    /// <param name="header">Controls which parts of the table receive fixed formatting.</param>
    /// <param name="values">The row data; property names become column headers.</param>
    internal void AddTable(TableFixedHeader header, object[] values)
    {
        if (values == null) { return; }

        var properties = values[0].GetType().GetProperties();
        var hasHeaderRow = header == TableFixedHeader.Header || header == TableFixedHeader.Both;
        var hasFirstColumnFixed = header == TableFixedHeader.FirstColumn || header == TableFixedHeader.Both;

        _html.AppendLine("<table>");

        // Add header row - with or without special formatting based on enum
        _html.AppendLine("<thead><tr>");
        var isFirstHeader = true;
        foreach (var prop in properties)
        {
            var shouldFormat = hasHeaderRow || (isFirstHeader && hasFirstColumnFixed);

            if (shouldFormat)
            {
                _html.AppendLine($"<th>{prop.Name}</th>");
            }
            else
            {
                _html.AppendLine($"<th style=\"background: transparent; font-weight: normal;\">{prop.Name}</th>");
            }
            isFirstHeader = false;
        }
        _html.AppendLine("</tr></thead>");

        // Add body rows
        _html.AppendLine("<tbody>");
        foreach (var value in values)
        {
            _html.AppendLine("<tr>");
            var isFirstCell = true;
            foreach (var prop in properties)
            {
                var cellValue = prop.GetValue(value)?.ToString() ?? "";
                if (isFirstCell && hasFirstColumnFixed)
                {
                    _html.AppendLine($"<td style=\"background: #f1f3f5; font-weight: 600;\">{cellValue}</td>");
                }
                else
                {
                    _html.AppendLine($"<td>{cellValue}</td>");
                }
                isFirstCell = false;
            }
            _html.AppendLine("</tr>");
        }
        _html.AppendLine("</tbody>");

        _html.AppendLine("</table>");
    }
}
