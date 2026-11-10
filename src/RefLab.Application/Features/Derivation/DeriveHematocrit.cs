using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Derivation;

/// <summary>
/// Derives the haematocrit from the measured haemoglobin: <c>Hct = Hgb x factor</c>.
/// </summary>
/// <remarks>
/// Legacy evidence: the constant group <c>CBCCONS</c> holds the factor 3.3, and the shipped manual states <c>Hct = Hgb x 3.3</c>. The factor is read from the configured constants, never hard-coded.
/// </remarks>
public sealed record DeriveHematocritQuery(decimal Hemoglobin) : IRequest<DerivationResultDto>;


/// <summary>
/// Handles the DeriveHematocrit use case.
/// </summary>
public sealed class DeriveHematocritHandler : IRequestHandler<DeriveHematocritQuery, DerivationResultDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public DeriveHematocritHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<DerivationResultDto> Handle(DeriveHematocritQuery request, CancellationToken cancellationToken)
    {
        var constants = _db.Set<ClinicalConstant>().ToArray();

        var group = ClinicalConstantResolver.Resolve(constants, ClinicalConstant.CbcGroupName)
            ?? throw new ConflictException(
                $"The constant group '{ClinicalConstant.CbcGroupName}' is not configured.");

        var value = ClinicalDerivationPolicy.DeriveHematocrit(request.Hemoglobin, group);

        return Task.FromResult<DerivationResultDto>(new DerivationResultDto("Hct", value, "%"));
    }
}
