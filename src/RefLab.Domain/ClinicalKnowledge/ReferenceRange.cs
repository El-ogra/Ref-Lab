using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// A reference range for one analyte, scoped by sex and by age.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>normalrange</c> carries 18 columns and 852 shipped rows
/// spanning 304 analytes. The distribution by sex was <c>Male</c> 422,
/// <c>Female</c> 421, <c>All</c> 5 and empty 4; by age unit
/// <c>Years</c> 708, <c>Days</c> 70, <c>Mons.</c> 65 and empty 9.
/// </para>
/// <para>
/// The selection rule among overlapping bands is <em>not established</em> in the
/// legacy system. Ref Lab applies a deterministic rule: the most specific band
/// wins — a band scoped to a single sex beats a band scoped to both sexes, and
/// among equally specific bands the narrowest age band wins.
/// </para>
/// </remarks>
public sealed class ReferenceRange
{
    private ReferenceRange()
    {
    }

    /// <summary>The range identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The analyte the range applies to.</summary>
    public int TestCode { get; private set; }

    /// <summary>The simplified name of the analyte, denormalised for reporting.</summary>
    public string TestSimpleName { get; private set; } = string.Empty;

    /// <summary>The clinical sub-group the range belongs to.</summary>
    public int SubGroupCode { get; private set; }

    /// <summary>The sexes the range applies to.</summary>
    public SexScope SexScope { get; private set; }

    /// <summary>The age window the range applies to.</summary>
    public AgeBand AgeBand { get; private set; } = null!;

    /// <summary>
    /// The pregnancy and smoking factor carried by every shipped range row.
    /// </summary>
    /// <remarks>
    /// The column is populated on all 852 rows but its role was never traced.
    /// It is carried for fidelity and is deliberately <em>not</em> used when
    /// selecting a range.
    /// </remarks>
    public decimal? PregnancySmokingFactor { get; private set; }

    /// <summary>The limits, texts and the human-readable range of this row.</summary>
    public ReferenceInterval Interval { get; private set; } = null!;

    /// <summary>The reference-range type code shared with the test catalogue.</summary>
    public string? ReferenceRangeType { get; private set; }

    /// <summary>Creates a reference range.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The test code is not positive or the analyte name is empty.
    /// </exception>
    public static ReferenceRange Create(
        int id,
        int testCode,
        string testSimpleName,
        int subGroupCode,
        SexScope sexScope,
        AgeBand ageBand,
        ReferenceInterval interval,
        string? referenceRangeType = null,
        decimal? pregnancySmokingFactor = null)
    {
        if (id <= 0)
        {
            throw new InvalidTestDefinitionException("A reference range identifier must be a positive integer.");
        }

        if (testCode <= 0)
        {
            throw new InvalidTestDefinitionException("A reference range must reference a positive test code.");
        }

        if (string.IsNullOrWhiteSpace(testSimpleName))
        {
            throw new InvalidTestDefinitionException("A reference range must carry the analyte name.");
        }

        ArgumentNullException.ThrowIfNull(ageBand);
        ArgumentNullException.ThrowIfNull(interval);

        return new ReferenceRange
        {
            Id = id,
            TestCode = testCode,
            TestSimpleName = testSimpleName.Trim(),
            SubGroupCode = subGroupCode,
            SexScope = sexScope,
            AgeBand = ageBand,
            Interval = interval,
            ReferenceRangeType = string.IsNullOrWhiteSpace(referenceRangeType) ? null : referenceRangeType.Trim(),
            PregnancySmokingFactor = pregnancySmokingFactor,
        };
    }

    /// <summary>
    /// Updates the limits and the texts of the range.
    /// </summary>
    /// <exception cref="InvalidReferenceIntervalException">
    /// The lower limit exceeds the upper limit.
    /// </exception>
    public void UpdateLimits(
        decimal? lowLimit = null,
        decimal? highLimit = null,
        string? lowExceptionText = null,
        string? highExceptionText = null,
        string? lowComment = null,
        string? highComment = null,
        string? rangeText = null)
    {
        var effectiveLow = lowLimit ?? Interval.LowLimit;
        var effectiveHigh = highLimit ?? Interval.HighLimit;

        if (effectiveLow.HasValue && effectiveHigh.HasValue && effectiveLow.Value > effectiveHigh.Value)
        {
            throw new InvalidReferenceIntervalException(effectiveLow, effectiveHigh);
        }

        Interval = ReferenceInterval.Create(
            effectiveLow,
            effectiveHigh,
            lowExceptionText ?? Interval.LowExceptionText,
            highExceptionText ?? Interval.HighExceptionText,
            lowComment ?? Interval.LowComment,
            highComment ?? Interval.HighComment,
            rangeText ?? Interval.RangeText,
            Interval.Unit,
            Interval.CriticalInterval);
    }

    /// <summary>
    /// Indicates whether this range applies to a patient of the given sex and age.
    /// </summary>
    public bool Matches(Sex sex, decimal age, AgeUnit ageUnit)
    {
        var scopeMatches = SexScope switch
        {
            SexScope.All => true,
            SexScope.Male => sex == Sex.Male,
            SexScope.Female => sex == Sex.Female,
            _ => false,
        };

        return scopeMatches && AgeBand.Contains(age, ageUnit);
    }

    /// <summary>
    /// Ranks how specific the sex scope of this range is. A higher value is
    /// more specific.
    /// </summary>
    public int SexSpecificity => SexScope == SexScope.All ? 0 : 1;

    /// <summary>
    /// The width of the age window, or <see langword="null"/> when the window
    /// is unbounded. A narrower window is more specific.
    /// </summary>
    public decimal? AgeSpan =>
        AgeBand.From.HasValue && AgeBand.To.HasValue ? AgeBand.To.Value - AgeBand.From.Value : null;

    /// <summary>
    /// Ranks how specific this range is, so that the selector can resolve
    /// overlapping bands deterministically. A higher value is more specific.
    /// </summary>
    public int Specificity => (SexScope == SexScope.All ? 0 : 1) + AgeSpanWeight();

    private int AgeSpanWeight()
    {
        if (!AgeBand.From.HasValue || !AgeBand.To.HasValue)
        {
            return 0;
        }

        var span = AgeBand.To.Value - AgeBand.From.Value;
        return span <= 0 ? 2 : 1;
    }
}
