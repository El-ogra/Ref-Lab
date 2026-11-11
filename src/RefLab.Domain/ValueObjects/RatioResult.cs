using System.Globalization;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// A result that is expressed as the ratio of two measured results.
/// </summary>
/// <remarks>
/// <para>
/// Some analytes are reported as a ratio of two others rather than as a
/// measurement of their own: the albumin/globulin ratio and the AST/ALT ratio
/// are the two the laboratory reports. The legacy system stores such a row as
/// an ordinary result row whose analyte is the ratio itself, and the reference
/// system shows it on the result sheet as a row of its own with its own normal
/// range and its own high/low flag.
/// </para>
/// <para>
/// A plain <see cref="ResultValue"/> cannot express the provenance of such a
/// result: it carries only the text the operator entered. This value object
/// carries the two measurements the ratio was computed from, so that the
/// derivation is reproducible and the report can show how the ratio arose.
/// </para>
/// <para>
/// The computed ratio is stored as an ordinary <see cref="ResultValue"/> through
/// <see cref="ToResultValue"/>, so a ratio row is classified against its own
/// reference interval by the existing
/// <see cref="Rules.ResultClassificationPolicy"/> and needs no special
/// treatment in the persistence or reporting layers.
/// </para>
/// </remarks>
public sealed class RatioResult : IEquatable<RatioResult>
{
    /// <summary>
    /// The number of decimal places a derived ratio is rounded to.
    /// </summary>
    /// <remarks>
    /// The convention matches the other derived results of the laboratory: the
    /// derivation rules round their outcome to two decimal places, away from
    /// zero.
    /// </remarks>
    public const int Scale = 2;

    private RatioResult(
        string numeratorAnalyte,
        string denominatorAnalyte,
        decimal numerator,
        decimal denominator,
        string? unit)
    {
        NumeratorAnalyte = numeratorAnalyte;
        DenominatorAnalyte = denominatorAnalyte;
        Numerator = numerator;
        Denominator = denominator;
        Unit = unit;
    }

    /// <summary>The analyte the ratio is measured from.</summary>
    public string NumeratorAnalyte { get; }

    /// <summary>The analyte the ratio is measured against.</summary>
    public string DenominatorAnalyte { get; }

    /// <summary>The measured value of the numerator.</summary>
    public decimal Numerator { get; }

    /// <summary>The measured value of the denominator.</summary>
    public decimal Denominator { get; }

    /// <summary>
    /// The ratio as a pure number, rounded to <see cref="Scale"/> decimal
    /// places away from zero.
    /// </summary>
    public decimal Value => Math.Round(Numerator / Denominator, Scale, MidpointRounding.AwayFromZero);

    /// <summary>
    /// The ratio before rounding, kept so that the derivation stays
    /// reproducible.
    /// </summary>
    public decimal ExactValue => Numerator / Denominator;

    /// <summary>
    /// The unit the ratio is expressed in. A ratio is usually dimensionless, so
    /// this is usually <see langword="null"/>.
    /// </summary>
    public string? Unit { get; }

    /// <summary>The name the ratio is reported under.</summary>
    public string DisplayName => $"{NumeratorAnalyte}/{DenominatorAnalyte}";

    /// <summary>
    /// Creates a ratio from the two measurements it is computed from.
    /// </summary>
    /// <exception cref="InvalidResultValueException">
    /// An analyte name is blank, or the denominator is zero.
    /// </exception>
    public static RatioResult Create(
        string numeratorAnalyte,
        string denominatorAnalyte,
        decimal numerator,
        decimal denominator,
        string? unit = null)
    {
        ArgumentNullException.ThrowIfNull(numeratorAnalyte);
        ArgumentNullException.ThrowIfNull(denominatorAnalyte);

        if (string.IsNullOrWhiteSpace(numeratorAnalyte))
        {
            throw new InvalidResultValueException(numeratorAnalyte);
        }

        if (string.IsNullOrWhiteSpace(denominatorAnalyte))
        {
            throw new InvalidResultValueException(denominatorAnalyte);
        }

        if (denominator == 0m)
        {
            throw new InvalidResultValueException(
                "The denominator of a ratio must not be zero.");
        }

        var normalizedUnit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();

        return new RatioResult(
            numeratorAnalyte.Trim(),
            denominatorAnalyte.Trim(),
            numerator,
            denominator,
            normalizedUnit);
    }

    /// <summary>
    /// Projects the ratio onto the ordinary result value that is stored on the
    /// order row.
    /// </summary>
    public ResultValue ToResultValue() =>
        ResultValue.Create(
            Value.ToString("0.##", CultureInfo.InvariantCulture),
            Unit);

    /// <inheritdoc />
    public bool Equals(RatioResult? other) =>
        other is not null &&
        string.Equals(NumeratorAnalyte, other.NumeratorAnalyte, StringComparison.Ordinal) &&
        string.Equals(DenominatorAnalyte, other.DenominatorAnalyte, StringComparison.Ordinal) &&
        Numerator == other.Numerator &&
        Denominator == other.Denominator &&
        string.Equals(Unit, other.Unit, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as RatioResult);

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(NumeratorAnalyte, DenominatorAnalyte, Numerator, Denominator, Unit);

    /// <inheritdoc />
    public override string ToString() => $"{DisplayName} = {Value}";
}
