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
public sealed record ReportDocument(
    string Title,
    string? Subtitle,
    IReadOnlyList<ReportSection> Sections,
    string? HeaderText,
    string? FooterText);

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
    IReadOnlyList<ReportSection> Sections);

/// <summary>
/// A PDF file produced by the exporter.
/// </summary>
public sealed record ExportedDocument(string FileName, string FullPath, byte[] Content, string Category);

/// <summary>
/// A rendered document written to disk.
/// </summary>
public sealed record RenderedDocumentDto(string FileName, string FullPath, string Category);
