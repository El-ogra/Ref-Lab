using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;

namespace RefLab.Domain.Rules;

/// <summary>
/// Selects the reference range that applies to an analyte for a given patient.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: each range row is scoped by sex, by an age window and by
/// an age unit, and the selection rule among overlapping bands is
/// <em>not established</em> in the legacy system.
/// </para>
/// <para>
/// Ref Lab applies a deterministic rule so that the same patient always receives
/// the same range: a band scoped to a single sex wins over a band scoped to
/// both sexes; among equally specific bands the narrowest age window wins; and
/// the lowest range identifier breaks any remaining tie.
/// </para>
/// </remarks>
public static class ReferenceRangeSelector
{
    /// <summary>
    /// Selects the applicable reference range.
    /// </summary>
    /// <returns>
    /// The most specific matching range, or <see langword="null"/> when no range
    /// applies.
    /// </returns>
    public static ReferenceRange? Select(
        IEnumerable<ReferenceRange> ranges,
        int testCode,
        Sex sex,
        decimal age,
        AgeUnit ageUnit)
    {
        ArgumentNullException.ThrowIfNull(ranges);

        return ranges
            .Where(range => range.TestCode == testCode)
            .Where(range => range.Matches(sex, age, ageUnit))
            .OrderByDescending(range => range.SexSpecificity)
            .ThenBy(range => range.AgeSpan ?? decimal.MaxValue)
            .ThenBy(range => range.Id)
            .FirstOrDefault();
    }

    /// <summary>
    /// Selects every range that applies, ordered from the most specific to the
    /// least specific.
    /// </summary>
    public static IReadOnlyList<ReferenceRange> SelectAll(
        IEnumerable<ReferenceRange> ranges,
        int testCode,
        Sex sex,
        decimal age,
        AgeUnit ageUnit)
    {
        ArgumentNullException.ThrowIfNull(ranges);

        return ranges
            .Where(range => range.TestCode == testCode)
            .Where(range => range.Matches(sex, age, ageUnit))
            .OrderByDescending(range => range.SexSpecificity)
            .ThenBy(range => range.AgeSpan ?? decimal.MaxValue)
            .ThenBy(range => range.Id)
            .ToArray();
    }
}
