namespace RefLab.Domain.Exceptions;

/// <summary>
/// The base type of every exception raised by the Domain layer.
/// </summary>
/// <remarks>
/// Domain exceptions describe a broken business invariant. They are raised by
/// entities and policies and are translated into an application-level outcome
/// by the Application layer. The Domain layer never throws them for
/// infrastructure failures.
/// </remarks>
public abstract class DomainException : Exception
{
    /// <summary>Initialises a new domain exception.</summary>
    protected DomainException(string message)
        : base(message)
    {
    }

    /// <summary>Initialises a new domain exception with an inner cause.</summary>
    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
