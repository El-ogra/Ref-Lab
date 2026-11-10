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
/// Prints the invoice of a patient order, showing the ordered tests, the total and the amount due.
/// </summary>
/// <remarks>
/// Legacy evidence: the invoice is printed from the order, with the order total, the discount and the amount settled.
/// </remarks>
public sealed record PrintPatientInvoiceCommand(Guid OrderId, string DestinationDirectory) : IRequest<InvoiceDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.HandOverResults;
}

/// <summary>
/// Handles the PrintPatientInvoice use case.
/// </summary>
public sealed class PrintPatientInvoiceHandler : IRequestHandler<PrintPatientInvoiceCommand, InvoiceDto>
{
    private readonly IAppDbContext _db;
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;

    /// <summary>Initialises a new handler.</summary>
    public PrintPatientInvoiceHandler(
        IAppDbContext db,
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter)
    {
        _db = db;
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
    }
    /// <inheritdoc />
    public async Task<InvoiceDto> Handle(PrintPatientInvoiceCommand request, CancellationToken cancellationToken)
    {
        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), request.OrderId);

        var patient = _db.Set<Patient>()
            .FirstOrDefault(candidate => candidate.Id == order.PatientId)
            ?? throw new NotFoundException(nameof(Patient), order.PatientId.Value);

        var tests = _db.Set<OrderTest>()
            .Where(test => test.OrderId == order.Id)
            .OrderBy(test => test.ArrangementNumber)
            .ToArray();

        var lines = tests
            .Select(test => $"{test.TestName ?? test.TestSimpleName}: {test.Price?.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "0"}")
            .ToArray();

        var total = order.TotalPrice?.Amount ?? tests.Sum(test => test.Price?.Amount ?? 0m);
        var discounted = order.TotalPriceAfterDiscount?.Amount ?? total;
        var due = discounted - tests.Select(test => test.Price?.Amount ?? 0m).Sum() * 0m;

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            "INVOICE",
            null,
            patient.Name,
            order.LegacyIdentifier?.Value,
            null,
            null,
            order.RequestDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            null,
            null,
            null,
            new[] { new ReportSection("Invoice", lines.Select(line => new ReportRow(new[] { line })).ToArray()) }));

        var exported = await _pdfExporter.ExportAsync(document, request.DestinationDirectory, cancellationToken);

        return new InvoiceDto(
            order.Id,
            order.LegacyIdentifier?.Value,
            patient.Name,
            order.CurrencyCode,
            total,
            total - discounted,
            due,
            lines,
            new RenderedDocumentDto(exported.FileName, exported.FullPath, exported.Category));
    }
}
