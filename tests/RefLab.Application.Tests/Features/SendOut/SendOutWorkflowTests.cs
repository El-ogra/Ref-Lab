using RefLab.Application.Common;
using RefLab.Application.Features.SendOut;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Enums;
using RefLab.Domain.SendOut;

namespace RefLab.Application.Tests.Features.SendOut;

public class SendOutWorkflowTests
{
    [Fact]
    public async Task The_test_is_enrolled_on_the_send_out_list()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new CreateSendOutRequestCommand(orderTest.Id, "Central Lab", 200m, 250m),
            CancellationToken.None);

        Assert.Equal("Central Lab", result.DestinationLabName);
        Assert.Equal(SendOutState.Pending, result.State);
        Assert.Single(host.Db.Items<SendOutRequest>());
    }

    [Fact]
    public async Task A_test_cannot_be_enrolled_twice()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);
        host.Db.Seed(SendOutRequest.Create(orderTest.Id, "Central Lab"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new CreateSendOutRequestCommand(orderTest.Id, "Central Lab"),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_requested_test_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new CreateSendOutRequestCommand(Guid.NewGuid(), "Central Lab"), CancellationToken.None));
    }

    [Fact]
    public async Task The_queue_lists_the_unsettled_referrals()
    {
        using var host = new ApplicationTestHost();
        var pending = SendOutRequest.Create(Guid.NewGuid(), "Central Lab");
        var settled = SendOutRequest.Create(Guid.NewGuid(), "Central Lab");
        settled.MarkDispatched(TestData.Now);
        settled.MarkSettled(TestData.Now);
        host.Db.Seed(pending, settled);

        var result = await host.Sender.Send(new GetSendOutQueueQuery(PendingOnly: true), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Central Lab", result[0].DestinationLabName);
    }

    [Fact]
    public async Task A_sample_withdrawal_is_recorded()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new SplitOrWithdrawSampleCommand(orderTest.Id, SampleDispositionAction.Withdrawn, "Central Lab"),
            CancellationToken.None);

        Assert.Equal(SampleDispositionAction.Withdrawn, result.Action);
        Assert.Equal("lab.tech", result.RecordedBy);
        Assert.Single(host.Db.Items<SampleDisposition>());
    }

    [Fact]
    public async Task An_unknown_disposition_action_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new SplitOrWithdrawSampleCommand(Guid.NewGuid(), (SampleDispositionAction)99),
                CancellationToken.None));
    }
}
