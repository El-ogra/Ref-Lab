using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Enums;
using RefLab.Domain.Billing;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Registers a new patient and, optionally, the first order of the visit.
/// </summary>
/// <remarks>
/// The order carries the legacy identity of the visit: the accession number
/// under which the laboratory filed it, the referring party it was received
/// from, the physician who treated the patient and the amount on which the
/// referring party's commission is computed. Those four facts are recorded
/// when the order is created, because none of them can be recovered later —
/// the accession number is never issued twice, and the party and the physician
/// are properties of the visit rather than of the patient.
/// </remarks>
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
    bool CreateOrder,
    string? LegacyOrderIdentifier = null,
    string? ReferringPartyCode = null) : IRequest<PatientOrderDto>, IRequirePermission
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

        // The legacy accession number. It is issued once and never changes, so
        // it is attached to the order as it is captured. It is also the
        // barcode printed on the specimen label.
        if (!string.IsNullOrWhiteSpace(request.LegacyOrderIdentifier))
        {
            order.AttachLegacyIdentifier(OrderIdentifier.Create(request.LegacyOrderIdentifier));
        }

        // The physician the patient was referred by is the treating physician
        // of the order. The stock-take by treating physician filters on it.
        order.SetTreatingPhysician(request.ReferringPhysicianCode);

        // The party the order was received from. The commission is computed on
        // the base amount of the orders received from that party.
        if (!string.IsNullOrWhiteSpace(request.ReferringPartyCode))
        {
            var party = _db.Set<ReferringParty>()
                .FirstOrDefault(candidate => candidate.Code == request.ReferringPartyCode)
                ?? throw new NotFoundException(nameof(ReferringParty), request.ReferringPartyCode);

            order.SetReferringParty(party.Id);
        }

        _unitOfWork.Add(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return PatientOrderDto.FromDomain(order);
    }
}
