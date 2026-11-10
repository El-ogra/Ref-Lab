namespace RefLab.Application.Common;

/// <summary>
/// Raised when the signed-in operator lacks the permission a request needs.
/// </summary>
public sealed class ForbiddenException : ApplicationException
{
    /// <summary>Initialises a new exception.</summary>
    public ForbiddenException(string message)
        : base(message)
    {
    }
}
