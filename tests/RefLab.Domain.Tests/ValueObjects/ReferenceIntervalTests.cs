using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// The reference interval carries the limits, the texts and the human-readable
/// sentence that the printed report renders.
/// </summary>
public class ReferenceIntervalTests
{
    [Fact]
    public void Create_keeps_the_limits_and_the_texts()
    {
        var interval = ReferenceInterval.Create(
            lowLimit: 3.8m,
            highLimit: 5.2m,
            lowExceptionText: "منخفضة",
            highExceptionText: "عالٍ",
            rangeText: "Adult 3.8 - 5.2 g/dL",
            unit: "g/dL");

        Assert.Equal(3.8m, interval.LowLimit);
        Assert.Equal(5.2m, interval.HighLimit);
        Assert.Equal("منخفضة", interval.LowExceptionText);
        Assert.Equal("عالٍ", interval.HighExceptionText);
        Assert.Equal("Adult 3.8 - 5.2 g/dL", interval.RangeText);
        Assert.Equal("g/dL", interval.Unit);
        Assert.True(interval.HasLimits);
    }

    [Fact]
    public void A_lower_limit_above_the_upper_limit_is_rejected()
    {
        Assert.Throws<InvalidReferenceIntervalException>(() =>
            ReferenceInterval.Create(lowLimit: 5.2m, highLimit: 3.8m));
    }

    [Fact]
    public void An_interval_without_limits_reports_no_limits()
    {
        var interval = ReferenceInterval.Create();

        Assert.False(interval.HasLimits);
    }

    [Fact]
    public void The_exception_text_is_selected_by_the_classification()
    {
        var interval = ReferenceInterval.Create(
            lowLimit: 3.8m,
            highLimit: 5.2m,
            lowExceptionText: "منخفضة",
            highExceptionText: "عالٍ",
            criticalInterval: CriticalInterval.Create(criticalExceptionText: "حرجة"));

        Assert.Equal("منخفضة", interval.ExceptionTextFor(ResultClassification.Low));
        Assert.Equal("عالٍ", interval.ExceptionTextFor(ResultClassification.High));
        Assert.Equal("حرجة", interval.ExceptionTextFor(ResultClassification.Critical));
        Assert.Null(interval.ExceptionTextFor(ResultClassification.Normal));
    }

    [Fact]
    public void The_comment_is_selected_by_the_classification()
    {
        var interval = ReferenceInterval.Create(
            lowLimit: 3.8m,
            highLimit: 5.2m,
            lowComment: "low comment",
            highComment: "high comment");

        Assert.Equal("low comment", interval.CommentFor(ResultClassification.Low));
        Assert.Equal("high comment", interval.CommentFor(ResultClassification.High));
        Assert.Null(interval.CommentFor(ResultClassification.Normal));
    }

    [Fact]
    public void Blank_texts_become_null()
    {
        var interval = ReferenceInterval.Create(lowLimit: 1m, highExceptionText: "   ");

        Assert.Null(interval.HighExceptionText);
    }
}
