using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// The legacy system stores money in floating point columns. Ref Lab uses
/// decimal, so these tests pin the arithmetic that the legacy system could not
/// represent exactly.
/// </summary>
public class MoneyTests
{
    [Fact]
    public void Create_normalises_the_currency_code_to_upper_case()
    {
        var money = Money.Create(10m, "egp");

        Assert.Equal("EGP", money.CurrencyCode);
    }

    [Theory]
    [InlineData("EG")]
    [InlineData("EGPP")]
    [InlineData("E1P")]
    [InlineData("")]
    public void Create_rejects_a_currency_code_that_is_not_three_letters(string code)
    {
        Assert.Throws<InvalidMoneyException>(() => Money.Create(10m, code));
    }

    [Fact]
    public void Addition_preserves_the_currency()
    {
        var left = Money.Create(10.10m, "EGP");
        var right = Money.Create(20.20m, "EGP");

        var sum = left + right;

        Assert.Equal(30.30m, sum.Amount);
        Assert.Equal("EGP", sum.CurrencyCode);
    }

    [Fact]
    public void Addition_of_two_amounts_representing_one_tenth_is_exact()
    {
        var left = Money.Create(0.1m, "EGP");
        var right = Money.Create(0.2m, "EGP");

        var sum = left + right;

        Assert.Equal(0.3m, sum.Amount);
    }

    [Fact]
    public void Subtraction_preserves_the_currency()
    {
        var left = Money.Create(30.30m, "EGP");
        var right = Money.Create(20.20m, "EGP");

        var difference = left - right;

        Assert.Equal(10.10m, difference.Amount);
    }

    [Fact]
    public void Combining_two_currencies_is_rejected()
    {
        var left = Money.Create(10m, "EGP");
        var right = Money.Create(10m, "USD");

        Assert.Throws<CurrencyMismatchException>(() => left + right);
        Assert.Throws<CurrencyMismatchException>(() => left > right);
    }

    [Fact]
    public void Multiplication_by_a_factor_scales_the_amount()
    {
        var money = Money.Create(12.50m, "EGP");

        var scaled = money * 3m;

        Assert.Equal(37.50m, scaled.Amount);
    }

    [Fact]
    public void Negation_reverses_the_sign()
    {
        var money = Money.Create(12.50m, "EGP");

        var negated = -money;

        Assert.Equal(-12.50m, negated.Amount);
    }

    [Fact]
    public void Zero_keeps_the_currency()
    {
        var zero = Money.Zero("EGP");

        Assert.Equal(0m, zero.Amount);
        Assert.Equal("EGP", zero.CurrencyCode);
    }

    [Fact]
    public void Two_amounts_with_the_same_value_and_currency_are_equal()
    {
        var left = Money.Create(10m, "EGP");
        var right = Money.Create(10m, "EGP");

        Assert.Equal(left, right);
    }

    [Fact]
    public void Two_amounts_in_different_currencies_are_not_equal()
    {
        var left = Money.Create(10m, "EGP");
        var right = Money.Create(10m, "USD");

        Assert.False(left.Equals(right));
    }
}
