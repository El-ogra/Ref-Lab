namespace RefLab.Domain.Enums;

/// <summary>
/// The kind of attendance event recorded against an operator.
/// </summary>
/// <remarks>
/// Legacy evidence: the operator table carries <c>startwork</c>,
/// <c>endwork</c>, <c>breakyorn</c> and <c>breaktime</c>, and the version 2
/// addition <c>loger</c> records attendance as an event log.
/// </remarks>
public enum AttendanceEventType
{
    /// <summary>The operator started working.</summary>
    StartedWork = 1,

    /// <summary>The operator ended working.</summary>
    EndedWork = 2,

    /// <summary>The operator started a break.</summary>
    StartedBreak = 3,

    /// <summary>The operator ended a break.</summary>
    EndedBreak = 4,
}
