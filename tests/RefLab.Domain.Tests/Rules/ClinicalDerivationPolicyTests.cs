using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Rules;

namespace RefLab.Domain.Tests.Rules;

/// <summary>
/// The clinical derivation constants match the shipped manual value for value.
/// The factors are resolved from the configured constant group, never hard-coded.
/// </summary>
public class ClinicalDerivationPolicyTests
{
    private static ClinicalConstant CbcConstants() =>
        ClinicalConstant.Create("CBCCONS", "3.3", "6.25", "6.75", "7.5", "8.25");

    [Fact]
    public void The_haematocrit_is_the_haemoglobin_multiplied_by_the_factor()
    {
        var hematocrit = ClinicalDerivationPolicy.DeriveHematocrit(14m, CbcConstants());

        Assert.Equal(46.2m, hematocrit);
    }

    [Fact]
    public void The_haemoglobin_percentage_of_an_adult_male_uses_the_adult_male_factor()
    {
        var percentage = ClinicalDerivationPolicy.DeriveHemoglobinPercentage(14m, Sex.Male, 30m, CbcConstants());

        Assert.Equal(87.5m, percentage);
    }

    [Fact]
    public void The_haemoglobin_percentage_of_an_adult_female_uses_the_adult_female_factor()
    {
        var percentage = ClinicalDerivationPolicy.DeriveHemoglobinPercentage(14m, Sex.Female, 30m, CbcConstants());

        Assert.Equal(94.5m, percentage);
    }

    [Fact]
    public void The_haemoglobin_percentage_of_a_child_uses_the_child_factor()
    {
        var percentage = ClinicalDerivationPolicy.DeriveHemoglobinPercentage(12m, Sex.Male, 5m, CbcConstants());

        Assert.Equal(90m, percentage);
    }

    [Fact]
    public void The_haemoglobin_percentage_of_an_infant_uses_the_infant_factor()
    {
        var percentage = ClinicalDerivationPolicy.DeriveHemoglobinPercentage(10m, Sex.Female, 0.5m, CbcConstants());

        Assert.Equal(82.5m, percentage);
    }

    [Theory]
    [InlineData(0.5, Sex.Male, 5)]
    [InlineData(1, Sex.Female, 4)]
    [InlineData(12, Sex.Male, 4)]
    [InlineData(12.1, Sex.Female, 3)]
    [InlineData(30, Sex.Male, 2)]
    public void The_factor_slot_is_selected_by_age_and_sex(double age, Sex sex, int expectedSlot)
    {
        var slot = ClinicalDerivationPolicy.SelectHemoglobinFactorSlot(sex, (decimal)age);

        Assert.Equal(expectedSlot, slot);
    }

    [Fact]
    public void A_missing_factor_is_rejected()
    {
        var incomplete = ClinicalConstant.Create("CBCCONS");

        Assert.Throws<InvalidClinicalConstantException>(() =>
            ClinicalDerivationPolicy.DeriveHematocrit(14m, incomplete));
    }

    [Fact]
    public void A_non_positive_factor_is_rejected()
    {
        var broken = ClinicalConstant.Create("CBCCONS", "0");

        Assert.Throws<InvalidClinicalDerivationException>(() =>
            ClinicalDerivationPolicy.DeriveHematocrit(14m, broken));
    }

    [Fact]
    public void The_international_sensitivity_index_is_derived_from_the_control_times()
    {
        var index = ClinicalDerivationPolicy.DeriveInternationalSensitivityIndex(
            patientProthrombinTime: 20m,
            controlProthrombinTime: 12m,
            normalProthrombinTime: 18m,
            controlIsi: 1.33m);

        // (20 - 12) / (18 - 12) * 1.33 = 1.77333... -> 1.77
        Assert.Equal(1.77m, index);
    }

    [Fact]
    public void An_undefined_international_sensitivity_index_is_rejected()
    {
        Assert.Throws<InvalidClinicalDerivationException>(() =>
            ClinicalDerivationPolicy.DeriveInternationalSensitivityIndex(20m, 12m, 12m, 1.33m));
    }

