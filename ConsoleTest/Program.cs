using System.Diagnostics;

var report =
    CDS
    .FluentHtmlReports
    .Generator
    .Create(title: "Monthly Report")
    .AddParagraph("This is the monthly report for June 2024.")
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
