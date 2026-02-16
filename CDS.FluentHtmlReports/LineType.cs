namespace CDS.FluentHtmlReports;

/// <summary>
/// Specifies the visual style of a separator line in the report.
/// </summary>
public enum LineType
{
    /// <summary>An invisible spacer (blank vertical gap).</summary>
    Blank,

    /// <summary>A solid horizontal rule.</summary>
    Solid,

    /// <summary>A dashed horizontal rule.</summary>
    Dashed,

    /// <summary>A dotted horizontal rule.</summary>
    Dotted
}
