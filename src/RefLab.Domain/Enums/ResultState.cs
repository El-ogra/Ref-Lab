namespace RefLab.Domain.Enums;

/// <summary>
/// The lifecycle state of a single requested test.
/// </summary>
/// <remarks>
/// This is a <em>derived projection</em> of the legacy boolean flags on
/// <c>patienttest</c>, which are preserved verbatim on
/// <c>RefLab.Domain.Patients.OrderTest</c>:
/// <c>resultfinsh</c>, <c>resultreview</c>, <c>resultprint</c> and <c>give</c>.
/// The documented shortcut keys are F9 (save), F8 (review) and F12 (print).
/// </remarks>
public enum ResultState
{
    /// <summary>The result has not been entered yet.</summary>
    NotEntered = 1,

    /// <summary>The result has been entered and saved.</summary>
    Entered = 2,

    /// <summary>The result has been confirmed as reviewed (F8).</summary>
    Reviewed = 3,

    /// <summary>The result has been printed (F12).</summary>
    Printed = 4,

    /// <summary>The result has been handed over to the patient.</summary>
    Delivered = 5,
}
