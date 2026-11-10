using RefLab.Infrastructure.Security;

namespace RefLab.Infrastructure.Tests;

/// <summary>
/// The legacy system stores the operator password in the clear. Correcting that
/// is one of the two categories of change the project owner approved.
/// </summary>
public class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void A_correct_password_is_verified()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.Hash("Password2026!");

        Assert.True(hasher.Verify("Password2026!", hash));
    }

    [Fact]
    public void An_incorrect_password_is_rejected()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.Hash("Password2026!");

        Assert.False(hasher.Verify("Password2027!", hash));
    }

    [Fact]
    public void The_password_is_never_stored_in_the_clear()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.Hash("Password2026!");

        Assert.DoesNotContain("Password2026!", hash, StringComparison.Ordinal);
        Assert.StartsWith("pbkdf2$sha256$", hash, StringComparison.Ordinal);
    }

    [Fact]
    public void The_same_password_produces_a_different_hash_every_time()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var first = hasher.Hash("Password2026!");
        var second = hasher.Hash("Password2026!");

        Assert.NotEqual(first, second);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("pbkdf2$sha256$notanumber$c2FsdA==$aGFzaA==")]
    [InlineData("pbkdf2$sha256$1000$notbase64$notbase64")]
    [InlineData("md5$1$c2FsdA==$aGFzaA==")]
    public void A_malformed_hash_is_rejected(string hash)
    {
        var hasher = new Pbkdf2PasswordHasher();

        Assert.False(hasher.Verify("Password2026!", hash));
    }
}
