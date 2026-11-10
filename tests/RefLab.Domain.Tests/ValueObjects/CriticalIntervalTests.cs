using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// The critical limits are reproduced from the five columns that the legacy
/// backup schema carries and the shipped database lacks.
/// </summary>
public class CriticalIntervalTests
{
    [Fact]
    public void Create_keeps_the_critical_limits_and_texts()
    {
        var interval = CriticalInterval.Create(
            lowCriticalLimit: 5m,
            highCriticalLimit: 20m,
            criticalRangeText: "Hgb < 5 or > 20 g/dL",
            criticalExceptionText: "حرجة",
            criticalComment: "Immediate notification required");

        Assert.Equal(5m, interval.LowCriticalLimit);
        Assert.Equal(20m, interval.HighCriticalLimit);
        Assert.Equal("Hgb < 5 or > 20 g/dL", interval.CriticalRangeText);
        Assert.Equal("حرجة", interval.CriticalExceptionText);
        Assert.Equal("Immediate notification required", interval.CriticalComment);
        Assert.True(interval.HasLimits);
    }

    [Fact]
    public void A_lower_critical_limit_above_the_upper_critical_limit_is_rejected()
    {
        Assert.Throws<InvalidCriticalIntervalException>(() =>
            CriticalInterval.Create(lowCriticalLimit: 20m, highCriticalLimit: 5m));
    }

    [Fact]
    public void An_interval_without_limits_reports_no_limits()
    {
        Assert.False(CriticalInterval.None.HasLimits);
    }
}
