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
/// Prints the identity card of a patient.
/// </summary>
/// <remarks>
/// Legacy evidence: the identity card is printed from the patient record and carries the barcode that identifies the patient.
/// </remarks>
public sealed record PrintPatientIdCardCommand(Guid PatientId, string DestinationDirectory) : IRequest<IdCardDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.PrintReports;
}

/// <summary>
/// Handles the PrintPatientIdCard use case.
/// </summary>
public sealed class PrintPatientIdCardHandler : IRequestHandler<PrintPatientIdCardCommand, IdCardDto>
{
    private readonly IAppDbContext _db;
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;

    /// <summary>Initialises a new handler.</summary>
    public PrintPatientIdCardHandler(
        IAppDbContext db,
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter)
    {
        _db = db;
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
    }
    /// <inheritdoc />
    public async Task<IdCardDto> Handle(PrintPatientIdCardCommand request, CancellationToken cancellationToken)
    {
        var patient = _db.Set<Patient>()
            .FirstOrDefault(candidate => candidate.Id.Value == request.PatientId)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var orders = _db.Set<PatientOrder>()
            .Where(order => order.PatientId == patient.Id)
            .OrderByDescending(order => order.RequestDate)
            .ToArray();

        var barcode = orders.FirstOrDefault(order => order.Barcode is not null)?.Barcode;

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            "PATIENT CARD",
            null,
            patient.Name,
            null,
            patient.Sex.ToString(),
            patient.Age?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            null,
            null,
            null,
            null,
            new[]
            {
                new ReportSection(
                    "Patient card",
                    new[]
                    {
                        new ReportRow(new[] { patient.Name, patient.Address ?? string.Empty, patient.Phone ?? string.Empty }),
                    }),
            }));

        var exported = await _pdfExporter.ExportAsync(document, request.DestinationDirectory, cancellationToken);

        return new IdCardDto(
            patient.Id.Value,
            patient.Name,
            patient.Sex,
            patient.Age,
            patient.Address,
            patient.Phone,
            barcode,
            new RenderedDocumentDto(exported.FileName, exported.FullPath, exported.Category));
    }
}
