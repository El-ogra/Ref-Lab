using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;

namespace RefLab.Application.Tests.Features.Patients;

public class AllocateOrderIdentifierTests
{
    [Fact]
    public async Task The_identifier_is_allocated_from_its_positional_segments()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new AllocateOrderIdentifierQuery("12", "AB", "34567", "890"),
            CancellationToken.None);

        Assert.Equal(13, result.Value.Length);
        Assert.Equal("12", result.PersonKey);
        Assert.Equal("890", result.DisplayCode);
    }

    [Fact]
    public async Task The_allocated_identifier_carries_no_branch_code()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new AllocateOrderIdentifierQuery("12", "AB", "34567", "890"),
            CancellationToken.None);

        Assert.DoesNotContain("BranchCode", result.GetType().GetProperties().Select(property => property.Name));
    }

    [Fact]
    public async Task A_person_key_of_the_wrong_length_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new AllocateOrderIdentifierQuery("1", "AB", "34567", "890"), CancellationToken.None));
    }

    [Fact]
    public async Task A_middle_segment_of_the_wrong_length_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new AllocateOrderIdentifierQuery("12", "AB", "3456", "890"), CancellationToken.None));
    }
}

public class GeneratePatientBarcodeTests
{
    [Fact]
    public async Task The_barcode_is_allocated_and_stored_on_the_order()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        var result = await host.Sender.Send(
            new GeneratePatientBarcodeCommand(order.Id),
            CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(result.Barcode));
        Assert.Equal(result.Barcode, order.Barcode);
        Assert.Equal(order.Id, result.OrderId);
    }

    [Fact]
    public async Task An_unknown_order_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new GeneratePatientBarcodeCommand(Guid.NewGuid()), CancellationToken.None));
    }
}

public class AttachPatientPictureTests
{
    [Fact]
    public async Task The_picture_is_attached_to_the_patient()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);

        var result = await host.Sender.Send(
            new AttachPatientPictureCommand(patient.Id.Value, 3, @"C:\pictures\a.png", "caption"),
            CancellationToken.None);

        Assert.Equal(3, result.Slot);
        Assert.Equal("caption", result.Caption);
        Assert.Single(host.Db.Items<PatientPicture>());
    }

    [Fact]
    public async Task An_occupied_slot_is_rejected()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);
        host.Db.Seed(PatientPicture.Create(patient.Id, 3, @"C:\pictures\a.png"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new AttachPatientPictureCommand(patient.Id.Value, 3, @"C:\pictures\b.png", null),
                CancellationToken.None));
    }

    [Fact]
    public async Task A_slot_outside_the_available_range_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new AttachPatientPictureCommand(patient.Id.Value, 20, @"C:\pictures\a.png", null),
                CancellationToken.None));
    }
}

public class FollowUpCaseTests
{
    [Fact]
    public async Task The_register_reports_the_next_action_of_every_case()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(new FollowUpCaseQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(1, result[0].RequestedTestCount);
        Assert.Equal(0, result[0].ReviewedTestCount);
        Assert.Equal("Enter and confirm the outstanding results.", result[0].NextAction);
    }

    [Fact]
    public async Task A_completed_case_is_reported_as_complete()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateReviewedOrderTest(order);
        orderTest.MarkPrinted("lab.tech", TestData.Now, 1984);
        orderTest.MarkGiven("reception", TestData.Now);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(new FollowUpCaseQuery(), CancellationToken.None);

        Assert.True(result[0].IsComplete);
        Assert.Equal("The case is complete.", result[0].NextAction);
    }

    [Fact]
    public async Task The_open_cases_only_are_returned_when_requested()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateReviewedOrderTest(order);
        orderTest.MarkPrinted("lab.tech", TestData.Now, 1984);
        orderTest.MarkGiven("reception", TestData.Now);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(new FollowUpCaseQuery(OpenOnly: true), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task A_period_that_ends_before_it_starts_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new FollowUpCaseQuery(TestData.Now.AddDays(1), TestData.Now, false),
                CancellationToken.None));
    }
}
