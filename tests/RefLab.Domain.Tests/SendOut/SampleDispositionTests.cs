using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.SendOut;

namespace RefLab.Domain.Tests.SendOut;

/// <summary>
/// A sample may be split or withdrawn.
/// </summary>
public class SampleDispositionTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_stores_the_action_and_the_audit()
    {
        var orderTestId = Guid.NewGuid();

        var disposition = SampleDisposition.Create(
            orderTestId,
            SampleDispositionAction.Split,
            Moment,
            "lab.tech",
            "Central Lab");

        Assert.Equal(orderTestId, disposition.OrderTestId);
        Assert.Equal(SampleDispositionAction.Split, disposition.Action);
        Assert.Equal("Central Lab", disposition.DestinationLabName);
        Assert.Equal("lab.tech", disposition.RecordedBy);
    }

    [Fact]
    public void Create_rejects_a_record_without_an_operator()
    {
        Assert.Throws<InvalidSampleDispositionException>(() =>
            SampleDisposition.Create(Guid.NewGuid(), SampleDispositionAction.Withdrawn, Moment, "  "));
    }

    [Fact]
    public void Exactly_two_disposition_actions_are_defined()
    {
        var count = Enum.GetValues<SampleDispositionAction>().Length;

        Assert.Equal(2, count);
    }
}
