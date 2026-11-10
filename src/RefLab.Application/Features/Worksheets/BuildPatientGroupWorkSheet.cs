using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Worksheets;

/// <summary>
/// Builds a work sheet over a group of patients in a period, and writes it to a PDF.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the work sheet over a group of patients in a period.
/// </remarks>
public sealed record BuildPatientGroupWorkSheetCommand(DateTimeOffset From, DateTimeOffset To, IReadOnlyList<Guid> PatientIds, string DestinationDirectory) : IRequest<WorkSheetDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.PrintReports;
}

/// <summary>
/// Handles the BuildPatientGroupWorkSheet use case.
/// </summary>
public sealed class BuildPatientGroupWorkSheetHandler : IRequestHandler<BuildPatientGroupWorkSheetCommand, WorkSheetDto>
{
    private readonly IAppDbContext _db;
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;

    /// <summary>Initialises a new handler.</summary>
    public BuildPatientGroupWorkSheetHandler(
        IAppDbContext db,
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter)
    {
        _db = db;
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
    }
    /// <inheritdoc />
    public async Task<WorkSheetDto> Handle(BuildPatientGroupWorkSheetCommand request, CancellationToken cancellationToken)
    {
        var orders = _db.Set<PatientOrder>()
            .Where(order => order.RequestDate >= request.From && order.RequestDate <= request.To)
            .ToArray();

        if (request.PatientIds.Count > 0)
        {
            orders = orders.Where(order => request.PatientIds.Contains(order.PatientId.Value)).ToArray();
        }

        var orderIds = orders.Select(order => order.Id).ToArray();

        var tests = _db.Set<OrderTest>()
            .Where(test => orderIds.Contains(test.OrderId))
            .ToArray();

        var patients = _db.Set<Patient>().ToArray();

        var rows = orders
            .OrderBy(order => order.RequestDate)
            .Select(order =>
            {
                var patientName = patients
                    .FirstOrDefault(patient => patient.Id == order.PatientId)?.Name ?? string.Empty;

                var orderTests = tests.Where(test => test.OrderId == order.Id).ToArray();

                return new WorkSheetRowDto(
                    order.Id.ToString("N"),
                    patientName,
                    new[]
                    {
                        order.LegacyIdentifier?.Value ?? string.Empty,
                        order.RequestDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                        orderTests.Length.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        orderTests.Count(test => test.IsResultReviewed).ToString(System.Globalization.CultureInfo.InvariantCulture),
                        orderTests.Count(test => test.IsGiven).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    });
            })
            .ToArray();

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            "PATIENT GROUP WORK SHEET",
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
                    "Patients",
                    rows.Select(row => new ReportRow(new[] { row.Label }.Concat(row.Cells).ToArray())).ToArray()),
            }));

        var exported = await _pdfExporter.ExportAsync(document, request.DestinationDirectory, cancellationToken);

        return new WorkSheetDto(
            "PATIENT GROUP WORK SHEET",
            request.From,
            request.To,
            "Patients",
            rows.Length,
            rows);
    }
}
