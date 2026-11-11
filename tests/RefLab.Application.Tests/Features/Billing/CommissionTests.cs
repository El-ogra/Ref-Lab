using RefLab.Application.Features.Billing;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Billing;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Billing;

public sealed class CommissionTests
{
    [Fact]
    public async Task The_stock_take_of_a_referring_party_reports_the_commission_it_earned()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var party = ReferringParty.Create(19, "PRY-1", "Clinic One");
        party.SetCommissionRate(Percentage.Create(10m));

        var order = TestData.CreateOrder(patient);
        order.SetReferringParty(party.Id);
        order.SetAmounts(
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"));

        host.Db.Seed(patient, party, order);

        var result = await host.Sender.Send(
            new RunStockTakeByPhysicianOrPartyQuery(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                "EGP",
                "PRY-1",
                null),
            CancellationToken.None);

        Assert.Equal(1, result.OrderCount);
        Assert.Equal(10m, result.CommissionAmount);
    }

    [Fact]
    public async Task A_party_without_a_commission_rate_earns_no_commission()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var party = ReferringParty.Create(19, "PRY-1", "Clinic One");

        var order = TestData.CreateOrder(patient);
        order.SetReferringParty(party.Id);
        order.SetAmounts(
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"));

        host.Db.Seed(patient, party, order);

        var byParty = await host.Sender.Send(
            new RunStockTakeByPhysicianOrPartyQuery(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                "EGP",
                "PRY-1",
                null),
            CancellationToken.None);

        var byPeriod = await host.Sender.Send(
            new RunPeriodStockTakeQuery(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                "EGP"),
            CancellationToken.None);

        Assert.Equal(0m, byParty.CommissionAmount);
        Assert.Equal(0m, byPeriod.CommissionAmount);
    }

    [Fact]
    public async Task The_period_stock_take_reports_the_same_commission_as_the_stock_take_of_the_party()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var party = ReferringParty.Create(19, "PRY-1", "Clinic One");
        party.SetCommissionRate(Percentage.Create(10m));

        var order = TestData.CreateOrder(patient);
        order.SetReferringParty(party.Id);
        order.SetAmounts(
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"));

        host.Db.Seed(patient, party, order);

        var byParty = await host.Sender.Send(
            new RunStockTakeByPhysicianOrPartyQuery(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                "EGP",
                "PRY-1",
                null),
            CancellationToken.None);

        var byPeriod = await host.Sender.Send(
            new RunPeriodStockTakeQuery(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                "EGP"),
            CancellationToken.None);

        Assert.Equal(10m, byParty.CommissionAmount);
        Assert.Equal(byParty.CommissionAmount, byPeriod.CommissionAmount);
    }

    [Fact]
    public async Task An_order_without_a_referring_party_earns_no_commission()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var party = ReferringParty.Create(19, "PRY-1", "Clinic One");
        party.SetCommissionRate(Percentage.Create(10m));

        var order = TestData.CreateOrder(patient);
        order.SetAmounts(
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"),
            Money.Create(100m, "EGP"));

        host.Db.Seed(patient, party, order);

        var result = await host.Sender.Send(
            new RunPeriodStockTakeQuery(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                "EGP"),
            CancellationToken.None);

        Assert.Equal(1, result.OrderCount);
        Assert.Equal(0m, result.CommissionAmount);
    }
}
