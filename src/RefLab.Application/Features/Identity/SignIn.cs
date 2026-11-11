using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Identity;
using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Identity;

/// <summary>
/// Signs an operator into the laboratory.
/// </summary>
public sealed record SignInCommand(string OperatorName, string Password) : IRequest<SignInResultDto>;

/// <summary>
/// Handles the operator sign-in.
/// </summary>
public sealed class SignInHandler : IRequestHandler<SignInCommand, SignInResultDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public SignInHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<SignInResultDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = request.OperatorName.Trim();

        // The comparison is made on the upper-cased form of the name. EF Core
        // cannot translate the string.Equals overload that takes a
        // StringComparison into SQL, so the case-insensitive match is
        // expressed in a form every provider can translate.
        var comparisonName = normalizedName.ToUpper();

        var @operator = _db.Set<Operator>()
            .FirstOrDefault(candidate => candidate.Name.ToUpper() == comparisonName);

        if (@operator is null)
        {
            throw new NotFoundException(nameof(Operator), normalizedName);
        }

        if (!_passwordHasher.Verify(request.Password, @operator.PasswordHash))
        {
            throw new ForbiddenException("The operator name or the password is incorrect.");
        }

        if (!@operator.IsActive)
        {
            throw new ForbiddenException("The operator account is not active.");
        }

        var now = _clock.UtcNow;

        @operator.RecordAttendanceStart(now);
        @operator.RecordLastVisit(now);

        var attendance = AttendanceEntry.Create(
            @operator.Id,
            @operator.Name,
            AttendanceEventType.StartedWork,
            now);

        _unitOfWork.Update(@operator);
        _unitOfWork.Add(attendance);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SignInResultDto(OperatorDto.FromDomain(@operator), now);
    }
}
