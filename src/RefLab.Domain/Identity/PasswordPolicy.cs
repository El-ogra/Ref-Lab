using System.Text.RegularExpressions;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Identity;

/// <summary>
/// The password policy applied to every operator account.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the operator password is stored in the clear in
/// <c>patients.patientword</c> and the shipped manual documents no password
/// complexity, expiry or lockout. Replacing the cleartext storage is one of the
/// two categories of change the project owner approved — security
/// vulnerabilities and data-integrity flaws.
/// </para>
/// <para>
/// This type owns the <em>policy</em>. The hashing <em>algorithm</em> is an
/// infrastructure concern and is deliberately not referenced from the domain.
/// </para>
/// </remarks>
public static partial class PasswordPolicy
{
    /// <summary>The minimum accepted password length.</summary>
    public const int MinimumLength = 12;

    /// <summary>The maximum accepted password length.</summary>
    public const int MaximumLength = 128;

    /// <summary>
    /// Validates a clear-text password against the policy.
    /// </summary>
    /// <exception cref="WeakPasswordException">The password does not satisfy the policy.</exception>
    public static void Validate(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length < MinimumLength)
        {
            throw new WeakPasswordException($"it must be at least {MinimumLength} characters long.");
        }

        if (password.Length > MaximumLength)
        {
            throw new WeakPasswordException($"it must be at most {MaximumLength} characters long.");
        }

        if (!HasLetterRegex().IsMatch(password))
        {
            throw new WeakPasswordException("it must contain at least one letter.");
        }

        if (!HasDigitRegex().IsMatch(password))
        {
            throw new WeakPasswordException("it must contain at least one digit.");
        }
    }

    /// <summary>Indicates whether a clear-text password satisfies the policy.</summary>
    public static bool IsSatisfiedBy(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        try
        {
            Validate(password);
            return true;
        }
        catch (WeakPasswordException)
        {
            return false;
        }
    }

    [GeneratedRegex(@"[A-Za-z]")]
    private static partial Regex HasLetterRegex();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex HasDigitRegex();
}
