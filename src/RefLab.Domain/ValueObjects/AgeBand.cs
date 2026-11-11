using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// An age window, expressed in a given unit.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>normalrange.firstold</c> and <c>normalrange.secondold</c>
/// are nullable <c>Double</c> columns, paired with <c>normalrange.ageunit</c>.
/// The observed age windows span 0-120 years, 1-120 days and 0-24 months.
/// A band with no upper bound is expressed with a <see langword="null"/>
/// <see cref="To"/>.
/// </remarks>
public sealed class AgeBand : IEquatable<AgeBand>
{
    private AgeBand(decimal? from, decimal? to, AgeUnit unit)
    {
        From = from;
        To = to;
        Unit = unit;
    }

    /// <summary>The inclusive lower bound of the band.</summary>
    public decimal? From { get; }

    /// <summary>The inclusive upper bound of the band, or <see langword="null"/> when unbounded.</summary>
    public decimal? To { get; }

    /// <summary>The unit in which both bounds are expressed.</summary>
    public AgeUnit Unit { get; }

    /// <summary>Creates an age band.</summary>
    /// <exception cref="InvalidAgeBandException">
    /// The lower bound exceeds the upper bound, or a bound is negative.
    /// </exception>
    public static AgeBand Create(decimal? from, decimal? to, AgeUnit unit)
    {
        if (from is < 0)
        {
            throw new InvalidAgeBandException(from, to, "The lower age bound cannot be negative.");
        }

        if (to is < 0)
        {
            throw new InvalidAgeBandException(from, to, "The upper age bound cannot be negative.");
        }

        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            throw new InvalidAgeBandException(from, to, "The lower age bound cannot exceed the upper age bound.");
        }

        return new AgeBand(from, to, unit);
    }

    /// <summary>
    /// Creates a copy of this age band.
    /// </summary>
    /// <remarks>
    /// A value object must never be shared between two aggregates. The band is
    /// captured onto the order row when a test is ordered, and two orders may
    /// capture the same catalogue range. Copying the band keeps the two
    /// aggregates independent.
    /// </remarks>
    /// <returns>An equal but independent band.</returns>
    public AgeBand Copy() => Create(From, To, Unit);

    /// <summary>
    /// Determines whether an age expressed in <paramref name="unit"/> falls inside this band.
    /// </summary>
    public bool Contains(decimal age, AgeUnit unit)
    {
        if (unit != Unit)
        {
            return false;
        }

        if (From.HasValue && age < From.Value)
        {
            return false;
        }

        return !To.HasValue || age <= To.Value;
    }

    /// <inheritdoc />
    public bool Equals(AgeBand? other) =>
        other is not null &&
        Nullable.Equals(From, other.From) &&
        Nullable.Equals(To, other.To) &&
        Unit == other.Unit;

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as AgeBand);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(From, To, Unit);

    /// <inheritdoc />
    public override string ToString() =>
        $"{From?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "-"}..{To?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "-"} {Unit}";
}
