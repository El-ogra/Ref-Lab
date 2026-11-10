using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Derivation;

/// <summary>
/// Derives the haemoglobin percentage from the measured haemoglobin, selecting the factor by sex and age.
/// </summary>
/// <remarks>
/// Legacy evidence: the constant group <c>CBCCONS</c> holds 6.25 for a male above 12 years, 6.75 for a female above 12 years, 7.5 for 1 to 12 years and 8.25 below 1 year.
/// </remarks>
public sealed record DeriveHemoglobinPercentageQuery(decimal Hemoglobin, Sex Sex, decimal AgeYears) : IRequest<DerivationResultDto>;


/// <summary>
/// Handles the DeriveHemoglobinPercentage use case.
/// </summary>
public sealed class DeriveHemoglobinPercentageHandler : IRequestHandler<DeriveHemoglobinPercentageQuery, DerivationResultDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public DeriveHemoglobinPercentageHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<DerivationResultDto> Handle(DeriveHemoglobinPercentageQuery request, CancellationToken cancellationToken)
    {
        var constants = _db.Set<ClinicalConstant>().ToArray();

        var group = ClinicalConstantResolver.Resolve(constants, ClinicalConstant.CbcGroupName)
            ?? throw new ConflictException(
                $"The constant group '{ClinicalConstant.CbcGroupName}' is not configured.");

        var value = ClinicalDerivationPolicy.DeriveHemoglobinPercentage(
            request.Hemoglobin,
            request.Sex,
            request.AgeYears,
            group);

        return Task.FromResult<DerivationResultDto>(new DerivationResultDto("Hgb%", value, "%"));
    }
}
