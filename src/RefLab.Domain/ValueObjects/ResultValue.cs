using System.Globalization;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// A measured result value together with the text that was actually entered.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>patienttest.RESULTT</c> is <c>Text (50)</c>, and the
/// profile result tables store their analytes as text as well. The legacy
/// classification compares the stored text against the numeric limits, so the
/// text form is the source of truth and the numeric form is a derived view.
/// </para>
/// <para>
/// A result is not necessarily numeric — a qualitative urine or culture result
/// such as <c>Positive</c> has no numeric value. In that case
/// <see cref="NumericValue"/> is <see langword="null"/> and the value cannot
/// be classified against a reference interval.
/// </para>
/// </remarks>
public sealed class ResultValue : IEquatable<ResultValue>
{
    private ResultValue(string text, decimal? numericValue, string? unit)
    {
        Text = text;
        NumericValue = numericValue;
        Unit = unit;
    }

    /// <summary>The raw text as entered by the operator.</summary>
    public string Text { get; }

    /// <summary>
    /// The numeric interpretation of <see cref="Text"/>, or <see langword="null"/>
    /// when the text is not a number.
    /// </summary>
    public decimal? NumericValue { get; }

    /// <summary>The measurement unit, when the analyte is quantified.</summary>
    public string? Unit { get; }

    /// <summary>Indicates whether the value can take part in a numeric comparison.</summary>
    public bool IsNumeric => NumericValue.HasValue;

    /// <summary>
    /// Creates a result value from the text entered by the operator.
    /// </summary>
    /// <exception cref="InvalidResultValueException">The text is empty.</exception>
    public static ResultValue Create(string text, string? unit = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        var trimmed = text.Trim();

        if (trimmed.Length == 0)
        {
            throw new InvalidResultValueException(text);
        }

        decimal? numeric = null;

        if (decimal.TryParse(trimmed, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
        {
            numeric = parsed;
        }

        var normalizedUnit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();

        return new ResultValue(trimmed, numeric, normalizedUnit);
    }

    /// <summary>
    /// Creates a purely numeric result value.
    /// </summary>
    public static ResultValue FromNumber(decimal number, string? unit = null)
    {
        var text = number.ToString(CultureInfo.InvariantCulture);
        return new ResultValue(text, number, string.IsNullOrWhiteSpace(unit) ? null : unit.Trim());
    }

    /// <inheritdoc />
    public bool Equals(ResultValue? other) =>
        other is not null &&
        string.Equals(Text, other.Text, StringComparison.Ordinal) &&
        Nullable.Equals(NumericValue, other.NumericValue) &&
        string.Equals(Unit, other.Unit, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ResultValue);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Text, NumericValue, Unit);

    /// <inheritdoc />
    public override string ToString() => Text;
}
