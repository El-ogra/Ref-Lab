using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ClinicalKnowledge;

public class CatalogueUpdateTests
{
    [Fact]
    public void UpdateDetails_leaves_an_unsupplied_value_untouched()
    {
        var test = TestDefinition.Create(56, "CBC", "Hgb", 11);
        test.SetArabicName("صورة دم");

        test.UpdateDetails(sampleType: "EDTA Blood");

        Assert.Equal("صورة دم", test.ArabicName);
        Assert.Equal("EDTA Blood", test.SampleType);
    }

    [Fact]
    public void UpdateDetails_amends_a_supplied_value()
    {
        var test = TestDefinition.Create(56, "CBC", "Hgb", 11);

        test.UpdateDetails(arabicName: "صورة دم كاملة");

        Assert.Equal("صورة دم كاملة", test.ArabicName);
    }

    [Fact]
    public void UpdateDetails_rejects_an_empty_simplified_name()
    {
        var test = TestDefinition.Create(56, "CBC", "Hgb", 11);

        Assert.Throws<InvalidTestDefinitionException>(() => test.UpdateDetails(simpleName: "  "));
    }

    [Fact]
    public void UpdateLimits_amends_the_interval()
    {
        var range = ReferenceRange.Create(
            1, 56, "Hgb", 11, Domain.Enums.SexScope.All,
            AgeBand.Create(0m, 120m, Domain.Enums.AgeUnit.Years),
            ReferenceInterval.Create(lowLimit: 12m, highLimit: 16m));

        range.UpdateLimits(13m, 17m);

        Assert.Equal(13m, range.Interval.LowLimit);
        Assert.Equal(17m, range.Interval.HighLimit);
    }

    [Fact]
    public void UpdateLimits_rejects_inverted_limits()
    {
        var range = ReferenceRange.Create(
            1, 56, "Hgb", 11, Domain.Enums.SexScope.All,
            AgeBand.Create(0m, 120m, Domain.Enums.AgeUnit.Years),
            ReferenceInterval.Create(lowLimit: 12m, highLimit: 16m));

        Assert.Throws<InvalidReferenceIntervalException>(() => range.UpdateLimits(17m, 13m));
    }

    [Fact]
    public void A_unit_can_be_renamed_and_a_factor_can_be_updated()
    {
        var unit = TestUnit.Create(1, "g/dL");
        unit.UpdateName("g/L");
        Assert.Equal("g/L", unit.Name);

        var factor = ConversionFactor.Create("Albumin", "g/dL", "g/L", 10m);
        factor.UpdateFactor(20m, 1, 0);
        Assert.Equal(20m, factor.Factor);
        Assert.Equal(40m, factor.Convert(2m));
    }

    [Fact]
    public void An_abbreviation_and_a_sub_group_can_be_amended()
    {
        var abbreviation = Abbreviation.Create(1, "CBC", "Complete Blood Count");
        abbreviation.UpdateMeaning("Complete Blood Count with differential");
        Assert.Equal("Complete Blood Count with differential", abbreviation.Meaning);

        var subGroup = ClinicalSubGroup.Create(11, "BLOOD PICTURE");
        subGroup.UpdateName("BLOOD COUNT");
        subGroup.UpdateArabicName("صورة دم");
        Assert.Equal("BLOOD COUNT", subGroup.Name);
        Assert.Equal("صورة دم", subGroup.ArabicName);
    }
}
