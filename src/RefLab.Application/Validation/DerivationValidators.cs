using FluentValidation;
using RefLab.Application.Features.Derivation;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the haematocrit derivation.
/// </summary>
public sealed class DeriveHematocritValidator : AbstractValidator<DeriveHematocritQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public DeriveHematocritValidator()
    {
        RuleFor(request => request.Hemoglobin)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("A measured haemoglobin cannot be negative.");
    }
}

/// <summary>
/// Validates the haemoglobin-percentage derivation.
/// </summary>
public sealed class DeriveHemoglobinPercentageValidator : AbstractValidator<DeriveHemoglobinPercentageQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public DeriveHemoglobinPercentageValidator()
    {
        RuleFor(request => request.Hemoglobin)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("A measured haemoglobin cannot be negative.");

        RuleFor(request => request.AgeYears)
            .InclusiveBetween(0m, 120m)
            .WithMessage("An age must be between 0 and 120 years.");
    }
}

/// <summary>
/// Validates the international-sensitivity-index derivation.
/// </summary>
public sealed class DeriveInternationalSensitivityIndexValidator
    : AbstractValidator<DeriveInternationalSensitivityIndexQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public DeriveInternationalSensitivityIndexValidator()
    {
        RuleFor(request => request.PatientProthrombinTime)
            .GreaterThan(0m)
            .WithMessage("A prothrombin time must be positive.");

        RuleFor(request => request.ControlProthrombinTime)
            .GreaterThan(0m)
            .WithMessage("A control prothrombin time must be positive.");

        RuleFor(request => request.NormalProthrombinTime)
            .GreaterThan(0m)
            .WithMessage("A normal control prothrombin time must be positive.");

        RuleFor(request => request.ControlIndex)
            .GreaterThan(0m)
            .WithMessage("The control index must be positive.");
    }
}

/// <summary>
/// Validates the interpolation of a clotting control concentration.
/// </summary>
public sealed class InterpolateProthrombinControlValidator : AbstractValidator<InterpolateProthrombinControlQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public InterpolateProthrombinControlValidator()
    {
        RuleFor(request => request.ControlTime)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("A control time cannot be negative.");
    }
}
