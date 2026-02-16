namespace CDS.FluentHtmlReports;

/// <summary>
/// Specifies the type of summary calculation for a table column.
/// </summary>
public enum SummaryType
{
    /// <summary>No summary for this column.</summary>
    None,

    /// <summary>Sum of all numeric values.</summary>
    Sum,

    /// <summary>Average of all numeric values.</summary>
    Average,

    /// <summary>Count of rows.</summary>
    Count,

    /// <summary>Minimum value.</summary>
    Min,

    /// <summary>Maximum value.</summary>
    Max
}
