namespace CDS.FluentHtmlReports;

/// <summary>
/// Predefined cell styles for conditional table formatting.
/// </summary>
public enum CellStyle
{
    /// <summary>Default cell appearance.</summary>
    Default,

    /// <summary>Green background for positive/success values.</summary>
    Green,

    /// <summary>Red background for negative/error values.</summary>
    Red,

    /// <summary>Amber/orange background for warning values.</summary>
    Amber,

    /// <summary>Blue background for informational values.</summary>
    Blue,

    /// <summary>Light gray background for muted values.</summary>
    Muted
}
