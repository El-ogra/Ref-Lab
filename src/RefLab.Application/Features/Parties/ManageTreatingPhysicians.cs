using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Billing;
using RefLab.Domain.Billing;

namespace RefLab.Application.Features.Parties;

/// <summary>
/// Adds a treating physician to the register, or amends the contact details of an existing one.
/// </summary>
/// <remarks>
/// Legacy evidence: version 2 split the physician out of the referring party — the physician register holds the codes that the referring party points at.
/// </remarks>
public sealed record ManageTreatingPhysiciansCommand(int Id, string Code, string Name, string? Phone = null) : IRequest<TreatingPhysicianDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManagePricing;
}

/// <summary>
/// Handles the ManageTreatingPhysicians use case.
/// </summary>
public sealed class ManageTreatingPhysiciansHandler : IRequestHandler<ManageTreatingPhysiciansCommand, TreatingPhysicianDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ManageTreatingPhysiciansHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<TreatingPhysicianDto> Handle(ManageTreatingPhysiciansCommand request, CancellationToken cancellationToken)
    {
        var physician = _db.Set<TreatingPhysician>()
            .FirstOrDefault(candidate => candidate.Id == request.Id);

        if (physician is null)
        {
            physician = TreatingPhysician.Create(request.Id, request.Code, request.Name);
            physician.SetPhone(request.Phone);
            _unitOfWork.Add(physician);
        }
        else
        {
            physician.SetPhone(request.Phone);
            _unitOfWork.Update(physician);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return TreatingPhysicianDto.FromDomain(physician);
    }
}
