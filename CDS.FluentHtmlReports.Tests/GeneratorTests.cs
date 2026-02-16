using CDS.FluentHtmlReports;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CDS.FluentHtmlReports.Tests;

/// <summary>
/// Comprehensive test suite for CDS.FluentHtmlReports library.
/// Tests content presence and structure, not exact HTML formatting.
/// </summary>
[TestClass]
public class GeneratorTests
{
    // ── Core API Tests ─────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Core")]
    public void Create_ReturnsGeneratorInstance()
    {
        var generator = Generator.Create("Test Report");

        generator.Should().NotBeNull();
        generator.Should().BeOfType<Generator>();
    }

    [TestMethod]
    [TestCategory("Core")]
    public void Generate_ProducesValidHtmlStructure()
    {
        var html = Generator.Create("Test").Generate();

        html.Should().Contain("<!DOCTYPE html>");
        html.Should().Contain("<html");
        html.Should().Contain("<head>");
        html.Should().Contain("<body>");
        html.Should().Contain("</html>");
        html.Should().Contain("<meta charset=\"utf-8\"");
    }

    [TestMethod]
    [TestCategory("Core")]
    public void Create_IncludesTitleInOutput()
    {
        var html = Generator.Create("My Report Title").Generate();

        html.Should().Contain("My Report Title");
    }

    [TestMethod]
    [TestCategory("Core")]
    public void MethodChaining_AllowsMultipleCalls()
    {
        var html = Generator.Create("Test")
            .AddHeading("Section 1")
            .AddParagraph("Content 1")
            .AddHeading("Section 2")
            .AddParagraph("Content 2")
            .Generate();

        html.Should().Contain("Section 1");
        html.Should().Contain("Content 1");
        html.Should().Contain("Section 2");
        html.Should().Contain("Content 2");
    }

    // ── Text Features ──────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Text")]
    public void AddHeading_H2_ProducesCorrectTag()
    {
        var html = Generator.Create("Test")
            .AddHeading("Subsection", HeadingLevel.H2)
            .Generate();

        html.Should().Contain("<h2>Subsection</h2>");
    }

    [TestMethod]
    [TestCategory("Text")]
    public void AddParagraph_IncludesText()
    {
        var html = Generator.Create("Test")
            .AddParagraph("This is a test paragraph.")
            .Generate();

        html.Should().Contain("This is a test paragraph.");
    }

    [TestMethod]
    [TestCategory("Text")]
    public void AddUnorderedList_ProducesListWithItems()
    {
        var html = Generator.Create("Test")
            .AddUnorderedList(["First", "Second", "Third"])
            .Generate();

        html.Should().Contain("<ul");
        html.Should().Contain("First");
        html.Should().Contain("Second");
        html.Should().Contain("Third");
    }

    [TestMethod]
    [TestCategory("Text")]
    public void AddAlert_Info_IncludesContent()
    {
        var html = Generator.Create("Test")
            .AddAlert(AlertLevel.Info, "This is informational.")
            .Generate();

        html.Should().Contain("This is informational.");
        html.Should().Contain("alert-info");
    }

    [TestMethod]
    [TestCategory("Text")]
    public void AddCodeBlock_PreservesFormatting()
    {
        var code = "var x = 10;\nConsole.WriteLine(x);";
        var html = Generator.Create("Test")
            .AddCodeBlock(code)
            .Generate();

        html.Should().Contain("<pre class=\"code-block\">");
        html.Should().Contain("<code>");
        html.Should().Contain("var x = 10;");
    }

    [TestMethod]
    [TestCategory("Text")]
    public void AddLink_ProducesAnchorTag()
    {
        var html = Generator.Create("Test")
            .AddLink("GitHub", "https://github.com")
            .Generate();

        html.Should().Contain("<a ");
        html.Should().Contain("href=\"https://github.com\"");
        html.Should().Contain("GitHub");
    }

    [TestMethod]
    [TestCategory("Text")]
    public void AddLabelValueRow_IncludesAllPairs()
    {
        var html = Generator.Create("Test")
            .AddLabelValueRow([
                ("Name", "Alice"),
                ("Age", "30")
            ])
            .Generate();

        html.Should().Contain("Name");
        html.Should().Contain("Alice");
        html.Should().Contain("Age");
        html.Should().Contain("30");
    }

    // ── Table Features ─────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Table")]
    public void AddTable_WithAnonymousObjects_IncludesAllData()
    {
        var data = new[]
        {
            new { Name = "Alice", Age = 30 },
            new { Name = "Bob", Age = 25 }
        };

        var html = Generator.Create("Test")
            .AddTable(TableFixedHeader.Header, data)
            .Generate();

        html.Should().Contain("<table");
        html.Should().Contain("Alice");
        html.Should().Contain("Bob");
        html.Should().Contain("30");
        html.Should().Contain("25");
    }

