namespace RefLab.Domain.Enums;

/// <summary>
/// The sex recorded for a patient.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>patientinfo.patientsex</c> is <c>Text (7)</c> and stores
/// the literal values <c>Male</c> and <c>Female</c>. Reference ranges carry a
/// separate, wider scoping value which is modelled by <see cref="SexScope"/>.
/// </para>
/// <para>
/// The legacy column was free text and could therefore hold any value the
/// operator typed, including an unrecognised one. Ref Lab models the recorded
/// sex as a closed set and adds <see cref="Unknown"/> for the case where the
/// sex was not established — a case the legacy free-text column admitted
/// implicitly. <see cref="Male"/> and <see cref="Female"/> keep the values
/// they have always had, so every stored record keeps its meaning.
/// </para>
/// </remarks>
public enum Sex
{
    /// <summary>The patient's sex was established as male.</summary>
    Male = 1,

    /// <summary>The patient's sex was established as female.</summary>
    Female = 2,

    /// <summary>
    /// The patient's sex was not established.
    /// </summary>
    /// <remarks>
    /// An unknown sex is a recorded fact, not a missing value: it says that
    /// the laboratory could not establish the sex, and it must never be
    /// silently read as either of the other two. A reference range that is
    /// scoped to a single sex therefore does not apply to an unknown-sex
    /// patient, and a derivation that depends on the sex cannot be performed.
    /// </remarks>
    Unknown = 3,
}
