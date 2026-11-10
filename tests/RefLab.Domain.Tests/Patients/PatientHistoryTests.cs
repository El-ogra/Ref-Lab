using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Patients;

/// <summary>
/// The free-form history sheet carries twenty untyped slots, exactly as the
/// legacy <c>history</c> table does.
/// </summary>
public class PatientHistoryTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    private static PatientHistory CreateHistory() =>
        PatientHistory.Create(PatientId.New(), selector: 1, Moment);

    [Fact]
    public void A_value_written_into_a_slot_can_be_read_back()
    {
        var history = CreateHistory();

        history.SetSlot(3, "Diabetes mellitus");

        Assert.Equal("Diabetes mellitus", history.GetSlot(3));
    }

    [Fact]
    public void The_used_slot_count_follows_the_highest_written_slot()
    {
        var history = CreateHistory();

        history.SetSlot(2, "value");
        Assert.Equal(2, history.UsedSlotCount);

        history.SetSlot(7, "value");
        Assert.Equal(7, history.UsedSlotCount);
    }

    [Fact]
    public void A_blank_value_leaves_the_slot_empty()
    {
        var history = CreateHistory();

        history.SetSlot(1, "   ");

        Assert.Null(history.GetSlot(1));
        Assert.Equal(0, history.UsedSlotCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void A_slot_outside_the_twenty_slots_is_rejected(int slot)
    {
        var history = CreateHistory();

        Assert.Throws<Exceptions.InvalidPatientOrderException>(() => history.SetSlot(slot, "value"));
    }

    [Fact]
    public void The_print_flag_is_set_by_default_and_can_be_changed()
    {
        var history = CreateHistory();

        Assert.True(history.PrintFlag);

        history.SetPrintFlag(false);

        Assert.False(history.PrintFlag);
    }
}
