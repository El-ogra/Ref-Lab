using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Auditing;
using RefLab.Domain.Configuration;
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
    /// <summary>
    /// The standard comment of a specimen that was taken outside the laboratory.
    /// </summary>
    /// <remarks>
    /// The reference system prints exactly this sentence under the result of a
    /// specimen that was collected away from the laboratory.
    /// </remarks>
    public const string SampleOutsideLabComment = "Sample taken outside lab.";

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

        // The laboratory settings carry the branding of the printed document.
        var settings = _db.Set<LaboratorySettings>().FirstOrDefault();

        var interval = orderTest.ReferenceSnapshot?.Interval;

        var classification = orderTest.Classification;

        // The report heading mode decides which results the document carries.
        var included = settings?.HeadingMode switch
        {
            ReportHeadingMode.Normal => classification is ResultClassification.Normal,
            ReportHeadingMode.Abnormal => classification is ResultClassification.High
                or ResultClassification.Low
                or ResultClassification.Critical,
            _ => true,
        };

        var rows = new List<ReportRow>();

        if (included)
        {
            rows.Add(new ReportRow(new[]
            {
                orderTest.TestName ?? orderTest.TestSimpleName,
                orderTest.Result?.Text ?? string.Empty,
                orderTest.TestUnit ?? string.Empty,
                interval?.RangeText ?? string.Empty,
                classification?.ToString() ?? string.Empty,
                interval?.CriticalInterval.CriticalRangeText ?? string.Empty,
            }));
        }

        var sections = new List<ReportSection>
        {
            new ReportSection(
                "REFERENCE RESULT",
                new[]
                {
                    new ReportRow(new[]
                    {
                        "Sex",
                        patient.Sex.ToString(),
                        "Age",
                        patient.Age?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                        "Normal range for test",
                        interval?.RangeText ?? string.Empty,
                    }),
                }),
            new ReportSection(
                orderTest.TestName ?? orderTest.TestSimpleName,
                rows.ToArray()),
        };

        // The comment line. The reference system prints the comment of the
        // result under the table, and the standard comment of a specimen taken
        // outside the laboratory is its own entry.
        var comments = new List<string>();

        if (orderTest.SampleTakenOutsideLab)
        {
            comments.Add(PrintResultReportCommand.SampleOutsideLabComment);
        }

        if (!string.IsNullOrWhiteSpace(orderTest.Comment))
        {
            comments.Add(orderTest.Comment);
        }

        if (comments.Count > 0)
        {
            sections.Add(new ReportSection(
                "Comment",
                comments.Select(comment => new ReportRow(new[] { "Comment : " + comment })).ToArray()));
        }

        var reportingDate = now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        var document = _reportRenderer.RenderLaboratoryReport(new ReportRenderRequest(
            "LABORATORY REPORT",
            orderTest.TestName ?? orderTest.TestSimpleName,
            patient.Name,
            order.LegacyIdentifier?.Value,
            patient.Sex.ToString(),
            patient.Age?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            order.RequestDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            reportingDate,
            settings?.GlobalHeaderText,
            $"Issued on: {reportingDate}    Sent to: {patient.Name}",
            sections,
            settings?.DoctorSignature,
            settings?.DoctorName,
            settings?.ThanksText,
            settings?.LabLogoPath,
            settings?.ReportLogoPath,
            settings?.LogoWidthPx,
            settings?.LogoHeightPx,
            request.Category,
            settings?.HeadingMode ?? ReportHeadingMode.All));

        // The sequence number is allocated before the document is produced, so
        // that a gap in the series reveals a report that was never issued.
        var sequenceNumber = _printSequence.Next();

        var exported = await _pdfExporter.ExportAsync(
            document,
            request.DestinationDirectory,
            cancellationToken);

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
