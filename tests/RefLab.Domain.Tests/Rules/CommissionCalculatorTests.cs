using RefLab.Domain.Billing;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Rules;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Rules;

/// <summary>
/// The commission is the referring party's rate applied to the patient-level
/// base amount: <c>SUM(omola * elmadfoo3 / 100)</c>.
/// </summary>
public class CommissionCalculatorTests
{
    [Fact]
    public void The_commission_is_the_rate_applied_to_the_base_amount()
    {
        var rate = Percentage.Create(10m);
        var baseAmount = Money.Create(1000m, "EGP");

        var commission = CommissionCalculator.Calculate(rate, baseAmount);

        Assert.Equal(100m, commission.Amount);
        Assert.Equal("EGP", commission.CurrencyCode);
    }

    [Fact]
    public void A_referring_party_without_a_rate_yields_no_commission()
    {
        var party = ReferringParty.Create(19, "CL-01", "Clinic");
        var baseAmount = Money.Create(1000m, "EGP");

        Assert.Throws<InvalidPercentageException>(() =>
            CommissionCalculator.Calculate(party, baseAmount));
    }

    [Fact]
    public void A_referring_party_with_a_rate_yields_its_commission()
    {
        var party = ReferringParty.Create(19, "CL-01", "Clinic");
        party.SetCommissionRate(Percentage.Create(12.5m));

        var commission = CommissionCalculator.Calculate(party, Money.Create(800m, "EGP"));

        Assert.Equal(100m, commission.Amount);
    }

    [Fact]
    public void The_total_commission_is_the_sum_over_every_base_amount()
    {
        var rate = Percentage.Create(10m);
        var baseAmounts = new[] { Money.Create(1000m, "EGP"), Money.Create(500m, "EGP") };

        var total = CommissionCalculator.CalculateTotal(rate, baseAmounts);

        Assert.Equal(150m, total.Amount);
    }

    [Fact]
    public void A_total_over_amounts_in_two_currencies_is_rejected()
    {
        var rate = Percentage.Create(10m);
        var baseAmounts = new[] { Money.Create(1000m, "EGP"), Money.Create(500m, "USD") };

        Assert.Throws<CurrencyMismatchException>(() => CommissionCalculator.CalculateTotal(rate, baseAmounts));
    }

    [Fact]
    public void A_total_over_no_amounts_is_rejected()
    {
        var rate = Percentage.Create(10m);

        Assert.Throws<InvalidPaymentException>(() =>
            CommissionCalculator.CalculateTotal(rate, Array.Empty<Money>()));
    }
}
