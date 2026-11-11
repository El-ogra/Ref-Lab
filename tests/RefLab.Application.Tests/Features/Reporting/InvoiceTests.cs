using RefLab.Application.Common;
using RefLab.Application.Features.Reporting;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Billing;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Reporting;

public sealed class InvoiceTests
{
    [Fact]
    public async Task The_amount_due_is_the_discounted_total_less_the_payments_recorded_against_the_order()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        order.SetAmounts(
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"));
        host.Db.Seed(patient, order);
        host.Db.Seed(Payment.Create(order.Id, TestData.Now, Money.Create(40m, "EGP"), 1));

        var result = await host.Sender.Send(
            new PrintPatientInvoiceCommand(order.Id, @"D:\out"),
            CancellationToken.None);

        Assert.Equal(100m, result.TotalPrice);
        Assert.Equal(40m, result.AmountPaid);
        Assert.Equal(60m, result.AmountDue);
    }

    [Fact]
    public async Task An_order_without_a_payment_is_due_its_discounted_total()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        order.SetAmounts(
            Money.Create(100m, "EGP"),
            Money.Create(80m, "EGP"),
            Money.Create(80m, "EGP"));
        host.Db.Seed(patient, order);

        var result = await host.Sender.Send(
            new PrintPatientInvoiceCommand(order.Id, @"D:\out"),
            CancellationToken.None);

        Assert.Equal(0m, result.AmountPaid);
        Assert.Equal(80m, result.AmountDue);
    }

    [Fact]
    public async Task The_amount_due_is_never_negative()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        order.SetAmounts(
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"));
        host.Db.Seed(patient, order);
        host.Db.Seed(Payment.Create(order.Id, TestData.Now, Money.Create(150m, "EGP"), 1));

        var result = await host.Sender.Send(
            new PrintPatientInvoiceCommand(order.Id, @"D:\out"),
            CancellationToken.None);

        Assert.Equal(150m, result.AmountPaid);
        Assert.Equal(0m, result.AmountDue);
    }

    [Fact]
    public async Task An_unknown_order_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new PrintPatientInvoiceCommand(Guid.NewGuid(), @"D:\out"), CancellationToken.None));
    }
}
