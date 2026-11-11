using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Domain.Tests.ClinicalKnowledge;

/// <summary>
/// The custom test group — the clinical sub-group that carries the tests an
/// operator has grouped together.
/// </summary>
/// <remarks>
/// The membership of a test group is the clinical sub-group the test belongs
/// to, so a group is composed by moving the chosen tests into a new
/// sub-group. The tests below cover that move in the domain.
/// </remarks>
public sealed class TestGroupTests
{
    [Fact]
    public void MoveTo_changes_the_sub_group_of_the_test()
    {
        var test = TestDefinition.Create(56, "Complete Blood Count", "CBC", 11);

        test.MoveTo(43, test.ArrangementNumber);

        Assert.Equal(43, test.SubGroupCode);
    }

    [Fact]
    public void MoveTo_keeps_the_arrangement_number()
    {
        var test = TestDefinition.Create(56, "Complete Blood Count", "CBC", 11, arrangementNumber: 7);

        test.MoveTo(43, test.ArrangementNumber);

        Assert.Equal(7, test.ArrangementNumber);
    }

    [Fact]
    public void MoveTo_leaves_the_identity_of_the_test_untouched()
    {
        var test = TestDefinition.Create(56, "Complete Blood Count", "CBC", 11);

        test.MoveTo(43, test.ArrangementNumber);

        Assert.Equal(56, test.Code);
        Assert.Equal("Complete Blood Count", test.Name);
        Assert.Equal("CBC", test.SimpleName);
    }

    [Fact]
    public void A_test_may_be_moved_more_than_once()
    {
        var test = TestDefinition.Create(56, "Complete Blood Count", "CBC", 11);

        test.MoveTo(43, test.ArrangementNumber);
        test.MoveTo(44, test.ArrangementNumber);

        Assert.Equal(44, test.SubGroupCode);
    }

    [Fact]
    public void A_new_sub_group_carries_its_code_and_name()
    {
        var group = ClinicalSubGroup.Create(43, "CORE PANEL");

        Assert.Equal(43, group.Code);
        Assert.Equal("CORE PANEL", group.Name);
        Assert.Null(group.ArabicName);
    }
}
