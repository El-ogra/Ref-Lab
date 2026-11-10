using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// The critical limits of an analyte, above or below which a result must be
/// presented as critical.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the <c>Patients_backup.mdb</c> variant of the
/// <c>normalrange</c> table carries five extra columns that the shipped
/// <c>Patients.mdb</c> does not — <c>criticalrang</c> (memo),
/// <c>lowlcrit</c> (double), <c>highlcrit</c> (double), <c>criticalexe</c>
/// (text 20) and <c>criticalcomment</c> (memo). This value object reproduces
/// those five fields exactly.
/// </para>
/// <para>
/// In the shipped databases the critical thresholds survive only in the method
/// sheets (<c>Cache\*.rtf</c>), which state them in the form
/// <c>Hgb &lt; 5</c> or <c>&gt; 20 g/dL</c>. The comparison is therefore
/// <em>strict</em>: a value equal to the critical limit is not itself critical.
/// </para>
/// </remarks>
public sealed class CriticalInterval : IEquatable<CriticalInterval>
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
    private CriticalInterval()
    {
    }

    private CriticalInterval(
        decimal? lowCriticalLimit,
        decimal? highCriticalLimit,
        string? criticalRangeText,
        string? criticalExceptionText,
        string? criticalComment)
    {
        LowCriticalLimit = lowCriticalLimit;
        HighCriticalLimit = highCriticalLimit;
        CriticalRangeText = criticalRangeText;
        CriticalExceptionText = criticalExceptionText;
        CriticalComment = criticalComment;
    }

    /// <summary>The value below which a result is critical.</summary>
    public decimal? LowCriticalLimit { get; }

    /// <summary>The value above which a result is critical.</summary>
    public decimal? HighCriticalLimit { get; }

    /// <summary>The human-readable critical range, as printed on the report.</summary>
    public string? CriticalRangeText { get; }

    /// <summary>The short exception text raised for a critical result.</summary>
    public string? CriticalExceptionText { get; }

    /// <summary>The long comment attached to a critical result.</summary>
    public string? CriticalComment { get; }

    /// <summary>Indicates whether any critical limit is defined.</summary>
    public bool HasLimits => LowCriticalLimit.HasValue || HighCriticalLimit.HasValue;

    /// <summary>Creates a critical interval.</summary>
    public static CriticalInterval Create(
        decimal? lowCriticalLimit = null,
        decimal? highCriticalLimit = null,
        string? criticalRangeText = null,
        string? criticalExceptionText = null,
        string? criticalComment = null)
    {
        if (lowCriticalLimit.HasValue && highCriticalLimit.HasValue &&
            lowCriticalLimit.Value > highCriticalLimit.Value)
        {
            throw new InvalidCriticalIntervalException(lowCriticalLimit, highCriticalLimit);
        }

        return new CriticalInterval(
            lowCriticalLimit,
            highCriticalLimit,
            Normalize(criticalRangeText),
            Normalize(criticalExceptionText),
            Normalize(criticalComment));
    }

    /// <summary>An interval with no critical limits defined.</summary>
    public static CriticalInterval None { get; } = new(null, null, null, null, null);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <inheritdoc />
    public bool Equals(CriticalInterval? other) =>
        other is not null &&
        Nullable.Equals(LowCriticalLimit, other.LowCriticalLimit) &&
        Nullable.Equals(HighCriticalLimit, other.HighCriticalLimit) &&
        string.Equals(CriticalRangeText, other.CriticalRangeText, StringComparison.Ordinal) &&
        string.Equals(CriticalExceptionText, other.CriticalExceptionText, StringComparison.Ordinal) &&
        string.Equals(CriticalComment, other.CriticalComment, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as CriticalInterval);

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(LowCriticalLimit, HighCriticalLimit, CriticalRangeText, CriticalExceptionText, CriticalComment);
}
