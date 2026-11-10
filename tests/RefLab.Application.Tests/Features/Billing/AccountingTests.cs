using RefLab.Application.Common;
using RefLab.Application.Features.Billing;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Billing;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Billing;

public class GetPaymentsTests
{
    [Fact]
    public async Task The_payments_are_listed_newest_first()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        var first = Payment.Create(order.Id, TestData.Now, Money.Create(100m, "EGP"), 1);
        var second = Payment.Create(order.Id, TestData.Now.AddDays(1), Money.Create(200m, "EGP"), 2);
        host.Db.Seed(first, second);

        var result = await host.Sender.Send(new GetPaymentsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal(200m, result[0].Amount);
    }

    [Fact]
    public async Task The_payments_are_filtered_by_order()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var first = TestData.CreateOrder(patient);
        var second = TestData.CreateOrder(patient);
        host.Db.Seed(patient, first, second);
        host.Db.Seed(Payment.Create(first.Id, TestData.Now, Money.Create(100m, "EGP"), 1));
        host.Db.Seed(Payment.Create(second.Id, TestData.Now, Money.Create(200m, "EGP"), 2));

        var result = await host.Sender.Send(new GetPaymentsQuery(OrderId: first.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(first.Id, result[0].OrderId);
        Assert.Equal(100m, result[0].Amount);
    }

    [Fact]
    public async Task Every_payment_is_returned_when_no_order_is_selected()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var first = TestData.CreateOrder(patient);
        var second = TestData.CreateOrder(patient);
        host.Db.Seed(patient, first, second);
        host.Db.Seed(Payment.Create(first.Id, TestData.Now, Money.Create(100m, "EGP"), 1));
        host.Db.Seed(Payment.Create(second.Id, TestData.Now, Money.Create(200m, "EGP"), 2));

        var result = await host.Sender.Send(new GetPaymentsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }
}

public class StockTakeTests
{
    [Fact]
    public async Task The_period_stock_take_reports_the_income_and_the_commission()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        order.SetReferringParty(19);
        order.SetAmounts(
            Money.Create(1000m, "EGP"),
            Money.Create(1000m, "EGP"),
            Money.Create(1000m, "EGP"));
        host.Db.Seed(patient, order, TestData.CreateReferringParty());
        host.Db.Seed(Payment.Create(order.Id, TestData.Now, Money.Create(1000m, "EGP"), 1));

        var result = await host.Sender.Send(
            new RunPeriodStockTakeQuery(TestData.Now.AddDays(-1), TestData.Now.AddDays(1), "EGP"),
            CancellationToken.None);

        Assert.Equal(1, result.OrderCount);
        Assert.Equal(1000m, result.GrossAmount);
        Assert.Equal(100m, result.CommissionAmount);
    }

    [Fact]
    public async Task A_period_that_ends_before_it_starts_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new RunPeriodStockTakeQuery(TestData.Now.AddDays(1), TestData.Now, "EGP"),
                CancellationToken.None));
    }

    [Fact]
    public async Task The_stock_take_of_a_referring_party_covers_only_its_orders()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var party = TestData.CreateReferringParty();
        var order = TestData.CreateOrder(patient);
        order.SetReferringParty(party.Id);
        var other = TestData.CreateOrder(patient);
        host.Db.Seed(patient, party, order, other);

        var result = await host.Sender.Send(
            new RunStockTakeByPhysicianOrPartyQuery(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                "EGP",
                "CL-01",
                null),
            CancellationToken.None);

        Assert.Equal(1, result.OrderCount);
        Assert.Equal("CL-01", result.ReferringPartyCode);
    }

    [Fact]
    public async Task A_stock_take_without_a_party_or_a_physician_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new RunStockTakeByPhysicianOrPartyQuery(
                    TestData.Now.AddDays(-1),
                    TestData.Now.AddDays(1),
                    "EGP",
                    null,
                    null),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_referring_party_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(
                new RunStockTakeByPhysicianOrPartyQuery(
                    TestData.Now.AddDays(-1),
                    TestData.Now.AddDays(1),
                    "EGP",
                    "MISSING",
                    null),
                CancellationToken.None));
    }
}

public class SetDefaultAccountTypeTests
{
    [Fact]
    public async Task The_default_account_type_is_stored_in_the_laboratory_settings()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(new SetDefaultAccountTypeCommand("CASH"), CancellationToken.None);

        Assert.Equal("CASH", result.DefaultAccountType);
        Assert.Single(host.Db.Items<Domain.Configuration.LaboratorySettings>());
    }
}
