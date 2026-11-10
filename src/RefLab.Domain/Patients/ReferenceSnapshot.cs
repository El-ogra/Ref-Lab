using RefLab.Domain.Enums;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Patients;

/// <summary>
/// The reference interval that was in force for an analyte when a test was
/// ordered.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>patienttest</c> carries a frozen copy of the reference
/// range on the order row itself — <c>normalrang</c>, <c>lowlimit</c>,
/// <c>highlimit</c>, <c>nolowexe</c> and <c>nohighexe</c> — so that the
/// printed report shows the policy that applied at the time of measurement and
/// is unaffected by a later change to the range table.
/// </para>
/// <para>
/// 112 of the 222 shipped <c>patienttest</c> rows carry a frozen
/// <c>normalrang</c>.
/// </para>
/// </remarks>
public sealed class ReferenceSnapshot
{
    private ReferenceSnapshot()
    {
    }

    /// <summary>The analyte the snapshot belongs to.</summary>
    public int TestCode { get; private set; }

    /// <summary>The simplified analyte name, denormalised for reporting.</summary>
    public string TestSimpleName { get; private set; } = string.Empty;

    /// <summary>The sexes the snapshot was scoped to.</summary>
    public SexScope SexScope { get; private set; }

    /// <summary>The age band the snapshot was scoped to.</summary>
    public AgeBand AgeBand { get; private set; } = null!;

    /// <summary>The frozen limits, texts and human-readable range.</summary>
    public ReferenceInterval Interval { get; private set; } = null!;

    /// <summary>The moment the snapshot was taken.</summary>
    public DateTimeOffset CapturedAt { get; private set; }

    /// <summary>Creates a reference snapshot.</summary>
    public static ReferenceSnapshot Capture(
        int testCode,
        string testSimpleName,
        SexScope sexScope,
        AgeBand ageBand,
        ReferenceInterval interval,
        DateTimeOffset capturedAt)
    {
        ArgumentNullException.ThrowIfNull(ageBand);
        ArgumentNullException.ThrowIfNull(interval);

        return new ReferenceSnapshot
        {
            TestCode = testCode,
            TestSimpleName = testSimpleName.Trim(),
            SexScope = sexScope,
            AgeBand = ageBand,
            Interval = interval,
            CapturedAt = capturedAt,
        };
    }

    /// <summary>
    /// Creates a snapshot from a catalogue reference range.
    /// </summary>
    public static ReferenceSnapshot Capture(
        ClinicalKnowledge.ReferenceRange range,
        DateTimeOffset capturedAt)
    {
        ArgumentNullException.ThrowIfNull(range);

        return Capture(
            range.TestCode,
            range.TestSimpleName,
            range.SexScope,
            range.AgeBand,
            range.Interval,
            capturedAt);
    }
}
