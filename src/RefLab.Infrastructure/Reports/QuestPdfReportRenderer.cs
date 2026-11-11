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

                    // The branding block comes first: the reference system
                    // prints the product identity and the currency above
                    // everything else on every document.
                    ReportBranding.Apply(column);

                    if (!string.IsNullOrWhiteSpace(request.Title))
                    {
                        column.Item().Text(request.Title).FontSize(16).SemiBold();
                    }

                    if (!string.IsNullOrWhiteSpace(request.Subtitle))
                    {
                        column.Item().Text(request.Subtitle).FontSize(11).SemiBold();
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

                // The signature block. The reference system prints three fixed
                // lines at the foot of the clinical report; in Ref Lab the
                // laboratory owns them.
                if (!string.IsNullOrWhiteSpace(request.DoctorSignature) ||
                    !string.IsNullOrWhiteSpace(request.DoctorName) ||
                    !string.IsNullOrWhiteSpace(request.ThanksText))
                {
                    page.Footer().Column(column =>
                    {
                        column.Spacing(0);

                        if (!string.IsNullOrWhiteSpace(request.DoctorSignature))
                        {
                            column.Item().Text(request.DoctorSignature).FontSize(9);
                        }

                        if (!string.IsNullOrWhiteSpace(request.DoctorName))
                        {
                            column.Item().Text(request.DoctorName).FontSize(9);
                        }

                        if (!string.IsNullOrWhiteSpace(request.ThanksText))
                        {
                            column.Item().Text(request.ThanksText).FontSize(9);
                        }
                    });
                }

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
            request.FooterText,
            BuildPatientLine(request),
            request.Category,
            request.DoctorSignature,
            request.DoctorName,
            request.ThanksText,
            request.LabLogoPath,
            request.ReportLogoPath,
            request.LogoWidthPx,
            request.LogoHeightPx);
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