    [TestMethod]
    [TestCategory("Table")]
    public void AddKeyValueTable_IncludesAllPairs()
    {
        var html = Generator.Create("Test")
            .AddKeyValueTable([
                ("Version", "1.0.0"),
                ("Status", "Active")
            ])
            .Generate();

        html.Should().Contain("Version");
        html.Should().Contain("1.0.0");
        html.Should().Contain("Status");
        html.Should().Contain("Active");
    }

    [TestMethod]
    [TestCategory("Table")]
    public void AddTable_WithSummary_CalculatesSum()
    {
        var data = new[]
        {
            new { Item = "A", Quantity = 10 },
            new { Item = "B", Quantity = 20 }
        };

        var summaries = new Dictionary<string, SummaryType>
        {
            ["Quantity"] = SummaryType.Sum
        };

        var html = Generator.Create("Test")
            .AddTable(TableFixedHeader.Header, data, summaries)
            .Generate();

        html.Should().Contain("30"); // Sum of 10 + 20
    }

    // ── Chart Features ─────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Chart")]
    public void AddVerticalBarChart_IncludesLabelsAndValues()
    {
        var data = new[]
        {
            ("Q1", 100, "#4285F4"),
            ("Q2", 150, "#EA4335")
        };

        var html = Generator.Create("Test")
            .AddVerticalBarChart("Sales", data)
            .Generate();

        html.Should().Contain("Sales");
        html.Should().Contain("<svg");
        html.Should().Contain("Q1");
        html.Should().Contain("Q2");
    }

    [TestMethod]
    [TestCategory("Chart")]
    public void AddHorizontalBarChart_IncludesLabels()
    {
        var data = new[]
        {
            ("Team A", 85, "#34A853"),
            ("Team B", 92, "#FBBC04")
        };

        var html = Generator.Create("Test")
            .AddHorizontalBarChart("Performance", data)
            .Generate();

        html.Should().Contain("Performance");
        html.Should().Contain("<svg");
        html.Should().Contain("Team A");
        html.Should().Contain("Team B");
    }

    [TestMethod]
    [TestCategory("Chart")]
    public void AddPieChart_IncludesLabels()
    {
        var data = new[]
        {
            ("Product A", 60, "#4285F4"),
            ("Product B", 40, "#EA4335")
        };

        var html = Generator.Create("Test")
            .AddPieChart("Market Share", data)
            .Generate();

        html.Should().Contain("Market Share");
        html.Should().Contain("<svg");
        html.Should().Contain("Product A");
        html.Should().Contain("Product B");
    }

    [TestMethod]
    [TestCategory("Chart")]
    public void AddLineChart_SingleSeries_IncludesDataPoints()
    {
        var data = new[]
        {
            ("Mon", 20),
            ("Tue", 22),
            ("Wed", 21)
        };

        var html = Generator.Create("Test")
            .AddLineChart("Temperature", data)
            .Generate();

        html.Should().Contain("Temperature");
        html.Should().Contain("<svg");
        html.Should().Contain("Mon");
        html.Should().Contain("Tue");
    }

    // ── Layout Features ────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Layout")]
    public void AddKpiCards_IncludesAllCards()
    {
        var html = Generator.Create("Test")
            .AddKpiCards([
                ("Metric 1", "100"),
                ("Metric 2", "200")
            ])
            .Generate();

        html.Should().Contain("Metric 1");
        html.Should().Contain("100");
        html.Should().Contain("Metric 2");
        html.Should().Contain("200");
    }

    [TestMethod]
    [TestCategory("Layout")]
    public void AddProgressBar_IncludesLabelAndPercentage()
    {
        var html = Generator.Create("Test")
            .AddProgressBar("Completion", 75)
            .Generate();

        html.Should().Contain("Completion");
        html.Should().Contain("75");
    }

    [TestMethod]
    [TestCategory("Layout")]
    public void BeginColumns_EndColumns_ProducesTwoColumnLayout()
    {
        var html = Generator.Create("Test")
            .BeginColumns()
                .BeginColumn()
                    .AddParagraph("Left column")
                .EndColumn()
                .BeginColumn()
                    .AddParagraph("Right column")
                .EndColumn()
            .EndColumns()
            .Generate();

        html.Should().Contain("Left column");
        html.Should().Contain("Right column");
    }

