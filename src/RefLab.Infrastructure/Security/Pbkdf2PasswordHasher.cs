using System.Security.Cryptography;
using RefLab.Application.Abstractions;

namespace RefLab.Infrastructure.Security;

/// <summary>
/// Hashes and verifies operator passwords with PBKDF2-HMAC-SHA256.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the operator password is stored in the clear in the
/// operator table, and the database credential is stored in the clear in the
/// configuration files. Correcting that is one of the two categories of change
/// the project owner approved — security vulnerabilities and data-integrity
/// flaws.
/// </para>
/// <para>
/// The stored format is <c>pbkdf2$sha256$&lt;iterations&gt;$&lt;salt&gt;$&lt;hash&gt;</c>,
/// so the algorithm and the iteration count travel with the hash and the cost
/// can be raised later without invalidating the existing accounts.
/// </para>
/// </remarks>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 210_000;
    private const string Prefix = "pbkdf2$sha256";

    /// <inheritdoc />
    public string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <inheritdoc />
    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
        {
            return false;
        }

        // The stored format is pbkdf2$sha256$<iterations>$<salt>$<hash>,
        // so the prefix itself contributes two segments.
        var parts = hash.Split('$');

        if (parts.Length != 5 ||
            !string.Equals(parts[0], "pbkdf2", StringComparison.Ordinal) ||
            !string.Equals(parts[1], "sha256", StringComparison.Ordinal))
        {
            return false;
        }

        if (!int.TryParse(parts[2], out var iterations) || iterations <= 0)
        {
            return false;
        }

        byte[] salt;
        byte[] expected;

        try
        {
            salt = Convert.FromBase64String(parts[3]);
            expected = Convert.FromBase64String(parts[4]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actual = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
