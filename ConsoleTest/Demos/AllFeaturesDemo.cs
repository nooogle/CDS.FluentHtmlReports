using CDS.FluentHtmlReports;

namespace ConsoleTest.Demos;

/// <summary>
/// Master demo that exercises every feature in a single cohesive report.
/// </summary>
public static class AllFeaturesDemo
{
    public static string Generate()
    {
        return Generator
            .Create("CDS.FluentHtmlReports — Complete Feature Showcase")

            // ── KPI overview ────────────────────────────────────────────
            .AddParagraph("This report demonstrates every feature available in the library, assembled into a realistic scenario: a nightly build and test pipeline summary.")
            .AddLabelValueRow([
                ("Pipeline", "nightly-build-2024-06-15"),
                ("Branch", "main"),
                ("Commit", "a3f7c2d")
            ])
            .AddLine(LineType.Blank)

            .AddKpiCards([
                ("Total Tests", "1,247"),
                ("Passed", "1,198"),
                ("Failed", "42"),
                ("Skipped", "7"),
                ("Duration", "04:12"),
                ("Coverage", "87%")
            ])

            .AddAlert(AlertLevel.Warning, "42 test failures detected — review the breakdown below.")

            .AddLine()

            // ── Results overview with charts ────────────────────────────
            .AddHeading("Results Overview")
            .BeginColumns()
                .BeginColumn()
                    .AddPieChart("Test Outcomes", [
                        ("Passed", 1198, "#4CAF50"),
                        ("Failed", 42, "#F44336"),
                        ("Skipped", 7, "#9E9E9E")
                    ])
                .EndColumn()
                .BeginColumn()
                    .AddDonutChart("Coverage by Module", [
                        ("Core", 94, "#4CAF50"),
                        ("API", 88, "#2196F3"),
                        ("UI", 72, "#FF9800"),
                        ("Data", 91, "#9C27B0")
                    ])
                .EndColumn()
            .EndColumns()

            // ── Trend chart ─────────────────────────────────────────────
            .AddHeading("Failure Trend (Last 10 Builds)")
            .AddLineChart("Test Failures Over Time", [
                ("Unit Tests", [
                    ("Build 91", 2), ("Build 92", 5), ("Build 93", 3), ("Build 94", 8),
                    ("Build 95", 4), ("Build 96", 12), ("Build 97", 6), ("Build 98", 15),
                    ("Build 99", 10), ("Build 100", 28)
                ]),
                ("Integration", [
                    ("Build 91", 1), ("Build 92", 0), ("Build 93", 2), ("Build 94", 1),
                    ("Build 95", 3), ("Build 96", 2), ("Build 97", 5), ("Build 98", 3),
                    ("Build 99", 8), ("Build 100", 14)
                ])
            ])

            // ── Horizontal bar chart ────────────────────────────────────
            .AddHeading("Failures by Module")
            .AddHorizontalBarChart("Module Failure Count", [
                ("Authentication", 15, "#F44336"),
                ("Payment", 12, "#FF9800"),
                ("Search", 8, "#FFEB3B"),
                ("User Profile", 5, "#2196F3"),
                ("Notifications", 2, "#4CAF50")
            ])

            // ── Table with formatting and summary ───────────────────────
            .AddHeading("Detailed Test Results")
            .AddTable(
                TableFixedHeader.Header,
                new object[]
                {
                    new { Suite = "Auth.Login", Tests = 45, Passed = 42, Failed = 3, Coverage = 91.2 },
                    new { Suite = "Auth.OAuth", Tests = 32, Passed = 20, Failed = 12, Coverage = 68.5 },
                    new { Suite = "Payment.Charge", Tests = 28, Passed = 26, Failed = 2, Coverage = 94.1 },
                    new { Suite = "Payment.Refund", Tests = 18, Passed = 8, Failed = 10, Coverage = 55.0 },
                    new { Suite = "Search.Index", Tests = 52, Passed = 44, Failed = 8, Coverage = 82.3 },
                    new { Suite = "User.Profile", Tests = 38, Passed = 33, Failed = 5, Coverage = 88.7 },
                    new { Suite = "Notifications", Tests = 22, Passed = 20, Failed = 2, Coverage = 95.2 },
                },
                cellFormat: (col, val) =>
                {
                    if (col == "Failed" && int.TryParse(val, out int f))
                    {
                        if (f >= 10) return CellStyle.Red;
                        if (f >= 5) return CellStyle.Amber;
                        if (f > 0) return CellStyle.Muted;
                    }
                    if (col == "Coverage" && double.TryParse(val, out double c))
                    {
                        if (c >= 90) return CellStyle.Green;
                        if (c < 70) return CellStyle.Red;
                    }
                    return CellStyle.Default;
                },
                summaryColumns: new Dictionary<string, SummaryType>
                {
                    { "Tests", SummaryType.Sum },
                    { "Passed", SummaryType.Sum },
                    { "Failed", SummaryType.Sum },
                    { "Coverage", SummaryType.Average }
                })

            // ── Progress bars ───────────────────────────────────────────
            .AddHeading("Module Coverage")
            .AddProgressBar("Core Library", 94, suffix: "%")
            .AddProgressBar("API Layer", 88, suffix: "%")
            .AddProgressBar("UI Components", 72, suffix: "%")
            .AddProgressBar("Data Access", 91, suffix: "%")
            .AddProgressBar("Overall", 87, suffix: "%")

            // ── Badges ──────────────────────────────────────────────────
            .AddLine()
            .AddHeading("Build Status")
            .AddBadge("BUILD PASSED", "#4CAF50")
            .AddBadge("42 FAILURES", "#F44336")
            .AddBadge("DEPLOY BLOCKED", "#FF9800")
            .AddBadge("REVIEW REQUIRED", "#2196F3")

            // ── Key-value table ─────────────────────────────────────────
            .AddLine()
            .AddHeading("Build Environment")
            .AddKeyValueTable([
                ("Build Agent", "runner-prod-03"),
                ("SDK Version", ".NET 10.0.100"),
                ("Configuration", "Release"),
                ("Platform", "linux-x64"),
                ("Git Branch", "main"),
                ("Git Commit", "a3f7c2d8e1b4f6a9c3d5e7f8"),
                ("Triggered By", "Schedule (nightly)")
            ])

            // ── Collapsible error details ───────────────────────────────
            .AddHeading("Error Details")
            .BeginCollapsible("Auth.OAuth — 12 failures (click to expand)")
                .AddAlert(AlertLevel.Error, "OAuth token refresh failing consistently — upstream provider timeout.")
                .AddCodeBlock("""
                    System.Net.Http.HttpRequestException: Connection refused (oauth.provider.com:443)
                       at System.Net.Http.HttpConnectionPool.ConnectAsync()
                       at AuthService.RefreshTokenAsync(string refreshToken)
                       at AuthTests.OAuth_RefreshToken_ShouldSucceed() in Tests/Auth/OAuthTests.cs:line 87
                    """)
                .AddParagraph("Affected tests:")
                .AddOrderedList([
                    "OAuth_RefreshToken_ShouldSucceed",
                    "OAuth_RefreshToken_WithExpiredToken_ShouldReauth",
                    "OAuth_Login_WithProvider_Google",
                    "OAuth_Login_WithProvider_GitHub",
                    "OAuth_Callback_ShouldExchangeCode",
                    "OAuth_Callback_WithInvalidState_ShouldFail",
                    "OAuth_Logout_ShouldRevokeToken",
                    "OAuth_Logout_ShouldClearSession",
                    "OAuth_TokenCache_ShouldExpire",
                    "OAuth_TokenCache_ShouldRefresh",
                    "OAuth_Scope_ShouldRequest",
                    "OAuth_Scope_ShouldValidate"
                ])
            .EndCollapsible()

            .BeginCollapsible("Payment.Refund — 10 failures")
                .AddAlert(AlertLevel.Warning, "Refund processing is returning 503 from payment gateway.")
                .AddUnorderedList([
                    "Refund_FullAmount_ShouldSucceed",
                    "Refund_PartialAmount_ShouldSucceed",
                    "Refund_DuplicateRequest_ShouldReject",
                    "Refund_ExpiredOrder_ShouldFail",
                    "Refund_WithNote_ShouldPersist"
                ])
            .EndCollapsible()

            // ── Definition list for next steps ──────────────────────────
            .AddLine()
            .AddHeading("Recommended Actions")
            .AddDefinitionList([
                ("Investigate OAuth", "Check oauth.provider.com connectivity and rate limits"),
                ("Payment Gateway", "Review 503 responses — possible maintenance window"),
                ("Search Index", "Reindex test data — possible schema drift after migration"),
                ("Block Deploy", "Do not deploy to production until failures are below 5")
            ])

            // ── Links ───────────────────────────────────────────────────
            .AddHeading("Related Resources")
            .AddLink("Build Pipeline Dashboard", "https://ci.example.com/pipeline/nightly")
            .AddLink("Previous Build Report", "https://ci.example.com/reports/build-99")
            .AddLink("Test Coverage Dashboard", "https://coverage.example.com/main")

            // ── Vertical bar chart ──────────────────────────────────────
            .AddHeading("Build Duration History")
            .AddVerticalBarChart("Build Time (minutes)", [
                ("Build 95", 38),
                ("Build 96", 42),
                ("Build 97", 35),
                ("Build 98", 51),
                ("Build 99", 47),
                ("Build 100", 44)
            ])

            // ── Page break before raw HTML section ──────────────────────
            .AddPageBreak()
            .AddHeading("Appendix: Raw HTML Support")
            .AddParagraph("The AddHtml method allows inserting arbitrary HTML when the built-in features don't cover an edge case:")
            .AddHtml("<blockquote style=\"border-left: 4px solid #1976D2; padding: 8px 16px; margin: 12px 0; color: #555; font-style: italic;\">\"The best reporting library is the one you actually use.\" — A pragmatic developer</blockquote>")

            .AddFooter("Generated by CDS.FluentHtmlReports — Build Pipeline Report — {timestamp}")
            .Generate();
    }
}
