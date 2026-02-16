using CDS.FluentHtmlReports;

namespace ConsoleTest.Demos;

/// <summary>
/// Demonstrates text-related features: headings, paragraphs, lists, alerts,
/// code blocks, badges, links, definition lists, and metadata.
/// </summary>
public static class TextFeaturesDemo
{
    public static string Generate()
    {
        return Generator
            .Create("Text Features Demo")

            // Headings
            .AddHeading("Heading Levels")
            .AddParagraph("The library supports all six HTML heading levels for document structure.")
            .AddHeading("Heading 2 (H2)", HeadingLevel.H2)
            .AddHeading("Heading 3 (H3)", HeadingLevel.H3)
            .AddHeading("Heading 4 (H4)", HeadingLevel.H4)
            .AddHeading("Heading 5 (H5)", HeadingLevel.H5)
            .AddHeading("Heading 6 (H6)", HeadingLevel.H6)

            .AddLine()

            // Paragraphs and metadata
            .AddHeading("Paragraphs & Metadata")
            .AddParagraph("This is a standard paragraph. It can contain any text content and will be rendered with consistent spacing and font sizing.")
            .AddMetadata("Author", "CDS FluentHtmlReports")
            .AddMetadata("Version", "2.0.0")
            .AddLabelValueRow([("User", Environment.UserName), ("Machine", Environment.MachineName), ("OS", Environment.OSVersion.ToString())])

            .AddLine(LineType.Dashed)

            // Lists
            .AddHeading("Bullet Lists")
            .AddParagraph("Unordered lists are great for feature summaries, checklists, and notes:")
            .AddUnorderedList([
                "Passed all validation checks",
                "Exported 1,204 rows to CSV",
                "No duplicate records found",
                "Processing completed in 3.2 seconds"
            ])

            .AddHeading("Numbered Lists")
            .AddParagraph("Ordered lists work well for step-by-step procedures:")
            .AddOrderedList([
                "Connect to the source database",
                "Run schema migration scripts",
                "Validate foreign key constraints",
                "Export reconciliation report",
                "Notify stakeholders via email"
            ])

            .AddLine(LineType.Dashed)

            // Alerts
            .AddHeading("Alert Boxes")
            .AddParagraph("Four severity levels for callout/notification boxes:")
            .AddAlert(AlertLevel.Info, "This is an informational message. Use it for general notes and context.")
            .AddAlert(AlertLevel.Success, "All 1,204 records were processed successfully with no errors.")
            .AddAlert(AlertLevel.Warning, "3 records had missing postal codes. Default values were applied.")
            .AddAlert(AlertLevel.Error, "Database connection timed out after 5 retry attempts. Please check connectivity.")

            .AddLine(LineType.Dashed)

            // Code blocks
            .AddHeading("Code Blocks")
            .AddParagraph("Preformatted code blocks with monospace font — ideal for SQL, config, or stack traces:")
            .AddCodeBlock("""
                SELECT o.OrderId, c.CustomerName, o.Total
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerId = c.Id
                WHERE o.Status = 'Pending'
                ORDER BY o.CreatedDate DESC;
                """, "sql")
            .AddParagraph("Plain text code block (no language specified):")
            .AddCodeBlock("""
                [2024-06-15 14:32:01] INFO  Application started
                [2024-06-15 14:32:02] INFO  Connected to database (prod-db-03)
                [2024-06-15 14:32:02] WARN  Slow query detected (1.2s)
                [2024-06-15 14:32:05] ERROR Connection reset by peer
                """)

            .AddLine(LineType.Dashed)

            // Badges
            .AddHeading("Badges")
            .AddParagraph("Small colored labels for status indicators:")
            .AddBadge("PASS", "#4CAF50")
            .AddBadge("FAIL", "#F44336")
            .AddBadge("PENDING", "#FF9800")
            .AddBadge("SKIPPED", "#9E9E9E")
            .AddBadge("CRITICAL", "#9C27B0")

            .AddLine(LineType.Dashed)

            // Links
            .AddHeading("Links")
            .AddParagraph("Simple hyperlinks for navigation between reports or to external resources:")
            .AddLink("View the full documentation", "https://example.com/docs")
            .AddLink("Download raw data (CSV)", "https://example.com/data.csv")

            .AddLine(LineType.Dashed)

            // Definition lists
            .AddHeading("Definition Lists")
            .AddParagraph("Structured term/definition pairs for configuration or glossary entries:")
            .AddDefinitionList([
                ("Environment", "Production (us-east-1)"),
                ("Database", "PostgreSQL 15.3"),
                ("Connection Pool", "Min: 5, Max: 100"),
                ("Timeout", "30 seconds"),
                ("SSL", "Enabled (TLS 1.3)")
            ])

            .AddLine(LineType.Dashed)

            // Separator lines
            .AddHeading("Separator Lines")
            .AddParagraph("Four line styles for visual structure:")
            .AddParagraph("Solid:")
            .AddLine(LineType.Solid)
            .AddParagraph("Dashed:")
            .AddLine(LineType.Dashed)
            .AddParagraph("Dotted:")
            .AddLine(LineType.Dotted)
            .AddParagraph("Blank (invisible spacer):")
            .AddLine(LineType.Blank)
            .AddParagraph("(the blank line added a 20px gap above this paragraph)")

            .AddFooter()
            .Generate();
    }
}
