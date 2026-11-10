namespace RefLab.Application.Abstractions;

/// <summary>
/// Hashes and verifies operator passwords.
/// </summary>
/// <remarks>
/// The algorithm is an infrastructure concern and is deliberately not referenced
/// from the Domain layer. The Domain layer owns the <em>policy</em>;
/// this abstraction owns the <em>implementation</em>.
/// </remarks>
public interface IPasswordHasher
{
    /// <summary>Produces a storable hash for the given clear-text password.</summary>
    string Hash(string password);

    /// <summary>Verifies a clear-text password against a stored hash.</summary>
    bool Verify(string password, string hash);
}
