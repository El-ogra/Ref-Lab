namespace RefLab.Domain.Exceptions;

/// <summary>
/// Raised when the test catalogue is manipulated in a way the catalogue forbids.
/// </summary>
public class TestCatalogueException : DomainException
{
    /// <summary>Initialises a new catalogue exception.</summary>
    public TestCatalogueException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Raised when an attempt is made to change the identity of a protected test.
/// </summary>
/// <remarks>
/// Legacy evidence: the shipped manual states that the urine analysis carries
/// the number 4 in the system and that neither its number nor its name may be
/// changed. The legacy system did not enforce this in code; Ref Lab does.
/// </remarks>
public sealed class TestNumberImmutableException : TestCatalogueException
{
    /// <summary>Initialises a new exception.</summary>
    /// <param name="testCode">The protected test code.</param>
    public TestNumberImmutableException(int testCode)
        : base($"Test number {testCode} is protected: neither its number nor its name may be changed.")
    {
        TestCode = testCode;
    }

    /// <summary>The protected test code.</summary>
    public int TestCode { get; }
}

/// <summary>
/// Raised when a test definition is created with inconsistent data.
/// </summary>
public sealed class InvalidTestDefinitionException : TestCatalogueException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidTestDefinitionException(string message)
        : base(message)
    {
    }
}
