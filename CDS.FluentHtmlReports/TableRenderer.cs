using System.Globalization;
using System.Text;
using static CDS.FluentHtmlReports.HtmlHelpers;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Renders HTML table elements from object arrays using reflection.
/// </summary>
internal class TableRenderer(StringBuilder _html)
{
    /// <summary>
    /// Appends an HTML table with formatting controlled by the specified header mode.
    /// </summary>
    internal void AddTable(TableFixedHeader header, object[] values)
    {
        AddTable(header, values, cellFormat: null, summaryColumns: null);
    }

    /// <summary>
    /// Appends an HTML table with optional conditional cell formatting and column summaries.
    /// </summary>
    internal void AddTable(
        TableFixedHeader header,
        object[] values,
        Func<string, string, CellStyle>? cellFormat,
        Dictionary<string, SummaryType>? summaryColumns)
    {
        if (values == null || values.Length == 0) { return; }

        var properties = values[0].GetType().GetProperties();
        var hasHeaderRow = header == TableFixedHeader.Header || header == TableFixedHeader.Both;
        var hasFirstColumnFixed = header == TableFixedHeader.FirstColumn || header == TableFixedHeader.Both;

        _html.AppendLine("<table>");

        // Header row
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

        // Body rows — collect numeric values for summary
        var numericValues = new Dictionary<string, List<double>>();
        if (summaryColumns != null)
        {
            foreach (var prop in properties)
            {
                if (summaryColumns.ContainsKey(prop.Name))
                {
                    numericValues[prop.Name] = [];
                }
            }
        }

        _html.AppendLine("<tbody>");
        foreach (var value in values)
        {
            _html.AppendLine("<tr>");
            var isFirstCell = true;
            foreach (var prop in properties)
            {
                var cellValue = prop.GetValue(value)?.ToString() ?? "";

                // Track numeric values for summary
                if (numericValues.ContainsKey(prop.Name) && double.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double numVal))
                {
                    numericValues[prop.Name].Add(numVal);
                }

                // Determine cell style
                string styleAttr = "";
                if (isFirstCell && hasFirstColumnFixed)
                {
                    styleAttr = " style=\"background: #f1f3f5; font-weight: 600;\"";
                }
                else if (cellFormat != null)
                {
                    var cellStyle = cellFormat(prop.Name, cellValue);
                    styleAttr = GetCellStyleAttribute(cellStyle);
                }

                _html.AppendLine($"<td{styleAttr}>{Enc(cellValue)}</td>");
                isFirstCell = false;
            }
            _html.AppendLine("</tr>");
        }
        _html.AppendLine("</tbody>");

        // Summary footer row
        if (summaryColumns != null && summaryColumns.Count > 0)
        {
            _html.AppendLine("<tfoot><tr class=\"total-row\">");
            var isFirst = true;
            foreach (var prop in properties)
            {
                if (isFirst && !summaryColumns.ContainsKey(prop.Name))
                {
                    _html.AppendLine("<td style=\"font-weight: 600;\">Total</td>");
                    isFirst = false;
                    continue;
                }

                if (summaryColumns.TryGetValue(prop.Name, out var summaryType) &&
                    numericValues.TryGetValue(prop.Name, out var vals) && vals.Count > 0)
                {
                    var summaryValue = summaryType switch
                    {
                        SummaryType.Sum => vals.Sum(),
                        SummaryType.Average => vals.Average(),
                        SummaryType.Count => vals.Count,
                        SummaryType.Min => vals.Min(),
                        SummaryType.Max => vals.Max(),
                        _ => 0.0
                    };

                    string formatted = summaryType == SummaryType.Average
                        ? summaryValue.ToString("F2", CultureInfo.InvariantCulture)
                        : summaryValue.ToString("G", CultureInfo.InvariantCulture);

                    string label = summaryType switch
                    {
                        SummaryType.Average => "Avg: ",
                        SummaryType.Count => "Count: ",
                        SummaryType.Min => "Min: ",
                        SummaryType.Max => "Max: ",
                        _ => ""
                    };

                    _html.AppendLine($"<td>{label}{formatted}</td>");
                }
                else
                {
                    _html.AppendLine("<td></td>");
                }

                isFirst = false;
            }
            _html.AppendLine("</tr></tfoot>");
        }

        _html.AppendLine("</table>");
    }

    /// <summary>
    /// Appends a two-column key-value table.
    /// </summary>
    internal void AddKeyValueTable(IEnumerable<(string key, string value)> items)
    {
        if (items == null) { return; }

        _html.AppendLine("<table class=\"kv-table\">");
        _html.AppendLine("<tbody>");
        foreach (var (key, value) in items)
        {
            _html.AppendLine("<tr>");
            _html.AppendLine($"<td style=\"background: #f1f3f5; font-weight: 600; width: 30%;\">{Enc(key)}</td>");
            _html.AppendLine($"<td>{Enc(value)}</td>");
            _html.AppendLine("</tr>");
        }
        _html.AppendLine("</tbody>");
        _html.AppendLine("</table>");
    }

    private static string GetCellStyleAttribute(CellStyle style) => style switch
    {
        CellStyle.Green => " style=\"background: #C8E6C9; font-weight: 600;\"",
        CellStyle.Red => " style=\"background: #FFCDD2;\"",
        CellStyle.Amber => " style=\"background: #FFE0B2;\"",
        CellStyle.Blue => " style=\"background: #BBDEFB;\"",
        CellStyle.Muted => " style=\"background: #f8f9fa; color: #999;\"",
        _ => ""
    };
}
