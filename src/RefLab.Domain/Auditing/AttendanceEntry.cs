using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Auditing;

/// <summary>
/// A single attendance event of an operator.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the operator table carries the current attendance state
/// (<c>startwork</c>, <c>endwork</c>, <c>breakyorn</c>, <c>breaktime</c>) and
/// the version 2 addition <c>loger</c> records attendance as an event log.
/// The vendor guide describes the capability as
/// <c>&#x062a;&#x0633;&#x062c;&#x064a;&#x0644; &#x0627;&#x0644;&#x062d;&#x0636;&#x0648;&#x0631; &#x0648;&#x0627;&#x0644;&#x0627;&#x0646;&#x0635;&#x0631;&#x0627;&#x0641;</c>.
/// </para>
/// <para>
/// The column structure of the <c>loger</c> table was never extracted, so this
/// entity asserts only the four events the operator table evidences.
/// </para>
/// </remarks>
public sealed class AttendanceEntry
{
    private AttendanceEntry()
    {
    }

    /// <summary>The identity of the entry.</summary>
    public Guid Id { get; private set; }

    /// <summary>The operator the event belongs to.</summary>
    public int OperatorId { get; private set; }

    /// <summary>The operator name at the time of the event.</summary>
    public string OperatorName { get; private set; } = string.Empty;

    /// <summary>The kind of event.</summary>
    public AttendanceEventType EventType { get; private set; }

    /// <summary>The moment the event occurred.</summary>
    public DateTimeOffset OccurredAt { get; private set; }

    /// <summary>Creates an attendance entry.</summary>
    /// <exception cref="InvalidOperatorException">
    /// The operator identifier is not positive or the name is empty.
    /// </exception>
    public static AttendanceEntry Create(
        int operatorId,
        string operatorName,
        AttendanceEventType eventType,
        DateTimeOffset occurredAt)
    {
        if (operatorId <= 0)
        {
            throw new InvalidOperatorException("An attendance entry must reference a positive operator identifier.");
        }

        if (string.IsNullOrWhiteSpace(operatorName))
        {
            throw new InvalidOperatorException("An attendance entry must name the operator.");
        }

        return new AttendanceEntry
        {
            Id = Guid.NewGuid(),
            OperatorId = operatorId,
            OperatorName = operatorName.Trim(),
            EventType = eventType,
            OccurredAt = occurredAt,
        };
    }
}
