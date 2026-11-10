using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Identity;

/// <summary>
/// Changes the password of an operator account.
/// </summary>
public sealed record ChangePasswordCommand(
    int OperatorId,
    string CurrentPassword,
    string NewPassword) : IRequest<NoResult>;

/// <summary>
/// Handles the operator password change.
/// </summary>
public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, NoResult>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>Initialises a new handler.</summary>
    public ChangePasswordHandler(IAppDbContext db, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public async Task<NoResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var @operator = _db.Set<Operator>()
            .FirstOrDefault(candidate => candidate.Id == request.OperatorId)
            ?? throw new NotFoundException(nameof(Operator), request.OperatorId);

        if (!_passwordHasher.Verify(request.CurrentPassword, @operator.PasswordHash))
        {
            throw new ForbiddenException("The current password is incorrect.");
        }

        try
        {
            PasswordPolicy.Validate(request.NewPassword);
        }
        catch (WeakPasswordException exception)
        {
            throw new RequestValidationException(new[] { exception.Message });
        }

        if (_passwordHasher.Verify(request.NewPassword, @operator.PasswordHash))
        {
            throw new RequestValidationException(new[] { "The new password must differ from the current one." });
        }

        @operator.SetPasswordHash(_passwordHasher.Hash(request.NewPassword));

        _unitOfWork.Update(@operator);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new NoResult();
    }
}
