using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;

namespace RefLab.Application.Tests.Features.Patients;

public class RegisterPatientHandlerTests
{
    [Fact]
    public async Task A_patient_and_an_order_are_created()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new RegisterPatientCommand(
                "محمد أحمد",
                Sex.Male,
                30m,
                AgeUnit.Years,
                null,
                "Cairo",
                "0100000000",
                "0111111111",
                null,
                null,
                null,
                null,
                "EGP",
                CreateOrder: true),
            CancellationToken.None);

        Assert.Single(host.Db.Items<Patient>());
        Assert.Single(host.Db.Items<PatientOrder>());
        Assert.Empty(result.Tests);
        Assert.Equal("EGP", result.CurrencyCode);
    }

    [Fact]
    public async Task An_unsigned_operator_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.User.OperatorId = null;

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(
                new RegisterPatientCommand(
                    "Name", Sex.Male, null, null, null, null, null, null, null, null, null, null, "EGP", true),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_empty_patient_name_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new RegisterPatientCommand(
                    "  ", Sex.Male, null, null, null, null, null, null, null, null, null, null, "EGP", true),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_invalid_currency_code_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new RegisterPatientCommand(
                    "Name", Sex.Male, null, null, null, null, null, null, null, null, null, null, "EG", true),
                CancellationToken.None));
    }
}

public class SearchPatientsHandlerTests
{
    [Fact]
    public async Task Patients_are_found_by_name()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreatePatient("أحمد"), TestData.CreatePatient("سعيد"));

        var result = await host.Sender.Send(
            new SearchPatientsQuery("أحمد", null, PageRequest.First()),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("أحمد", result.Items[0].Name);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task Patients_are_found_by_phone_or_mobile()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(
            TestData.CreatePatient("أحمد", mobile: "0111111111"),
            TestData.CreatePatient("سعيد", mobile: "0222222222"));

        var result = await host.Sender.Send(
            new SearchPatientsQuery(null, "0222222222", PageRequest.First()),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("سعيد", result.Items[0].Name);
    }

    [Fact]
    public async Task An_empty_search_returns_every_patient()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreatePatient("أحمد"), TestData.CreatePatient("سعيد"));

        var result = await host.Sender.Send(
            new SearchPatientsQuery(null, null, PageRequest.First()),
            CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task The_paging_metadata_is_reported()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreatePatient("أحمد"), TestData.CreatePatient("سعيد"));

        var result = await host.Sender.Send(
            new SearchPatientsQuery(null, null, new PageRequest(1, 1)),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task An_invalid_page_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new SearchPatientsQuery(null, null, new PageRequest(0, 25)),
                CancellationToken.None));
    }
}

public class GetPatientOrderHandlerTests
{
    [Fact]
    public async Task An_order_is_returned_with_its_requested_tests()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);

        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(new GetPatientOrderQuery(order.Id), CancellationToken.None);

        Assert.Equal(order.Id, result.Id);
        Assert.Single(result.Tests);
        Assert.Equal(56, result.Tests[0].TestCode);
    }

    [Fact]
    public async Task An_unknown_order_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new GetPatientOrderQuery(Guid.NewGuid()), CancellationToken.None));
    }
}

public class GetPatientHistoryHandlerTests
{
    [Fact]
    public async Task The_history_sheets_of_a_patient_are_returned()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var sheet = Domain.Patients.PatientHistory.Create(patient.Id, 1, TestData.Now);
        sheet.SetSlot(2, "Diabetes mellitus");

        host.Db.Seed(patient, sheet);

        var result = await host.Sender.Send(
            new GetPatientHistoryQuery(patient.Id.Value),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Diabetes mellitus", result[0].Slots[2]);
        Assert.Equal(2, result[0].UsedSlotCount);
    }

    [Fact]
    public async Task A_patient_without_a_history_returns_no_sheet()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);

        var result = await host.Sender.Send(
            new GetPatientHistoryQuery(patient.Id.Value),
            CancellationToken.None);

        Assert.Empty(result);
    }
}
