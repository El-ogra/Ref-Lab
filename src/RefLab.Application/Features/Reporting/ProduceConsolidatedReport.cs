using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Results;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Reporting;

/// <summary>
/// Builds a consolidated report over several orders in a period, and writes it to a PDF.
/// </summary>
/// <remarks>
/// Legacy evidence: the consolidated report combines the results of several orders into one document.
/// </remarks>
public sealed record ProduceConsolidatedReportCommand(DateTimeOffset From, DateTimeOffset To, string DestinationDirectory, Guid? OrderId = null) : IRequest<ConsolidatedReportDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.PrintReports;
}

/// <summary>
/// Handles the ProduceConsolidatedReport use case.
/// </summary>
public sealed class ProduceConsolidatedReportHandler : IRequestHandler<ProduceConsolidatedReportCommand, ConsolidatedReportDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;

    /// <summary>Initialises a new handler.</summary>
    public ProduceConsolidatedReportHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
    }
    /// <inheritdoc />
    public async Task<ConsolidatedReportDto> Handle(ProduceConsolidatedReportCommand request, CancellationToken cancellationToken)
    {
        var orders = _db.Set<PatientOrder>()
            .Where(order => order.RequestDate >= request.From && order.RequestDate <= request.To)
            .ToArray();

        if (request.OrderId.HasValue)
        {
            orders = orders.Where(order => order.Id == request.OrderId.Value).ToArray();
        }

        var orderIds = orders.Select(order => order.Id).ToArray();

        var tests = _db.Set<OrderTest>()
            .Where(test => orderIds.Contains(test.OrderId))
            .ToArray();

        var patients = _db.Set<Patient>().ToArray();

        var rows = tests
            .OrderBy(test => test.RequestDate)
            .Select(test =>
            {
                var order = orders.First(candidate => candidate.Id == test.OrderId);
                var patientName = patients
                    .FirstOrDefault(patient => patient.Id == order.PatientId)?.Name ?? string.Empty;

                return new ConsolidatedReportRowDto(
                    patientName,
                    order.LegacyIdentifier?.Value,
                    test.TestName ?? test.TestSimpleName,
                    test.Result?.Text,
                    test.TestUnit,
                    test.Classification?.ToString());
            })
            .ToArray();

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            "CONSOLIDATED REPORT",
            null,
            null,
            null,
            null,
            null,
            request.From.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            request.To.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            null,
            null,
            new[]
            {
                new ReportSection(
                    "Results",
                    rows.Select(row => new ReportRow(new[]
                    {
                        row.PatientName,
                        row.LegacyIdentifier ?? string.Empty,
                        row.TestName,
                        row.Value ?? string.Empty,
                        row.Unit ?? string.Empty,
                        row.Flag ?? string.Empty,
                    })).ToArray()),
            }));

        var exported = await _pdfExporter.ExportAsync(document, request.DestinationDirectory, cancellationToken);

        var report = new ConsolidatedReportDto(
            "CONSOLIDATED REPORT",
            request.From,
            request.To,
            orders.Length,
            rows,
            new RenderedDocumentDto(exported.FileName, exported.FullPath, exported.Category));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return report;
    }
}
