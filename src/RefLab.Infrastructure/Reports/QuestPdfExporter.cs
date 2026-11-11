using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;

namespace RefLab.Infrastructure.Reports;

/// <summary>
/// Writes a rendered laboratory report to a PDF file with QuestPDF.
/// </summary>
/// <remarks>
/// The exporter owns the file naming and the destination directory. The four
/// destination categories of the legacy system — contracts, free, individual
/// and laboratory-to-laboratory — are a commercial classification, so the
/// category is carried on the exported document rather than baked into the
/// exporter.
/// </remarks>
public sealed class QuestPdfExporter : IPdfExporter
{
    /// <summary>
    /// Initialises a new exporter.
    /// </summary>
    public QuestPdfExporter()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <inheritdoc />
    public Task<ExportedDocument> ExportAsync(
        ReportDocument document,
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (string.IsNullOrWhiteSpace(destinationDirectory))
        {
            throw new ArgumentException("A destination directory is required.", nameof(destinationDirectory));
        }

        Directory.CreateDirectory(destinationDirectory);

        var fileName = BuildFileName(document);

        var path = Path.Combine(destinationDirectory, fileName);

        var bytes = QuestPDF.Fluent.Document
            .Create(container => container.Page(page =>
            {
                page.Size(QuestPDF.Helpers.PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(10));

                page.Content().Column(column =>
                {
                    column.Spacing(8);

                    // The laboratory branding is applied to every document. This is
                    // the single point at which the report layer reaches the
                    // branding; the presentation layer never sees the strings and
                    // never assembles report content.
                    ReportBranding.Apply(column);

                    // The laboratory logo, when the laboratory has set one. The
                    // file is read from the path the laboratory configured; a
                    // missing or unreadable file does not stop the document.
                    var logo = ReadLogo(document.LabLogoPath);

                    if (logo is not null)
                    {
                        column.Item().Image(logo).FitArea();
                    }

                    if (!string.IsNullOrWhiteSpace(document.Title))
                    {
                        column.Item().Text(document.Title).FontSize(16).SemiBold();
                    }

                    if (!string.IsNullOrWhiteSpace(document.Subtitle))
                    {
                        column.Item().Text(document.Subtitle).FontSize(11);
                    }

                    if (!string.IsNullOrWhiteSpace(document.HeaderText))
                    {
                        column.Item().Text(document.HeaderText).FontSize(9);
                    }

                    // The patient line identifies whose results the document
                    // carries. It is printed directly under the header, so
                    // that a report that is separated from its cover sheet
                    // can still be attributed.
                    if (!string.IsNullOrWhiteSpace(document.PatientLine))
                    {
                        column.Item().Text(document.PatientLine).FontSize(9);
                    }

                    foreach (var section in document.Sections)
                    {
                        column.Item().Text(section.Name).FontSize(12).SemiBold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var _ in section.Rows.FirstOrDefault()?.Cells ?? Array.Empty<string>())
                                {
                                    columns.RelativeColumn();
                                }
                            });

                            foreach (var row in section.Rows)
                            {
                                foreach (var cell in row.Cells)
                                {
                                    table.Cell().Text(cell ?? string.Empty);
                                }
                            }
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(document.FooterText))
                    {
                        column.Item().Text(document.FooterText).FontSize(8);
                    }

                    // The signature block. The reference system prints three
                    // fixed lines at the foot of the clinical report; in Ref
                    // Lab the laboratory owns them.
                    if (!string.IsNullOrWhiteSpace(document.DoctorSignature))
                    {
                        column.Item().Text(document.DoctorSignature).FontSize(9);
                    }

                    if (!string.IsNullOrWhiteSpace(document.DoctorName))
                    {
                        column.Item().Text(document.DoctorName).FontSize(9);
                    }

                    if (!string.IsNullOrWhiteSpace(document.ThanksText))
                    {
                        column.Item().Text(document.ThanksText).FontSize(9);
                    }
                });
            }))
            .GeneratePdf();

        File.WriteAllBytes(path, bytes);

        // The category is the commercial classification the report is filed
        // under. It travels on the document so that the exporter does not have
        // to guess it.
        return Task.FromResult(new ExportedDocument(fileName, path, bytes, document.Category.ToString()));
    }

    /// <summary>
    /// Reads the laboratory logo, when the laboratory has configured one.
    /// </summary>
    /// <returns>The image bytes, or <see langword="null"/> when there is no logo or it cannot be read.</returns>
    private static byte[]? ReadLogo(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null;
        }

        try
        {
            return File.ReadAllBytes(path);
        }
        catch (IOException)
        {
            // The logo could not be read; the document is still produced.
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            // The logo could not be read; the document is still produced.
            return null;
        }
    }

    private static string BuildFileName(ReportDocument document)
    {
        var title = string.IsNullOrWhiteSpace(document.Title) ? "report" : document.Title;
        var safe = new string(title.Select(character =>
            char.IsLetterOrDigit(character) ? character : '_').ToArray());

        return $"{safe}_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmssfff}.pdf";
    }
}
