using RefLab.Domain.Configuration;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Tests.Configuration;

public class LaboratorySettingsTests
{
    [Fact]
    public void The_settings_are_created_with_the_legacy_defaults()
    {
        var settings = LaboratorySettings.Create();

        Assert.Equal(LaboratorySettings.SingletonId, settings.Id);
        Assert.Equal(PaperSize.A4, settings.PaperSize);
        Assert.Equal(1984, settings.ReportSequenceStart);
        Assert.True(settings.IsBarcodeAutoCreated);
        Assert.True(settings.IsWorkSheetPrinted);
    }

    [Fact]
    public void A_printer_is_bound_to_a_role_and_can_be_unbound()
    {
        var settings = LaboratorySettings.Create();

        settings.BindPrinter(PrinterRole.Report, "HP LaserJet");
        Assert.Equal("HP LaserJet", settings.GetPrinter(PrinterRole.Report));

        settings.UnbindPrinter(PrinterRole.Report);
        Assert.Null(settings.GetPrinter(PrinterRole.Report));
    }

    [Fact]
    public void An_empty_printer_name_is_rejected()
    {
        var settings = LaboratorySettings.Create();

        Assert.Throws<InvalidPatientOrderException>(() => settings.BindPrinter(PrinterRole.Report, "  "));
    }

    [Fact]
    public void A_negative_margin_is_rejected()
    {
        var settings = LaboratorySettings.Create();

        Assert.Throws<InvalidPatientOrderException>(() => settings.SetMargins(-1m, 0m, 0m, 0m));
    }

    [Fact]
    public void The_report_sequence_origin_must_be_positive()
    {
        var settings = LaboratorySettings.Create();

        Assert.Throws<InvalidPatientOrderException>(() => settings.SetReportSequenceStart(0));
    }

    [Fact]
    public void The_global_header_texts_are_normalised()
    {
        var settings = LaboratorySettings.Create();

        settings.SetGlobalHeaderTexts("  المنظومة البرمجية  ", "   ");

        Assert.Equal("المنظومة البرمجية", settings.GlobalHeaderText);
        Assert.Null(settings.GlobalHeaderSecondaryText);
    }
}
