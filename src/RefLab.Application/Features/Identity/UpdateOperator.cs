using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Identity;

/// <summary>
/// Updates the identity, the role and the active state of an operator account.
/// </summary>
/// <remarks>
/// Legacy evidence: the operator table carries the name, the role caption and the active flag.
/// </remarks>
public sealed record UpdateOperatorCommand(int OperatorId, string Name, string? RoleCaption, bool IsActive) : IRequest<OperatorDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageOperators;
}

/// <summary>
/// Handles the UpdateOperator use case.
/// </summary>
public sealed class UpdateOperatorHandler : IRequestHandler<UpdateOperatorCommand, OperatorDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public UpdateOperatorHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<OperatorDto> Handle(UpdateOperatorCommand request, CancellationToken cancellationToken)
    {
        var @operator = _db.Set<Operator>()
            .FirstOrDefault(candidate => candidate.Id == request.OperatorId)
            ?? throw new NotFoundException(nameof(Operator), request.OperatorId);

        @operator.SetName(request.Name);

        if (request.RoleCaption is not null)
        {
            @operator.SetRoleCaption(request.RoleCaption);
        }

        @operator.SetActive(request.IsActive);

        _unitOfWork.Update(@operator);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return OperatorDto.FromDomain(@operator);
    }
}
