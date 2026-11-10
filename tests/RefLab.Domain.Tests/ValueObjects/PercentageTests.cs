using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// The referring party's commission rate.
/// </summary>
public class PercentageTests
{
    [Fact]
    public void Apply_scales_the_base_amount_by_the_rate()
    {
        var rate = Percentage.Create(10m);

        var commission = rate.Apply(1000m);

        Assert.Equal(100m, commission);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public void A_rate_outside_zero_to_one_hundred_is_rejected(decimal value)
    {
        Assert.Throws<InvalidPercentageException>(() => Percentage.Create(value));
    }

    [Fact]
    public void FromRatio_converts_a_ratio_into_a_percentage()
    {
        var rate = Percentage.FromRatio(0.075m);

        Assert.Equal(7.5m, rate.Value);
    }

    [Fact]
    public void Zero_and_one_hundred_are_accepted()
    {
        Assert.Equal(0m, Percentage.Create(0m).Value);
        Assert.Equal(100m, Percentage.Create(100m).Value);
    }
}
