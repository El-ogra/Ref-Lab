using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Worksheets;

/// <summary>
/// Builds a work sheet over a test or a test group in a period, and writes it to a PDF.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the work sheet over a test or a test group in a period.
/// </remarks>
public sealed record BuildTestGroupWorkSheetCommand(DateTimeOffset From, DateTimeOffset To, string DestinationDirectory, int? TestCode = null, int? SubGroupCode = null) : IRequest<WorkSheetDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.PrintReports;
}

/// <summary>
/// Handles the BuildTestGroupWorkSheet use case.
/// </summary>
public sealed class BuildTestGroupWorkSheetHandler : IRequestHandler<BuildTestGroupWorkSheetCommand, WorkSheetDto>
{
    private readonly IAppDbContext _db;
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;

    /// <summary>Initialises a new handler.</summary>
    public BuildTestGroupWorkSheetHandler(
        IAppDbContext db,
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter)
    {
        _db = db;
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
    }
    /// <inheritdoc />
    public async Task<WorkSheetDto> Handle(BuildTestGroupWorkSheetCommand request, CancellationToken cancellationToken)
    {
        if (request.TestCode is null && request.SubGroupCode is null)
        {
            throw new RequestValidationException(
                new[] { "A test code or a test-group code must be supplied." });
        }

        var orders = _db.Set<PatientOrder>()
            .Where(order => order.RequestDate >= request.From && order.RequestDate <= request.To)
            .ToArray();

        var orderIds = orders.Select(order => order.Id).ToArray();

        var tests = _db.Set<OrderTest>()
            .Where(test => orderIds.Contains(test.OrderId))
            .ToArray();

        if (request.TestCode.HasValue)
        {
            tests = tests.Where(test => test.TestCode == request.TestCode.Value).ToArray();
        }

        if (request.SubGroupCode.HasValue)
        {
            tests = tests.Where(test => test.SubGroupCode == request.SubGroupCode.Value).ToArray();
        }

        var patients = _db.Set<Patient>().ToArray();

        var rows = tests
            .OrderBy(test => test.RequestDate)
            .Select(test =>
            {
                var order = orders.First(candidate => candidate.Id == test.OrderId);
                var patientName = patients
                    .FirstOrDefault(patient => patient.Id == order.PatientId)?.Name ?? string.Empty;

                return new WorkSheetRowDto(
                    test.Id.ToString("N"),
                    patientName,
                    new[]
                    {
                        order.LegacyIdentifier?.Value ?? string.Empty,
                        test.TestName ?? test.TestSimpleName,
                        test.Result?.Text ?? string.Empty,
                        test.TestUnit ?? string.Empty,
                        test.Classification?.ToString() ?? string.Empty,
                    });
            })
            .ToArray();

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            "TEST GROUP WORK SHEET",
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
                    "Tests",
                    rows.Select(row => new ReportRow(new[] { row.Label }.Concat(row.Cells).ToArray())).ToArray()),
            }));

        var exported = await _pdfExporter.ExportAsync(document, request.DestinationDirectory, cancellationToken);

        return new WorkSheetDto(
            "TEST GROUP WORK SHEET",
            request.From,
            request.To,
            request.TestCode.HasValue
                ? $"Test {request.TestCode}"
                : $"Test group {request.SubGroupCode}",
            rows.Length,
            rows);
    }
}
