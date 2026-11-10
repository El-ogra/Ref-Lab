using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Derivation;

/// <summary>
/// Interpolates the concentration of a clotting control from its control time, using the laboratory calibration curve.
/// </summary>
/// <remarks>
/// Legacy evidence: the calibration table holds 33 points, for example 12 seconds to 100 and 12.5 seconds to 92, and the concentration of an unknown control is interpolated between the two adjacent points.
/// </remarks>
public sealed record InterpolateProthrombinControlQuery(decimal ControlTime) : IRequest<DerivationResultDto>;


/// <summary>
/// Handles the InterpolateProthrombinControl use case.
/// </summary>
public sealed class InterpolateProthrombinControlHandler : IRequestHandler<InterpolateProthrombinControlQuery, DerivationResultDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public InterpolateProthrombinControlHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<DerivationResultDto> Handle(InterpolateProthrombinControlQuery request, CancellationToken cancellationToken)
    {
        var calibration = _db.Set<ProthrombinControlCalibration>()
            .Select(point => point.ToPoint())
            .ToArray();

        if (calibration.Length == 0)
        {
            throw new ConflictException("The prothrombin control curve is not configured.");
        }

        var concentration = ClinicalDerivationPolicy.InterpolateControlConcentration(
            calibration,
            request.ControlTime);

        return Task.FromResult<DerivationResultDto>(new DerivationResultDto("Control concentration", concentration, null));
    }
}
