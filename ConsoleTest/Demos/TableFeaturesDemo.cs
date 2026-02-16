using CDS.FluentHtmlReports;

namespace ConsoleTest.Demos;

/// <summary>
/// Demonstrates table features: basic tables, header modes, conditional formatting,
/// summary rows, and key-value tables.
/// </summary>
public static class TableFeaturesDemo
{
    public static string Generate()
    {
        var salesData = new object[]
        {
            new { Product = "Widget A", Quantity = 150, Revenue = 4500.00, Status = "Active" },
            new { Product = "Widget B", Quantity = 85, Revenue = 3400.00, Status = "Active" },
            new { Product = "Widget C", Quantity = 12, Revenue = 720.00, Status = "Discontinued" },
            new { Product = "Widget D", Quantity = 230, Revenue = 9200.00, Status = "Active" },
            new { Product = "Widget E", Quantity = 5, Revenue = 150.00, Status = "Low Stock" },
        };

        return Generator
            .Create("Table Features Demo")

            // Basic table modes
            .AddHeading("Table Header Modes")
            .AddParagraph("TableFixedHeader.None — no special formatting:")
            .AddTable(TableFixedHeader.None, salesData)

            .AddParagraph("TableFixedHeader.Header — styled header row:")
            .AddTable(TableFixedHeader.Header, salesData)

            .AddParagraph("TableFixedHeader.FirstColumn — styled first column:")
            .AddTable(TableFixedHeader.FirstColumn, salesData)

            .AddParagraph("TableFixedHeader.Both — header row and first column styled:")
            .AddTable(TableFixedHeader.Both, salesData)

            .AddLine()

            // Conditional cell formatting
            .AddHeading("Conditional Cell Formatting")
            .AddParagraph("Cells colored based on their values — green for 'Active', red for 'Discontinued', amber for 'Low Stock', and quantities highlighted by threshold:")
            .AddTable(TableFixedHeader.Header, salesData, (columnName, value) =>
            {
                if (columnName == "Status")
                {
                    return value switch
                    {
                        "Active" => CellStyle.Green,
                        "Discontinued" => CellStyle.Red,
                        "Low Stock" => CellStyle.Amber,
                        _ => CellStyle.Default
                    };
                }
                if (columnName == "Quantity" && int.TryParse(value, out int qty))
                {
                    if (qty < 10) return CellStyle.Red;
                    if (qty > 200) return CellStyle.Green;
                }
                return CellStyle.Default;
            })

            .AddLine()

            // Summary rows
            .AddHeading("Table with Summary Row")
            .AddParagraph("Automatic column summaries (sum, average, min, max, count):")
            .AddTable(TableFixedHeader.Header, salesData, new Dictionary<string, SummaryType>
            {
                { "Quantity", SummaryType.Sum },
                { "Revenue", SummaryType.Sum }
            })

            .AddParagraph("With average and count:")
            .AddTable(TableFixedHeader.Header, salesData, new Dictionary<string, SummaryType>
            {
                { "Quantity", SummaryType.Average },
                { "Revenue", SummaryType.Max }
            })

            .AddLine()

            // Combined formatting + summary
            .AddHeading("Combined: Formatting + Summary")
            .AddParagraph("Both conditional formatting and summary in one table:")
            .AddTable(
                TableFixedHeader.Header,
                salesData,
                cellFormat: (col, val) =>
                {
                    if (col == "Status" && val == "Active") return CellStyle.Green;
                    if (col == "Status" && val == "Discontinued") return CellStyle.Red;
                    if (col == "Status" && val == "Low Stock") return CellStyle.Amber;
                    return CellStyle.Default;
                },
                summaryColumns: new Dictionary<string, SummaryType>
                {
                    { "Quantity", SummaryType.Sum },
                    { "Revenue", SummaryType.Sum }
                })

            .AddLine()

            // Key-value table
            .AddHeading("Key-Value Table")
            .AddParagraph("A two-column table for configuration, settings, or summary data:")
            .AddKeyValueTable([
                ("Database Server", "prod-db-03.internal"),
                ("Database Name", "OrdersDB"),
                ("Connection String", "Server=prod-db-03;Database=OrdersDB;Trusted=true"),
                ("Records Processed", "12,847"),
                ("Processing Time", "00:03:42.156"),
                ("Start Time", "2024-06-15 14:00:00 UTC"),
                ("End Time", "2024-06-15 14:03:42 UTC"),
                ("Status", "Completed Successfully")
            ])

            .AddFooter()
            .Generate();
    }
}
