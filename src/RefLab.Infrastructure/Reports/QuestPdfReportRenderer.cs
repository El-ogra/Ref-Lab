using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;

namespace RefLab.Infrastructure.Reports;

/// <summary>
/// Renders a laboratory report with QuestPDF.
/// </summary>
/// <remarks>
/// <para>
/// The legacy system produced no PDF at all: it dispatched the print job to a
/// hard-coded external driver named <c>Bullzip PDF Printer</c>. Ref Lab
/// replaces that coupling with a first-class renderer behind the
/// <see cref="IReportRenderer"/> abstraction, so the PDF engine can be replaced
/// without touching the application layer.
/// </para>
/// </remarks>
public sealed class QuestPdfReportRenderer : IReportRenderer
{
    /// <summary>
    /// Initialises a new renderer.
    /// </summary>
    public QuestPdfReportRenderer()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <inheritdoc />
    public ReportDocument RenderLaboratoryReport(ReportRenderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(10));

                if (!string.IsNullOrWhiteSpace(request.HeaderText))
                {
                    page.Header().Text(request.HeaderText);
                }

                page.Content().Column(column =>
                {
                    column.Spacing(8);

                    if (!string.IsNullOrWhiteSpace(request.Title))
                    {
                        column.Item().Text(request.Title).FontSize(16).SemiBold();
                    }

                    if (!string.IsNullOrWhiteSpace(request.Subtitle))
                    {
                        column.Item().Text(request.Subtitle).FontSize(11);
                    }

                    column.Item().Text(BuildPatientLine(request)).FontSize(9);

                    foreach (var section in request.Sections)
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
                });

                if (!string.IsNullOrWhiteSpace(request.FooterText))
                {
                    page.Footer().Text(request.FooterText).FontSize(8);
                }
            });
        });

        var sections = request.Sections
            .Select(section => new ReportSection(section.Name, section.Rows))
            .ToArray();

        return new ReportDocument(
            request.Title,
            request.Subtitle,
            sections,
            request.HeaderText,
            request.FooterText);
    }

    private static string BuildPatientLine(ReportRenderRequest request)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.PatientName))
        {
            parts.Add(request.PatientName);
        }

        if (!string.IsNullOrWhiteSpace(request.OrderIdentifier))
        {
            parts.Add(request.OrderIdentifier);
        }

        if (!string.IsNullOrWhiteSpace(request.PatientSex))
        {
            parts.Add(request.PatientSex);
        }

        if (!string.IsNullOrWhiteSpace(request.PatientAge))
        {
            parts.Add(request.PatientAge);
        }

        if (!string.IsNullOrWhiteSpace(request.RequestDate))
        {
            parts.Add(request.RequestDate);
        }

        if (!string.IsNullOrWhiteSpace(request.ReportingDate))
        {
            parts.Add(request.ReportingDate);
        }

        return string.Join("  |  ", parts);
    }
}
