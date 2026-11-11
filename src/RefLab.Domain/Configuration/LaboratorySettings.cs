using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Configuration;

/// <summary>
/// The printer roles the laboratory binds its report output to.
/// </summary>
/// <remarks>
/// Legacy evidence: six printer roles are established — the report printer, an
/// alternate report printer, the barcode printer, the ZARF printer, the
/// work-sheet printer and the identity-card printer. The first four exist in
/// both legacy versions; the last two were added in version 2.
/// </remarks>
public enum PrinterRole
{
    /// <summary>The printer that receives laboratory reports.</summary>
    Report = 1,

    /// <summary>The alternate printer that receives laboratory reports.</summary>
    ReportAlternate = 2,

    /// <summary>The printer that receives barcode labels.</summary>
    Barcode = 3,

    /// <summary>The ZARF printer.</summary>
    Zarf = 4,

    /// <summary>The printer that receives work sheets.</summary>
    WorkSheet = 5,

    /// <summary>The printer that receives identity cards.</summary>
    IdentityCard = 6,
}

/// <summary>
/// The paper size applied to every printed report.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>SETTING.ini [setup] papersize</c> carries the value
/// <c>A4</c>; the vendor guide documents both A4 and A5.
/// </remarks>
public enum PaperSize
{
    /// <summary>ISO A4.</summary>
    A4 = 1,

    /// <summary>ISO A5.</summary>
    A5 = 2,
}

/// <summary>
/// The laboratory-wide configuration: the paper size, the report margins, the
/// header and footer texts, the printer bindings and the accounting defaults.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the configuration is spread across <c>SETTING.ini</c>
/// (paper size, report sequence origin, barcode auto-create,
/// work-sheet printing, history display mode and the two global header texts),
/// <c>Data\*.ini</c> (the per-profile margins, header and footer texts, the
/// twenty colour keys and the background colour) and <c>IDCARD.INI</c> (the
/// connection receipt).
/// </para>
/// <para>
/// Ref Lab models the settings that carry business meaning as a domain
/// aggregate, so that the presentation layer has a single source for them and
/// Phase 4 has a single table to persist.
/// </para>
/// </remarks>
public sealed class LaboratorySettings
{
    private Dictionary<PrinterRole, string> _printers = new();

    private LaboratorySettings()
    {
    }

    /// <summary>
    /// The identity of the settings record. The laboratory settings are a
    /// singleton, so the identifier is always one.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>The paper size applied to every printed report.</summary>
    public PaperSize PaperSize { get; private set; } = PaperSize.A4;

    /// <summary>The origin of the report sequence numbers.</summary>
    public int ReportSequenceStart { get; private set; } = 1984;

    /// <summary>Whether a barcode is created automatically for every order.</summary>
    public bool IsBarcodeAutoCreated { get; private set; } = true;

    /// <summary>Whether work sheets are printed automatically.</summary>
    public bool IsWorkSheetPrinted { get; private set; } = true;

    /// <summary>The medical-history display mode.</summary>
    public int HistoryDisplayMode { get; private set; }

    /// <summary>The first global report header text.</summary>
    public string? GlobalHeaderText { get; private set; }

    /// <summary>The second global report header text.</summary>
    public string? GlobalHeaderSecondaryText { get; private set; }

    /// <summary>
    /// The footer text printed at the foot of every laboratory report.
    /// </summary>
    /// <remarks>
    /// The report footer is a document element of its own. It was previously
    /// written into <see cref="ConnectionReceiptFooter"/>, which is the footer
    /// of the connection receipt — a different document — so configuring the
    /// report silently destroyed the receipt footer. The two are now separate
    /// settings.
    /// </remarks>
    public string? ReportFooterText { get; private set; }

    /// <summary>
    /// The signature the laboratory prints at the foot of every report.
    /// </summary>
    /// <remarks>
    /// The reference system prints three fixed lines — the doctor's signature,
    /// the doctor's name and a line of thanks — at the foot of the clinical
    /// report. The laboratory that prints them is the one that owns them, so
    /// Ref Lab makes them settings rather than fixed text.
    /// </remarks>
    public string? DoctorSignature { get; private set; }

