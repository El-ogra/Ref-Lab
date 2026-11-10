namespace RefLab.Domain.Exceptions;

/// <summary>
/// Raised when a send-out record is manipulated into an invalid state.
/// </summary>
public sealed class InvalidSendOutStateException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidSendOutStateException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Raised when a sample disposition record is created with invalid data.
/// </summary>
public sealed class InvalidSampleDispositionException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidSampleDispositionException(string message)
        : base(message)
    {
    }
}
