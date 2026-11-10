using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Confirms an entered result as reviewed.
/// </summary>
public sealed record ReviewResultCommand(Guid OrderTestId) : IRequest<OrderTestDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.ReviewResults;
}

/// <summary>
/// Handles the confirmation of a reviewed result.
/// </summary>
public sealed class ReviewResultHandler : IRequestHandler<ReviewResultCommand, OrderTestDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public ReviewResultHandler(
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
    public async Task<OrderTestDto> Handle(ReviewResultCommand request, CancellationToken cancellationToken)
    {
        var orderTest = _db.Set<OrderTest>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderTestId)
            ?? throw new NotFoundException(nameof(OrderTest), request.OrderTestId);

        orderTest.ConfirmReviewed(_clock.UtcNow, _currentUser.OperatorName ?? "unknown");

        _unitOfWork.Update(orderTest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderTestDto.FromDomain(orderTest);
    }
}
