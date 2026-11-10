namespace RefLab.Application.Common;

/// <summary>
/// Raised when the requested change conflicts with the current state.
/// </summary>
public sealed class ConflictException : ApplicationException
{
    /// <summary>Initialises a new exception.</summary>
    public ConflictException(string message)
        : base(message)
    {
    }
}
