using RefLab.Application.Common;

namespace RefLab.Application.Abstractions;

/// <summary>
/// Writes a rendered report to a PDF file.
/// </summary>
/// <remarks>
/// <para>
/// The legacy system produced no PDF at all — it dispatched the print job to a
/// hard-coded external driver named <c>Bullzip PDF Printer</c>. Ref Lab
/// replaces that coupling with this abstraction, so the PDF engine can be
/// replaced without touching application logic.
/// </para>
/// </remarks>
public interface IPdfExporter
{
    /// <summary>Exports the rendered report and returns the produced file.</summary>
    Task<ExportedDocument> ExportAsync(
        ReportDocument document,
        string destinationDirectory,
        CancellationToken cancellationToken = default);
}
