using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Billing;
using RefLab.Domain.Billing;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// Computes the commission earned by a referring party over a set of orders.
/// </summary>
public sealed record ComputeCommissionQuery(
    int ReferringPartyId,
    IReadOnlyList<Guid> OrderIds) : IRequest<CommissionDto>;

/// <summary>
/// Handles the commission computation.
/// </summary>
public sealed class ComputeCommissionHandler : IRequestHandler<ComputeCommissionQuery, CommissionDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public ComputeCommissionHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<CommissionDto> Handle(ComputeCommissionQuery request, CancellationToken cancellationToken)
    {
        var party = _db.Set<ReferringParty>()
            .FirstOrDefault(candidate => candidate.Id == request.ReferringPartyId)
            ?? throw new NotFoundException(nameof(ReferringParty), request.ReferringPartyId);

        if (party.CommissionRate is null)
        {
            throw new ConflictException(
                $"The referring party '{party.Name}' carries no commission rate.");
        }

        if (request.OrderIds.Count == 0)
        {
            throw new RequestValidationException(new[] { "At least one order must be supplied." });
        }

        var orders = _db.Set<PatientOrder>()
            .Where(order => request.OrderIds.Contains(order.Id))
            .ToArray();

        var baseAmounts = orders
            .Where(order => order.CommissionBaseAmount is not null)
            .Select(order => order.CommissionBaseAmount!)
            .ToArray();

        if (baseAmounts.Length == 0)
        {
            throw new ConflictException("None of the supplied orders carries a commission base amount.");
        }

        var total = CommissionCalculator.CalculateTotal(party.CommissionRate, baseAmounts);
        var baseAmount = Money.Create(
            baseAmounts.Sum(amount => amount.Amount),
            baseAmounts[0].CurrencyCode);

        return Task.FromResult(CommissionDto.FromDomain(party, party.CommissionRate, baseAmount, total));
    }
}
