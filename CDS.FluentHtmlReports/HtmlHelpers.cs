using System.Net;

namespace CDS.FluentHtmlReports;

/// <summary>
/// Shared HTML utility methods used by renderers.
/// </summary>
internal static class HtmlHelpers
{
    /// <summary>
    /// HTML-encodes the specified value, treating <c>null</c> as an empty string.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <returns>The HTML-encoded string.</returns>
    internal static string Enc(string value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
