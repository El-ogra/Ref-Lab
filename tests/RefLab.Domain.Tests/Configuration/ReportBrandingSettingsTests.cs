using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Tests.Configuration;

/// <summary>
/// The laboratory owns the branding of the documents it prints: the doctor's
/// signature, the doctor's name, the line of thanks, the logos and the size
/// the logos are printed at. These tests prove that the branding is stored
/// exactly as it was configured, and that a logo which could never be
/// printed is rejected rather than stored.
/// </summary>
public sealed class ReportBrandingSettingsTests
{
    [Fact]
    public void The_branding_is_stored()
    {
        var settings = LaboratorySettings.Create();

        settings.SetBranding(
            "DrSignature",
            "Dr Ahmed Hassan",
            "Thanks for your trust",
            "/logos/lab.png",
            "/logos/report.png",
            120,
            40,
            ReportHeadingMode.Abnormal);

        Assert.Equal("DrSignature", settings.DoctorSignature);
        Assert.Equal("Dr Ahmed Hassan", settings.DoctorName);
        Assert.Equal("Thanks for your trust", settings.ThanksText);
        Assert.Equal("/logos/lab.png", settings.LabLogoPath);
        Assert.Equal("/logos/report.png", settings.ReportLogoPath);
        Assert.Equal(120, settings.LogoWidthPx);
        Assert.Equal(40, settings.LogoHeightPx);
        Assert.Equal(ReportHeadingMode.Abnormal, settings.HeadingMode);
    }

    [Fact]
    public void A_logo_width_that_is_not_positive_is_rejected()
    {
        var settings = LaboratorySettings.Create();

        Assert.Throws<InvalidPatientOrderException>(() =>
            settings.SetBranding(null, null, null, null, null, 0, 40, null));

        Assert.Throws<InvalidPatientOrderException>(() =>
            settings.SetBranding(null, null, null, null, null, -1, 40, null));
    }

    [Fact]
    public void A_logo_height_that_is_not_positive_is_rejected()
    {
        var settings = LaboratorySettings.Create();

        Assert.Throws<InvalidPatientOrderException>(() =>
            settings.SetBranding(null, null, null, null, null, 120, 0, null));

        Assert.Throws<InvalidPatientOrderException>(() =>
            settings.SetBranding(null, null, null, null, null, 120, -1, null));
    }

    [Fact]
    public void A_branding_value_that_is_blank_is_normalised_to_null()
    {
        var settings = LaboratorySettings.Create();

        settings.SetBranding(
            "   ",
            "Dr Ahmed Hassan",
            "  ",
            "/logos/lab.png",
            "   ",
            120,
            40,
            ReportHeadingMode.All);

        Assert.Null(settings.DoctorSignature);
        Assert.Null(settings.ThanksText);
        Assert.Null(settings.ReportLogoPath);
        Assert.Equal("Dr Ahmed Hassan", settings.DoctorName);
        Assert.Equal("/logos/lab.png", settings.LabLogoPath);
    }

    [Fact]
    public void The_report_carries_every_result_until_the_heading_mode_is_chosen()
    {
        var settings = LaboratorySettings.Create();

        Assert.Equal(ReportHeadingMode.All, settings.HeadingMode);
    }
}
