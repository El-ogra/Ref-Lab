using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Billing;
using RefLab.Domain.Billing;

namespace RefLab.Application.Features.Parties;

/// <summary>
/// Prints the price list of the laboratory.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability of printing the list of test prices.
/// </remarks>
public sealed record PrintPriceListCommand(string PriceListName, string DestinationDirectory) : IRequest<RenderedDocumentDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManagePricing;
}

/// <summary>
/// Handles the PrintPriceList use case.
/// </summary>
public sealed class PrintPriceListHandler : IRequestHandler<PrintPriceListCommand, RenderedDocumentDto>
{
    private readonly IAppDbContext _db;
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;

    /// <summary>Initialises a new handler.</summary>
    public PrintPriceListHandler(
        IAppDbContext db,
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter)
    {
        _db = db;
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
    }
    /// <inheritdoc />
    public async Task<RenderedDocumentDto> Handle(PrintPriceListCommand request, CancellationToken cancellationToken)
    {
        var lines = _db.Set<PriceList>()
            .Where(priceList => priceList.PriceListName == request.PriceListName)
            .OrderBy(priceList => priceList.TestSimpleName)
            .ToArray();

        if (lines.Length == 0)
        {
            throw new NotFoundException(nameof(PriceList), request.PriceListName);
        }

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            $"PRICE LIST - {request.PriceListName}",
            null,
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
                    request.PriceListName,
                    lines.Select(line => new ReportRow(new[]
                    {
                        line.TestSimpleName,
                        line.Price.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    })).ToArray()),
            }));

        var exported = await _pdfExporter.ExportAsync(document, request.DestinationDirectory, cancellationToken);

        return new RenderedDocumentDto(exported.FileName, exported.FullPath, exported.Category);
    }
}