    [TestMethod]
    [TestCategory("Layout")]
    public void BeginCollapsible_EndCollapsible_ProducesDetailsElement()
    {
        var html = Generator.Create("Test")
            .BeginCollapsible("Click to expand")
                .AddParagraph("Hidden content")
            .EndCollapsible()
            .Generate();

        html.Should().Contain("<details");
        html.Should().Contain("<summary");
        html.Should().Contain("Click to expand");
        html.Should().Contain("Hidden content");
    }

    // ── Encoding & Security ────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Security")]
    public void AddParagraph_EncodesLessThan()
    {
        var html = Generator.Create("Test")
            .AddParagraph("Value < 10")
            .Generate();

        html.Should().Contain("&lt;");
    }

    [TestMethod]
    [TestCategory("Security")]
    public void AddParagraph_PreventsCrossSiteScripting()
    {
        var html = Generator.Create("Test")
            .AddParagraph("<script>alert('XSS')</script>")
            .Generate();

        html.Should().Contain("&lt;script&gt;");
        html.Should().NotContain("<script>alert");
    }

    [TestMethod]
    [TestCategory("Security")]
    public void AddTable_EncodesValuesInCells()
    {
        var data = new[]
        {
            new { Name = "Alice", Note = "<important>" }
        };

        var html = Generator.Create("Test")
            .AddTable(TableFixedHeader.Header, data)
            .Generate();

        html.Should().Contain("&lt;important&gt;");
        html.Should().NotContain("<important>");
    }

    // ── Edge Cases ─────────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("EdgeCase")]
    public void Create_WithEmptyTitle_DoesNotThrow()
    {
        var act = () => Generator.Create("").Generate();

        act.Should().NotThrow();
    }

    [TestMethod]
    [TestCategory("EdgeCase")]
    public void AddParagraph_WithEmptyString_DoesNotThrow()
    {
        var act = () => Generator.Create("Test")
            .AddParagraph("")
            .Generate();

        act.Should().NotThrow();
    }

    [TestMethod]
    [TestCategory("EdgeCase")]
    public void AddTable_WithEmptyArray_DoesNotThrow()
    {
        var data = Array.Empty<object>();

        var act = () => Generator.Create("Test")
            .AddTable(TableFixedHeader.Header, data)
            .Generate();

        act.Should().NotThrow();
    }

    [TestMethod]
    [TestCategory("EdgeCase")]
    public void AddUnorderedList_WithEmptyArray_DoesNotThrow()
    {
        var act = () => Generator.Create("Test")
            .AddUnorderedList([])
            .Generate();

        act.Should().NotThrow();
    }

    // ── Options ────────────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Options")]
    public void WithOptions_AppliesChartWidthPercent()
    {
        var options = new ReportOptions { ChartWidthPercent = 75 };

        var data = new[] { ("A", 10, "#4285F4") };

        var html = Generator.Create("Test")
            .WithOptions(options)
            .AddVerticalBarChart("Test", data)
            .Generate();

        // ChartWidthPercent of 75% on 900px container = 675px
        html.Should().Contain("max-width:675px");
    }

    [TestMethod]
    [TestCategory("Options")]
    public void WithOptions_AppliesChartHeight()
    {
        var options = new ReportOptions { ChartHeight = 500 };

        var data = new[] { ("A", 10, "#4285F4") };

        var html = Generator.Create("Test")
            .WithOptions(options)
            .AddVerticalBarChart("Test", data)
            .Generate();

        html.Should().Contain("500");
    }

    // ── Integration ────────────────────────────────────────────────────

    [TestMethod]
    [TestCategory("Integration")]
    public void ComplexReport_AllFeaturesWorkTogether()
    {
        var tableData = new[] { new { Name = "Test", Value = 123 } };
        var chartData = new[] { ("Q1", 100, "#4285F4"), ("Q2", 150, "#EA4335") };

        var html = Generator.Create("Integration Test")
            .AddHeading("Introduction")
            .AddParagraph("This is a test.")
            .AddTable(TableFixedHeader.Header, tableData)
            .AddVerticalBarChart("Performance", chartData)
            .BeginColumns()
                .BeginColumn()
                    .AddParagraph("Left")
                .EndColumn()
                .BeginColumn()
                    .AddParagraph("Right")
                .EndColumn()
            .EndColumns()
            .AddKpiCards([("Metric", "999")])
            .AddProgressBar("Progress", 80)
            .AddFooter("Footer text")
            .Generate();

        html.Should().Contain("Introduction");
        html.Should().Contain("This is a test.");
        html.Should().Contain("Test");
        html.Should().Contain("Performance");
        html.Should().Contain("Left");
        html.Should().Contain("Right");
        html.Should().Contain("Metric");
        html.Should().Contain("80");
        html.Should().Contain("Footer text");
    }
}
