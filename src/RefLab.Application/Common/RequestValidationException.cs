namespace RefLab.Application.Common;

/// <summary>
/// Raised when a request fails its validation rules.
/// </summary>
/// <remarks>
/// The name is deliberately distinct from the FluentValidation exception so
/// that both can be referenced in the same file without ambiguity.
/// </remarks>
public sealed class RequestValidationException : ApplicationException
{
    /// <summary>Initialises a new exception.</summary>
    public RequestValidationException(IReadOnlyList<string> errors)
        : base("The request is not valid. " + string.Join(" ", errors))
    {
        Errors = errors;
    }

    /// <summary>The individual validation failures.</summary>
    public IReadOnlyList<string> Errors { get; }
}
