using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Adds a clinical sub-group to the catalogue, or amends the name of an existing one.
/// </summary>
/// <remarks>
/// Legacy evidence: the sub-group catalogue holds 46 rows, three of which are not protected against deletion because they are the most recently added.
/// </remarks>
public sealed record ManageClinicalSubGroupsCommand(int Code, string Name, string? ArabicName = null, bool? IsDeleteProtected = null) : IRequest<ClinicalSubGroupDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageTestCatalogue;
}

/// <summary>
/// Handles the ManageClinicalSubGroups use case.
/// </summary>
public sealed class ManageClinicalSubGroupsHandler : IRequestHandler<ManageClinicalSubGroupsCommand, ClinicalSubGroupDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ManageClinicalSubGroupsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<ClinicalSubGroupDto> Handle(ManageClinicalSubGroupsCommand request, CancellationToken cancellationToken)
    {
        var subGroup = _db.Set<ClinicalSubGroup>()
            .FirstOrDefault(candidate => candidate.Code == request.Code);

        if (subGroup is null)
        {
            subGroup = ClinicalSubGroup.Create(
                request.Code,
                request.Name,
                request.ArabicName,
                request.IsDeleteProtected ?? false);

            _unitOfWork.Add(subGroup);
        }
        else
        {
            subGroup.UpdateName(request.Name);

            if (request.ArabicName is not null)
            {
                subGroup.UpdateArabicName(request.ArabicName);
            }

            _unitOfWork.Update(subGroup);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ClinicalSubGroupDto.FromDomain(subGroup);
    }
}
