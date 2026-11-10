using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Rules;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Rules;

/// <summary>
/// The legacy selection rule among overlapping bands is not established. Ref
/// Lab applies a deterministic rule instead.
/// </summary>
public class ReferenceRangeSelectorTests
{
    private static ReferenceRange CreateRange(
        int id,
        SexScope sexScope,
        decimal? from,
        decimal? to,
        AgeUnit unit = AgeUnit.Years)
    {
        return ReferenceRange.Create(
            id,
            56,
            "Hgb",
            11,
            sexScope,
            AgeBand.Create(from, to, unit),
            ReferenceInterval.Create(lowLimit: 12m, highLimit: 16m));
    }

    [Fact]
    public void A_sex_specific_band_wins_over_a_sex_agnostic_band()
    {
        var general = CreateRange(1, SexScope.All, 0m, 120m);
        var male = CreateRange(2, SexScope.Male, 0m, 120m);

        var selected = ReferenceRangeSelector.Select(new[] { general, male }, 56, Sex.Male, 30m, AgeUnit.Years);

        Assert.Equal(2, selected!.Id);
    }

    [Fact]
    public void A_narrower_band_wins_over_a_wider_band()
    {
        var wide = CreateRange(1, SexScope.All, 0m, 120m);
        var narrow = CreateRange(2, SexScope.All, 10m, 20m);

        var selected = ReferenceRangeSelector.Select(new[] { wide, narrow }, 56, Sex.Male, 15m, AgeUnit.Years);

        Assert.Equal(2, selected!.Id);
    }

    [Fact]
    public void A_band_of_another_analyte_is_ignored()
    {
        var otherTest = ReferenceRange.Create(
            1, 57, "WBC", 11, SexScope.All, AgeBand.Create(0m, 120m, AgeUnit.Years),
            ReferenceInterval.Create(lowLimit: 4m, highLimit: 11m));

        var selected = ReferenceRangeSelector.Select(new[] { otherTest }, 56, Sex.Male, 30m, AgeUnit.Years);

        Assert.Null(selected);
    }

    [Fact]
    public void No_matching_band_yields_no_selection()
    {
        var male = CreateRange(1, SexScope.Male, 0m, 120m);

        var selected = ReferenceRangeSelector.Select(new[] { male }, 56, Sex.Female, 30m, AgeUnit.Years);

        Assert.Null(selected);
    }

    [Fact]
    public void SelectAll_returns_every_matching_band_ordered_by_specificity()
    {
        var general = CreateRange(1, SexScope.All, 0m, 120m);
        var male = CreateRange(2, SexScope.Male, 0m, 120m);
        var narrow = CreateRange(3, SexScope.Male, 10m, 20m);

        var selected = ReferenceRangeSelector.SelectAll(
            new[] { general, male, narrow }, 56, Sex.Male, 15m, AgeUnit.Years);

        Assert.Equal(3, selected.Count);
        Assert.Equal(3, selected[0].Id);
        Assert.Equal(2, selected[1].Id);
        Assert.Equal(1, selected[2].Id);
    }
}
