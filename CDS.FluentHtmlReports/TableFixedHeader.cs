namespace CDS.FluentHtmlReports;

/// <summary>
/// Controls which parts of a table receive fixed (bold/shaded) formatting.
/// </summary>
public enum TableFixedHeader
{
    /// <summary>No fixed formatting applied.</summary>
    None,

    /// <summary>The header row receives fixed formatting.</summary>
    Header,

    /// <summary>The first column receives fixed formatting.</summary>
    FirstColumn,

    /// <summary>Both the header row and the first column receive fixed formatting.</summary>
    Both
}
