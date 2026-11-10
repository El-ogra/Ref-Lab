using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ClinicalKnowledge;

/// <summary>
/// The test catalogue. The urine analysis carries the number 4 and neither its
/// number nor its name may be changed.
/// </summary>
public class TestDefinitionTests
{
    [Fact]
    public void Create_stores_the_catalogue_fields()
    {
        var test = TestDefinition.Create(
            code: 56,
            name: "Complete Blood Count",
            simpleName: "CBC",
            subGroupCode: 11,
            arrangementNumber: 3);

        Assert.Equal(56, test.Code);
        Assert.Equal("Complete Blood Count", test.Name);
        Assert.Equal("CBC", test.SimpleName);
        Assert.Equal(11, test.SubGroupCode);
        Assert.Equal(3, test.ArrangementNumber);
        Assert.True(test.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_rejects_a_non_positive_code(int code)
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            TestDefinition.Create(code, "Test", "Test", 1));
    }

    [Fact]
    public void Create_rejects_an_empty_name()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            TestDefinition.Create(1, "  ", "Test", 1));
    }

    [Fact]
    public void Create_rejects_an_empty_simplified_name()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            TestDefinition.Create(1, "Test", "  ", 1));
    }

    [Fact]
    public void Rename_changes_the_name_of_an_ordinary_test()
    {
        var test = TestDefinition.Create(56, "Complete Blood Count", "CBC", 11);

        test.Rename("Complete Blood Count with Differential");

        Assert.Equal("Complete Blood Count with Differential", test.Name);
    }

    [Fact]
    public void Rename_of_the_urine_analysis_is_rejected()
    {
        var test = TestDefinition.Create(
            TestDefinition.UrineAnalysisTestCode,
            "Urine Analysis",
            "URINE",
            4);

        var exception = Assert.Throws<TestNumberImmutableException>(() => test.Rename("Urine Examination"));

        Assert.Equal(4, exception.TestCode);
        Assert.Equal("Urine Analysis", test.Name);
    }

    [Fact]
    public void ChangeCode_of_the_urine_analysis_is_rejected()
    {
        var test = TestDefinition.Create(
            TestDefinition.UrineAnalysisTestCode,
            "Urine Analysis",
            "URINE",
            4);

        Assert.Throws<TestNumberImmutableException>(() => test.ChangeCode(99));

        Assert.Equal(4, test.Code);
    }

    [Fact]
    public void The_urine_analysis_is_recognised_by_its_code()
    {
        var urine = TestDefinition.Create(4, "Urine Analysis", "URINE", 4);
        var other = TestDefinition.Create(5, "Stool Analysis", "STOOL", 4);

        Assert.True(urine.IsUrineAnalysis);
        Assert.False(other.IsUrineAnalysis);
    }

    [Fact]
    public void UpdatePrices_stores_the_three_catalogue_prices()
    {
        var test = TestDefinition.Create(56, "CBC", "CBC", 11);

        test.UpdatePrices(patientPrice: 150m, labPrice: 100m, otherLabPrice: 200m);

        Assert.Equal(150m, test.PatientPrice);
        Assert.Equal(100m, test.LabPrice);
        Assert.Equal(200m, test.OtherLabPrice);
    }

    [Fact]
    public void ConfigureSendOut_sets_the_flag_and_the_destination()
    {
        var test = TestDefinition.Create(56, "CBC", "CBC", 11);

        test.ConfigureSendOut(true, "  Central Lab  ");

        Assert.True(test.IsSendOut);
        Assert.Equal("Central Lab", test.OtherLabName);
    }

    [Fact]
    public void SetCriticalInterval_stores_the_critical_limits()
    {
        var test = TestDefinition.Create(56, "CBC", "CBC", 11);

        test.SetCriticalInterval(CriticalInterval.Create(lowCriticalLimit: 5m, highCriticalLimit: 20m));

        Assert.True(test.CriticalInterval.HasLimits);
        Assert.Equal(5m, test.CriticalInterval.LowCriticalLimit);
    }

    [Fact]
    public void Deactivate_and_Activate_toggle_the_active_flag()
    {
        var test = TestDefinition.Create(56, "CBC", "CBC", 11);

        test.Deactivate();
        Assert.False(test.IsActive);

        test.Activate();
        Assert.True(test.IsActive);
    }
}
