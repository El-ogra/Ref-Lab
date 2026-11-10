using RefLab.Application.Common;
using RefLab.Application.Features.Billing;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Billing;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Billing;

public class RecordPaymentHandlerTests
{
    [Fact]
    public async Task A_payment_is_recorded_against_the_order()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        var result = await host.Sender.Send(
            new RecordPaymentCommand(order.Id, 150m, 2, "CL-01"),
            CancellationToken.None);

        Assert.Equal(150m, result.Amount);
        Assert.Equal("EGP", result.CurrencyCode);
        Assert.Equal("CL-01", result.ReferringPartyCode);
        Assert.Single(host.Db.Items<Payment>());
    }

    [Fact]
    public async Task The_payment_number_advances_with_every_receipt()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var first = TestData.CreateOrder(patient);
        var second = TestData.CreateOrder(patient);
        host.Db.Seed(patient, first, second);

        var firstReceipt = await host.Sender.Send(
            new RecordPaymentCommand(first.Id, 150m),
            CancellationToken.None);

        var secondReceipt = await host.Sender.Send(
            new RecordPaymentCommand(second.Id, 200m),
            CancellationToken.None);

        Assert.Equal(1, firstReceipt.Number);
        Assert.Equal(2, secondReceipt.Number);
    }

    [Fact]
    public async Task A_zero_amount_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new RecordPaymentCommand(order.Id, 0m), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_order_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new RecordPaymentCommand(Guid.NewGuid(), 150m), CancellationToken.None));
    }
}

public class ComputeCommissionHandlerTests
{
    private static ReferringParty CreateParty(decimal rate)
    {
        var party = ReferringParty.Create(19, "CL-01", "Clinic One");
        party.SetCommissionRate(Percentage.Create(rate));
        return party;
    }

    [Fact]
    public async Task The_commission_is_the_rate_applied_to_the_base_amount()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        order.SetAmounts(
            Money.Create(1000m, "EGP"),
            Money.Create(1000m, "EGP"),
            Money.Create(1000m, "EGP"));

        host.Db.Seed(patient, order, CreateParty(10m));

        var result = await host.Sender.Send(
            new ComputeCommissionQuery(19, new[] { order.Id }),
            CancellationToken.None);

        Assert.Equal(100m, result.CommissionAmount);
        Assert.Equal(1000m, result.BaseAmount);
    }

    [Fact]
    public async Task A_party_without_a_commission_rate_is_rejected()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);

        host.Db.Seed(patient, order, ReferringParty.Create(19, "CL-01", "Clinic One"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(new ComputeCommissionQuery(19, new[] { order.Id }), CancellationToken.None));
    }

    [Fact]
    public async Task An_empty_order_list_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(CreateParty(10m));

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new ComputeCommissionQuery(19, Array.Empty<Guid>()), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_party_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new ComputeCommissionQuery(999, new[] { Guid.NewGuid() }), CancellationToken.None));
    }
}

public class GetReferringPartiesTests
{
    [Fact]
    public async Task The_referring_parties_are_listed()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateReferringParty(19, "CL-01", "Clinic One"));

        var result = await host.Sender.Send(new GetReferringPartiesQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Clinic One", result[0].Name);
        Assert.Equal(10m, result[0].CommissionRate);
    }

    [Fact]
    public async Task The_referring_parties_are_found_by_name_or_code()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(
            TestData.CreateReferringParty(19, "CL-01", "Clinic One"),
            TestData.CreateReferringParty(20, "CL-02", "Clinic Two"));

        var result = await host.Sender.Send(new GetReferringPartiesQuery("CL-02"), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Clinic Two", result[0].Name);
    }

    [Fact]
    public async Task An_empty_search_returns_every_party()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(
            TestData.CreateReferringParty(19, "CL-01", "Clinic One"),
            TestData.CreateReferringParty(20, "CL-02", "Clinic Two"));

        var result = await host.Sender.Send(new GetReferringPartiesQuery(null), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }
}
