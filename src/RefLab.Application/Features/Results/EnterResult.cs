using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Enters a result value against a requested test.
/// </summary>
public sealed record EnterResultCommand(
    Guid OrderTestId,
    string Value,
    string? Unit = null,
    string? Comment = null,
    bool? SampleTakenOutsideLab = null) : IRequest<OrderTestDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.EnterResults;
}

/// <summary>
/// Handles the entry of a result value.
/// </summary>
public sealed class EnterResultHandler : IRequestHandler<EnterResultCommand, OrderTestDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public EnterResultHandler(
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
    public async Task<OrderTestDto> Handle(EnterResultCommand request, CancellationToken cancellationToken)
    {
        var orderTest = _db.Set<OrderTest>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderTestId)
            ?? throw new NotFoundException(nameof(OrderTest), request.OrderTestId);

        var now = _clock.UtcNow;
        var operatorName = _currentUser.OperatorName ?? "unknown";

        orderTest.EnterResult(ResultValue.Create(request.Value, request.Unit), now, operatorName);

        if (!string.IsNullOrWhiteSpace(request.Comment))
        {
            orderTest.SetComment(request.Comment, now, operatorName);
        }

        if (request.SampleTakenOutsideLab.HasValue)
        {
            orderTest.SetSampleTakenOutsideLab(request.SampleTakenOutsideLab.Value, now, operatorName);
        }

        _unitOfWork.Update(orderTest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderTestDto.FromDomain(orderTest);
    }
}
