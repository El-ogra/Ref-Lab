using RefLab.Domain.Billing;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Rules;

/// <summary>
/// Computes the commission earned by a referring party.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the commission is computed as
/// <c>SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)</c>. The referring
/// party's rate is applied to the patient-level base amount.
/// </para>
/// <para>
/// The three amount fields on the legacy order row — <c>totalprice</c>,
/// <c>totalpricedisc</c> and <c>elmadfoo3</c> — are distinct and only
/// <c>elmadfoo3</c> participates in the formula.
/// </para>
/// </remarks>
public static class CommissionCalculator
{
    /// <summary>
    /// Computes the commission earned on a single patient-level base amount.
    /// </summary>
    public static Money Calculate(Percentage commissionRate, Money baseAmount)
    {
        ArgumentNullException.ThrowIfNull(commissionRate);
        ArgumentNullException.ThrowIfNull(baseAmount);

        return Money.Create(commissionRate.Apply(baseAmount.Amount), baseAmount.CurrencyCode);
    }

    /// <summary>
    /// Computes the commission earned on a single patient-level base amount.
    /// </summary>
    /// <exception cref="Exceptions.InvalidPercentageException">
    /// The referring party carries no commission rate.
    /// </exception>
    public static Money Calculate(ReferringParty referringParty, Money baseAmount)
    {
        ArgumentNullException.ThrowIfNull(referringParty);
        ArgumentNullException.ThrowIfNull(baseAmount);

        if (referringParty.CommissionRate is null)
        {
            throw new Exceptions.InvalidPercentageException(0m);
        }

        return Calculate(referringParty.CommissionRate, baseAmount);
    }

    /// <summary>
    /// Computes the total commission over a collection of patient-level base
    /// amounts.
    /// </summary>
    public static Money CalculateTotal(Percentage commissionRate, IEnumerable<Money> baseAmounts)
    {
        ArgumentNullException.ThrowIfNull(commissionRate);
        ArgumentNullException.ThrowIfNull(baseAmounts);

        var total = 0m;
        var currency = (string?)null;

        foreach (var baseAmount in baseAmounts)
        {
            ArgumentNullException.ThrowIfNull(baseAmount);

            if (currency is null)
            {
                currency = baseAmount.CurrencyCode;
            }
            else if (!string.Equals(currency, baseAmount.CurrencyCode, StringComparison.Ordinal))
            {
                throw new Exceptions.CurrencyMismatchException(currency, baseAmount.CurrencyCode);
            }

            total += commissionRate.Apply(baseAmount.Amount);
        }

        if (currency is null)
        {
            throw new Exceptions.InvalidPaymentException(
                "A commission total requires at least one base amount.");
        }

        return Money.Create(total, currency);
    }
}
