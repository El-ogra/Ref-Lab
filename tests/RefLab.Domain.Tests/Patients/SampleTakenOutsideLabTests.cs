using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Patients;

/// <summary>
/// The reference system prints a standard comment under the result of a
/// specimen that was collected away from the laboratory. Ref Lab records the
/// fact on the order row itself, so that the laboratory can tell the two
/// cases apart when it reads its own data. The flag is orthogonal to the
/// result state machine: it survives every transition and gates none of
/// them.
/// </summary>
public sealed class SampleTakenOutsideLabTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    private static OrderTest CreateOrderTest() =>
        OrderTest.Create(Guid.NewGuid(), 56, "Hgb", 11, Moment, "lab.tech");

    [Fact]
    public void The_specimen_is_taken_inside_the_laboratory_by_default()
    {
        var test = CreateOrderTest();

        Assert.False(test.SampleTakenOutsideLab);
    }

    [Fact]
    public void The_flag_is_recorded_with_the_amendment_that_set_it()
    {
        var test = CreateOrderTest();

        test.SetSampleTakenOutsideLab(true, Moment, "lab.tech");

        Assert.True(test.SampleTakenOutsideLab);
        Assert.Equal("lab.tech", test.EditedBy);
        Assert.Equal(Moment, test.EditedAt);
    }

    [Fact]
    public void The_flag_survives_the_result_state_machine()
    {
        var test = CreateOrderTest();

        test.SetSampleTakenOutsideLab(true, Moment, "lab.tech");
        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");
        test.ConfirmReviewed(Moment, "lab.tech");
        test.MarkPrinted("lab.tech", Moment, 1984);

        Assert.True(test.SampleTakenOutsideLab);
        Assert.Equal(ResultState.Printed, test.State);
    }
}
