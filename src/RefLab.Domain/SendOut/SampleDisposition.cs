using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.SendOut;

/// <summary>
/// A record that a sample was split or withdrawn.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the vendor guide documents the capability
/// <c>&#x0641;&#x0635;&#x0644; &#x0648;&#x0633;&#x062d;&#x0628; &#x0627;&#x0644;&#x0639;&#x064a;&#x0646;&#x0627;&#x062a;</c>
/// (split and withdraw samples) recorded against the microbiology tables
/// <c>cultures</c>, <c>cultureanti</c> and <c>sample</c>. The structure of the
/// <c>sample</c> table was never extracted, so only the two documented actions
/// and the audit fields are asserted.
/// </para>
/// </remarks>
public sealed class SampleDisposition
{
    private SampleDisposition()
    {
    }

    /// <summary>The identity of the disposition record.</summary>
    public Guid Id { get; private set; }

    /// <summary>The order test whose sample was affected.</summary>
    public Guid OrderTestId { get; private set; }

    /// <summary>The action that was performed.</summary>
    public SampleDispositionAction Action { get; private set; }

    /// <summary>The laboratory the withdrawn portion was sent to.</summary>
    public string? DestinationLabName { get; private set; }

    /// <summary>The moment the disposition was recorded.</summary>
    public DateTimeOffset RecordedAt { get; private set; }

    /// <summary>The operator who recorded the disposition.</summary>
    public string RecordedBy { get; private set; } = string.Empty;

    /// <summary>Creates a sample disposition record.</summary>
    /// <exception cref="InvalidSampleDispositionException">
    /// The order test identity is empty or the operator is not named.
    /// </exception>
    public static SampleDisposition Create(
        Guid orderTestId,
        SampleDispositionAction action,
        DateTimeOffset recordedAt,
        string recordedBy,
        string? destinationLabName = null)
    {
        if (orderTestId == Guid.Empty)
        {
            throw new InvalidSampleDispositionException(
                "A sample disposition must reference an order test.");
        }

        if (string.IsNullOrWhiteSpace(recordedBy))
        {
            throw new InvalidSampleDispositionException(
                "A sample disposition must name the operator who recorded it.");
        }

        return new SampleDisposition
        {
            Id = Guid.NewGuid(),
            OrderTestId = orderTestId,
            Action = action,
            DestinationLabName = string.IsNullOrWhiteSpace(destinationLabName)
                ? null
                : destinationLabName.Trim(),
            RecordedAt = recordedAt,
            RecordedBy = recordedBy.Trim(),
        };
    }
}
