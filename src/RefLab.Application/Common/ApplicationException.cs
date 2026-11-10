namespace RefLab.Application.Common;

/// <summary>
/// The base type of every exception raised by the Application layer.
/// </summary>
/// <remarks>
/// Application exceptions describe an expected operational failure — a missing
/// record, a rejected request, a conflict — as distinct from an infrastructure
/// fault. The presentation layer maps them to user-facing outcomes.
/// </remarks>
public abstract class ApplicationException : Exception
{
    /// <summary>Initialises a new application exception.</summary>
    protected ApplicationException(string message)
        : base(message)
    {
    }

    /// <summary>Initialises a new application exception with an inner cause.</summary>
    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
