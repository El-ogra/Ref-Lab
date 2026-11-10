using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Derivation;

/// <summary>
/// Derives the international sensitivity index of a prothrombin time control.
/// </summary>
/// <remarks>
/// Legacy evidence: the constant group <c>PTCONS</c> holds 1.33 as the control index, which establishes that the index is computed. The legacy formula was not recoverable from the static evidence, so this implementation follows the standard laboratory definition.
/// </remarks>
public sealed record DeriveInternationalSensitivityIndexQuery(decimal PatientProthrombinTime, decimal ControlProthrombinTime, decimal NormalProthrombinTime, decimal ControlIndex) : IRequest<DerivationResultDto>;


/// <summary>
/// Handles the DeriveInternationalSensitivityIndex use case.
/// </summary>
public sealed class DeriveInternationalSensitivityIndexHandler : IRequestHandler<DeriveInternationalSensitivityIndexQuery, DerivationResultDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public DeriveInternationalSensitivityIndexHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<DerivationResultDto> Handle(DeriveInternationalSensitivityIndexQuery request, CancellationToken cancellationToken)
    {
        var value = ClinicalDerivationPolicy.DeriveInternationalSensitivityIndex(
            request.PatientProthrombinTime,
            request.ControlProthrombinTime,
            request.NormalProthrombinTime,
            request.ControlIndex);

        return Task.FromResult<DerivationResultDto>(new DerivationResultDto("ISI", value, null));
    }
}
