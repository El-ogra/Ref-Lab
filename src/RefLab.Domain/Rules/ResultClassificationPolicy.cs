using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Rules;

/// <summary>
/// Classifies an entered result against the reference interval that was frozen
/// onto the order row.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the classification is three-valued and ordered — normal,
/// then abnormal, then <em>critical</em>. A value that breaches a critical limit
/// is presented as critical whatever its abnormal classification would be.
/// The legacy system compares the entered value against the limits frozen on
/// the test row (<c>lowlimit</c>, <c>highlimit</c>), not against the live range
/// table.
/// </para>
/// <para>
/// The comparison against the normal interval is <em>inclusive</em>: a value
/// equal to a limit lies inside the interval. The comparison against a critical
/// limit is <em>strict</em>, because the method sheets state the thresholds in
/// the form <c>Hgb &lt; 5</c> and <c>&gt; 20 g/dL</c>.
/// </para>
/// <para>
/// A classification is impossible — and <see langword="null"/> is returned —
/// when the result is not numeric, when no reference snapshot was frozen, or
/// when the snapshot defines no limits at all.
/// </para>
/// </remarks>
public static class ResultClassificationPolicy
{
    /// <summary>
    /// Classifies a result value against a frozen reference snapshot.
    /// </summary>
    /// <returns>
    /// The classification, or <see langword="null"/> when no classification can
    /// be made.
    /// </returns>
    public static ResultClassification? Classify(ResultValue? result, ReferenceSnapshot? snapshot)
    {
        if (result is null || !result.IsNumeric || snapshot is null)
        {
            return null;
        }

        var value = result.NumericValue!.Value;
        var interval = snapshot.Interval;
        var critical = interval.CriticalInterval;

        if (critical.LowCriticalLimit.HasValue && value < critical.LowCriticalLimit.Value)
        {
            return ResultClassification.Critical;
        }

        if (critical.HighCriticalLimit.HasValue && value > critical.HighCriticalLimit.Value)
        {
            return ResultClassification.Critical;
        }

        if (!interval.HasLimits)
        {
            return null;
        }

        if (interval.LowLimit.HasValue && value < interval.LowLimit.Value)
        {
            return ResultClassification.Low;
        }

        if (interval.HighLimit.HasValue && value > interval.HighLimit.Value)
        {
            return ResultClassification.High;
        }

        return ResultClassification.Normal;
    }
}
