using RefLab.Domain.Billing;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Billing;

/// <summary>
/// A payment settles an order.
/// </summary>
public class PaymentTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_stores_the_payment_fields()
    {
        var orderId = Guid.NewGuid();

        var payment = Payment.Create(
            orderId,
            Moment,
            Money.Create(300m, "EGP"),
            number: 1,
            type: 2,
            referringPartyCode: "CL-01",
            testSimpleName: "CBC",
            treatingPhysicianCode: "PW-9");

        Assert.Equal(orderId, payment.OrderId);
        Assert.Equal(300m, payment.Amount.Amount);
        Assert.Equal(1, payment.Number);
        Assert.Equal(2, payment.Type);
        Assert.Equal("CL-01", payment.ReferringPartyCode);
        Assert.Equal("PW-9", payment.TreatingPhysicianCode);
    }

    [Fact]
    public void Create_rejects_a_payment_that_settles_no_order()
    {
        Assert.Throws<InvalidPaymentException>(() =>
            Payment.Create(Guid.Empty, Moment, Money.Create(300m, "EGP"), 1));
    }

    [Fact]
    public void The_comment_is_recorded()
    {
        var payment = Payment.Create(Guid.NewGuid(), Moment, Money.Create(300m, "EGP"), 1);

        payment.SetComment("  Paid in cash  ");

        Assert.Equal("Paid in cash", payment.Comment);
    }
}
