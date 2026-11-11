using RefLab.Application.Common;
using RefLab.Application.Features.Configuration;
using RefLab.Application.Features.Reporting;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Configuration;

namespace RefLab.Application.Tests.Features.Configuration;

public class SettingsTests
{
    [Fact]
    public async Task The_report_margins_and_the_background_are_set()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new CustomiseReportLayoutCommand(1m, 1.5m, 2m, 2.5m, "#FFFFFF", "LAYOUT"),
            CancellationToken.None);

        Assert.Equal(1m, result.LeftMargin);
        Assert.Equal(2.5m, result.BottomMargin);
        Assert.Equal("#FFFFFF", result.BackgroundColour);

        // The layout text describes the report layout. It is not the global
        // header, which carries the laboratory identity printed above the
        // report titles, so configuring the layout must leave the identity
        // header alone.
        Assert.Equal("LAYOUT", result.LayoutText);
        Assert.Null(result.GlobalHeaderText);
    }

    [Fact]
    public async Task The_layout_text_does_not_overwrite_the_global_header()
    {
        using var host = new ApplicationTestHost();

        await host.Sender.Send(
            new ConfigureReportHeaderFooterCommand(GlobalHeaderText: "IDENTITY"),
            CancellationToken.None);

        var result = await host.Sender.Send(
            new CustomiseReportLayoutCommand(null, null, null, null, null, "LAYOUT"),
            CancellationToken.None);

        Assert.Equal("LAYOUT", result.LayoutText);
        Assert.Equal("IDENTITY", result.GlobalHeaderText);
    }

    [Fact]
    public async Task A_negative_margin_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new CustomiseReportLayoutCommand(-1m, null, null, null, null, null), CancellationToken.None));
    }

    [Fact]
    public async Task The_connection_receipt_is_configured()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ConfigureConnectionReceiptCommand("RECEIPT HEADER", "RECEIPT FOOTER"),
            CancellationToken.None);

        Assert.Equal("RECEIPT HEADER", result.ConnectionReceiptHeader);
        Assert.Equal("RECEIPT FOOTER", result.ConnectionReceiptFooter);
    }

    [Fact]
    public async Task A_receipt_without_a_header_or_a_footer_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new ConfigureConnectionReceiptCommand(null, null), CancellationToken.None));
    }

    [Fact]
    public async Task The_operating_settings_are_set()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ConfigureLaboratorySettingsCommand(PaperSize.A4, 2000, false, true, 1, "CASH"),
            CancellationToken.None);

        Assert.Equal(2000, result.ReportSequenceStart);
        Assert.False(result.IsBarcodeAutoCreated);
        Assert.Equal("CASH", result.DefaultAccountType);
    }

    [Fact]
    public async Task The_settings_are_created_when_they_do_not_exist_yet()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ConfigureLaboratorySettingsCommand(null, null, null, null, null, null),
            CancellationToken.None);

        Assert.Equal(PaperSize.A4, result.PaperSize);
        Assert.Single(host.Db.Items<LaboratorySettings>());
    }

    [Fact]
    public async Task An_operator_without_the_settings_permission_is_rejected()
    {
        using var host = new ApplicationTestHost(grantAllPermissions: false);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(new ConfigureConnectionReceiptCommand("h", "f"), CancellationToken.None));
    }
}
