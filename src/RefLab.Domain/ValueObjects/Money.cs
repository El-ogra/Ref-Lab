using System.Globalization;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// A monetary amount together with its ISO 4217 currency code.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the legacy system stores monetary amounts in
/// <c>Double</c> columns (<c>patientinfo.totalprice</c>,
/// <c>patientinfo.totalpricedisc</c>, <c>patientinfo.elmadfoo3</c>,
/// <c>patienttest.testprice</c>, <c>payment.money</c>). Binary floating point
/// cannot represent decimal currency values exactly, so Ref Lab uses
/// <see cref="decimal"/> throughout. This is a data-integrity correction, not a
/// change of business behaviour.
/// </para>
/// <para>
/// The currency code is mandatory and has no default. The legacy system carried
/// no currency column at all, so the laboratory's currency is a configuration
/// decision for the owner rather than a value this layer may invent.
/// </para>
/// </remarks>
public sealed class Money : IEquatable<Money>, IComparable<Money>
{
    private Money(decimal amount, string currencyCode)
    {
        Amount = amount;
        CurrencyCode = currencyCode;
    }

    /// <summary>The amount, expressed in <see cref="CurrencyCode"/>.</summary>
    public decimal Amount { get; }

    /// <summary>The ISO 4217 currency code, always three upper-case letters.</summary>
    public string CurrencyCode { get; }

    /// <summary>Creates a monetary amount.</summary>
    /// <exception cref="InvalidMoneyException">
    /// The currency code is not three upper-case letters.
    /// </exception>
    public static Money Create(decimal amount, string currencyCode)
    {
        ArgumentNullException.ThrowIfNull(currencyCode);

        var normalized = currencyCode.Trim().ToUpperInvariant();

        if (normalized.Length != 3 || !normalized.All(char.IsAsciiLetter))
        {
            throw new InvalidMoneyException(currencyCode);
        }

        return new Money(amount, normalized);
    }

    /// <summary>Creates a zero amount in the given currency.</summary>
    public static Money Zero(string currencyCode) => Create(0m, currencyCode);

    /// <summary>Adds two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.CurrencyCode);
    }

    /// <summary>Subtracts two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.CurrencyCode);
    }

    /// <summary>Negates an amount.</summary>
    public static Money operator -(Money value) => new(-value.Amount, value.CurrencyCode);

    /// <summary>Multiplies an amount by a factor.</summary>
    public static Money operator *(Money value, decimal factor) =>
        new(value.Amount * factor, value.CurrencyCode);

    /// <summary>Compares two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount > right.Amount;
    }

    /// <summary>Compares two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount < right.Amount;
    }

    /// <summary>Compares two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static bool operator >=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount >= right.Amount;
    }

    /// <summary>Compares two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static bool operator <=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount <= right.Amount;
    }

    /// <summary>Compares two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null || right is null)
        {
            return ReferenceEquals(left, right);
        }

        EnsureSameCurrency(left, right);
        return left.Amount == right.Amount;
    }

    /// <summary>Compares two amounts of the same currency.</summary>
    /// <exception cref="CurrencyMismatchException">The currencies differ.</exception>
    public static bool operator !=(Money? left, Money? right) => !(left == right);

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (!string.Equals(left.CurrencyCode, right.CurrencyCode, StringComparison.Ordinal))
        {
            throw new CurrencyMismatchException(left.CurrencyCode, right.CurrencyCode);
        }
    }

    /// <inheritdoc />
    public int CompareTo(Money? other)
    {
        if (other is null)
        {
            return 1;
        }

        EnsureSameCurrency(this, other);
        return Amount.CompareTo(other.Amount);
    }

    /// <inheritdoc />
    public bool Equals(Money? other) =>
        other is not null &&
        Amount == other.Amount &&
        string.Equals(CurrencyCode, other.CurrencyCode, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Money);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Amount, CurrencyCode);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{Amount} {CurrencyCode}");
}
