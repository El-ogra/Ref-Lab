using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Microbiology;

namespace RefLab.Application.Features.Microbiology;

/// <summary>
/// Adds an antibiotic to the catalogue, or amends the susceptibility and the clinical flags of an existing one.
/// </summary>
/// <remarks>
/// Legacy evidence: the antibiotic catalogue carries the symbol, the scientific name, eight common names, the pregnancy and child flags and the susceptibility.
/// </remarks>
public sealed record ManageAntibioticCatalogueCommand(int Id, string Symbol, string ScientificName, IReadOnlyList<string> CommonNames, bool IsAllowedInPregnancy = false, bool IsAllowedInChildren = false, string? CultureType = null, string? Sensitivity = null, string? SensitivityUnit = null) : IRequest<AntibioticDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageTestCatalogue;
}

/// <summary>
/// Handles the ManageAntibioticCatalogue use case.
/// </summary>
public sealed class ManageAntibioticCatalogueHandler : IRequestHandler<ManageAntibioticCatalogueCommand, AntibioticDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ManageAntibioticCatalogueHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<AntibioticDto> Handle(ManageAntibioticCatalogueCommand request, CancellationToken cancellationToken)
    {
        var antibiotic = _db.Set<Antibiotic>()
            .FirstOrDefault(candidate => candidate.Id == request.Id);

        if (antibiotic is null)
        {
            antibiotic = Antibiotic.Create(
                request.Id,
                request.Symbol,
                request.ScientificName,
                request.CultureType,
                request.IsAllowedInPregnancy,
                request.IsAllowedInChildren,
                request.CommonNames);

            _unitOfWork.Add(antibiotic);
        }
        else
        {
            antibiotic.SetClinicalFlags(request.IsAllowedInPregnancy, request.IsAllowedInChildren);
            antibiotic.SetCommonNames(request.CommonNames);
            _unitOfWork.Update(antibiotic);
        }

        antibiotic.SetSusceptibility(request.Sensitivity, request.SensitivityUnit);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return AntibioticDto.FromDomain(antibiotic);
    }
}
