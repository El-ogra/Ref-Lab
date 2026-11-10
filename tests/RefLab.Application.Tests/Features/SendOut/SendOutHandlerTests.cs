using RefLab.Application.Common;
using RefLab.Application.Features.SendOut;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Enums;
using RefLab.Domain.SendOut;

namespace RefLab.Application.Tests.Features.SendOut;

public class SendOutHandlerTests
{
    private static SendOutRequest CreateRequest() =>
        SendOutRequest.Create(Guid.NewGuid(), "Central Lab", 200m, 250m);

    [Fact]
    public async Task A_dispatched_sample_is_dispatched_but_not_settled()
    {
        using var host = new ApplicationTestHost();
        var sendOut = CreateRequest();
        host.Db.Seed(sendOut);

        var state = await host.Sender.Send(
            new DispatchSendOutSampleCommand(sendOut.Id),
            CancellationToken.None);

        Assert.Equal(SendOutState.Dispatched, state);
    }

    [Fact]
    public async Task A_settled_sample_is_settled()
    {
        using var host = new ApplicationTestHost();
        var sendOut = CreateRequest();
        sendOut.MarkDispatched(DateTimeOffset.UtcNow);
        host.Db.Seed(sendOut);

        var state = await host.Sender.Send(
            new SettleSendOutSampleCommand(sendOut.Id),
            CancellationToken.None);

        Assert.Equal(SendOutState.Settled, state);
    }

    [Fact]
    public async Task A_sample_cannot_be_settled_before_it_is_dispatched()
    {
        using var host = new ApplicationTestHost();
        var sendOut = CreateRequest();
        host.Db.Seed(sendOut);

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new SettleSendOutSampleCommand(sendOut.Id), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_send_out_request_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new DispatchSendOutSampleCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
