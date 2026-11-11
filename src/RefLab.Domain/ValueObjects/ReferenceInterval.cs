using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// The reference interval of an analyte, together with the texts raised when a
/// result falls outside it.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>normalrange</c> carries <c>lowlimit</c> and
/// <c>highlimit</c> (both <c>Double</c>), the short exception texts
/// <c>nolowexe</c> and <c>nohighexe</c> (both <c>Text (20)</c>), the long
/// comments <c>lowcomment</c> and <c>highcomment</c> (both memo), the
/// human-readable sentence <c>normalrang</c> and the unit <c>testunit</c>.
/// </para>
/// <para>
/// The reference range is printed as a human-readable sentence — for example
/// <c>Adult 3.8 - 5.2 g/dL</c> or <c>Male Up To 45 U/L</c> — and never as bare
/// numbers, which is why <see cref="RangeText"/> is a first-class field.
/// </para>
/// <para>
/// The comparison is <em>inclusive</em>: a value equal to a limit lies inside
/// the interval and is therefore not abnormal.
/// </para>
/// </remarks>
public sealed class ReferenceInterval : IEquatable<ReferenceInterval>
{
    /// <summary>
    /// Initialises an empty interval.
    /// </summary>
    /// <remarks>
    /// The persistence layer needs a constructor it can call without arguments
    /// before it fills the properties in. The public factory
    /// <see cref="Create"/> remains the only way to build a validated
    /// instance.
    /// </remarks>
    private ReferenceInterval()
    {
        CriticalInterval = RefLab.Domain.ValueObjects.CriticalInterval.None;
    }

    private ReferenceInterval(
        decimal? lowLimit,
        decimal? highLimit,
        string? lowExceptionText,
        string? highExceptionText,
        string? lowComment,
        string? highComment,
        string? rangeText,
        string? unit,
        CriticalInterval criticalInterval)
    {
        LowLimit = lowLimit;
        HighLimit = highLimit;
        LowExceptionText = lowExceptionText;
        HighExceptionText = highExceptionText;
        LowComment = lowComment;
        HighComment = highComment;
        RangeText = rangeText;
        Unit = unit;
        CriticalInterval = criticalInterval;
    }

    /// <summary>The lower bound of the reference interval.</summary>
    public decimal? LowLimit { get; }

    /// <summary>The upper bound of the reference interval.</summary>
    public decimal? HighLimit { get; }

    /// <summary>The short text raised when a result is below <see cref="LowLimit"/>.</summary>
    public string? LowExceptionText { get; }

    /// <summary>The short text raised when a result is above <see cref="HighLimit"/>.</summary>
    public string? HighExceptionText { get; }

    /// <summary>The long comment raised when a result is below <see cref="LowLimit"/>.</summary>
    public string? LowComment { get; }

    /// <summary>The long comment raised when a result is above <see cref="HighLimit"/>.</summary>
    public string? HighComment { get; }

    /// <summary>The human-readable reference range as printed on the report.</summary>
    public string? RangeText { get; }

    /// <summary>The measurement unit the limits are expressed in.</summary>
    public string? Unit { get; }

    /// <summary>The critical limits applicable to this analyte, when defined.</summary>
    public CriticalInterval CriticalInterval { get; private set; }

    /// <summary>Indicates whether any normal limit is defined.</summary>
    public bool HasLimits => LowLimit.HasValue || HighLimit.HasValue;

    /// <summary>Creates a reference interval.</summary>
    /// <exception cref="InvalidReferenceIntervalException">
    /// The lower limit exceeds the upper limit.
    /// </exception>
    public static ReferenceInterval Create(
        decimal? lowLimit = null,
        decimal? highLimit = null,
        string? lowExceptionText = null,
        string? highExceptionText = null,
        string? lowComment = null,
        string? highComment = null,
        string? rangeText = null,
        string? unit = null,
        CriticalInterval? criticalInterval = null)
    {
        if (lowLimit.HasValue && highLimit.HasValue && lowLimit.Value > highLimit.Value)
        {
            throw new InvalidReferenceIntervalException(lowLimit, highLimit);
        }

        return new ReferenceInterval(
            lowLimit,
            highLimit,
            Normalize(lowExceptionText),
            Normalize(highExceptionText),
            Normalize(lowComment),
            Normalize(highComment),
            Normalize(rangeText),
            Normalize(unit),
            criticalInterval ?? CriticalInterval.None);
    }

    /// <summary>
    /// Creates a copy of this interval.
    /// </summary>
    /// <remarks>
    /// A value object must never be shared between two aggregates. The
    /// reference interval is captured onto the order row when the test is
    /// ordered, and two orders may capture the same catalogue range. Copying
    /// the interval keeps the two aggregates independent.
    /// </remarks>
    /// <returns>An equal but independent interval.</returns>
    public ReferenceInterval Copy() => Create(
        LowLimit,
        HighLimit,
        LowExceptionText,
        HighExceptionText,
        LowComment,
        HighComment,
        RangeText,
        Unit,
        CriticalInterval.Copy());

    /// <summary>Returns the short exception text that applies to a classification.</summary>
    public string? ExceptionTextFor(ResultClassification classification) => classification switch
    {
        ResultClassification.Low => LowExceptionText,
        ResultClassification.High => HighExceptionText,
        ResultClassification.Critical => CriticalInterval.CriticalExceptionText,
        _ => null,
    };

    /// <summary>Returns the long comment that applies to a classification.</summary>
    public string? CommentFor(ResultClassification classification) => classification switch
    {
        ResultClassification.Low => LowComment,
        ResultClassification.High => HighComment,
        ResultClassification.Critical => CriticalInterval.CriticalComment,
        _ => null,
    };

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <inheritdoc />
    public bool Equals(ReferenceInterval? other) =>
        other is not null &&
        Nullable.Equals(LowLimit, other.LowLimit) &&
        Nullable.Equals(HighLimit, other.HighLimit) &&
        string.Equals(LowExceptionText, other.LowExceptionText, StringComparison.Ordinal) &&
        string.Equals(HighExceptionText, other.HighExceptionText, StringComparison.Ordinal) &&
        string.Equals(LowComment, other.LowComment, StringComparison.Ordinal) &&
        string.Equals(HighComment, other.HighComment, StringComparison.Ordinal) &&
        string.Equals(RangeText, other.RangeText, StringComparison.Ordinal) &&
        string.Equals(Unit, other.Unit, StringComparison.Ordinal) &&
        CriticalInterval.Equals(other.CriticalInterval);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ReferenceInterval);

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(LowLimit, HighLimit, LowExceptionText, HighExceptionText, LowComment, HighComment, RangeText, Unit);
}
