namespace RefLab.Domain.Enums;

/// <summary>
/// The lifecycle status of a patient order (a laboratory visit).
/// </summary>
/// <remarks>
/// This is a <em>derived projection</em> of the order's requested tests, not a
/// separately stored legacy field. The legacy system persisted a single
/// <c>patientinfo.completes</c> flag of type <c>Text (1)</c> and expressed the
/// remaining states through the per-test boolean flags. Modelling the status as
/// a projection keeps the legacy semantics intact while giving the presentation
/// layer a single value to bind to.
/// </remarks>
public enum RecordStatus
{
    /// <summary>No test has been requested yet.</summary>
    Open = 1,

    /// <summary>Tests have been requested but the order is not yet completed.</summary>
    InProgress = 2,

    /// <summary>Every requested test has been entered and reviewed.</summary>
    Completed = 3,

    /// <summary>Every requested test has been printed and handed over.</summary>
    Delivered = 4,
}
