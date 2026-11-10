namespace RefLab.Domain.Exceptions;

/// <summary>
/// Raised when a reference interval is constructed with inconsistent limits.
/// </summary>
public sealed class InvalidReferenceIntervalException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidReferenceIntervalException(decimal? lowLimit, decimal? highLimit)
        : base($"The lower reference limit ({lowLimit}) cannot exceed the upper reference limit ({highLimit}).")
    {
        LowLimit = lowLimit;
        HighLimit = highLimit;
    }

    /// <summary>The rejected lower limit.</summary>
    public decimal? LowLimit { get; }

    /// <summary>The rejected upper limit.</summary>
    public decimal? HighLimit { get; }
}

/// <summary>
/// Raised when a critical interval is constructed with inconsistent limits.
/// </summary>
public sealed class InvalidCriticalIntervalException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidCriticalIntervalException(decimal? lowCriticalLimit, decimal? highCriticalLimit)
        : base($"The lower critical limit ({lowCriticalLimit}) cannot exceed the upper critical limit ({highCriticalLimit}).")
    {
        LowCriticalLimit = lowCriticalLimit;
        HighCriticalLimit = highCriticalLimit;
    }

    /// <summary>The rejected lower critical limit.</summary>
    public decimal? LowCriticalLimit { get; }

    /// <summary>The rejected upper critical limit.</summary>
    public decimal? HighCriticalLimit { get; }
}

/// <summary>
/// Raised when an age band is constructed with inconsistent bounds.
/// </summary>
public sealed class InvalidAgeBandException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidAgeBandException(decimal? from, decimal? to, string reason)
        : base($"The age band is invalid: {reason}")
    {
        From = from;
        To = to;
    }

    /// <summary>The rejected lower bound.</summary>
    public decimal? From { get; }

    /// <summary>The rejected upper bound.</summary>
    public decimal? To { get; }
}

/// <summary>
/// Raised when no reference range matches the analyte, sex and age.
/// </summary>
public sealed class ReferenceRangeNotFoundException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public ReferenceRangeNotFoundException(int testCode, string scope)
        : base($"No reference range is defined for test {testCode} within the scope '{scope}'.")
    {
        TestCode = testCode;
        Scope = scope;
    }

    /// <summary>The test code for which no range was found.</summary>
    public int TestCode { get; }

    /// <summary>The scope that was searched.</summary>
    public string Scope { get; }
}