    /// <summary>The doctor's name, as printed at the foot of every report.</summary>
    public string? DoctorName { get; private set; }

    /// <summary>The line of thanks printed at the foot of every report.</summary>
    public string? ThanksText { get; private set; }

    /// <summary>The path of the laboratory logo printed on a document.</summary>
    public string? LabLogoPath { get; private set; }

    /// <summary>The path of the report heading logo printed on a document.</summary>
    public string? ReportLogoPath { get; private set; }

    /// <summary>The width, in pixels, the laboratory logo is printed at.</summary>
    public int? LogoWidthPx { get; private set; }

    /// <summary>The height, in pixels, the laboratory logo is printed at.</summary>
    public int? LogoHeightPx { get; private set; }

    /// <summary>Which results a printed report carries.</summary>
    public ReportHeadingMode HeadingMode { get; private set; }

    /// <summary>
    /// The layout text that accompanies the report body.
    /// </summary>
    /// <remarks>
    /// The layout text is the text that describes the report layout itself. It
    /// was previously written into <see cref="GlobalHeaderText"/>, which is the
    /// laboratory identity printed above the report titles, so that configuring
    /// the layout silently destroyed the identity header. The two are now
    /// separate settings.
    /// </remarks>
    public string? LayoutText { get; private set; }

    /// <summary>The left report margin.</summary>
    public decimal LeftMargin { get; private set; }

    /// <summary>The right report margin.</summary>
    public decimal RightMargin { get; private set; }

    /// <summary>The top report margin.</summary>
    public decimal TopMargin { get; private set; }

    /// <summary>The bottom report margin.</summary>
    public decimal BottomMargin { get; private set; }

    /// <summary>The report background colour, as a hexadecimal value.</summary>
    public string? BackgroundColour { get; private set; }

    /// <summary>The default account type applied to new orders.</summary>
    public string? DefaultAccountType { get; private set; }

    /// <summary>The header text of the connection receipt.</summary>
    public string? ConnectionReceiptHeader { get; private set; }

    /// <summary>The footer text of the connection receipt.</summary>
    public string? ConnectionReceiptFooter { get; private set; }

    /// <summary>The printers bound to each role.</summary>
    /// <remarks>
    /// The map is replaced — never mutated in place — whenever a printer is
    /// bound or unbound, so that the persistence layer can detect the change
    /// by comparing the stored value with the current one.
    /// </remarks>
    public IReadOnlyDictionary<PrinterRole, string> Printers
    {
        get => _printers;
        private set => _printers = value is null
            ? new Dictionary<PrinterRole, string>()
            : new Dictionary<PrinterRole, string>(value);
    }

    /// <summary>The identifier of the singleton settings record.</summary>
    public const int SingletonId = 1;

    /// <summary>Creates the laboratory settings.</summary>
    /// <exception cref="InvalidPatientOrderException">A margin is negative.</exception>
    public static LaboratorySettings Create()
    {
        var settings = new LaboratorySettings { Id = SingletonId };

        settings.SetMargins(0m, 0m, 0m, 0m);

        return settings;
    }

    /// <summary>Sets the paper size.</summary>
    public void SetPaperSize(PaperSize paperSize) => PaperSize = paperSize;

    /// <summary>Sets the origin of the report sequence numbers.</summary>
    public void SetReportSequenceStart(int start)
    {
        if (start <= 0)
        {
            throw new InvalidPatientOrderException("The report sequence origin must be a positive integer.");
        }

        ReportSequenceStart = start;
    }

    /// <summary>Sets whether a barcode is created automatically for every order.</summary>
    public void SetBarcodeAutoCreated(bool isBarcodeAutoCreated) => IsBarcodeAutoCreated = isBarcodeAutoCreated;

    /// <summary>Sets whether work sheets are printed automatically.</summary>
    public void SetWorkSheetPrinted(bool isWorkSheetPrinted) => IsWorkSheetPrinted = isWorkSheetPrinted;

