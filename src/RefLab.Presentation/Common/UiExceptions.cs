using RefLab.Application.Common;

namespace RefLab.Presentation.Common;

/// <summary>
/// Raised when the Application layer rejected a request and carries the
/// individual validation failures.
/// </summary>
public sealed class UiValidationException : Exception
{
    /// <summary>Initialises a new instance.</summary>
    public UiValidationException(IReadOnlyList<string> errors)
        : base("The request is not valid.")
    {
        Errors = errors;
    }

    /// <summary>The individual validation failures.</summary>
    public IReadOnlyList<string> Errors { get; }
}

/// <summary>
/// Raised when an operation failed for an operator-visible reason.
/// </summary>
public sealed class UiOperationException : Exception
{
    /// <summary>Initialises a new instance.</summary>
    public UiOperationException(string message)
        : base(message)
    {
    }

    /// <summary>Initialises a new instance with an inner cause.</summary>
    public UiOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
