using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ClinicalKnowledge;

/// <summary>
/// A reference range is scoped by sex, by an age window and by an age unit.
/// </summary>
public class ReferenceRangeTests
{
    private static ReferenceRange CreateRange(
        int id = 1,
        int testCode = 56,
        SexScope sexScope = SexScope.Male,
        decimal? from = 0m,
        decimal? to = 120m,
        AgeUnit unit = AgeUnit.Years)
    {
        return ReferenceRange.Create(
            id,
            testCode,
            "Hgb",
            11,
            sexScope,
            AgeBand.Create(from, to, unit),
            ReferenceInterval.Create(lowLimit: 12m, highLimit: 16m));
    }

    [Fact]
    public void Matches_is_true_for_a_patient_inside_the_band()
    {
        var range = CreateRange();

        Assert.True(range.Matches(Sex.Male, 30m, AgeUnit.Years));
    }

    [Fact]
    public void Matches_is_false_for_a_patient_of_the_other_sex()
    {
        var range = CreateRange(sexScope: SexScope.Male);

        Assert.False(range.Matches(Sex.Female, 30m, AgeUnit.Years));
    }

    [Fact]
    public void Matches_is_true_for_a_sex_agnostic_band()
    {
        var range = CreateRange(sexScope: SexScope.All);

        Assert.True(range.Matches(Sex.Male, 30m, AgeUnit.Years));
        Assert.True(range.Matches(Sex.Female, 30m, AgeUnit.Years));
    }

    [Fact]
    public void Matches_is_false_for_a_patient_outside_the_age_band()
    {
        var range = CreateRange(from: 0m, to: 12m);

        Assert.False(range.Matches(Sex.Male, 30m, AgeUnit.Years));
    }

    [Fact]
    public void A_sex_specific_range_is_more_specific_than_a_sex_agnostic_one()
    {
        var specific = CreateRange(id: 1, sexScope: SexScope.Male);
        var general = CreateRange(id: 2, sexScope: SexScope.All);

        Assert.True(specific.SexSpecificity > general.SexSpecificity);
    }

    [Fact]
    public void A_narrow_age_band_reports_a_smaller_span()
    {
        var narrow = CreateRange(id: 1, from: 0m, to: 1m);
        var wide = CreateRange(id: 2, from: 0m, to: 120m);

        Assert.True(narrow.AgeSpan < wide.AgeSpan);
    }

    [Fact]
    public void Create_rejects_a_non_positive_identifier()
    {
        Assert.Throws<InvalidTestDefinitionException>(() => ReferenceRange.Create(
            0, 56, "Hgb", 11, SexScope.All, AgeBand.Create(0m, 120m, AgeUnit.Years),
            ReferenceInterval.Create()));
    }
}
