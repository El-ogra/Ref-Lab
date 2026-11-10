using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Amends the limits and the texts of a reference range.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability of adding or amending the normal reference values of a test.
/// </remarks>
public sealed record UpdateReferenceRangeCommand(int Id, decimal? LowLimit = null, decimal? HighLimit = null, string? LowExceptionText = null, string? HighExceptionText = null, string? RangeText = null) : IRequest<ReferenceRangeDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageReferenceRanges;
}

/// <summary>
/// Handles the UpdateReferenceRange use case.
/// </summary>
public sealed class UpdateReferenceRangeHandler : IRequestHandler<UpdateReferenceRangeCommand, ReferenceRangeDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public UpdateReferenceRangeHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<ReferenceRangeDto> Handle(UpdateReferenceRangeCommand request, CancellationToken cancellationToken)
    {
        var range = _db.Set<ReferenceRange>()
            .FirstOrDefault(candidate => candidate.Id == request.Id)
            ?? throw new NotFoundException(nameof(ReferenceRange), request.Id);

        range.UpdateLimits(
            request.LowLimit,
            request.HighLimit,
            request.LowExceptionText,
            request.HighExceptionText,
            request.RangeText);

        _unitOfWork.Update(range);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ReferenceRangeDto.FromDomain(range);
    }
}
