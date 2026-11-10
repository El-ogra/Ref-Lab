using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Billing;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Hands the reviewed results of an order over to the patient and settles the
/// account.
/// </summary>
public sealed record HandOverResultsCommand(
    Guid OrderId,
    decimal? Amount = null,
    int PaymentType = 0) : IRequest<IReadOnlyList<OrderTestDto>>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.HandOverResults;
}

/// <summary>
/// Handles the hand-over of results.
/// </summary>
public sealed class HandOverResultsHandler : IRequestHandler<HandOverResultsCommand, IReadOnlyList<OrderTestDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public HandOverResultsHandler(
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
    public async Task<IReadOnlyList<OrderTestDto>> Handle(
        HandOverResultsCommand request,
        CancellationToken cancellationToken)
    {
        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), request.OrderId);

        var now = _clock.UtcNow;
        var operatorName = _currentUser.OperatorName ?? "unknown";

        var tests = _db.Set<OrderTest>()
            .Where(test => test.OrderId == order.Id)
            .OrderBy(test => test.ArrangementNumber)
            .ToArray();

        if (tests.Length == 0)
        {
            throw new ConflictException("An order without a requested test cannot be handed over.");
        }

        foreach (var test in tests)
        {
            test.RequireReviewed();
            test.MarkGiven(operatorName, now);
            _unitOfWork.Update(test);
        }

        if (request.Amount.HasValue)
        {
            var nextNumber = _db.Set<Domain.Billing.Payment>()
                .Select(payment => payment.Number)
                .DefaultIfEmpty(0)
                .Max() + 1;

            var payment = Domain.Billing.Payment.Create(
                order.Id,
                now,
                Money.Create(request.Amount.Value, order.CurrencyCode),
                nextNumber,
                request.PaymentType,
                legacyOrderIdentifier: order.LegacyIdentifier?.Value);

            _unitOfWork.Add(payment);
        }

        order.Close(now);
        order.MarkComplete(true);

        _unitOfWork.Update(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tests.Select(OrderTestDto.FromDomain).ToArray();
    }
}
