using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.SendOut;

namespace RefLab.Domain.Tests.SendOut;

/// <summary>
/// Dispatch and settlement are tracked separately so that an unsettled
/// referral stays visible.
/// </summary>
public class SendOutRequestTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void A_new_request_is_pending()
    {
        var request = SendOutRequest.Create(Guid.NewGuid(), "Central Lab");

        Assert.Equal(SendOutState.Pending, request.State);
        Assert.Equal("Central Lab", request.DestinationLabName);
    }

    [Fact]
    public void A_dispatched_request_is_dispatched_but_not_settled()
    {
        var request = SendOutRequest.Create(Guid.NewGuid(), "Central Lab");

        request.MarkDispatched(Moment);

        Assert.True(request.IsDispatched);
        Assert.False(request.IsSettled);
        Assert.Equal(SendOutState.Dispatched, request.State);
    }

    [Fact]
    public void A_settled_request_is_settled()
    {
        var request = SendOutRequest.Create(Guid.NewGuid(), "Central Lab");

        request.MarkDispatched(Moment);
        request.MarkSettled(Moment.AddDays(1));

        Assert.True(request.IsSettled);
        Assert.Equal(SendOutState.Settled, request.State);
    }

    [Fact]
    public void A_request_cannot_be_settled_before_it_is_dispatched()
    {
        var request = SendOutRequest.Create(Guid.NewGuid(), "Central Lab");

        Assert.Throws<InvalidSendOutStateException>(() => request.MarkSettled(Moment));
    }

    [Fact]
    public void A_settled_request_cannot_be_dispatched_again()
    {
        var request = SendOutRequest.Create(Guid.NewGuid(), "Central Lab");
        request.MarkDispatched(Moment);
        request.MarkSettled(Moment);

        Assert.Throws<InvalidSendOutStateException>(() => request.MarkDispatched(Moment.AddDays(2)));
    }

    [Fact]
    public void Create_rejects_a_request_without_a_destination()
    {
        Assert.Throws<InvalidSendOutStateException>(() =>
            SendOutRequest.Create(Guid.NewGuid(), "  "));
    }

    [Fact]
    public void The_send_out_prices_are_recorded()
    {
        var request = SendOutRequest.Create(Guid.NewGuid(), "Central Lab", outPrice: 200m, outPriceBeforeDiscount: 250m);

        Assert.Equal(200m, request.OutPrice);
        Assert.Equal(250m, request.OutPriceBeforeDiscount);
    }
}
