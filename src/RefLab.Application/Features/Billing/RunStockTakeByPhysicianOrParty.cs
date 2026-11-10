using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Billing;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// Runs the stock-take of one treating physician or one referring party over a period.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the stock-take by treating physician and the stock-take by referring party as separate capabilities.
/// </remarks>
public sealed record RunStockTakeByPhysicianOrPartyQuery(DateTimeOffset From, DateTimeOffset To, string CurrencyCode, string? ReferringPartyCode = null, string? TreatingPhysicianCode = null) : IRequest<StockTakeDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageAccounting;
}

/// <summary>
/// Handles the RunStockTakeByPhysicianOrParty use case.
/// </summary>
public sealed class RunStockTakeByPhysicianOrPartyHandler : IRequestHandler<RunStockTakeByPhysicianOrPartyQuery, StockTakeDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public RunStockTakeByPhysicianOrPartyHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<StockTakeDto> Handle(RunStockTakeByPhysicianOrPartyQuery request, CancellationToken cancellationToken)
    {
        if (request.ReferringPartyCode is null && request.TreatingPhysicianCode is null)
        {
            throw new RequestValidationException(
                new[] { "A referring party or a treating physician must be supplied." });
        }

        var orders = _db.Set<PatientOrder>()
            .Where(order => order.RequestDate >= request.From && order.RequestDate <= request.To)
            .ToArray();

        if (request.ReferringPartyCode is not null)
        {
            var party = _db.Set<ReferringParty>()
                .FirstOrDefault(candidate => candidate.Code == request.ReferringPartyCode)
                ?? throw new NotFoundException(nameof(ReferringParty), request.ReferringPartyCode);

            orders = orders.Where(order => order.ReferringPartyId == party.Id).ToArray();
        }

        if (request.TreatingPhysicianCode is not null)
        {
            orders = orders
                .Where(order => order.TreatingPhysicianCode == request.TreatingPhysicianCode)
                .ToArray();
        }

        var orderIds = orders.Select(order => order.Id).ToArray();

        var payments = _db.Set<Payment>()
            .Where(payment => orderIds.Contains(payment.OrderId))
            .ToArray();

        var gross = payments.Sum(payment => payment.Amount.Amount);
        var discounted = orders
            .Where(order => order.TotalPriceAfterDiscount is not null)
            .Sum(order => order.TotalPriceAfterDiscount!.Amount);

        return Task.FromResult<StockTakeDto>(new StockTakeDto(
            request.From,
            request.To,
            request.ReferringPartyCode,
            request.TreatingPhysicianCode,
            orders.Length,
            gross,
            discounted,
            0m,
            request.CurrencyCode));
    }
}
