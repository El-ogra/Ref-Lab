using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;

namespace RefLab.Application.Features.Auditing;

/// <summary>
/// A print audit record as presented to the application layer.
/// </summary>
public sealed record PrintAuditDto(
    Guid Id,
    Guid OrderTestId,
    string Operator,
    DateTimeOffset PrintedAt,
    long SequenceNumber,
    ReportCategory Category)
{
    /// <summary>Projects a print audit record onto its DTO.</summary>
    public static PrintAuditDto FromDomain(PrintAudit audit) =>
        new(audit.Id, audit.OrderTestId, audit.Operator, audit.PrintedAt, audit.SequenceNumber, audit.Category);
}

/// <summary>
/// An attendance entry as presented to the application layer.
/// </summary>
public sealed record AttendanceEntryDto(
    Guid Id,
    int OperatorId,
    string OperatorName,
    AttendanceEventType EventType,
    DateTimeOffset OccurredAt)
{
    /// <summary>Projects an attendance entry onto its DTO.</summary>
    public static AttendanceEntryDto FromDomain(AttendanceEntry entry) =>
        new(entry.Id, entry.OperatorId, entry.OperatorName, entry.EventType, entry.OccurredAt);
}
