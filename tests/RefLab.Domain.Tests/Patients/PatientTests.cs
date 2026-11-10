using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Patients;

namespace RefLab.Domain.Tests.Patients;

/// <summary>
/// The patient record. The legacy database has no patient master table, so
/// Ref Lab introduces a permanent patient identity.
/// </summary>
public class PatientTests
{
    [Fact]
    public void Create_stores_the_demographics()
    {
        var patient = Patient.Create("محمد أحمد", Sex.Male, age: 30m, ageUnit: AgeUnit.Years);

        Assert.Equal("محمد أحمد", patient.Name);
        Assert.Equal(Sex.Male, patient.Sex);
        Assert.Equal(30m, patient.Age);
        Assert.Equal(AgeUnit.Years, patient.AgeUnit);
        Assert.NotEqual(Guid.Empty, patient.Id.Value);
    }

    [Fact]
    public void Create_rejects_an_empty_name()
    {
        Assert.Throws<InvalidPatientOrderException>(() => Patient.Create("  ", Sex.Male));
    }

    [Fact]
    public void Create_rejects_a_negative_age()
    {
        Assert.Throws<InvalidPatientOrderException>(() => Patient.Create("Name", Sex.Male, age: -1m));
    }

    [Fact]
    public void The_date_of_birth_is_recorded()
    {
        var patient = Patient.Create("Name", Sex.Female);
        var birthDate = new DateOnly(1995, 4, 12);

        patient.SetBirthDate(birthDate);

        Assert.Equal(birthDate, patient.BirthDate);
    }

    [Fact]
    public void The_contact_details_are_normalised()
    {
        var patient = Patient.Create("Name", Sex.Male);

        patient.UpdateContactDetails(
            address: "  Cairo  ",
            phone: "0100000000",
            mobile: "0111111111",
            fax: "0222222222",
            email: "a@b.com");

        Assert.Equal("Cairo", patient.Address);
        Assert.Equal("0111111111", patient.Mobile);
        Assert.Equal("a@b.com", patient.Email);
    }

    [Fact]
    public void The_referring_physician_is_recorded()
    {
        var patient = Patient.Create("Name", Sex.Male);

        patient.SetReferringPhysician("  PW-12  ");

        Assert.Equal("PW-12", patient.ReferringPhysicianCode);
    }
}
