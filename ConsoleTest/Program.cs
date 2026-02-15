using System.Diagnostics;

var report =
    CDS
    .FluentHtmlReports
    .Generator
    .Create(title: "Monthly Report")
    .WithOptions(new CDS.FluentHtmlReports.ReportOptions() { ChartWidthPercent = 75, ChartAlignment = CDS.FluentHtmlReports.ChartAlignment.Right})
    .AddParagraph("This is the monthly report for June 2024.")
    .AddSection("Sales")
    .AddParagraph("Total sales for the month: $100,000")
    .AddLine(CDS.FluentHtmlReports.LineType.Blank)
    .AddLabelValueRow([("User", Environment.UserName), ("PC", Environment.MachineName)])
    .AddLine(CDS.FluentHtmlReports.LineType.Dashed)
    .AddLine(CDS.FluentHtmlReports.LineType.Solid)
    .AddLine(CDS.FluentHtmlReports.LineType.Dotted)
    .AddTable(CDS.FluentHtmlReports.TableFixedHeader.None, new[]
    {
        new { Product = "Product A", Quantity = 10, Price = 9.99 },
        new { Product = "Product B", Quantity = 5, Price = 19.99 },
        new { Product = "Product C", Quantity = 2, Price = 29.99 }
    })
    .AddTable(CDS.FluentHtmlReports.TableFixedHeader.Header, new[]
    {
        new { Product = "Product A", Quantity = 10, Price = 9.99 },
        new { Product = "Product B", Quantity = 5, Price = 19.99 },
        new { Product = "Product C", Quantity = 2, Price = 29.99 }
    })
    .AddTable(CDS.FluentHtmlReports.TableFixedHeader.FirstColumn, new[]
    {
        new { Product = "Product A", Quantity = 10, Price = 9.99 },
        new { Product = "Product B", Quantity = 5, Price = 19.99 },
        new { Product = "Product C", Quantity = 2, Price = 29.99 }
    })
    .AddTable(CDS.FluentHtmlReports.TableFixedHeader.Both, new[]
    {
        new { Product = "Product A", Quantity = 10, Price = 9.99 },
        new { Product = "Product B", Quantity = 5, Price = 19.99 },
        new { Product = "Product C", Quantity = 2, Price = 29.99 }
    })
    .AddVerticalBarChart("Chart 1", [("Pass", 1, "lime"), ("Fail", 2, "red"), ("NoLens", 4, "orange")])
    .AddVerticalBarChart("Chart 2", Enumerable.Range(1, 6).Select(x =>  (x.ToString(), x * x)).ToArray())
    .AddVerticalBarChart("Chart 3", Enumerable.Range(1, 20).Select(x => (x.ToString(), x * x)).ToArray())
    .AddHorizontalBarChart("Horizontal Chart 1", [("Pass", 42, "lime"), ("Fail", 17, "red"), ("Skipped", 5, "orange")])
    .AddHorizontalBarChart("Horizontal Chart 2", Enumerable.Range(1, 8).Select(x => ($"Category {x}", x * 7)).ToArray())
    .AddHorizontalBarChart("Horizontal Chart 3", Enumerable.Range(1, 20).Select(x => ($"Item {x}", x * 3)).ToArray())
    .Generate();

var fileName =
    Path
    .Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "report.html");

await File.WriteAllTextAsync(fileName, report);

// Open the report in the default web browser
var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = fileName,
        UseShellExecute = true
    }
};
process.Start();    
