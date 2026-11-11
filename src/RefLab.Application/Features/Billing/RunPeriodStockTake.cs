using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Billing;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// Runs the stock-take of the laboratory over a period and reports the income, the discount and the commission.
/// </summary>
/// <remarks>
/// Legacy evidence: the stock-take of a period reports the laboratory income, the discount and the commission earned by each referring party.
/// </remarks>
public sealed record RunPeriodStockTakeQuery(DateTimeOffset From, DateTimeOffset To, string CurrencyCode) : IRequest<StockTakeDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageAccounting;
}

/// <summary>
/// Handles the RunPeriodStockTake use case.
/// </summary>
public sealed class RunPeriodStockTakeHandler : IRequestHandler<RunPeriodStockTakeQuery, StockTakeDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public RunPeriodStockTakeHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<StockTakeDto> Handle(RunPeriodStockTakeQuery request, CancellationToken cancellationToken)
    {
        var orders = _db.Set<PatientOrder>()
            .Where(order => order.RequestDate >= request.From && order.RequestDate <= request.To)
            .ToArray();

        var payments = _db.Set<Payment>()
            .Where(payment => payment.PaidDate >= request.From && payment.PaidDate <= request.To)
            .ToArray();

        var gross = payments.Sum(payment => payment.Amount.Amount);
        var discounted = orders
            .Where(order => order.TotalPriceAfterDiscount is not null)
            .Sum(order => order.TotalPriceAfterDiscount!.Amount);

        var commission = 0m;

        var parties = _db.Set<ReferringParty>().ToArray();

        foreach (var order in orders)
        {
            if (order.ReferringPartyId is null || order.CommissionBaseAmount is null)
            {
                continue;
            }

            var party = parties.FirstOrDefault(candidate => candidate.Id == order.ReferringPartyId.Value);

            if (party?.CommissionRate is null)
            {
                continue;
            }

            commission += CommissionCalculator
                .Calculate(party.CommissionRate, order.CommissionBaseAmount)
                .Amount;
        }

        return Task.FromResult<StockTakeDto>(new StockTakeDto(
            request.From,
            request.To,
            null,
            null,
            orders.Length,
            gross,
            discounted,
            commission,
            request.CurrencyCode));
    }
}
