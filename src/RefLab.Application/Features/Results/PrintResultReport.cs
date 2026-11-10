using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Results;

/// <summary>
/// The outcome of printing a laboratory report.
/// </summary>
public sealed record PrintResultReportDto(
    Guid OrderTestId,
    long SequenceNumber,
    string FileName,
    string FullPath,
    string Category);

/// <summary>
/// Prints a laboratory report for a reviewed result.
/// </summary>
public sealed record PrintResultReportCommand(
    Guid OrderTestId,
    ReportCategory Category,
    string DestinationDirectory) : IRequest<PrintResultReportDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.PrintReports;
}

/// <summary>
/// Handles the printing of a laboratory report.
/// </summary>
public sealed class PrintResultReportHandler : IRequestHandler<PrintResultReportCommand, PrintResultReportDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly IReportRenderer _reportRenderer;
    private readonly IPdfExporter _pdfExporter;
    private readonly IPrintSequenceGenerator _printSequence;

    /// <summary>Initialises a new handler.</summary>
    public PrintResultReportHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IClock clock,
        IReportRenderer reportRenderer,
        IPdfExporter pdfExporter,
        IPrintSequenceGenerator printSequence)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
        _reportRenderer = reportRenderer;
        _pdfExporter = pdfExporter;
        _printSequence = printSequence;
    }

    /// <inheritdoc />
    public async Task<PrintResultReportDto> Handle(
        PrintResultReportCommand request,
        CancellationToken cancellationToken)
    {
        var orderTest = _db.Set<OrderTest>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderTestId)
            ?? throw new NotFoundException(nameof(OrderTest), request.OrderTestId);

        orderTest.RequireReviewed();

        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == orderTest.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), orderTest.OrderId);

        var patient = _db.Set<Patient>()
            .FirstOrDefault(candidate => candidate.Id == order.PatientId)
            ?? throw new NotFoundException(nameof(Patient), order.PatientId.Value);

        var now = _clock.UtcNow;
        var operatorName = _currentUser.OperatorName ?? "unknown";

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            "LABORATORY REPORT",
            null,
            patient.Name,
            order.LegacyIdentifier?.Value,
            patient.Sex.ToString(),
            patient.Age?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            order.RequestDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            null,
            null,
            new[]
            {
                new ReportSection(
                    orderTest.TestName ?? orderTest.TestSimpleName,
                    new[] { new ReportRow(new[] { orderTest.Result?.Text ?? string.Empty, orderTest.TestUnit ?? string.Empty }) }),
            }));

        var exported = await _pdfExporter.ExportAsync(
            document,
            request.DestinationDirectory,
            cancellationToken);

        var sequenceNumber = _printSequence.Next();

        orderTest.MarkPrinted(operatorName, now, sequenceNumber);

        var audit = PrintAudit.Create(
            orderTest.Id,
            operatorName,
            now,
            sequenceNumber,
            request.Category);

        _unitOfWork.Update(orderTest);
        _unitOfWork.Add(audit);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PrintResultReportDto(
            orderTest.Id,
            sequenceNumber,
            exported.FileName,
            exported.FullPath,
            request.Category.ToString());
    }
}
