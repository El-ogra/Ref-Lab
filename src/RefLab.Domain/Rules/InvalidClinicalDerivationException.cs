namespace RefLab.Domain.Rules;

/// <summary>
/// Raised when a clinical derivation cannot be performed.
/// </summary>
public sealed class InvalidClinicalDerivationException : Exceptions.DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidClinicalDerivationException(string message)
        : base(message)
    {
    }
}
