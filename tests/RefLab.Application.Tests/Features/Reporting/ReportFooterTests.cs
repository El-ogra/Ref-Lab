using RefLab.Application.Features.Configuration;
using RefLab.Application.Features.Reporting;
using RefLab.Application.Tests.Fakes;

namespace RefLab.Application.Tests.Features.Reporting;

public sealed class ReportFooterTests
{
    [Fact]
    public async Task The_report_footer_is_set_and_the_connection_receipt_footer_is_left_alone()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ConfigureReportHeaderFooterCommand(FooterText: "REPORT FOOTER"),
            CancellationToken.None);

        Assert.Equal("REPORT FOOTER", result.ReportFooterText);
        Assert.Null(result.ConnectionReceiptFooter);
    }

    [Fact]
    public async Task The_report_footer_does_not_overwrite_the_connection_receipt_footer()
    {
        using var host = new ApplicationTestHost();

        await host.Sender.Send(
            new ConfigureConnectionReceiptCommand(Footer: "RECEIPT FOOTER"),
            CancellationToken.None);

        var result = await host.Sender.Send(
            new ConfigureReportHeaderFooterCommand(FooterText: "REPORT FOOTER"),
            CancellationToken.None);

        Assert.Equal("REPORT FOOTER", result.ReportFooterText);
        Assert.Equal("RECEIPT FOOTER", result.ConnectionReceiptFooter);
    }

    [Fact]
    public async Task The_connection_receipt_footer_is_set_and_the_report_footer_is_left_alone()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ConfigureConnectionReceiptCommand(Footer: "RECEIPT FOOTER"),
            CancellationToken.None);

        Assert.Equal("RECEIPT FOOTER", result.ConnectionReceiptFooter);
        Assert.Null(result.ReportFooterText);
    }

    [Fact]
    public async Task The_connection_receipt_footer_does_not_overwrite_the_report_footer()
    {
        using var host = new ApplicationTestHost();

        await host.Sender.Send(
            new ConfigureReportHeaderFooterCommand(FooterText: "REPORT FOOTER"),
            CancellationToken.None);

        var result = await host.Sender.Send(
            new ConfigureConnectionReceiptCommand(Footer: "RECEIPT FOOTER"),
            CancellationToken.None);

        Assert.Equal("RECEIPT FOOTER", result.ConnectionReceiptFooter);
        Assert.Equal("REPORT FOOTER", result.ReportFooterText);
    }

    [Fact]
    public async Task The_layout_text_is_set_and_the_global_header_is_left_alone()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new CustomiseReportLayoutCommand(LayoutText: "LAYOUT"),
            CancellationToken.None);

        Assert.Equal("LAYOUT", result.LayoutText);
        Assert.Null(result.GlobalHeaderText);
    }
}
