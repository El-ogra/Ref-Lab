using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Patients;

/// <summary>
/// The patient order is the laboratory visit header.
/// </summary>
public class PatientOrderTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    private static PatientOrder CreateOrder()
    {
        var patient = Patient.Create("Name", Sex.Male);
        return PatientOrder.Create(patient.Id, Moment, "EGP", "lab.tech");
    }

    [Fact]
    public void Create_stores_the_order_header()
    {
        var order = CreateOrder();

        Assert.Equal("EGP", order.CurrencyCode);
        Assert.Equal("lab.tech", order.CreatedBy);
        Assert.Equal(RecordStatus.Open, order.Status);
    }

    [Fact]
    public void Create_rejects_a_missing_currency()
    {
        var patient = Patient.Create("Name", Sex.Male);

        Assert.Throws<InvalidPatientOrderException>(() =>
            PatientOrder.Create(patient.Id, Moment, "  ", "lab.tech"));
    }

    [Fact]
    public void AddTest_attaches_a_requested_test_to_the_order()
    {
        var order = CreateOrder();
        var test = OrderTest.Create(order.Id, 56, "Hgb", 11, Moment, "lab.tech");

        order.AddTest(test);

        Assert.Single(order.Tests);
        Assert.Equal(RecordStatus.InProgress, order.Status);
    }

    [Fact]
    public void AddTest_rejects_a_test_that_belongs_to_another_order()
    {
        var order = CreateOrder();
        var foreign = OrderTest.Create(Guid.NewGuid(), 56, "Hgb", 11, Moment, "lab.tech");

        Assert.Throws<InvalidPatientOrderException>(() => order.AddTest(foreign));
    }

    [Fact]
    public void The_status_becomes_completed_when_every_test_is_reviewed()
    {
        var order = CreateOrder();
        var test = OrderTest.Create(order.Id, 56, "Hgb", 11, Moment, "lab.tech");
        order.AddTest(test);

        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");
        test.ConfirmReviewed(Moment, "lab.tech");

        Assert.True(order.IsFullyReviewed());
        Assert.Equal(RecordStatus.Completed, order.Status);
    }

    [Fact]
    public void The_status_becomes_delivered_when_every_test_is_handed_over()
    {
        var order = CreateOrder();
        var test = OrderTest.Create(order.Id, 56, "Hgb", 11, Moment, "lab.tech");
        order.AddTest(test);

        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");
        test.ConfirmReviewed(Moment, "lab.tech");
        test.MarkGiven("reception", Moment);

        Assert.True(order.IsFullyDelivered());
        Assert.Equal(RecordStatus.Delivered, order.Status);
    }

    [Fact]
    public void An_order_without_tests_is_open()
    {
        var order = CreateOrder();

        Assert.False(order.IsFullyDelivered());
        Assert.False(order.IsFullyReviewed());
        Assert.Equal(RecordStatus.Open, order.Status);
    }

    [Fact]
    public void The_legacy_identifier_is_attached_to_the_order()
    {
        var order = CreateOrder();
        var identifier = OrderIdentifier.Create("AB1234567890X");

        order.AttachLegacyIdentifier(identifier);

        Assert.Equal(identifier, order.LegacyIdentifier);
    }

    [Fact]
    public void The_amounts_are_recorded()
    {
        var order = CreateOrder();

        order.SetAmounts(
            totalPrice: Money.Create(300m, "EGP"),
            totalPriceAfterDiscount: Money.Create(270m, "EGP"),
            commissionBaseAmount: Money.Create(300m, "EGP"),
            discount: Percentage.Create(10m),
            discount100: 10m);

        Assert.Equal(300m, order.TotalPrice!.Amount);
        Assert.Equal(270m, order.TotalPriceAfterDiscount!.Amount);
        Assert.Equal(300m, order.CommissionBaseAmount!.Amount);
        Assert.Equal(10m, order.Discount!.Value);
    }
}
