using RefLab.Application.Features.Results;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Patients;

namespace RefLab.Application.Tests.Features.Results;

public sealed class OrderAmountsTests
{
    [Fact]
    public async Task The_order_totals_are_recomputed_from_the_prices_of_the_requested_tests()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);
        host.Db.Seed(
            TestData.CreateTestDefinition(
                code: 56,
                name: "Complete Blood Count",
                simpleName: "Hgb",
                subGroupCode: 11,
                patientPrice: 150m),
            TestData.CreateTestDefinition(
                code: 57,
                name: "Glucose",
                simpleName: "GLU",
                subGroupCode: 12,
                patientPrice: 250m));

        await host.Sender.Send(
            new OrderTestsCommand(order.Id, new[] { 56, 57 }),
            CancellationToken.None);

        var stored = Assert.Single(host.Db.Items<PatientOrder>());

        Assert.Equal(400m, stored.TotalPrice!.Amount);
        Assert.Equal(400m, stored.TotalPriceAfterDiscount!.Amount);
        Assert.Equal(400m, stored.CommissionBaseAmount!.Amount);
    }

    [Fact]
    public async Task A_further_test_recomputes_the_totals_over_every_test_of_the_order()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);
        host.Db.Seed(
            TestData.CreateTestDefinition(
                code: 56,
                name: "Complete Blood Count",
                simpleName: "Hgb",
                subGroupCode: 11,
                patientPrice: 150m),
            TestData.CreateTestDefinition(
                code: 57,
                name: "Glucose",
                simpleName: "GLU",
                subGroupCode: 12,
                patientPrice: 250m),
            TestData.CreateTestDefinition(
                code: 58,
                name: "Urea",
                simpleName: "UREA",
                subGroupCode: 12,
                patientPrice: 50m));

        await host.Sender.Send(
            new OrderTestsCommand(order.Id, new[] { 56, 57 }),
            CancellationToken.None);

        await host.Sender.Send(
            new OrderTestsCommand(order.Id, new[] { 58 }),
            CancellationToken.None);

        var stored = Assert.Single(host.Db.Items<PatientOrder>());

        Assert.Equal(3, stored.Tests.Count);
        Assert.Equal(450m, stored.TotalPrice!.Amount);
        Assert.Equal(450m, stored.TotalPriceAfterDiscount!.Amount);
        Assert.Equal(450m, stored.CommissionBaseAmount!.Amount);
    }

    [Fact]
    public async Task The_amounts_are_recorded_in_the_currency_of_the_order()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient, currencyCode: "USD");
        host.Db.Seed(patient, order);
        host.Db.Seed(
            TestData.CreateTestDefinition(
                code: 56,
                name: "Complete Blood Count",
                simpleName: "Hgb",
                subGroupCode: 11,
                patientPrice: 150m),
            TestData.CreateTestDefinition(
                code: 57,
                name: "Glucose",
                simpleName: "GLU",
                subGroupCode: 12,
                patientPrice: 250m));

        await host.Sender.Send(
            new OrderTestsCommand(order.Id, new[] { 56, 57 }),
            CancellationToken.None);

        var stored = Assert.Single(host.Db.Items<PatientOrder>());

        Assert.Equal("USD", stored.CurrencyCode);
        Assert.Equal("USD", stored.TotalPrice!.CurrencyCode);
        Assert.Equal("USD", stored.TotalPriceAfterDiscount!.CurrencyCode);
        Assert.Equal("USD", stored.CommissionBaseAmount!.CurrencyCode);
        Assert.Equal(400m, stored.TotalPrice!.Amount);
    }
}
