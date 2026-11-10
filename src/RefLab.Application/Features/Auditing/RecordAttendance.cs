using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Auditing;
using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Auditing;

/// <summary>
/// Records an attendance event for the signed-in operator.
/// </summary>
public sealed record RecordAttendanceCommand(AttendanceEventType EventType) : IRequest<AttendanceEntryDto>;

/// <summary>
/// Handles the recording of an attendance event.
/// </summary>
public sealed class RecordAttendanceHandler : IRequestHandler<RecordAttendanceCommand, AttendanceEntryDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public RecordAttendanceHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<AttendanceEntryDto> Handle(RecordAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.OperatorId is null)
        {
            throw new ForbiddenException("An operator must be signed in to record attendance.");
        }

        var operatorId = _currentUser.OperatorId.Value;

        var @operator = _db.Set<Operator>()
            .FirstOrDefault(candidate => candidate.Id == operatorId)
            ?? throw new NotFoundException(nameof(Operator), operatorId);

        var now = _clock.UtcNow;

        switch (request.EventType)
        {
            case AttendanceEventType.StartedWork:
                @operator.RecordAttendanceStart(now);
                break;
            case AttendanceEventType.EndedWork:
                @operator.RecordAttendanceEnd(now);
                break;
            case AttendanceEventType.StartedBreak:
                @operator.RecordBreakStart(now);
                break;
            case AttendanceEventType.EndedBreak:
                @operator.RecordBreakEnd();
                break;
            default:
                throw new RequestValidationException(new[] { "The attendance event type is not known." });
        }

        var entry = AttendanceEntry.Create(@operator.Id, @operator.Name, request.EventType, now);

        _unitOfWork.Update(@operator);
        _unitOfWork.Add(entry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return AttendanceEntryDto.FromDomain(entry);
    }
}
