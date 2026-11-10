using RefLab.Domain.Billing;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.Microbiology;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Fakes;

/// <summary>
/// Factory helpers that build valid domain objects for the test suite.
/// </summary>
public static class TestData
{
    /// <summary>The instant every test starts from.</summary>
    public static readonly DateTimeOffset Now = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    /// <summary>Creates an operator account.</summary>
    public static Operator CreateOperator(
        int id = 1,
        string name = "lab.tech",
        string passwordHash = "hash:Password2026!",
        bool isAdministrator = false)
    {
        var @operator = Operator.Create(id, name, passwordHash, "Technician");
        @operator.SetAdministrator(isAdministrator);

        return @operator;
    }

    /// <summary>Creates a patient.</summary>
    public static Patient CreatePatient(
        string name = "محمد أحمد",
        Sex sex = Sex.Male,
        decimal? age = 30m,
        AgeUnit? ageUnit = AgeUnit.Years,
        string? mobile = "0111111111")
    {
        var patient = Patient.Create(name, sex, age, ageUnit);
        patient.UpdateContactDetails(mobile: mobile);
        return patient;
    }

    /// <summary>Creates a patient order.</summary>
    public static PatientOrder CreateOrder(
        Patient patient,
        string currencyCode = "EGP",
        DateTimeOffset? requestDate = null)
    {
        return PatientOrder.Create(
            patient.Id,
            requestDate ?? Now,
            currencyCode,
            "lab.tech");
    }

    /// <summary>Creates a requested test.</summary>
    public static OrderTest CreateOrderTest(
        PatientOrder order,
        int testCode = 56,
        string simpleName = "Hgb",
        int subGroupCode = 11)
    {
        var orderTest = OrderTest.Create(
            order.Id,
            testCode,
            simpleName,
            subGroupCode,
            Now,
            "lab.tech");

        order.AddTest(orderTest);

        return orderTest;
    }

    /// <summary>Creates a requested test with an entered and reviewed result.</summary>
    public static OrderTest CreateReviewedOrderTest(
        PatientOrder order,
        decimal value = 13.4m,
        decimal? low = 12m,
        decimal? high = 16m)
    {
        var orderTest = CreateOrderTest(order);

        orderTest.EnterResult(ResultValue.FromNumber(value), Now, "lab.tech");
        orderTest.ConfirmReviewed(Now, "lab.tech");

        _ = low;
        _ = high;

        return orderTest;
    }

    /// <summary>Creates a test definition.</summary>
    public static TestDefinition CreateTestDefinition(
        int code = 56,
        string name = "Complete Blood Count",
        string simpleName = "Hgb",
        int subGroupCode = 11,
        decimal patientPrice = 150m)
    {
        var definition = TestDefinition.Create(code, name, simpleName, subGroupCode, arrangementNumber: 1);
        definition.UpdatePrices(patientPrice, 100m, 200m);
        return definition;
    }

    /// <summary>Creates a reference range.</summary>
    public static ReferenceRange CreateReferenceRange(
        int id = 1,
        int testCode = 56,
        SexScope sexScope = SexScope.All,
        decimal? low = 12m,
        decimal? high = 16m)
    {
        return ReferenceRange.Create(
            id,
            testCode,
            "Hgb",
            11,
            sexScope,
            AgeBand.Create(0m, 120m, AgeUnit.Years),
            ReferenceInterval.Create(lowLimit: low, highLimit: high));
    }

    /// <summary>Creates the blood-count constant group of the legacy system.</summary>
    public static ClinicalConstant CbcConstants() =>
        ClinicalConstant.Create("CBCCONS", "3.3", "6.25", "6.75", "7.5", "8.25");

    /// <summary>Creates the prothrombin-time constant group of the legacy system.</summary>
    public static ClinicalConstant PtConstants() =>
        ClinicalConstant.Create("PTCONS", "12", "35", "1.33", "2CVJW");

    /// <summary>Creates a calibration point of the prothrombin control curve.</summary>
    public static ProthrombinControlCalibration ControlPoint(
        int id,
        decimal controlTime,
        decimal concentration) =>
        ProthrombinControlCalibration.Create(id, controlTime, concentration);

    /// <summary>Creates the laboratory settings.</summary>
    public static LaboratorySettings Settings() => LaboratorySettings.Create();

    /// <summary>Creates a referring party.</summary>
    public static ReferringParty CreateReferringParty(
        int id = 19,
        string code = "CL-01",
        string name = "Clinic One")
    {
        var party = ReferringParty.Create(id, code, name);
        party.SetCommissionRate(Percentage.Create(10m));
        return party;
    }

    /// <summary>Creates a treating physician.</summary>
    public static TreatingPhysician Physician(
        int id = 7,
        string code = "PW-9",
        string name = "Dr Name") =>
        TreatingPhysician.Create(id, code, name);

    /// <summary>Creates a price list line.</summary>
    public static PriceList CreatePriceList(
        long id = 1,
        string listName = "Standard",
        string analyte = "CBC",
        decimal price = 150m) =>
        PriceList.Create(id, analyte, listName, price);

    /// <summary>Creates a culture result.</summary>
    public static CultureResult CreateCultureResult(PatientId patientId) =>
        CultureResult.Create(patientId, "C-1", "Escherichia coli", Now);
}
