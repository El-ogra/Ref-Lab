using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// A ratio result carries the two measurements it was computed from, so that
/// the derivation stays reproducible, and it is reported as an ordinary result
/// value.
/// </summary>
public class RatioResultTests
{
    [Fact]
    public void The_analyte_names_and_the_unit_are_trimmed()
    {
        var ratio = RatioResult.Create("  SGOT (AST)  ", "  SGPT (ALT)  ", 41m, 12m, "  ratio  ");

        Assert.Equal("SGOT (AST)", ratio.NumeratorAnalyte);
        Assert.Equal("SGPT (ALT)", ratio.DenominatorAnalyte);
        Assert.Equal("ratio", ratio.Unit);
    }

    [Fact]
    public void A_blank_unit_becomes_null()
    {
        var ratio = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m, "   ");

        Assert.Null(ratio.Unit);
    }

    [Fact]
    public void The_measurements_are_kept()
    {
        var ratio = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m);

        Assert.Equal(41m, ratio.Numerator);
        Assert.Equal(12m, ratio.Denominator);
    }

    [Fact]
    public void A_zero_denominator_is_rejected()
    {
        Assert.Throws<InvalidResultValueException>(() =>
            RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 0m));
    }

    [Fact]
    public void A_blank_numerator_analyte_is_rejected()
    {
        Assert.Throws<InvalidResultValueException>(() =>
            RatioResult.Create("   ", "SGPT (ALT)", 41m, 12m));
    }

    [Fact]
    public void A_blank_denominator_analyte_is_rejected()
    {
        Assert.Throws<InvalidResultValueException>(() =>
            RatioResult.Create("SGOT (AST)", "   ", 41m, 12m));
    }

    [Theory]
    [InlineData(41, 12, 3.42)]
    [InlineData(3, 8, 0.38)]
    [InlineData(1, 3, 0.33)]
    public void The_ratio_is_rounded_to_two_decimal_places_away_from_zero(
        int numerator,
        int denominator,
        double expected)
    {
        var ratio = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", numerator, denominator);

        Assert.Equal((decimal)expected, ratio.Value);
    }

    [Fact]
    public void The_exact_value_is_the_unrounded_quotient()
    {
        var ratio = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m);

        Assert.Equal(41m / 12m, ratio.ExactValue);
        Assert.NotEqual(ratio.Value, ratio.ExactValue);
    }

    [Fact]
    public void The_display_name_is_the_two_analytes()
    {
        var ratio = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m);

        Assert.Equal("SGOT (AST)/SGPT (ALT)", ratio.DisplayName);
    }

    [Fact]
    public void The_result_value_carries_the_rounded_ratio()
    {
        var ratio = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m);

        var value = ratio.ToResultValue();

        Assert.Equal("3.42", value.Text);
        Assert.Equal(3.42m, value.NumericValue);
        Assert.Null(value.Unit);
    }

    [Fact]
    public void The_result_value_carries_the_unit()
    {
        var ratio = RatioResult.Create("Albumin", "Globulin", 3m, 8m, "ratio");

        var value = ratio.ToResultValue();

        Assert.Equal("ratio", value.Unit);
    }

    [Fact]
    public void The_result_value_writes_at_most_two_decimals()
    {
        var ratio = RatioResult.Create("Albumin", "Globulin", 10m, 4m);

        var value = ratio.ToResultValue();

        Assert.Equal("2.5", value.Text);
    }

    [Fact]
    public void Two_ratios_built_from_the_same_measurements_are_equal()
    {
        var first = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m);
        var second = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m);

        Assert.Equal(first, second);
        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void A_ratio_built_from_another_analyte_is_not_equal()
    {
        var first = RatioResult.Create("SGOT (AST)", "SGPT (ALT)", 41m, 12m);
        var second = RatioResult.Create("SGPT (ALT)", "SGPT (ALT)", 41m, 12m);

        Assert.NotEqual(first, second);
    }
}