    [Fact]
    public void The_control_concentration_is_interpolated_between_two_calibration_points()
    {
        var curve = new[]
        {
            new ProthrombinControlPoint(12m, 100m),
            new ProthrombinControlPoint(12.5m, 92m),
        };

        var concentration = ClinicalDerivationPolicy.InterpolateControlConcentration(curve, 12.25m);

        Assert.Equal(96m, concentration);
    }

    [Fact]
    public void The_control_concentration_of_an_exact_calibration_point_is_its_own_value()
    {
        var curve = new[]
        {
            new ProthrombinControlPoint(12m, 100m),
            new ProthrombinControlPoint(12.5m, 92m),
        };

        var concentration = ClinicalDerivationPolicy.InterpolateControlConcentration(curve, 12.5m);

        Assert.Equal(92m, concentration);
    }

    [Fact]
    public void A_control_time_outside_the_calibrated_range_is_rejected()
    {
        var curve = new[]
        {
            new ProthrombinControlPoint(12m, 100m),
            new ProthrombinControlPoint(12.5m, 92m),
        };

        Assert.Throws<InvalidClinicalDerivationException>(() =>
            ClinicalDerivationPolicy.InterpolateControlConcentration(curve, 20m));
    }

    [Fact]
    public void An_empty_calibration_curve_is_rejected()
    {
        Assert.Throws<InvalidClinicalDerivationException>(() =>
            ClinicalDerivationPolicy.InterpolateControlConcentration(Array.Empty<ProthrombinControlPoint>(), 12m));
    }

    [Fact]
    public void The_derived_ratio_is_the_quotient_of_the_two_measurements()
    {
        var ratio = ClinicalDerivationPolicy.DeriveRatio("SGOT (AST)", "SGPT (ALT)", 41m, 12m);

        Assert.Equal(3.42m, ratio.Value);
        Assert.Equal("SGOT (AST)/SGPT (ALT)", ratio.DisplayName);
    }

    [Fact]
    public void The_adult_factor_of_a_patient_of_unknown_sex_is_refused()
    {
        // The adult factors are sex-specific: 6.25 for a male and 6.75 for a
        // female. There is no factor for a patient whose sex was never
        // established, so the derivation is refused rather than guessed.
        Assert.Throws<InvalidClinicalDerivationException>(() =>
            ClinicalDerivationPolicy.SelectHemoglobinFactorSlot(Sex.Unknown, 30m));
    }

    [Theory]
    [InlineData(Sex.Male)]
    [InlineData(Sex.Female)]
    [InlineData(Sex.Unknown)]
    public void The_infant_factor_does_not_depend_on_the_sex(Sex sex)
    {
        // The slot is selected on the age in years, and five months is below
        // the one-year limit.
        var ageInYears = 5m / 12m;

        var slot = ClinicalDerivationPolicy.SelectHemoglobinFactorSlot(sex, ageInYears);

        Assert.Equal(ClinicalDerivationPolicy.InfantFactorSlot, slot);
    }

    [Theory]
    [InlineData(Sex.Male)]
    [InlineData(Sex.Female)]
    [InlineData(Sex.Unknown)]
    public void The_child_factor_does_not_depend_on_the_sex(Sex sex)
    {
        var slot = ClinicalDerivationPolicy.SelectHemoglobinFactorSlot(sex, 10m);

        Assert.Equal(ClinicalDerivationPolicy.ChildFactorSlot, slot);
    }

    [Fact]
    public void The_adult_factors_of_a_male_and_of_a_female_are_unchanged()
    {
        var maleSlot = ClinicalDerivationPolicy.SelectHemoglobinFactorSlot(Sex.Male, 30m);
        var femaleSlot = ClinicalDerivationPolicy.SelectHemoglobinFactorSlot(Sex.Female, 30m);

        Assert.Equal(ClinicalDerivationPolicy.AdultMaleFactorSlot, maleSlot);
        Assert.Equal(ClinicalDerivationPolicy.AdultFemaleFactorSlot, femaleSlot);
    }
}
