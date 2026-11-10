using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Registers a new patient and, optionally, the first order of the visit.
/// </summary>
public sealed record RegisterPatientCommand(
    string Name,
    Sex Sex,
    decimal? Age,
    AgeUnit? AgeUnit,
    DateOnly? BirthDate,
    string? Address,
    string? Phone,
    string? Mobile,
    string? Fax,
    string? Email,
    string? ReferringPhysicianCode,
    string? ReceptionNote,
    string CurrencyCode,
    bool CreateOrder) : IRequest<PatientOrderDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.RegisterPatient;
}

/// <summary>
/// Handles the registration of a new patient.
/// </summary>
public sealed class RegisterPatientHandler : IRequestHandler<RegisterPatientCommand, PatientOrderDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public RegisterPatientHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<PatientOrderDto> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new ForbiddenException("An operator must be signed in to register a patient.");
        }

        var patient = Patient.Create(
            request.Name,
            request.Sex,
            request.Age,
            request.AgeUnit,
            request.BirthDate);

        patient.UpdateContactDetails(
            request.Address,
            request.Phone,
            request.Mobile,
            request.Fax,
            request.Email);

        patient.SetReferringPhysician(request.ReferringPhysicianCode);
        patient.SetReceptionNote(request.ReceptionNote);

        _unitOfWork.Add(patient);

        var now = _clock.UtcNow;
        var order = PatientOrder.Create(
            patient.Id,
            now,
            request.CurrencyCode,
            _currentUser.OperatorName ?? "unknown");

        _unitOfWork.Add(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return PatientOrderDto.FromDomain(order);
    }
}
