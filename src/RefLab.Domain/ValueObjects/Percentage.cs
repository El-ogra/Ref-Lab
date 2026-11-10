using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// A percentage expressed between 0 and 100.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>Gehats.omola</c> is a <c>Double</c> holding the
/// referring party's commission rate. The commission formula is
/// <c>SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)</c>.
/// </remarks>
public sealed class Percentage : IEquatable<Percentage>
{
    private Percentage(decimal value)
    {
        Value = value;
    }

    /// <summary>The percentage value.</summary>
    public decimal Value { get; }

    /// <summary>Creates a percentage.</summary>
    /// <exception cref="InvalidPercentageException">
    /// The value falls outside the range 0 to 100.
    /// </exception>
    public static Percentage Create(decimal value)
    {
        if (value is < 0m or > 100m)
        {
            throw new InvalidPercentageException(value);
        }

        return new Percentage(value);
    }

    /// <summary>Creates a percentage from a ratio expressed between 0 and 1.</summary>
    public static Percentage FromRatio(decimal ratio) => Create(ratio * 100m);

    /// <summary>Applies this percentage to an amount.</summary>
    public decimal Apply(decimal amount) => amount * Value / 100m;

    /// <inheritdoc />
    public bool Equals(Percentage? other) => other is not null && Value == other.Value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Percentage);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + "%";
}
