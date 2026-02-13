using System.Diagnostics;

var report =
    CDS
    .FluentHtmlReports
    .Generator
    .Create(title: "Monthly Report")
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
