using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Billing;
using RefLab.Domain.Billing;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// Records a payment against a patient order.
/// </summary>
public sealed record RecordPaymentCommand(
    Guid OrderId,
    decimal Amount,
    int PaymentType = 0,
    string? ReferringPartyCode = null,
    string? TestSimpleName = null,
    string? TreatingPhysicianCode = null,
    string? Comment = null) : IRequest<PaymentDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageAccounting;
}

/// <summary>
/// Handles the recording of a payment.
/// </summary>
public sealed class RecordPaymentHandler : IRequestHandler<RecordPaymentCommand, PaymentDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public RecordPaymentHandler(IAppDbContext db, IUnitOfWork unitOfWork, IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<PaymentDto> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), request.OrderId);

        var nextNumber = _db.Set<Payment>()
            .Select(payment => payment.Number)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var payment = Payment.Create(
            order.Id,
            _clock.UtcNow,
            Money.Create(request.Amount, order.CurrencyCode),
            nextNumber,
            request.PaymentType,
            request.ReferringPartyCode,
            request.TestSimpleName,
            request.TreatingPhysicianCode,
            order.LegacyIdentifier?.Value);

        payment.SetComment(request.Comment);

        _unitOfWork.Add(payment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return PaymentDto.FromDomain(payment);
    }
}
