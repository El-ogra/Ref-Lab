using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Microbiology;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Microbiology;

/// <summary>
/// Records a microbiology culture result, with the antimicrobial susceptibilities of the isolated organism.
/// </summary>
/// <remarks>
/// Legacy evidence: the culture table carries the culture identifier, the isolated organism, the condition, the colony description and the reactions; the susceptibility table records the antimicrobial susceptibility of the culture.
/// </remarks>
public sealed record RecordCultureResultCommand(Guid PatientId, string CultureId, string Organism, IReadOnlyList<AntibioticSensitivityDto> Susceptibilities, Guid? OrderId = null, string? SecondOrganism = null, string? Condition = null, string? Colony = null, string? Reactions = null) : IRequest<CultureResultDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.EnterResults;
}

/// <summary>
/// Handles the RecordCultureResult use case.
/// </summary>
public sealed class RecordCultureResultHandler : IRequestHandler<RecordCultureResultCommand, CultureResultDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public RecordCultureResultHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }
    /// <inheritdoc />
    public async Task<CultureResultDto> Handle(RecordCultureResultCommand request, CancellationToken cancellationToken)
    {
        var patient = _db.Set<Patient>()
            .FirstOrDefault(candidate => candidate.Id.Value == request.PatientId)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var culture = CultureResult.Create(
            patient.Id,
            request.CultureId,
            request.Organism,
            _clock.UtcNow,
            request.OrderId,
            request.SecondOrganism,
            request.Condition,
            request.Colony,
            request.Reactions);

        foreach (var susceptibility in request.Susceptibilities)
        {
            culture.AddSusceptibility(AntibioticSensitivity.Create(
                susceptibility.AntibioticSymbol,
                susceptibility.Reading,
                susceptibility.Unit));
        }

        _unitOfWork.Add(culture);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CultureResultDto.FromDomain(culture);
    }
}
