using CDS.FluentHtmlReports;

namespace ConsoleTest.Demos;

/// <summary>
/// Demonstrates all chart types: vertical bars, horizontal bars, pie, donut, and line charts.
/// </summary>
public static class ChartFeaturesDemo
{
    public static string Generate()
    {
        return Generator
            .Create("Chart Features Demo")

            // Vertical bar charts
            .AddHeading("Vertical Bar Charts")
            .AddParagraph("With explicit colors:")
            .AddVerticalBarChart("Test Results", [
                ("Pass", 42, "#4CAF50"),
                ("Fail", 8, "#F44336"),
                ("Skip", 3, "#FF9800")
            ])
            .AddParagraph("With auto-assigned colors (6 items):")
            .AddVerticalBarChart("Monthly Sales",
                Enumerable.Range(1, 6).Select(x => ($"Q{x}", x * 15 + 10)).ToArray())

            .AddLine()

            // Horizontal bar charts
            .AddHeading("Horizontal Bar Charts")
            .AddHorizontalBarChart("Department Headcount", [
                ("Engineering", 45, "#2196F3"),
                ("Marketing", 22, "#FF9800"),
                ("Sales", 31, "#4CAF50"),
                ("Support", 18, "#9C27B0"),
                ("HR", 8, "#607D8B")
            ])
            .AddParagraph("With auto-colors and many items:")
            .WithOptions(o => o.ChartHeight = 400)
            .AddHorizontalBarChart("Feature Requests by Category",
                Enumerable.Range(1, 12).Select(x => ($"Category {x}", 50 - x * 3)).ToArray())
            .WithOptions(o => o.ChartHeight = 280)

            .AddLine()

            // Pie charts
            .AddHeading("Pie Charts")
            .AddParagraph("With explicit colors:")
            .AddPieChart("Budget Allocation", [
                ("Engineering", 45, "#2196F3"),
                ("Marketing", 20, "#FF9800"),
                ("Operations", 15, "#4CAF50"),
                ("Sales", 12, "#F44336"),
                ("Other", 8, "#9E9E9E")
            ])
            .AddParagraph("With auto-assigned colors:")
            .AddPieChart("Browser Market Share", [
                ("Chrome", 65),
                ("Firefox", 12),
                ("Safari", 10),
                ("Edge", 8),
                ("Other", 5)
            ])

            .AddLine()

            // Donut charts
            .AddHeading("Donut Charts")
            .AddDonutChart("Task Status", [
                ("Completed", 78, "#4CAF50"),
                ("In Progress", 15, "#2196F3"),
                ("Blocked", 4, "#F44336"),
                ("Not Started", 3, "#9E9E9E")
            ])
            .AddDonutChart("Storage Usage", [
                ("Documents", 42),
                ("Images", 28),
                ("Videos", 18),
                ("Other", 12)
            ])

            .AddLine()

            // Line charts
            .AddHeading("Line Charts")
            .AddParagraph("Single series:")
            .AddLineChart("Daily Active Users (Last 2 Weeks)",
                Enumerable.Range(1, 14).Select(d =>
                    ($"Day {d}", 1200 + (int)(300 * Math.Sin(d * 0.8)))).ToArray())

            .AddParagraph("Multi-series line chart:")
            .AddLineChart("Error Rates by Service", [
                ("API Gateway", Enumerable.Range(1, 10).Select(d =>
                    ($"Day {d}", 12 + (int)(5 * Math.Sin(d * 0.9)))).ToArray()),
                ("Web App", Enumerable.Range(1, 10).Select(d =>
                    ($"Day {d}", 8 + (int)(3 * Math.Cos(d * 0.7)))).ToArray()),
                ("Background Jobs", Enumerable.Range(1, 10).Select(d =>
                    ($"Day {d}", 3 + (int)(2 * Math.Sin(d * 1.2)))).ToArray())
            ])

            .AddLine()

            // Chart options
            .AddHeading("Chart Options")
            .AddParagraph("Charts can be resized and aligned using WithOptions:")
            .WithOptions(new ReportOptions { ChartWidthPercent = 50, ChartAlignment = ChartAlignment.Center })
            .AddVerticalBarChart("50% Width, Centered", [("A", 10), ("B", 20), ("C", 15)])
            .WithOptions(new ReportOptions { ChartWidthPercent = 60, ChartAlignment = ChartAlignment.Right })
            .AddHorizontalBarChart("60% Width, Right-Aligned", [("X", 30), ("Y", 50), ("Z", 20)])
            .WithOptions(new ReportOptions())

            .AddFooter()
            .Generate();
    }
}
