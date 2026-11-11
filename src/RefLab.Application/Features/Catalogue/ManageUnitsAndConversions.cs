using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Maintains the measurement-unit dictionary and the unit-conversion dictionary in one step.
/// </summary>
/// <remarks>
/// Legacy evidence: the unit dictionary holds 69 units and the conversion dictionary 167 analyte-specific factors, for example Albumin g/dL to g/L with a factor of 10.
/// </remarks>
public sealed record ManageUnitsAndConversionsCommand(string? UnitName = null, string? ConversionTestSimpleName = null, string? FromUnit = null, string? ToUnit = null, decimal? Factor = null, int FromDecimals = 0, int ToDecimals = 0) : IRequest<UnitsAndConversionsDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageTestCatalogue;
}

/// <summary>
/// Handles the ManageUnitsAndConversions use case.
/// </summary>
public sealed class ManageUnitsAndConversionsHandler : IRequestHandler<ManageUnitsAndConversionsCommand, UnitsAndConversionsDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ManageUnitsAndConversionsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<UnitsAndConversionsDto> Handle(ManageUnitsAndConversionsCommand request, CancellationToken cancellationToken)
    {
        if (request.UnitName is not null)
        {
            // See SignIn.cs: the upper-cased comparison is the form EF Core can
            // translate, and it keeps the matching case-insensitive.
            var comparisonName = request.UnitName.Trim().ToUpper();

            var existingUnit = _db.Set<TestUnit>()
                .FirstOrDefault(unit => unit.Name.ToUpper() == comparisonName);

            if (existingUnit is null)
            {
                var nextUnitId = _db.Set<TestUnit>()
                    .Select(unit => unit.Id)
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                _unitOfWork.Add(TestUnit.Create(nextUnitId, request.UnitName));
            }
            else
            {
                existingUnit.UpdateName(request.UnitName);
                _unitOfWork.Update(existingUnit);
            }
        }

        if (request.ConversionTestSimpleName is not null &&
            request.FromUnit is not null &&
            request.ToUnit is not null &&
            request.Factor.HasValue)
        {
            var existingFactor = _db.Set<ConversionFactor>()
                .FirstOrDefault(factor =>
                    factor.TestSimpleName == request.ConversionTestSimpleName.Trim() &&
                    factor.FromUnit == request.FromUnit.Trim() &&
                    factor.ToUnit == request.ToUnit.Trim());

            if (existingFactor is null)
            {
                _unitOfWork.Add(ConversionFactor.Create(
                    request.ConversionTestSimpleName,
                    request.FromUnit,
                    request.ToUnit,
                    request.Factor.Value,
                    request.FromDecimals,
                    request.ToDecimals));
            }
            else
            {
                existingFactor.UpdateFactor(request.Factor.Value, request.FromDecimals, request.ToDecimals);
                _unitOfWork.Update(existingFactor);
            }
        }

        if (request.UnitName is null &&
            request.ConversionTestSimpleName is null)
        {
            throw new RequestValidationException(
                new[] { "A unit name or a conversion factor must be supplied." });
        }

        var units = _db.Set<TestUnit>()
            .OrderBy(unit => unit.Id)
            .Select(unit => TestUnitDto.FromDomain(unit))
            .ToArray();

        var conversions = _db.Set<ConversionFactor>()
            .OrderBy(factor => factor.TestSimpleName)
            .Select(factor => ConversionFactorDto.FromDomain(factor))
            .ToArray();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UnitsAndConversionsDto(units, conversions);
    }
}
