using FluentValidation;
using RefLab.Application.Features.Parties;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the treating-physician request.
/// </summary>
public sealed class ManageTreatingPhysiciansValidator : AbstractValidator<ManageTreatingPhysiciansCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ManageTreatingPhysiciansValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("The physician identifier must be a positive integer.");

        RuleFor(request => request.Code)
            .NotEmpty()
            .WithMessage("The physician code is required.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("The physician name is required.");
    }
}

/// <summary>
/// Validates the price-list request.
/// </summary>
public sealed class ManagePriceListsValidator : AbstractValidator<ManagePriceListsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ManagePriceListsValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("The price-list identifier must be a positive integer.");

        RuleFor(request => request.PriceListName)
            .NotEmpty()
            .WithMessage("The price-list name is required.");

        RuleFor(request => request.TestSimpleName)
            .NotEmpty()
            .WithMessage("The analyte name is required.");

        RuleFor(request => request.Price)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("A price cannot be negative.");
    }
}

/// <summary>
/// Validates the price-list printing request.
/// </summary>
public sealed class PrintPriceListValidator : AbstractValidator<PrintPriceListCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public PrintPriceListValidator()
    {
        RuleFor(request => request.PriceListName)
            .NotEmpty()
            .WithMessage("The price-list name is required.");

        RuleFor(request => request.DestinationDirectory)
            .NotEmpty()
            .WithMessage("The destination directory is required.");
    }
}
