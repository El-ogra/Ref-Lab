using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// The legacy system stores results as text, so the numeric interpretation is a
/// derived view rather than the source of truth.
/// </summary>
public class ResultValueTests
{
    [Fact]
    public void A_numeric_text_yields_a_numeric_value()
    {
        var value = ResultValue.Create("13.4");

        Assert.True(value.IsNumeric);
        Assert.Equal(13.4m, value.NumericValue);
        Assert.Equal("13.4", value.Text);
    }

    [Fact]
    public void A_numeric_text_is_parsed_with_the_invariant_culture()
    {
        var value = ResultValue.Create("13,4");

        Assert.True(value.IsNumeric);
        Assert.Equal(134m, value.NumericValue);
    }

    [Theory]
    [InlineData("Positive")]
    [InlineData("Negative")]
    [InlineData("Not detected")]
    [InlineData("++")]
    public void A_qualitative_text_has_no_numeric_value(string text)
    {
        var value = ResultValue.Create(text);

        Assert.False(value.IsNumeric);
        Assert.Null(value.NumericValue);
        Assert.Equal(text, value.Text);
    }

    [Fact]
    public void Surrounding_whitespace_is_removed()
    {
        var value = ResultValue.Create("  7.2  ");

        Assert.Equal("7.2", value.Text);
    }

    [Fact]
    public void An_empty_text_is_rejected()
    {
        Assert.Throws<InvalidResultValueException>(() => ResultValue.Create("   "));
    }

    [Fact]
    public void FromNumber_formats_the_text_in_the_invariant_culture()
    {
        var value = ResultValue.FromNumber(13.4m, "g/dL");

        Assert.Equal("13.4", value.Text);
        Assert.Equal(13.4m, value.NumericValue);
        Assert.Equal("g/dL", value.Unit);
    }

    [Fact]
    public void A_missing_unit_becomes_null()
    {
        var value = ResultValue.Create("5");

        Assert.Null(value.Unit);
    }
}
