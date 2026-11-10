namespace RefLab.Domain.Exceptions;

/// <summary>
/// Raised when a legacy order identifier does not respect the 13-character
/// positional layout.
/// </summary>
public sealed class InvalidOrderIdentifierException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidOrderIdentifierException(int actualLength, int expectedLength, string reason)
        : base($"The order identifier is invalid: {reason} Actual length {actualLength}, expected {expectedLength}.")
    {
        ActualLength = actualLength;
        ExpectedLength = expectedLength;
    }

    /// <summary>Initialises a new exception.</summary>
    public InvalidOrderIdentifierException(object offendingValue, string reason)
        : base($"The order identifier is invalid: {reason} Offending value '{offendingValue}'.")
    {
    }

    /// <summary>The actual length, when known.</summary>
    public int ActualLength { get; }

    /// <summary>The expected length, when known.</summary>
    public int ExpectedLength { get; }
}

/// <summary>
/// Raised when a result value is empty.
/// </summary>
public sealed class InvalidResultValueException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidResultValueException(string text)
        : base("A result value cannot be empty.")
    {
        Text = text;
    }

    /// <summary>The rejected text.</summary>
    public string Text { get; }
}

/// <summary>
/// Raised when a patient order is created with inconsistent data.
/// </summary>
public sealed class InvalidPatientOrderException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidPatientOrderException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Raised when an operation that requires a reviewed result is attempted on an
/// unreviewed result.
/// </summary>
/// <remarks>
/// Legacy evidence: the print path and the hand-over path both filter on the
/// review flag, and the PDF path refuses with
/// <c>Not found any Verified selected tests or result to print it</c>.
/// </remarks>
public sealed class ResultNotReviewedException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public ResultNotReviewedException(int testCode)
        : base($"Test {testCode} has not been confirmed as reviewed and cannot be released.")
    {
        TestCode = testCode;
    }

    /// <summary>The test code that is not reviewed.</summary>
    public int TestCode { get; }
}
