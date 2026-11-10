using RefLab.Application.Common;
using RefLab.Application.Features.Reporting;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;

namespace RefLab.Application.Tests.Features.Reporting;

public class ProduceConsolidatedReportTests
{
    [Fact]
    public async Task The_report_covers_every_order_in_the_period()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateReviewedOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new ProduceConsolidatedReportCommand(TestData.Now.AddDays(-1), TestData.Now.AddDays(1), @"D:\out"),
            CancellationToken.None);

        Assert.Equal(1, result.OrderCount);
        Assert.Single(result.Rows);
        Assert.Equal("13.4", result.Rows[0].Value);
        Assert.NotNull(result.Document);
        Assert.Equal(1, host.Renderer.CallCount);
        Assert.Equal(1, host.Pdf.CallCount);
    }

    [Fact]
    public async Task A_period_that_ends_before_it_starts_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new ProduceConsolidatedReportCommand(TestData.Now.AddDays(1), TestData.Now, @"D:\out"),
                CancellationToken.None));
    }
}

public class ProduceEmptyReportTests
{
    [Fact]
    public async Task The_empty_template_is_produced_with_its_column_headings()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ProduceEmptyReportCommand("EMPTY REPORT", new[] { "Test", "Value" }, @"D:\out"),
            CancellationToken.None);

        Assert.Equal(new[] { "Test", "Value" }, result.ColumnNames);
        Assert.NotNull(result.Document);
        Assert.Equal(1, host.Pdf.CallCount);
    }

    [Fact]
    public async Task A_report_without_column_headings_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new ProduceEmptyReportCommand("EMPTY", Array.Empty<string>(), @"D:\out"),
                CancellationToken.None));
    }
}

public class PrintingTests
{
    [Fact]
    public async Task The_invoice_carries_the_order_total_and_the_lines()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        orderTest.SetPrice(Domain.ValueObjects.Money.Create(150m, "EGP"));
        order.SetAmounts(Domain.ValueObjects.Money.Create(150m, "EGP"), Domain.ValueObjects.Money.Create(150m, "EGP"));
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new PrintPatientInvoiceCommand(order.Id, @"D:\out"),
            CancellationToken.None);

        Assert.Equal(150m, result.TotalPrice);
        Assert.Equal("EGP", result.CurrencyCode);
        Assert.Single(result.Lines);
        Assert.NotNull(result.Document);
    }

    [Fact]
    public async Task An_unknown_order_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new PrintPatientInvoiceCommand(Guid.NewGuid(), @"D:\out"), CancellationToken.None));
    }

    [Fact]
    public async Task The_identity_card_carries_the_patient_details()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);

        var result = await host.Sender.Send(
            new PrintPatientIdCardCommand(patient.Id.Value, @"D:\out"),
            CancellationToken.None);

        Assert.Equal("محمد أحمد", result.PatientName);
        Assert.Equal(Sex.Male, result.Sex);
        Assert.NotNull(result.Document);
    }

    [Fact]
    public async Task An_unknown_patient_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new PrintPatientIdCardCommand(Guid.NewGuid(), @"D:\out"), CancellationToken.None));
    }
}

public class PrinterAndHeaderTests
{
    [Fact]
    public async Task A_printer_is_bound_to_a_role()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ConfigurePrintersCommand(PrinterRole.Report, "HP LaserJet"),
            CancellationToken.None);

        Assert.Equal("HP LaserJet", result.Printers["Report"]);
    }

    [Fact]
    public async Task A_printer_is_unbound_when_no_name_is_supplied()
    {
        using var host = new ApplicationTestHost();
        var settings = TestData.Settings();
        settings.BindPrinter(PrinterRole.Report, "HP LaserJet");
        host.Db.Seed(settings);

        var result = await host.Sender.Send(
            new ConfigurePrintersCommand(PrinterRole.Report, null),
            CancellationToken.None);

        Assert.False(result.Printers.ContainsKey("Report"));
    }

    [Fact]
    public async Task The_paper_size_and_the_header_texts_are_set()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ConfigureReportHeaderFooterCommand(PaperSize.A5, "منظومة المختبر", "ريال لاب", null),
            CancellationToken.None);

        Assert.Equal(PaperSize.A5, result.PaperSize);
        Assert.Equal("منظومة المختبر", result.GlobalHeaderText);
        Assert.Equal("ريال لاب", result.GlobalHeaderSecondaryText);
    }

    [Fact]
    public async Task An_unknown_printer_role_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new ConfigurePrintersCommand((PrinterRole)99, "x"), CancellationToken.None));
    }
}
