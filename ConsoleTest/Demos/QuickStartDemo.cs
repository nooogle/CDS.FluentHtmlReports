using CDS.FluentHtmlReports;

namespace ConsoleTest.Demos;

/// <summary>
/// A minimal "Quick Start" demo perfect for showcasing the library's simplicity.
/// This example demonstrates the fluent API with a realistic but concise report.
/// </summary>
public static class QuickStartDemo
{
    public static string Generate()
    {
        // Sample data for our mini report
        var teamMembers = new[]
        {
            new { Name = "Alice Johnson", Role = "Backend", TasksCompleted = 23, Status = "Active" },
            new { Name = "Bob Smith", Role = "Frontend", TasksCompleted = 19, Status = "Active" },
            new { Name = "Carol White", Role = "QA", TasksCompleted = 31, Status = "Active" }
        };

        return Generator
            .Create("Weekly Team Report")

            .AddParagraph("Here's a quick overview of this week's progress for the development team.")

            .AddLabelValueRow([
                ("Week Ending", DateTime.Now.ToString("MMM dd, yyyy")),
                ("Team", "Engineering"),
                ("Sprint", "Sprint 24")
            ])

            .AddLine()

            .AddHeading("Team Summary")
            .AddKpiCards([
                ("Total Tasks", "73"),
                ("Completed", "68"),
                ("In Progress", "5"),
                ("Success Rate", "93%")
            ])

            .AddLine()

            .AddHeading("Team Members")
            .AddTable(TableFixedHeader.Header, teamMembers)

            .AddLine()

            .AddHeading("Task Completion by Role")
            .AddVerticalBarChart("Tasks Completed This Week", [
                ("Backend", 23),
                ("Frontend", 19),
                ("QA", 31)
            ])

            .AddLine()

            .AddAlert(AlertLevel.Success, "All sprint goals achieved! Great work team! 🎉")

            .AddFooter("Generated with CDS.FluentHtmlReports — {timestamp}")
            .Generate();
    }
}
