namespace RefLab.Domain.Exceptions;

/// <summary>
/// Raised when an operator is not permitted to perform an operation.
/// </summary>
public sealed class AuthorizationException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public AuthorizationException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Raised when an operator record is created or updated with invalid data.
/// </summary>
public sealed class InvalidOperatorException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidOperatorException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Raised when a password does not satisfy the security policy.
/// </summary>
/// <remarks>
/// Legacy evidence: the legacy system stores the operator password in the clear
/// in <c>patients.patientword</c> and the database credential in the clear in
/// <c>BASE.ini</c> and <c>data_connection1.udl</c>. Correcting this is one of
/// the two categories of change the owner approved: security vulnerabilities and
/// data-integrity flaws.
/// </remarks>
public sealed class WeakPasswordException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public WeakPasswordException(string reason)
        : base($"The password does not satisfy the password policy: {reason}")
    {
        Reason = reason;
    }

    /// <summary>The reason the password was rejected.</summary>
    public string Reason { get; }
}
