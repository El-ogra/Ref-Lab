using FluentValidation;
using RefLab.Application.Features.Catalogue;
using RefLab.Application.Common;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the creation of a catalogue test.
/// </summary>
public sealed class CreateTestDefinitionValidator : AbstractValidator<CreateTestDefinitionCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public CreateTestDefinitionValidator()
    {
        RuleFor(request => request.Code)
            .GreaterThan(0)
            .WithMessage("The test code must be a positive integer.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("The test name is required.");

        RuleFor(request => request.SimpleName)
            .NotEmpty()
            .WithMessage("The simplified analyte name is required.");

        RuleFor(request => request.SubGroupCode)
            .GreaterThan(0)
            .WithMessage("The clinical sub-group code must be a positive integer.");

        RuleFor(request => request.PatientPrice)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("A catalogue price cannot be negative.");
    }
}

/// <summary>
/// Validates the amendment of a catalogue test.
/// </summary>
public sealed class UpdateTestDefinitionValidator : AbstractValidator<UpdateTestDefinitionCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public UpdateTestDefinitionValidator()
    {
        RuleFor(request => request.Code)
            .GreaterThan(0)
            .WithMessage("The test code must be a positive integer.");

        RuleFor(request => request.PatientPrice)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.PatientPrice.HasValue)
            .WithMessage("A catalogue price cannot be negative.");

        RuleFor(request => request.LabPrice)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.LabPrice.HasValue)
            .WithMessage("A catalogue price cannot be negative.");

        RuleFor(request => request.OtherLabPrice)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.OtherLabPrice.HasValue)
            .WithMessage("A catalogue price cannot be negative.");
    }
}

/// <summary>
/// Validates the creation of a reference range.
/// </summary>
public sealed class CreateReferenceRangeValidator : AbstractValidator<CreateReferenceRangeCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public CreateReferenceRangeValidator()
    {
        RuleFor(request => request.TestCode)
            .GreaterThan(0)
            .WithMessage("The test code must be a positive integer.");

        RuleFor(request => request.TestSimpleName)
            .NotEmpty()
            .WithMessage("The analyte name is required.");

        RuleFor(request => request.AgeFrom)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.AgeFrom.HasValue)
            .WithMessage("An age bound cannot be negative.");

        RuleFor(request => request.AgeTo)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.AgeTo.HasValue)
            .WithMessage("An age bound cannot be negative.");

        RuleFor(request => request)
            .Must(request => !request.AgeFrom.HasValue || !request.AgeTo.HasValue || request.AgeFrom <= request.AgeTo)
            .WithMessage("The lower age bound cannot exceed the upper age bound.");
    }
}

/// <summary>
/// Validates the amendment of a reference range.
/// </summary>
public sealed class UpdateReferenceRangeValidator : AbstractValidator<UpdateReferenceRangeCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public UpdateReferenceRangeValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("The range identifier must be a positive integer.");

        RuleFor(request => request)
            .Must(request => !request.LowLimit.HasValue || !request.HighLimit.HasValue || request.LowLimit <= request.HighLimit)
            .WithMessage("The lower reference limit cannot exceed the upper reference limit.");
    }
}

/// <summary>
/// Validates the maintenance of the unit and conversion dictionaries.
/// </summary>
public sealed class ManageUnitsAndConversionsValidator : AbstractValidator<ManageUnitsAndConversionsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ManageUnitsAndConversionsValidator()
    {
        RuleFor(request => request)
            .Must(request => request.UnitName is not null || request.ConversionTestSimpleName is not null)
            .WithMessage("A unit name or a conversion factor must be supplied.");

        RuleFor(request => request.ConversionTestSimpleName)
            .NotEmpty()
            .When(request => request.ConversionTestSimpleName is not null)
            .WithMessage("The analyte name is required.");

        RuleFor(request => request.FromUnit)
            .NotEmpty()
            .When(request => request.ConversionTestSimpleName is not null)
            .WithMessage("The source unit is required.");

        RuleFor(request => request.ToUnit)
            .NotEmpty()
            .When(request => request.ConversionTestSimpleName is not null)
            .WithMessage("The target unit is required.");

        RuleFor(request => request.Factor)
            .NotNull()
            .When(request => request.ConversionTestSimpleName is not null)
            .WithMessage("The conversion factor is required.");

        RuleFor(request => request.FromDecimals)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A decimal count cannot be negative.");

        RuleFor(request => request.ToDecimals)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A decimal count cannot be negative.");
    }
}

/// <summary>
/// Validates the maintenance of the abbreviation dictionary.
/// </summary>
public sealed class ManageAbbreviationsValidator : AbstractValidator<ManageAbbreviationsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ManageAbbreviationsValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("The abbreviation identifier must be a positive integer.");

        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("The abbreviation is required.");
    }
}

/// <summary>
/// Validates the maintenance of the clinical sub-group catalogue.
/// </summary>
public sealed class ManageClinicalSubGroupsValidator : AbstractValidator<ManageClinicalSubGroupsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ManageClinicalSubGroupsValidator()
    {
        RuleFor(request => request.Code)
            .GreaterThan(0)
            .WithMessage("The sub-group code must be a positive integer.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("The sub-group name is required.");
    }
}

/// <summary>
/// Validates the test-catalogue request.
/// </summary>
public sealed class GetTestCatalogueValidator : AbstractValidator<GetTestCatalogueQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetTestCatalogueValidator()
    {
        RuleFor(request => request.Page!.PageNumber)
            .GreaterThan(0)
            .When(request => request.Page is not null)
            .WithMessage("The page number must be greater than zero.");

        RuleFor(request => request.Page!.PageSize)
            .InclusiveBetween(1, PageRequest.MaximumPageSize)
            .When(request => request.Page is not null)
            .WithMessage($"The page size must be between 1 and {PageRequest.MaximumPageSize}.");
    }
}

/// <summary>
/// Validates the test-profile request.
/// </summary>
public sealed class GetTestProfileValidator : AbstractValidator<GetTestProfileQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetTestProfileValidator()
    {
        RuleFor(request => request.TestCode)
            .GreaterThan(0)
            .WithMessage("The test code must be a positive integer.");
    }
}

/// <summary>
/// Validates the reference-range resolution request.
/// </summary>
public sealed class GetReferenceRangeForPatientValidator : AbstractValidator<GetReferenceRangeForPatientQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetReferenceRangeForPatientValidator()
    {
        RuleFor(request => request.PatientId)
            .NotEmpty()
            .WithMessage("The patient identifier is required.");

        RuleFor(request => request.TestCode)
            .GreaterThan(0)
            .WithMessage("The test code must be a positive integer.");
    }
}

/// <summary>
/// Validates the creation of a clinical test group.
/// </summary>
public sealed class CreateTestGroupValidator : AbstractValidator<CreateTestGroupCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public CreateTestGroupValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("The test group name is required.");

        RuleFor(request => request.Name)
            .MaximumLength(128)
            .WithMessage("The test group name cannot be longer than 128 characters.");

        RuleFor(request => request.TestCodes)
            .NotEmpty()
            .WithMessage("A test group must contain at least one test.");

        RuleFor(request => request.TestCodes)
            .Must(codes => codes.Distinct().Count() == codes.Count)
            .WithMessage("A test group cannot list the same test twice.");
    }
}
