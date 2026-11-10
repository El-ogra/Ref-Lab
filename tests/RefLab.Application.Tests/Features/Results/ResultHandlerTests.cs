using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Auditing;
using RefLab.Domain.Billing;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Results;

public class OrderTestsHandlerTests
{
    [Fact]
    public async Task A_requested_test_freezes_the_applicable_reference_range()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient(sex: Sex.Male, age: 30m);
        var order = TestData.CreateOrder(patient);

        host.Db.Seed(patient, order, TestData.CreateTestDefinition(), TestData.CreateReferenceRange());

        var result = await host.Sender.Send(
            new OrderTestsCommand(order.Id, new[] { 56 }),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(56, result[0].TestCode);
        Assert.NotNull(host.Db.Items<OrderTest>().Single().ReferenceSnapshot);
    }

    [Fact]
    public async Task The_catalogue_price_is_attached_to_the_requested_test()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);

        host.Db.Seed(patient, order, TestData.CreateTestDefinition(patientPrice: 150m));

        var result = await host.Sender.Send(
            new OrderTestsCommand(order.Id, new[] { 56 }),
            CancellationToken.None);

        Assert.Equal(150m, result[0].Price);
    }

    [Fact]
    public async Task An_empty_request_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new OrderTestsCommand(order.Id, Array.Empty<int>()), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_test_is_rejected()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new OrderTestsCommand(order.Id, new[] { 999 }), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_order_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new OrderTestsCommand(Guid.NewGuid(), new[] { 56 }), CancellationToken.None));
    }
}

public class EnterResultHandlerTests
{
    [Fact]
    public async Task A_entered_result_is_classified_against_the_frozen_range()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);

        orderTest.AttachReferenceSnapshot(ReferenceSnapshot.Capture(
            TestData.CreateReferenceRange(),
            DateTimeOffset.UtcNow));

        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new EnterResultCommand(orderTest.Id, "18.5"),
            CancellationToken.None);

        Assert.Equal("18.5", result.ResultText);
        Assert.Equal(18.5m, result.ResultValue);
        Assert.Equal("High", result.Classification);
    }

    [Fact]
    public async Task A_qualitative_result_is_entered_without_a_classification()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);

        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new EnterResultCommand(orderTest.Id, "Positive"),
            CancellationToken.None);

        Assert.Equal("Positive", result.ResultText);
        Assert.Null(result.ResultValue);
        Assert.Null(result.Classification);
    }

    [Fact]
    public async Task An_empty_result_value_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new EnterResultCommand(Guid.NewGuid(), "  "), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_requested_test_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new EnterResultCommand(Guid.NewGuid(), "12"), CancellationToken.None));
    }
}

public class ReviewResultHandlerTests
{
    [Fact]
    public async Task A_result_is_confirmed_as_reviewed()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        orderTest.EnterResult(ResultValue.FromNumber(13.4m), DateTimeOffset.UtcNow, "lab.tech");

        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new ReviewResultCommand(orderTest.Id),
            CancellationToken.None);

        Assert.True(result.IsReviewed);
        Assert.Equal("Reviewed", result.State.ToString());
    }

    [Fact]
    public async Task A_result_that_was_never_entered_cannot_be_reviewed()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);

        host.Db.Seed(patient, order, orderTest);

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new ReviewResultCommand(orderTest.Id), CancellationToken.None));
    }
}

public class PrintResultReportHandlerTests
{
    private static (PatientOrder Order, OrderTest OrderTest, Patient Patient) SeedReviewed(ApplicationTestHost host)
    {
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        orderTest.EnterResult(ResultValue.FromNumber(13.4m), DateTimeOffset.UtcNow, "lab.tech");
        orderTest.ConfirmReviewed(DateTimeOffset.UtcNow, "lab.tech");

        host.Db.Seed(patient, order, orderTest);

        return (order, orderTest, patient);
    }

    [Fact]
    public async Task A_reviewed_result_is_rendered_exported_and_audited()
    {
        using var host = new ApplicationTestHost();
        var (_, orderTest, _) = SeedReviewed(host);

        var result = await host.Sender.Send(
            new PrintResultReportCommand(orderTest.Id, ReportCategory.Individual, @"D:\PDF\Individual"),
            CancellationToken.None);

        Assert.Equal(1, host.Renderer.CallCount);
        Assert.Equal(1, host.Pdf.CallCount);
        Assert.Equal(@"D:\PDF\Individual", host.Pdf.LastDestinationDirectory);
        Assert.Equal(1984, result.SequenceNumber);
        Assert.Single(host.Db.Items<PrintAudit>());
    }

    [Fact]
    public async Task An_unreviewed_result_cannot_be_printed()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new PrintResultReportCommand(orderTest.Id, ReportCategory.Individual, @"D:\PDF"),
                CancellationToken.None));

        Assert.Equal(0, host.Renderer.CallCount);
    }

    [Fact]
    public async Task The_print_sequence_advances_with_every_report()
    {
        using var host = new ApplicationTestHost();
        var (_, orderTest, _) = SeedReviewed(host);

        var first = await host.Sender.Send(
            new PrintResultReportCommand(orderTest.Id, ReportCategory.Individual, @"D:\PDF"),
            CancellationToken.None);

        var second = await host.Sender.Send(
            new PrintResultReportCommand(orderTest.Id, ReportCategory.Individual, @"D:\PDF"),
            CancellationToken.None);

        Assert.Equal(1984, first.SequenceNumber);
        Assert.Equal(1985, second.SequenceNumber);
    }
}

public class HandOverResultsHandlerTests
{
    [Fact]
    public async Task Reviewed_results_are_handed_over_and_the_account_is_settled()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        orderTest.EnterResult(ResultValue.FromNumber(13.4m), DateTimeOffset.UtcNow, "lab.tech");
        orderTest.ConfirmReviewed(DateTimeOffset.UtcNow, "lab.tech");

        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new HandOverResultsCommand(order.Id, 150m),
            CancellationToken.None);

        Assert.True(result[0].IsGiven);
        Assert.Single(host.Db.Items<Payment>());
        Assert.True(order.IsComplete);
    }

    [Fact]
    public async Task An_unreviewed_result_cannot_be_handed_over()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new HandOverResultsCommand(order.Id), CancellationToken.None));
    }

    [Fact]
    public async Task An_order_without_a_test_cannot_be_handed_over()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new HandOverResultsCommand(order.Id), CancellationToken.None));
    }
}
