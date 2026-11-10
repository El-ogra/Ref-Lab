using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Identity;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Identity;

/// <summary>
/// Creates an operator account.
/// </summary>
public sealed record CreateOperatorCommand(
    string Name,
    string Password,
    string? RoleCaption,
    bool IsAdministrator,
    IReadOnlyList<string> Permissions) : IRequest<OperatorDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageOperators;
}

/// <summary>
/// Handles the creation of an operator account.
/// </summary>
public sealed class CreateOperatorHandler : IRequestHandler<CreateOperatorCommand, OperatorDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>Initialises a new handler.</summary>
    public CreateOperatorHandler(IAppDbContext db, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public async Task<OperatorDto> Handle(CreateOperatorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            PasswordPolicy.Validate(request.Password);
        }
        catch (WeakPasswordException exception)
        {
            throw new RequestValidationException(new[] { exception.Message });
        }

        var normalizedName = request.Name.Trim();

        var duplicate = _db.Set<Operator>()
            .Any(candidate => string.Equals(candidate.Name, normalizedName, StringComparison.OrdinalIgnoreCase));

        if (duplicate)
        {
            throw new ConflictException($"An operator named '{normalizedName}' already exists.");
        }

        var nextId = _db.Set<Operator>()
            .Select(candidate => candidate.Id)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var @operator = Operator.Create(nextId, normalizedName, _passwordHasher.Hash(request.Password), request.RoleCaption);

        @operator.SetAdministrator(request.IsAdministrator);

        foreach (var permission in request.Permissions)
        {
            if (!Enum.TryParse<Permission>(permission, ignoreCase: true, out var parsed))
            {
                throw new RequestValidationException(new[] { $"'{permission}' is not a known permission." });
            }

            @operator.Grant(parsed);
        }

        _unitOfWork.Add(@operator);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OperatorDto.FromDomain(@operator);
    }
}
