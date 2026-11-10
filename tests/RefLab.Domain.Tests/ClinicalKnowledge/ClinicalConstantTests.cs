using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Tests.ClinicalKnowledge;

/// <summary>
/// The clinical constants are held as data, not as code, so that a laboratory
/// can correct a factor without a software change.
/// </summary>
public class ClinicalConstantTests
{
    [Fact]
    public void Create_stores_the_values_in_their_slots()
    {
        var group = ClinicalConstant.Create("CBCCONS", "3.3", "6.25", "6.75", "7.5", "8.25");

        Assert.Equal("CBCCONS", group.GroupName);
        Assert.Equal("3.3", group.GetText(1));
        Assert.Equal("8.25", group.GetText(5));
    }

    [Fact]
    public void GetNumber_reads_a_numeric_slot()
    {
        var group = ClinicalConstant.Create("CBCCONS", "3.3", "6.25");

        Assert.Equal(3.3m, group.GetNumber(1));
        Assert.Equal(6.25m, group.GetNumber(2));
    }

    [Fact]
    public void GetNumber_rejects_a_slot_that_does_not_hold_a_number()
    {
        var group = ClinicalConstant.Create("PTCONS", "12", "35", "1.33", "2CVJW");

        Assert.Throws<InvalidClinicalConstantException>(() => group.GetNumber(4));
    }

    [Fact]
    public void TryGetNumber_reports_whether_a_slot_holds_a_number()
    {
        var group = ClinicalConstant.Create("PTCONS", "12", "35", "1.33", "2CVJW");

        Assert.True(group.TryGetNumber(3, out var value));
        Assert.Equal(1.33m, value);

        Assert.False(group.TryGetNumber(4, out _));
    }

    [Fact]
    public void A_slot_outside_the_available_range_is_rejected()
    {
        var group = ClinicalConstant.Create("CBCCONS", "3.3");

        Assert.Throws<ArgumentOutOfRangeException>(() => group.GetText(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => group.GetText(8));
    }

    [Fact]
    public void A_group_may_not_exceed_the_available_number_of_slots()
    {
        var values = new string?[ClinicalConstant.SlotCount + 1];

        Assert.Throws<InvalidTestDefinitionException>(() => ClinicalConstant.Create("X", values));
    }
}
