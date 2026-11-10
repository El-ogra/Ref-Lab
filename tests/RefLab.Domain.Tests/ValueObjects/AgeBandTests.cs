using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// Reference ranges are banded by age and by the unit the age is expressed in.
/// </summary>
public class AgeBandTests
{
    [Fact]
    public void A_band_contains_an_age_inside_its_bounds()
    {
        var band = AgeBand.Create(0m, 12m, AgeUnit.Years);

        Assert.True(band.Contains(5m, AgeUnit.Years));
    }

    [Fact]
    public void A_band_contains_its_inclusive_bounds()
    {
        var band = AgeBand.Create(0m, 12m, AgeUnit.Years);

        Assert.True(band.Contains(0m, AgeUnit.Years));
        Assert.True(band.Contains(12m, AgeUnit.Years));
    }

    [Fact]
    public void A_band_does_not_contain_an_age_outside_its_bounds()
    {
        var band = AgeBand.Create(0m, 12m, AgeUnit.Years);

        Assert.False(band.Contains(13m, AgeUnit.Years));
        Assert.False(band.Contains(-1m, AgeUnit.Years));
    }

    [Fact]
    public void A_band_never_contains_an_age_expressed_in_another_unit()
    {
        var band = AgeBand.Create(0m, 365m, AgeUnit.Days);

        Assert.False(band.Contains(5m, AgeUnit.Years));
        Assert.True(band.Contains(5m, AgeUnit.Days));
    }

    [Fact]
    public void An_unbounded_band_contains_any_age()
    {
        var band = AgeBand.Create(null, null, AgeUnit.Years);

        Assert.True(band.Contains(120m, AgeUnit.Years));
    }

    [Fact]
    public void A_lower_bound_above_the_upper_bound_is_rejected()
    {
        Assert.Throws<InvalidAgeBandException>(() => AgeBand.Create(12m, 5m, AgeUnit.Years));
    }

    [Fact]
    public void A_negative_bound_is_rejected()
    {
        Assert.Throws<InvalidAgeBandException>(() => AgeBand.Create(-1m, 5m, AgeUnit.Years));
    }
}
