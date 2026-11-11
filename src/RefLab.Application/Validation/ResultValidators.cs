using FluentValidation;
using RefLab.Application.Features.Results;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the entry of an additional result slot.
/// </summary>
public sealed class EnterAdditionalResultValidator : AbstractValidator<EnterAdditionalResultCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public EnterAdditionalResultValidator()
    {
        RuleFor(request => request.OrderTestId)
            .NotEmpty()
            .WithMessage("The requested test identifier is required.");

        RuleFor(request => request.SlotIndex)
            .InclusiveBetween(0, 3)
            .WithMessage("An additional result slot must be between 0 and 3.");
    }
}

/// <summary>
/// Validates the method-sheet query.
/// </summary>
public sealed class GetMethodSheetValidator : AbstractValidator<GetMethodSheetQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetMethodSheetValidator()
    {
        RuleFor(request => request.TestCode)
            .GreaterThan(0)
            .WithMessage("The test code must be a positive integer.");
    }
}

/// <summary>
/// Validates the critical-values query.
/// </summary>
public sealed class GetCriticalValuesValidator : AbstractValidator<GetCriticalValuesQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetCriticalValuesValidator()
    {
        RuleFor(request => request.TestCode)
            .GreaterThan(0)
            .WithMessage("The test code must be a positive integer.");
    }
}

/// <summary>
/// Validates the test-ordering request.
/// </summary>
public sealed class OrderTestsValidator : AbstractValidator<OrderTestsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public OrderTestsValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");

        RuleFor(request => request.TestCodes)
            .NotEmpty()
            .WithMessage("At least one test must be requested.");

        RuleForEach(request => request.TestCodes)
            .GreaterThan(0)
            .WithMessage("A test code must be a positive integer.");
    }
}

/// <summary>
/// Validates the result-entry request.
/// </summary>
public sealed class EnterResultValidator : AbstractValidator<EnterResultCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public EnterResultValidator()
    {
        RuleFor(request => request.OrderTestId)
            .NotEmpty()
            .WithMessage("The requested test identifier is required.");

        RuleFor(request => request.Value)
            .NotEmpty()
            .WithMessage("The result value is required.");
    }
}

/// <summary>
/// Validates the result-review request.
/// </summary>
public sealed class ReviewResultValidator : AbstractValidator<ReviewResultCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ReviewResultValidator()
    {
        RuleFor(request => request.OrderTestId)
            .NotEmpty()
            .WithMessage("The requested test identifier is required.");
    }
}

/// <summary>
/// Validates the report-printing request.
/// </summary>
public sealed class PrintResultReportValidator : AbstractValidator<PrintResultReportCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public PrintResultReportValidator()
    {
        RuleFor(request => request.OrderTestId)
            .NotEmpty()
            .WithMessage("The requested test identifier is required.");

        RuleFor(request => request.DestinationDirectory)
            .NotEmpty()
            .WithMessage("The destination directory is required.");
    }
}

/// <summary>
/// Validates the hand-over request.
/// </summary>
public sealed class HandOverResultsValidator : AbstractValidator<HandOverResultsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public HandOverResultsValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");

        RuleFor(request => request.Amount)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.Amount.HasValue)
            .WithMessage("The settled amount cannot be negative.");
    }
}

/// <summary>
/// Validates the entry of a derived ratio result.
/// </summary>
public sealed class EnterDerivedRatioValidator : AbstractValidator<EnterDerivedRatioCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public EnterDerivedRatioValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");

        RuleFor(request => request.NumeratorTestCode)
            .GreaterThan(0)
            .WithMessage("The numerator test code must be positive.");

        RuleFor(request => request.DenominatorTestCode)
            .GreaterThan(0)
            .WithMessage("The denominator test code must be positive.");

        RuleFor(request => request.RatioTestCode)
            .GreaterThan(0)
            .WithMessage("The ratio test code must be positive.");

        RuleFor(request => request.NumeratorTestCode)
            .NotEqual(request => request.DenominatorTestCode)
            .WithMessage("The numerator and the denominator must be different tests.");

        RuleFor(request => request.RatioTestCode)
            .NotEqual(request => request.NumeratorTestCode)
            .WithMessage("The ratio must be entered on a different test than the numerator.");

        RuleFor(request => request.RatioTestCode)
            .NotEqual(request => request.DenominatorTestCode)
            .WithMessage("The ratio must be entered on a different test than the denominator.");
    }
}