    /// <summary>Sets the medical-history display mode.</summary>
    public void SetHistoryDisplayMode(int mode) => HistoryDisplayMode = mode;

    /// <summary>Sets the two global report header texts.</summary>
    public void SetGlobalHeaderTexts(string? header, string? secondaryHeader)
    {
        GlobalHeaderText = Normalize(header);
        GlobalHeaderSecondaryText = Normalize(secondaryHeader);
    }

    /// <summary>Sets the footer text printed at the foot of every laboratory report.</summary>
    public void SetReportFooterText(string? reportFooterText) =>
        ReportFooterText = Normalize(reportFooterText);

    /// <summary>Sets the layout text that accompanies the report body.</summary>
    public void SetLayoutText(string? layoutText) =>
        LayoutText = Normalize(layoutText);

    /// <summary>Sets the signature, the doctor's name and the line of thanks.</summary>
    /// <exception cref="InvalidPatientOrderException">
    /// A logo dimension is not positive.
    /// </exception>
    public void SetBranding(
        string? doctorSignature,
        string? doctorName,
        string? thanksText,
        string? labLogoPath,
        string? reportLogoPath,
        int? logoWidthPx,
        int? logoHeightPx,
        ReportHeadingMode? headingMode)
    {
        if (logoWidthPx is <= 0)
        {
            throw new InvalidPatientOrderException("The logo width must be a positive number of pixels.");
        }

        if (logoHeightPx is <= 0)
        {
            throw new InvalidPatientOrderException("The logo height must be a positive number of pixels.");
        }

        DoctorSignature = Normalize(doctorSignature);
        DoctorName = Normalize(doctorName);
        ThanksText = Normalize(thanksText);
        LabLogoPath = Normalize(labLogoPath);
        ReportLogoPath = Normalize(reportLogoPath);
        LogoWidthPx = logoWidthPx;
        LogoHeightPx = logoHeightPx;

        if (headingMode.HasValue)
        {
            HeadingMode = headingMode.Value;
        }
    }

    /// <summary>Sets the report margins.</summary>
    /// <exception cref="InvalidPatientOrderException">A margin is negative.</exception>
    public void SetMargins(decimal left, decimal right, decimal top, decimal bottom)
    {
        if (left < 0m || right < 0m || top < 0m || bottom < 0m)
        {
            throw new InvalidPatientOrderException("A report margin cannot be negative.");
        }

        LeftMargin = left;
        RightMargin = right;
        TopMargin = top;
        BottomMargin = bottom;
    }

    /// <summary>Sets the report background colour.</summary>
    public void SetBackgroundColour(string? colour) => BackgroundColour = Normalize(colour);

    /// <summary>Sets the default account type applied to new orders.</summary>
    public void SetDefaultAccountType(string? accountType) => DefaultAccountType = Normalize(accountType);

    /// <summary>Sets the header and footer of the connection receipt.</summary>
    public void SetConnectionReceipt(string? header, string? footer)
    {
        ConnectionReceiptHeader = Normalize(header);
        ConnectionReceiptFooter = Normalize(footer);
    }

    /// <summary>Binds a printer to a role.</summary>
    public void BindPrinter(PrinterRole role, string printerName)
    {
        if (string.IsNullOrWhiteSpace(printerName))
        {
            throw new InvalidPatientOrderException("A printer name cannot be empty.");
        }

        var printers = new Dictionary<PrinterRole, string>(_printers)
        {
            [role] = printerName.Trim(),
        };

        _printers = printers;
    }

    /// <summary>Removes the printer bound to a role.</summary>
    public void UnbindPrinter(PrinterRole role)
    {
        if (!_printers.ContainsKey(role))
        {
            return;
        }

        var printers = new Dictionary<PrinterRole, string>(_printers);

        printers.Remove(role);

        _printers = printers;
    }

    /// <summary>Returns the printer bound to a role, when one is bound.</summary>
    public string? GetPrinter(PrinterRole role) =>
        _printers.TryGetValue(role, out var printer) ? printer : null;

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
