using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Adds a reference range for an analyte, scoped by sex and by age window.
/// </summary>
/// <remarks>
/// Legacy evidence: the range table carries 852 rows over 304 analytes, each scoped by sex, by an age window and by an age unit.
/// </remarks>
public sealed record CreateReferenceRangeCommand(int TestCode, string TestSimpleName, int SubGroupCode, SexScope SexScope, AgeUnit AgeUnit, decimal? AgeFrom = null, decimal? AgeTo = null, decimal? LowLimit = null, decimal? HighLimit = null, string? LowExceptionText = null, string? HighExceptionText = null, string? RangeText = null, string? Unit = null) : IRequest<ReferenceRangeDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageReferenceRanges;
}

/// <summary>
/// Handles the CreateReferenceRange use case.
/// </summary>
public sealed class CreateReferenceRangeHandler : IRequestHandler<CreateReferenceRangeCommand, ReferenceRangeDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public CreateReferenceRangeHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<ReferenceRangeDto> Handle(CreateReferenceRangeCommand request, CancellationToken cancellationToken)
    {
        var nextId = _db.Set<ReferenceRange>()
            .Select(range => range.Id)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var range = ReferenceRange.Create(
            nextId,
            request.TestCode,
            request.TestSimpleName,
            request.SubGroupCode,
            request.SexScope,
            AgeBand.Create(request.AgeFrom, request.AgeTo, request.AgeUnit),
            ReferenceInterval.Create(
                lowLimit: request.LowLimit,
                highLimit: request.HighLimit,
                lowExceptionText: request.LowExceptionText,
                highExceptionText: request.HighExceptionText,
                rangeText: request.RangeText,
                unit: request.Unit));

        _unitOfWork.Add(range);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ReferenceRangeDto.FromDomain(range);
    }
}
