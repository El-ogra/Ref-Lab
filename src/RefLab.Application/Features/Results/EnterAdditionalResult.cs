using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Writes one of the additional result slots of a requested test.
/// </summary>
/// <remarks>
/// Legacy evidence: the order row carries the primary result and four additional result slots.
/// </remarks>
public sealed record EnterAdditionalResultCommand(Guid OrderTestId, int SlotIndex, string? Value) : IRequest<OrderTestDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.EnterResults;
}

/// <summary>
/// Handles the EnterAdditionalResult use case.
/// </summary>
public sealed class EnterAdditionalResultHandler : IRequestHandler<EnterAdditionalResultCommand, OrderTestDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public EnterAdditionalResultHandler(
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
    public async Task<OrderTestDto> Handle(EnterAdditionalResultCommand request, CancellationToken cancellationToken)
    {
        var orderTest = _db.Set<OrderTest>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderTestId)
            ?? throw new NotFoundException(nameof(OrderTest), request.OrderTestId);

        var now = _clock.UtcNow;
        var operatorName = _currentUser.OperatorName ?? "unknown";

        orderTest.SetAdditionalResult(request.SlotIndex, request.Value, now, operatorName);

        _unitOfWork.Update(orderTest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return OrderTestDto.FromDomain(orderTest);
    }
}
