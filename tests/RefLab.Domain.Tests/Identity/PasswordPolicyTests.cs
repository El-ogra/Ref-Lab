using RefLab.Domain.Exceptions;
using RefLab.Domain.Identity;

namespace RefLab.Domain.Tests.Identity;

/// <summary>
/// The legacy system stores the operator password in the clear. The policy
/// below is one of the two approved categories of correction.
/// </summary>
public class PasswordPolicyTests
{
    [Fact]
    public void A_strong_password_satisfies_the_policy()
    {
        Assert.True(PasswordPolicy.IsSatisfiedBy("Laboratory2026!"));
    }

    [Fact]
    public void A_password_that_is_too_short_is_rejected()
    {
        var exception = Assert.Throws<WeakPasswordException>(() => PasswordPolicy.Validate("Ab1!"));

        Assert.Contains("at least", exception.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A_password_without_a_letter_is_rejected()
    {
        var exception = Assert.Throws<WeakPasswordException>(() => PasswordPolicy.Validate("1234567890123"));

        Assert.Contains("letter", exception.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A_password_without_a_digit_is_rejected()
    {
        var exception = Assert.Throws<WeakPasswordException>(() => PasswordPolicy.Validate("LaboratoryOnly!!"));

        Assert.Contains("digit", exception.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A_password_that_is_too_long_is_rejected()
    {
        var tooLong = new string('a', PasswordPolicy.MaximumLength + 1) + "1";

        Assert.Throws<WeakPasswordException>(() => PasswordPolicy.Validate(tooLong));
    }

    [Fact]
    public void An_empty_password_does_not_satisfy_the_policy()
    {
        Assert.False(PasswordPolicy.IsSatisfiedBy(null));
        Assert.False(PasswordPolicy.IsSatisfiedBy(string.Empty));
    }
}
