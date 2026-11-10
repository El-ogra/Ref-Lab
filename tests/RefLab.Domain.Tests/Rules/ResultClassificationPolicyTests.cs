using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Rules;

/// <summary>
/// The classification is three-valued and ordered: normal, then abnormal, then
/// critical. A critical limit overrides any abnormal classification.
/// </summary>
public class ResultClassificationPolicyTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    private static ReferenceSnapshot CreateSnapshot(
        decimal? low = null,
        decimal? high = null,
        decimal? lowCritical = null,
        decimal? highCritical = null)
    {
        return ReferenceSnapshot.Capture(
            56,
            "Hgb",
            SexScope.Male,
            AgeBand.Create(0m, 120m, AgeUnit.Years),
            ReferenceInterval.Create(
                lowLimit: low,
                highLimit: high,
                criticalInterval: CriticalInterval.Create(lowCritical, highCritical)),
            Moment);
    }

    [Fact]
    public void A_value_inside_the_interval_is_normal()
    {
        var result = ResultValue.FromNumber(14m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m));

        Assert.Equal(ResultClassification.Normal, classification);
    }

    [Fact]
    public void A_value_above_the_upper_limit_is_high()
    {
        var result = ResultValue.FromNumber(18m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m));

        Assert.Equal(ResultClassification.High, classification);
    }

    [Fact]
    public void A_value_below_the_lower_limit_is_low()
    {
        var result = ResultValue.FromNumber(9m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m));

        Assert.Equal(ResultClassification.Low, classification);
    }

    [Fact]
    public void A_value_equal_to_the_upper_limit_is_inside_the_interval()
    {
        var result = ResultValue.FromNumber(16m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m));

        Assert.Equal(ResultClassification.Normal, classification);
    }

    [Fact]
    public void A_value_equal_to_the_lower_limit_is_inside_the_interval()
    {
        var result = ResultValue.FromNumber(12m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m));

        Assert.Equal(ResultClassification.Normal, classification);
    }

    [Fact]
    public void A_value_beyond_the_high_critical_limit_is_critical()
    {
        var result = ResultValue.FromNumber(21m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m, highCritical: 20m));

        Assert.Equal(ResultClassification.Critical, classification);
    }

    [Fact]
    public void A_value_below_the_low_critical_limit_is_critical()
    {
        var result = ResultValue.FromNumber(4m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m, lowCritical: 5m));

        Assert.Equal(ResultClassification.Critical, classification);
    }

    [Fact]
    public void A_critical_limit_overrides_the_abnormal_classification()
    {
        var result = ResultValue.FromNumber(21m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m, highCritical: 20m));

        Assert.Equal(ResultClassification.Critical, classification);
        Assert.NotEqual(ResultClassification.High, classification);
    }

    [Fact]
    public void A_value_equal_to_the_critical_limit_is_not_critical()
    {
        var result = ResultValue.FromNumber(20m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m, highCritical: 20m));

        Assert.Equal(ResultClassification.High, classification);
    }

    [Fact]
    public void A_non_numeric_result_cannot_be_classified()
    {
        var result = ResultValue.Create("Positive");

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot(12m, 16m));

        Assert.Null(classification);
    }

    [Fact]
    public void A_result_without_a_frozen_range_cannot_be_classified()
    {
        var result = ResultValue.FromNumber(14m);

        var classification = ResultClassificationPolicy.Classify(result, snapshot: null);

        Assert.Null(classification);
    }

    [Fact]
    public void A_snapshot_without_limits_cannot_be_classified()
    {
        var result = ResultValue.FromNumber(14m);

        var classification = ResultClassificationPolicy.Classify(result, CreateSnapshot());

        Assert.Null(classification);
    }

    [Fact]
    public void An_absent_result_cannot_be_classified()
    {
        var classification = ResultClassificationPolicy.Classify(null, CreateSnapshot(12m, 16m));

        Assert.Null(classification);
    }

    [Fact]
    public void A_range_with_only_an_upper_limit_only_detects_high_values()
    {
        var snapshot = CreateSnapshot(low: null, high: 16m);

        Assert.Equal(ResultClassification.Normal, ResultClassificationPolicy.Classify(ResultValue.FromNumber(5m), snapshot));
        Assert.Equal(ResultClassification.High, ResultClassificationPolicy.Classify(ResultValue.FromNumber(18m), snapshot));
    }
}
