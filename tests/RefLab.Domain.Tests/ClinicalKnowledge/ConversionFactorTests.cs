using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Tests.ClinicalKnowledge;

/// <summary>
/// The unit conversion dictionary.
/// </summary>
public class ConversionFactorTests
{
    [Fact]
    public void Convert_multiplies_the_value_by_the_factor()
    {
        var factor = ConversionFactor.Create("Albumin", "g/dL", "g/L", 10m);

        Assert.Equal(50m, factor.Convert(5m));
    }

    [Fact]
    public void The_decimal_counts_are_stored()
    {
        var factor = ConversionFactor.Create("Albumin", "g/dL", "g/L", 10m, fromDecimals: 2, toDecimals: 1);

        Assert.Equal(2, factor.FromDecimals);
        Assert.Equal(1, factor.ToDecimals);
    }

    [Fact]
    public void Create_rejects_an_empty_analyte_name()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            ConversionFactor.Create("  ", "g/dL", "g/L", 10m));
    }

    [Fact]
    public void Create_rejects_a_negative_decimal_count()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            ConversionFactor.Create("Albumin", "g/dL", "g/L", 10m, fromDecimals: -1));
    }
}
