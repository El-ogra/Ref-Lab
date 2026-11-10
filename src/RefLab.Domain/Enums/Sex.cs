namespace RefLab.Domain.Enums;

/// <summary>
/// The sex recorded for a patient.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>patientinfo.patientsex</c> is <c>Text (7)</c> and stores
/// the literal values <c>Male</c> and <c>Female</c>. Reference ranges carry a
/// separate, wider scoping value which is modelled by <see cref="SexScope"/>.
/// </remarks>
public enum Sex
{
    Male = 1,
    Female = 2,
}
