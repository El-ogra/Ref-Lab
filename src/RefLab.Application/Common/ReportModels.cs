using RefLab.Domain.Enums;

namespace RefLab.Application.Common;

/// <summary>
/// One row of a rendered report.
/// </summary>
public sealed record ReportRow(IReadOnlyList<string> Cells);

/// <summary>
/// One section of a rendered report.
/// </summary>
public sealed record ReportSection(string Name, IReadOnlyList<ReportRow> Rows);

/// <summary>
/// A rendered report, ready to be written to a file or a printer.
/// </summary>
/// <remarks>
/// The patient line identifies whose results the document carries. It is part
/// of the document rather than of a section, because a laboratory report that
/// cannot be attributed to a patient is not a report. The property is
/// optional, so a document that has no patient — an empty report, for
/// instance — is still valid.
/// </remarks>
public sealed record ReportDocument(
    string Title,
    string? Subtitle,
    IReadOnlyList<ReportSection> Sections,
    string? HeaderText,
    string? FooterText,
    string? PatientLine = null,
    ReportCategory Category = ReportCategory.Individual,
    string? DoctorSignature = null,
    string? DoctorName = null,
    string? ThanksText = null,
    string? LabLogoPath = null,
    string? ReportLogoPath = null,
    int? LogoWidthPx = null,
    int? LogoHeightPx = null);

/// <summary>
/// The data needed to render a laboratory report.
/// </summary>
public sealed record ReportRenderRequest(
    string Title,
    string? Subtitle,
    string? PatientName,
    string? OrderIdentifier,
    string? PatientSex,
    string? PatientAge,
    string? RequestDate,
    string? ReportingDate,
    string? HeaderText,
    string? FooterText,
    IReadOnlyList<ReportSection> Sections,
    string? DoctorSignature = null,
    string? DoctorName = null,
    string? ThanksText = null,
    string? LabLogoPath = null,
    string? ReportLogoPath = null,
    int? LogoWidthPx = null,
    int? LogoHeightPx = null,
    ReportCategory Category = ReportCategory.Individual,
    ReportHeadingMode HeadingMode = ReportHeadingMode.All);

/// <summary>
/// A PDF file produced by the exporter.
/// </summary>
public sealed record ExportedDocument(string FileName, string FullPath, byte[] Content, string Category);

/// <summary>
/// A rendered document written to disk.
/// </summary>
public sealed record RenderedDocumentDto(string FileName, string FullPath, string Category);
