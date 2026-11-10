using FluentValidation;
using RefLab.Application.Features.Worksheets;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the patient-group work-sheet request.
/// </summary>
public sealed class BuildPatientGroupWorkSheetValidator : AbstractValidator<BuildPatientGroupWorkSheetCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public BuildPatientGroupWorkSheetValidator()
    {
        RuleFor(request => request)
            .Must(request => request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");

        RuleFor(request => request.DestinationDirectory)
            .NotEmpty()
            .WithMessage("The destination directory is required.");
    }
}

/// <summary>
/// Validates the test-group work-sheet request.
/// </summary>
public sealed class BuildTestGroupWorkSheetValidator : AbstractValidator<BuildTestGroupWorkSheetCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public BuildTestGroupWorkSheetValidator()
    {
        RuleFor(request => request)
            .Must(request => request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");

        RuleFor(request => request)
            .Must(request => request.TestCode.HasValue || request.SubGroupCode.HasValue)
            .WithMessage("A test code or a test-group code must be supplied.");

        RuleFor(request => request.DestinationDirectory)
            .NotEmpty()
            .WithMessage("The destination directory is required.");
    }
}
