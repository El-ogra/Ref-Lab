using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace RefLab.Infrastructure.Reports;

/// <summary>
/// The branding that the laboratory prints on every document.
/// </summary>
/// <remarks>
/// <para>
/// The branding belongs to the rendered document, and the renderer is the only
/// component that composes one. Keeping the strings here means the
/// presentation layer never sees them and never assembles report content.
/// </para>
/// <para>
/// Every Arabic string below is transcribed from
/// <c>Docs/ui-labels-verified.md</c>, which is the only authorised source of
/// Arabic user-interface labels. The English strings are transcribed from the
/// same document, section 6.2.
/// </para>
/// <para>
/// The Arabic lines are printed with an Arabic-capable font. The font that
/// ships with the PDF engine has no Arabic glyphs, and the engine refuses to
/// render a glyph it cannot resolve, so an Arabic-capable font is registered
/// from the operating system. When no such font can be found the Arabic lines
/// are omitted rather than rendered as replacement characters.
/// </para>
/// </remarks>
public static class ReportBranding
{
    /// <summary>The Arabic name of the system, as printed on every report.</summary>
    public const string ArabicSystemName = "ريال لاب سيستم";

    /// <summary>The Arabic title of the report.</summary>
    public const string ArabicReportTitle = "التقرير المختبرى";

    /// <summary>
    /// The Arabic laboratory-name line.
    /// </summary>
    /// <remarks>
    /// The source records this line as <c>[unclear]</c>: it is the name of the
    /// laboratory rather than of the product, and the laboratory name is not a
    /// setting of Ref Lab. The line is therefore recorded here for provenance
    /// and is deliberately not printed on a live document.
    /// </remarks>
    public const string UnverifiedLaboratoryNameLine = "معمل برقياسى";

    /// <summary>The English software line.</summary>
    public const string EnglishSoftwareLine = "Computer Software";

    /// <summary>The English name of the system.</summary>
    public const string EnglishSystemName = "real lab system";

    /// <summary>The English tagline.</summary>
    public const string EnglishTagline = "For Medical Clinical Laboratory";

    /// <summary>
    /// The currency of the laboratory, as fixed by the project owner.
    /// </summary>
    /// <remarks>
    /// The currency code is carried on every order by the domain, and the
    /// presentation layer supplies it when it registers an order. The renderer
    /// has no access to the order, because the report request carries no
    /// currency, so the laboratory's own currency is stated in the branding
    /// block of every document.
    /// </remarks>
    public const string CurrencyCode = "EGP";

    /// <summary>The English branding line.</summary>
    public static string EnglishLine { get; } = $"{EnglishSoftwareLine} | {EnglishSystemName} | {EnglishTagline}";

    /// <summary>The currency line.</summary>
    public static string CurrencyLine { get; } = $"Currency: {CurrencyCode}";

    /// <summary>The family name of the registered Arabic-capable font.</summary>
    public static string? ArabicFontFamily { get; }

    /// <summary>Whether an Arabic-capable font was found and registered.</summary>
    public static bool IsArabicFontAvailable => ArabicFontFamily is not null;

    /// <summary>The font families used for the Arabic lines, in order of preference.</summary>
    public static string[] FontFamilies { get; } =
        Array.Empty<string>();

    private static readonly (string Path, string Family)[] ArabicFontCandidates =
    {
        // Windows — the integration target.
        (@"C:\Windows\Fonts\tahoma.ttf", "Tahoma"),
        (@"C:\Windows\Fonts\arial.ttf", "Arial"),
        (@"C:\Windows\Fonts\segoeui.ttf", "Segoe UI"),
        (@"C:\Windows\Fonts\calibri.ttf", "Calibri"),
        // Linux — the build agent.
        ("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", "DejaVu Sans"),
        ("/usr/share/fonts/dejavu/DejaVuSans.ttf", "DejaVu Sans"),
        ("/usr/share/fonts/TTF/DejaVuSans.ttf", "DejaVu Sans"),
        ("/usr/share/fonts/truetype/freefont/FreeSerif.ttf", "FreeSerif"),
        ("/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf", "Liberation Sans"),
    };

    static ReportBranding()
    {
        foreach (var (path, family) in ArabicFontCandidates)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            try
            {
                using var stream = File.OpenRead(path);

                var before = FontManager.GetRegisteredFonts().Select(font => font.FamilyName).ToHashSet(StringComparer.Ordinal);

                FontManager.RegisterFontFromStream(stream);

                var registered = FontManager.GetRegisteredFonts()
                    .Select(font => font.FamilyName)
                    .FirstOrDefault(name => !before.Contains(name));

                ArabicFontFamily = registered ?? family;

                FontFamilies = new[] { "Lato", ArabicFontFamily };

                break;
            }
            catch (IOException)
            {
                // The candidate could not be opened; try the next one.
            }
            catch (UnauthorizedAccessException)
            {
                // The candidate could not be opened; try the next one.
            }
        }
    }

    /// <summary>
    /// Applies the branding block to the top of a column.
    /// </summary>
    /// <param name="column">The column the branding is added to.</param>
    public static void Apply(ColumnDescriptor column)
    {
        ArgumentNullException.ThrowIfNull(column);

        if (IsArabicFontAvailable)
        {
            column.Item()
                .Text(ArabicSystemName)
                .FontFamily(FontFamilies)
                .DirectionFromRightToLeft()
                .FontSize(11)
                .SemiBold();

            column.Item()
                .Text(ArabicReportTitle)
                .FontFamily(FontFamilies)
                .DirectionFromRightToLeft()
                .FontSize(9);
        }

        column.Item().Text(EnglishLine).FontSize(8);

        column.Item().Text(CurrencyLine).FontSize(8);
    }
}
