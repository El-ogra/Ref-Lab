using RefLab.Application.Common;
using RefLab.Application.Features.Derivation;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;

namespace RefLab.Application.Tests.Features.Derivation;

public class DerivationTests
{
    [Fact]
    public async Task The_haematocrit_is_the_haemoglobin_multiplied_by_the_configured_factor()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CbcConstants());

        var result = await host.Sender.Send(new DeriveHematocritQuery(14m), CancellationToken.None);

        Assert.Equal(46.2m, result.Value);
        Assert.Equal("Hct", result.Name);
    }

    [Fact]
    public async Task A_missing_constant_group_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new DeriveHematocritQuery(14m), CancellationToken.None));
    }

    [Fact]
    public async Task A_negative_haemoglobin_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new DeriveHematocritQuery(-1m), CancellationToken.None));
    }

    [Fact]
    public async Task The_haemoglobin_percentage_uses_the_adult_male_factor()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CbcConstants());

        var result = await host.Sender.Send(
            new DeriveHemoglobinPercentageQuery(14m, Sex.Male, 30m),
            CancellationToken.None);

        Assert.Equal(87.5m, result.Value);
    }

    [Fact]
    public async Task The_haemoglobin_percentage_uses_the_infant_factor_below_one_year()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CbcConstants());

        var result = await host.Sender.Send(
            new DeriveHemoglobinPercentageQuery(10m, Sex.Female, 0.5m),
            CancellationToken.None);

        Assert.Equal(82.5m, result.Value);
    }

    [Fact]
    public async Task The_international_sensitivity_index_is_derived()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new DeriveInternationalSensitivityIndexQuery(20m, 12m, 18m, 1.33m),
            CancellationToken.None);

        Assert.Equal(1.77m, result.Value);
        Assert.Equal("ISI", result.Name);
    }

    [Fact]
    public async Task A_prothrombin_time_that_is_not_positive_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new DeriveInternationalSensitivityIndexQuery(0m, 12m, 18m, 1.33m), CancellationToken.None));
    }

    [Fact]
    public async Task The_control_concentration_is_interpolated_from_the_calibration_curve()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.ControlPoint(1, 12m, 100m), TestData.ControlPoint(2, 12.5m, 92m));

        var result = await host.Sender.Send(new InterpolateProthrombinControlQuery(12.25m), CancellationToken.None);

        Assert.Equal(96m, result.Value);
    }

    [Fact]
    public async Task A_missing_calibration_curve_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new InterpolateProthrombinControlQuery(12m), CancellationToken.None));
    }

    [Fact]
    public async Task A_control_time_outside_the_calibrated_range_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.ControlPoint(1, 12m, 100m), TestData.ControlPoint(2, 12.5m, 92m));

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new InterpolateProthrombinControlQuery(30m), CancellationToken.None));
    }
}
