using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Allocates the barcode that identifies the patient order on the physical label.
/// </summary>
/// <remarks>
/// Legacy evidence: the barcode is a direct patient identifier on a physical artefact, stored in the barcode table. The legacy barcode format was never established, so Ref Lab derives it from the order identity and documents the format here.
/// </remarks>
public sealed record GeneratePatientBarcodeCommand(Guid OrderId) : IRequest<BarcodeDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.RegisterPatient;
}

/// <summary>
/// Handles the GeneratePatientBarcode use case.
/// </summary>
public sealed class GeneratePatientBarcodeHandler : IRequestHandler<GeneratePatientBarcodeCommand, BarcodeDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public GeneratePatientBarcodeHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }
    /// <inheritdoc />
    public async Task<BarcodeDto> Handle(GeneratePatientBarcodeCommand request, CancellationToken cancellationToken)
    {
        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), request.OrderId);

        var barcode = order.LegacyIdentifier is not null
            ? order.LegacyIdentifier.Value
            : "B" + order.Id.ToString("N").ToUpperInvariant()[..12];

        order.SetBarcode(barcode);

        _unitOfWork.Update(order);

        var generatedAt = _clock.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new BarcodeDto(order.Id, barcode, generatedAt);
    }
}
