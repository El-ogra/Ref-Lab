using UglyToad.PdfPig;

namespace RefLab.Integration.Tests.Integration;

/// <summary>
/// Reads the text of a produced PDF.
/// </summary>
/// <remarks>
/// <para>
/// The report is written with subsetted fonts, so the characters in the
/// content stream of the document are glyph indices rather than text. A
/// reader that honours the <c>ToUnicode</c> map of the document is therefore
/// the only way to assert on what the report actually says — and that is what
/// a PDF reader does.
/// </para>
/// <para>
/// PdfPig is a pure .NET library, so the same assertion runs on the Linux
/// build agent and on the owner's Windows machine.
/// </para>
/// </remarks>
public static class PdfTextReader
{
    /// <summary>
    /// Extracts the readable text of a produced document.
    /// </summary>
    /// <param name="path">The path of the document.</param>
    /// <returns>The text of the document.</returns>
    public static string ExtractText(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        using var document = PdfDocument.Open(path);

        return string.Join(
            Environment.NewLine,
            document.GetPages().Select(page => page.Text));
    }
}
