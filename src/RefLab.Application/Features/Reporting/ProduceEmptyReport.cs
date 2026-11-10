using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Results;

namespace RefLab.Application.Features.Reporting;

/// <summary>
/// Produces an empty report template with the requested column headings.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability of producing an empty report — a blank laboratory report form.
/// </remarks>
public sealed record ProduceEmptyReportCommand(string Title, IReadOnlyList<string> ColumnNames, string DestinationDirectory, string? Subtitle = null) : IRequest<EmptyReportDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.PrintReports;
}

/// <summary>
/// Handles the ProduceEmptyReport use case.
/// </summary>
public sealed class ProduceEmptyReportHandler : IRequestHandler<ProduceEmptyReportCommand, EmptyReportDto>
{
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;

    /// <summary>Initialises a new handler.</summary>
    public ProduceEmptyReportHandler(
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter)
    {
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
    }
    /// <inheritdoc />
    public async Task<EmptyReportDto> Handle(ProduceEmptyReportCommand request, CancellationToken cancellationToken)
    {
        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            request.Title,
            request.Subtitle,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new[]
            {
                new ReportSection(
                    request.Title,
                    new[]
                    {
                        new ReportRow(request.ColumnNames.Select(column => string.Empty).ToArray()),
                    }),
            }));

        var exported = await _pdfExporter.ExportAsync(document, request.DestinationDirectory, cancellationToken);

        return new EmptyReportDto(
            request.Title,
            request.Subtitle,
            request.ColumnNames,
            new RenderedDocumentDto(exported.FileName, exported.FullPath, exported.Category));
    }
}
