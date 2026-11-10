using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Identity;

/// <summary>
/// Replaces the set of function-level permissions granted to an operator account.
/// </summary>
/// <remarks>
/// Legacy evidence: the operator table carries seventeen permission bits; Ref Lab grants them by name instead, so that a permission can be audited and reported on.
/// </remarks>
public sealed record AssignOperatorPermissionsCommand(int OperatorId, IReadOnlyList<string> Permissions) : IRequest<OperatorDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageOperators;
}

/// <summary>
/// Handles the AssignOperatorPermissions use case.
/// </summary>
public sealed class AssignOperatorPermissionsHandler : IRequestHandler<AssignOperatorPermissionsCommand, OperatorDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public AssignOperatorPermissionsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<OperatorDto> Handle(AssignOperatorPermissionsCommand request, CancellationToken cancellationToken)
    {
        var @operator = _db.Set<Operator>()
            .FirstOrDefault(candidate => candidate.Id == request.OperatorId)
            ?? throw new NotFoundException(nameof(Operator), request.OperatorId);

        var requested = new HashSet<Permission>();

        foreach (var permission in request.Permissions)
        {
            if (!Enum.TryParse<Permission>(permission, ignoreCase: true, out var parsed))
            {
                throw new RequestValidationException(new[] { $"'{permission}' is not a known permission." });
            }

            requested.Add(parsed);
        }

        foreach (var existing in @operator.Permissions.ToArray())
        {
            if (!requested.Contains(existing))
            {
                @operator.Revoke(existing);
            }
        }

        foreach (var permission in requested)
        {
            @operator.Grant(permission);
        }

        _unitOfWork.Update(@operator);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return OperatorDto.FromDomain(@operator);
    }
}
