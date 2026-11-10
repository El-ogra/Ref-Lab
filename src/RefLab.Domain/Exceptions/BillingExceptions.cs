namespace RefLab.Domain.Exceptions;

/// <summary>
/// Raised when a monetary amount is created with an invalid currency code.
/// </summary>
public sealed class InvalidMoneyException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidMoneyException(string currencyCode)
        : base($"'{currencyCode}' is not a valid ISO 4217 currency code. Three upper-case letters are required.")
    {
        CurrencyCode = currencyCode;
    }

    /// <summary>The rejected currency code.</summary>
    public string CurrencyCode { get; }
}

/// <summary>
/// Raised when two amounts in different currencies are combined.
/// </summary>
public sealed class CurrencyMismatchException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public CurrencyMismatchException(string leftCurrency, string rightCurrency)
        : base($"Cannot combine amounts in different currencies: '{leftCurrency}' and '{rightCurrency}'.")
    {
        LeftCurrency = leftCurrency;
        RightCurrency = rightCurrency;
    }

    /// <summary>The currency of the left operand.</summary>
    public string LeftCurrency { get; }

    /// <summary>The currency of the right operand.</summary>
    public string RightCurrency { get; }
}

/// <summary>
/// Raised when a percentage falls outside the range 0 to 100.
/// </summary>
public sealed class InvalidPercentageException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidPercentageException(decimal value)
        : base($"A percentage must be between 0 and 100. Received {value}.")
    {
        Value = value;
    }

    /// <summary>The rejected value.</summary>
    public decimal Value { get; }
}

/// <summary>
/// Raised when a payment record is created with invalid data.
/// </summary>
public sealed class InvalidPaymentException : DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidPaymentException(string message)
        : base(message)
    {
    }
}
