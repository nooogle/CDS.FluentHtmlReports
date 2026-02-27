using System.Diagnostics;
using System.Reflection;
using CDS.FluentHtmlReports;
using ConsoleTest.Demos;

// ── Setup output folder in user's Downloads ─────────────────────────────
var outputFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "Downloads",
    "FluentHtmlReports-Demo");

// Clean and recreate
if (Directory.Exists(outputFolder))
{
    Directory.Delete(outputFolder, recursive: true);
}
Directory.CreateDirectory(outputFolder);

Console.WriteLine($"Generating reports to: {outputFolder}");

// ── Generate all demo reports ───────────────────────────────────────────
var demos = new (string fileName, string title, string description, Func<string> generate)[]
{
    ("00-quick-start.html",
     "Quick Start",
     "A minimal example showing the fluent API — perfect for README/documentation.",
     QuickStartDemo.Generate),

    ("01-text-features.html",
     "Text Features",
     "Headings, paragraphs, lists, alerts, code blocks, badges, links, definition lists, and separators.",
     TextFeaturesDemo.Generate),

    ("02-table-features.html",
     "Table Features",
     "Table header modes, conditional cell formatting, summary rows, and key-value tables.",
     TableFeaturesDemo.Generate),

    ("03-chart-features.html",
     "Chart Features",
     "Vertical bar, horizontal bar, pie, donut, and line charts with options for sizing and alignment.",
     ChartFeaturesDemo.Generate),

    ("04-layout-features.html",
     "Layout Features",
     "KPI cards, progress bars, collapsible sections, two-column layout, and page breaks.",
     LayoutFeaturesDemo.Generate),

    ("05-all-features.html",
     "Complete Feature Showcase",
     "A realistic build pipeline report that exercises every feature in one cohesive document.",
     AllFeaturesDemo.Generate),

    ("06-image-features.html",
     "Image Features",
     "Inline SVG, base64 bytes, streams, URL images, sizing, alignment, and captions.",
     ImageFeaturesDemo.Generate),
};

foreach (var (fileName, title, _, generate) in demos)
{
    var html = generate();
    var path = Path.Combine(outputFolder, fileName);
    await File.WriteAllTextAsync(path, html);
    Console.WriteLine($"  Created: {fileName}");
}

// ── Generate the index report with links to all demos ───────────────────
var fullVersion = typeof(Generator).Assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    ?.InformationalVersion ?? "unknown";

// Strip build metadata (the +commithash part) from version string
var libraryVersion = fullVersion.Split('+')[0];

var indexHtml = Generator
    .Create($"CDS.FluentHtmlReports version {libraryVersion} — Demo Index")
    .AddParagraph("Welcome! This index links to all feature demonstration reports. Each report showcases a different category of the library's capabilities.")
    .AddLabelValueRow([
        ("Generated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
        ("User", Environment.UserName),
        ("Machine", Environment.MachineName)
    ])
    .AddLine(LineType.Blank)

    .AddKpiCards([
        ("Demo Reports", demos.Length.ToString()),
        ("Features", "18+"),
        ("Dependencies", "0"),
        ("Output", "Self-Contained HTML")
    ])

    .AddLine()
    .AddHeading("Demo Reports")
    .AddParagraph("Click any link below to open the corresponding feature demo:")

    // Links to each demo
    .AddLink("0. Quick Start — minimal example perfect for README/documentation (⭐ Start here!)", "00-quick-start.html")
    .AddLink("1. Text Features — headings, lists, alerts, code blocks, badges, links, definitions", "01-text-features.html")
    .AddLink("2. Table Features — header modes, conditional formatting, summary rows, key-value tables", "02-table-features.html")
    .AddLink("3. Chart Features — vertical/horizontal bars, pie, donut, line charts", "03-chart-features.html")
    .AddLink("4. Layout Features — KPI cards, progress bars, collapsible sections, columns, page breaks", "04-layout-features.html")
    .AddLink("5. Complete Feature Showcase — all features in one realistic build pipeline report", "05-all-features.html")
    .AddLink("6. Image Features — inline SVG, base64, streams, URL images, sizing, alignment, captions", "06-image-features.html")

    .AddLine()

    .AddHeading("Feature Summary")
    .AddParagraph("Everything supported by the library:")

    .BeginColumns()
        .BeginColumn()
            .AddHeading("Text & Content", HeadingLevel.H3)
            .AddUnorderedList([
                "Headings (H1-H6)",
                "Paragraphs",
                "Bullet & numbered lists",
                "Alert boxes (info/success/warning/error)",
                "Code blocks",
                "Badges / status labels",
                "Hyperlinks",
                "Definition lists",
                "Metadata & label-value rows",
                "Separator lines (solid/dashed/dotted/blank)",
                "Raw HTML insertion",
                "Footer with timestamp"
            ])
        .EndColumn()
        .BeginColumn()
            .AddHeading("Tables", HeadingLevel.H3)
            .AddUnorderedList([
                "Auto-columns from object properties",
                "4 header formatting modes",
                "Conditional cell coloring",
                "Summary rows (sum/avg/min/max/count)",
                "Key-value tables"
            ])
            .AddHeading("Charts (SVG)", HeadingLevel.H3)
            .AddUnorderedList([
                "Vertical bar charts",
                "Horizontal bar charts",
                "Pie charts",
                "Donut charts",
                "Line charts (single & multi-series)",
                "Custom colors or auto-palette",
                "Configurable size & alignment"
            ])
        .EndColumn()
    .EndColumns()

    .BeginColumns()
        .BeginColumn()
            .AddHeading("Layout", HeadingLevel.H3)
            .AddUnorderedList([
                "Base64 image embedding (bytes, stream, file)",
                "Inline SVG injection",
                "URL-referenced images",
                "Image sizing (max-width %)",
                "Image alignment (left/center/right)",
                "Figure captions",
                "KPI summary cards",
                "Progress bars (auto-colored)",
                "Collapsible sections (HTML5 details)",
                "Two-column layout (CSS flexbox)",
                "Page breaks for print/PDF",
                "Base64 image embedding"
            ])
        .EndColumn()
        .BeginColumn()
            .AddHeading("Design Principles", HeadingLevel.H3)
            .AddUnorderedList([
                "Zero dependencies",
                "Self-contained HTML output",
                "No JavaScript required",
                "Print/PDF-friendly",
                "Fluent API — method chaining",
                "Reflection-based tables"
            ])
        .EndColumn()
    .EndColumns()

    .AddLine()
    .AddAlert(AlertLevel.Info, "All reports are self-contained HTML files. No external CSS, JavaScript, or image files are needed. Open them in any browser, email them as attachments, or print directly to PDF.")

    .AddFooter("CDS.FluentHtmlReports Demo Suite — {timestamp}")
    .Generate();

var indexPath = Path.Combine(outputFolder, "index.html");
await File.WriteAllTextAsync(indexPath, indexHtml);
Console.WriteLine($"  Created: index.html");
Console.WriteLine();
Console.WriteLine($"Opening index report in browser...");

// Open the index report in the default web browser
try
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = indexPath,
            UseShellExecute = true
        }
    };
    process.Start();
}
catch (Exception ex)
{
    Console.WriteLine($"Could not open browser automatically: {ex.Message}");
    Console.WriteLine($"Open manually: {indexPath}");
}
