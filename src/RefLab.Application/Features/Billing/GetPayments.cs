using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Billing;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// Lists the payments received, optionally for one order or for one period.
/// </summary>
/// <remarks>
/// Legacy evidence: the payment table records the order, the payment date, the type, the referring party, the analyte, the amount, the treating physician and the receipt number.
/// </remarks>
public sealed record GetPaymentsQuery(Guid? OrderId = null, DateTimeOffset? From = null, DateTimeOffset? To = null) : IRequest<IReadOnlyList<PaymentDto>>;


/// <summary>
/// Handles the GetPayments use case.
/// </summary>
public sealed class GetPaymentsHandler : IRequestHandler<GetPaymentsQuery, IReadOnlyList<PaymentDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetPaymentsHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<IReadOnlyList<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<Payment>().AsQueryable();

        if (request.OrderId.HasValue)
        {
            query = query.Where(payment => payment.OrderId == request.OrderId.Value);
        }

        if (request.From.HasValue)
        {
            query = query.Where(payment => payment.PaidDate >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(payment => payment.PaidDate <= request.To.Value);
        }

        var payments = query
            .OrderByDescending(payment => payment.PaidDate)
            .Select(payment => PaymentDto.FromDomain(payment))
            .ToArray();

        return Task.FromResult<IReadOnlyList<PaymentDto>>(payments);
    }
}
