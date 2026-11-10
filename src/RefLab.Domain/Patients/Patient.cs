using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Patients;

/// <summary>
/// A patient of the laboratory.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the legacy database has <em>no patient master table</em>.
/// The table named <c>patients</c> holds operators, and patient identity is
/// carried only by the person key embedded in the composite order identifier.
/// Ref Lab introduces a permanent patient identity and keeps every order's
/// legacy identifier for traceability.
/// </para>
/// <para>
/// The demographic fields reproduce <c>patientinfo</c>: <c>patientnamear</c>,
/// <c>patientsex</c>, <c>patientage</c> with <c>patientageunit</c>, and the
/// version 2 addition <c>PBirthDay</c>.
/// </para>
/// </remarks>
public sealed class Patient
{
    private Patient()
    {
    }

    /// <summary>The permanent identity of the patient.</summary>
    public PatientId Id { get; private set; }

    /// <summary>The patient's name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The patient's sex.</summary>
    public Sex Sex { get; private set; }

    /// <summary>The patient's age, as recorded on the order.</summary>
    public decimal? Age { get; private set; }

    /// <summary>The unit in which <see cref="Age"/> is expressed.</summary>
    public AgeUnit? AgeUnit { get; private set; }

    /// <summary>The patient's date of birth, when known.</summary>
    public DateOnly? BirthDate { get; private set; }

    /// <summary>The patient's address.</summary>
    public string? Address { get; private set; }

    /// <summary>The patient's telephone number.</summary>
    public string? Phone { get; private set; }

    /// <summary>The patient's mobile number.</summary>
    public string? Mobile { get; private set; }

    /// <summary>The patient's fax number.</summary>
    public string? Fax { get; private set; }

    /// <summary>The patient's email address.</summary>
    public string? Email { get; private set; }

    /// <summary>The code of the physician the patient was referred by.</summary>
    public string? ReferringPhysicianCode { get; private set; }

    /// <summary>The reception note.</summary>
    public string? ReceptionNote { get; private set; }

    /// <summary>The first additional-information field.</summary>
    public string? AdditionalInfo { get; private set; }

    /// <summary>The second additional-information field, added in version 2.</summary>
    public string? AdditionalInfo2 { get; private set; }

    /// <summary>Creates a patient.</summary>
    /// <exception cref="InvalidPatientOrderException">The name is empty.</exception>
    public static Patient Create(
        string name,
        Sex sex,
        decimal? age = null,
        AgeUnit? ageUnit = null,
        DateOnly? birthDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidPatientOrderException("A patient must have a name.");
        }

        if (age is < 0)
        {
            throw new InvalidPatientOrderException("A patient's age cannot be negative.");
        }

        return new Patient
        {
            Id = PatientId.New(),
            Name = name.Trim(),
            Sex = sex,
            Age = age,
            AgeUnit = ageUnit,
            BirthDate = birthDate,
        };
    }

    /// <summary>Renames the patient.</summary>
    /// <exception cref="InvalidPatientOrderException">The name is empty.</exception>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidPatientOrderException("A patient must have a name.");
        }

        Name = name.Trim();
    }

    /// <summary>Updates the patient's contact details.</summary>
    public void UpdateContactDetails(
        string? address = null,
        string? phone = null,
        string? mobile = null,
        string? fax = null,
        string? email = null)
    {
        Address = Normalize(address);
        Phone = Normalize(phone);
        Mobile = Normalize(mobile);
        Fax = Normalize(fax);
        Email = Normalize(email);
    }

    /// <summary>Records the patient's date of birth.</summary>
    public void SetBirthDate(DateOnly? birthDate) => BirthDate = birthDate;

    /// <summary>Records the patient's age as captured on the order.</summary>
    public void SetAge(decimal? age, AgeUnit? ageUnit)
    {
        if (age is < 0)
        {
            throw new InvalidPatientOrderException("A patient's age cannot be negative.");
        }

        Age = age;
        AgeUnit = ageUnit;
    }

    /// <summary>Records the code of the referring physician.</summary>
    public void SetReferringPhysician(string? referringPhysicianCode) =>
        ReferringPhysicianCode = Normalize(referringPhysicianCode);

    /// <summary>Records the additional information fields.</summary>
    public void SetAdditionalInfo(string? additionalInfo, string? additionalInfo2)
    {
        AdditionalInfo = Normalize(additionalInfo);
        AdditionalInfo2 = Normalize(additionalInfo2);
    }

    /// <summary>Records the reception note.</summary>
    public void SetReceptionNote(string? receptionNote) => ReceptionNote = Normalize(receptionNote);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
