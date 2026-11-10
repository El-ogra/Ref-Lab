using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Attaches a picture to a patient, in one of the fifteen available slots.
/// </summary>
/// <remarks>
/// Legacy evidence: the pictures table carries the patient, the order, the test and fifteen picture slots, each paired with a value label and a text label.
/// </remarks>
public sealed record AttachPatientPictureCommand(Guid PatientId, int Slot, string ImagePath, string? Caption = null, Guid? OrderId = null, int? TestCode = null) : IRequest<PatientPictureDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.EditPatient;
}

/// <summary>
/// Handles the AttachPatientPicture use case.
/// </summary>
public sealed class AttachPatientPictureHandler : IRequestHandler<AttachPatientPictureCommand, PatientPictureDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public AttachPatientPictureHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<PatientPictureDto> Handle(AttachPatientPictureCommand request, CancellationToken cancellationToken)
    {
        var patient = _db.Set<Patient>()
            .FirstOrDefault(candidate => candidate.Id.Value == request.PatientId)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var occupied = _db.Set<PatientPicture>()
            .Any(picture => picture.PatientId == patient.Id && picture.Slot == request.Slot);

        if (occupied)
        {
            throw new ConflictException(
                $"Picture slot {request.Slot} is already occupied for this patient.");
        }

        var picture = PatientPicture.Create(
            patient.Id,
            request.Slot,
            request.ImagePath,
            request.Caption,
            request.OrderId,
            request.TestCode);

        _unitOfWork.Add(picture);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return PatientPictureDto.FromDomain(picture);
    }
}
