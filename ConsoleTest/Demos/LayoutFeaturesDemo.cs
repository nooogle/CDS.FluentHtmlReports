using CDS.FluentHtmlReports;

namespace ConsoleTest.Demos;

/// <summary>
/// Demonstrates layout features: KPI cards, progress bars, collapsible sections,
/// two-column layout, and page breaks.
/// </summary>
public static class LayoutFeaturesDemo
{
    public static string Generate()
    {
        return Generator
            .Create("Layout Features Demo")

            // KPI cards
            .AddHeading("KPI Summary Cards")
            .AddParagraph("Prominent headline numbers in a responsive flex row:")
            .AddKpiCards([
                ("Total Records", "12,847"),
                ("Pass Rate", "96.4%"),
                ("Duration", "00:03:42"),
                ("Errors", "14")
            ])
            .AddParagraph("Works with any number of cards — wraps to multiple rows:")
            .AddKpiCards([
                ("Orders", "3,291"),
                ("Revenue", "$87,420"),
                ("Avg Order", "$26.56"),
                ("Returns", "42"),
                ("Customers", "1,847"),
                ("Satisfaction", "4.7/5")
            ])

            .AddLine()

            // Progress bars
            .AddHeading("Progress Bars")
            .AddParagraph("Auto-colored by percentage (green >= 80%, amber >= 50%, red below):")
            .AddProgressBar("Test Coverage", 92, suffix: "%")
            .AddProgressBar("Migration Progress", 1204, max: 1500)
            .AddProgressBar("Build Completion", 65, suffix: "%")
            .AddProgressBar("Data Quality", 35, suffix: "%")
            .AddProgressBar("Upload", 100, suffix: "%")

            .AddLine()

            // Collapsible sections
            .AddHeading("Collapsible Sections")
            .AddParagraph("Uses HTML5 details/summary — no JavaScript required:")

            .BeginCollapsible("Expanded by default — Configuration Details", expanded: true)
                .AddKeyValueTable([
                    ("Server", "prod-app-03"),
                    ("Port", "5432"),
                    ("Database", "OrdersDB"),
                    ("SSL", "Enabled")
                ])
            .EndCollapsible()

            .BeginCollapsible("Collapsed by default — Error Log")
                .AddAlert(AlertLevel.Error, "Connection timeout at 14:32:05 UTC")
                .AddCodeBlock("""
                    System.TimeoutException: The operation has timed out.
                       at Npgsql.NpgsqlConnector.Connect(TimeSpan timeout)
                       at Npgsql.ConnectorPool.AllocateLong(NpgsqlConnection conn)
                       at Program.Main() in /app/Program.cs:line 42
                    """)
            .EndCollapsible()

            .BeginCollapsible("Collapsed — Detailed Results Table")
                .AddTable(TableFixedHeader.Header, new object[]
                {
                    new { Test = "Login", Result = "Pass", Duration = "1.2s" },
                    new { Test = "Search", Result = "Pass", Duration = "0.8s" },
                    new { Test = "Checkout", Result = "Fail", Duration = "3.5s" },
                    new { Test = "Export", Result = "Pass", Duration = "2.1s" },
                })
            .EndCollapsible()

            .AddLine()

            // Two-column layout
            .AddHeading("Two-Column Layout")
            .AddParagraph("Side-by-side content using CSS flexbox:")

            .BeginColumns()
                .BeginColumn()
                    .AddHeading("Left Column", HeadingLevel.H3)
                    .AddParagraph("This content sits in the left column. You can place any report elements here — tables, lists, alerts, etc.")
                    .AddUnorderedList(["Item Alpha", "Item Beta", "Item Gamma"])
                .EndColumn()
                .BeginColumn()
                    .AddHeading("Right Column", HeadingLevel.H3)
                    .AddParagraph("The right column has equal width. Content flows independently in each column.")
                    .AddAlert(AlertLevel.Info, "Columns resize proportionally on smaller screens.")
                .EndColumn()
            .EndColumns()

            .AddParagraph("Columns with a table and chart side by side:")
            .BeginColumns()
                .BeginColumn()
                    .AddTable(TableFixedHeader.Header, new object[]
                    {
                        new { Region = "North", Sales = 4200 },
                        new { Region = "South", Sales = 3800 },
                        new { Region = "East", Sales = 5100 },
                        new { Region = "West", Sales = 2900 },
                    })
                .EndColumn()
                .BeginColumn()
                    .AddVerticalBarChart("Regional Sales", [
                        ("North", 4200, "#2196F3"),
                        ("South", 3800, "#4CAF50"),
                        ("East", 5100, "#FF9800"),
                        ("West", 2900, "#F44336")
                    ])
                .EndColumn()
            .EndColumns()

            .AddLine()

            // Page breaks
            .AddHeading("Page Breaks")
            .AddParagraph("When printing or saving to PDF, you can force content to start on a new page:")
            .AddParagraph("(A page break is inserted below this — visible when printing)")
            .AddPageBreak()
            .AddHeading("This Starts on a New Page (when printed)")
            .AddParagraph("Content after the page break appears on the next printed page.")

            .AddFooter()
            .Generate();
    }
}
