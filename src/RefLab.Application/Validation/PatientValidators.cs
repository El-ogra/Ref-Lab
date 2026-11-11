using FluentValidation;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the allocation of the legacy composite order identifier.
/// </summary>
public sealed class AllocateOrderIdentifierValidator : AbstractValidator<AllocateOrderIdentifierQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public AllocateOrderIdentifierValidator()
    {
        RuleFor(request => request.PersonKey)
            .NotEmpty()
            .WithMessage("The person key is required.");

        RuleFor(request => request.PersonKey)
            .Length(2)
            .WithMessage("The person key must be exactly 2 characters long.");

        RuleFor(request => request.LeadingSegment)
            .NotEmpty()
            .WithMessage("The leading segment is required.");

        RuleFor(request => request.LeadingSegment)
            .Length(2)
            .WithMessage("The leading segment must be exactly 2 characters long.");

        RuleFor(request => request.MiddleSegment)
            .NotEmpty()
            .WithMessage("The middle segment is required.");

        RuleFor(request => request.MiddleSegment)
            .Length(5)
            .WithMessage("The middle segment must be exactly 5 characters long.");

        RuleFor(request => request.DisplayCode)
            .NotEmpty()
            .WithMessage("The display code is required.");

        RuleFor(request => request.DisplayCode)
            .Length(3)
            .WithMessage("The display code must be exactly 3 characters long.");
    }
}

/// <summary>
/// Validates the generation of a patient barcode.
/// </summary>
public sealed class GeneratePatientBarcodeValidator : AbstractValidator<GeneratePatientBarcodeCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public GeneratePatientBarcodeValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");
    }
}

/// <summary>
/// Validates the attachment of a patient picture.
/// </summary>
public sealed class AttachPatientPictureValidator : AbstractValidator<AttachPatientPictureCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public AttachPatientPictureValidator()
    {
        RuleFor(request => request.PatientId)
            .NotEmpty()
            .WithMessage("The patient identifier is required.");

        RuleFor(request => request.Slot)
            .InclusiveBetween(1, 15)
            .WithMessage("A picture slot must be between 1 and 15.");

        RuleFor(request => request.ImagePath)
            .NotEmpty()
            .WithMessage("The image path is required.");
    }
}

/// <summary>
/// Validates the follow-up register query.
/// </summary>
public sealed class FollowUpCaseValidator : AbstractValidator<FollowUpCaseQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public FollowUpCaseValidator()
    {
        RuleFor(request => request)
            .Must(request => !request.From.HasValue || !request.To.HasValue || request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");
    }
}


/// <summary>
/// Validates the patient registration request.
/// </summary>
public sealed class RegisterPatientValidator : AbstractValidator<RegisterPatientCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public RegisterPatientValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("The patient name is required.");

        RuleFor(request => request.Sex)
            .IsInEnum()
            .WithMessage("The patient's sex is not one of the recorded values.");

        RuleFor(request => request.Age)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.Age.HasValue)
            .WithMessage("The patient's age cannot be negative.");

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .WithMessage("The currency code is required.");

        RuleFor(request => request.CurrencyCode)
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("The currency code must be three letters.");

        // The legacy accession number is a fixed thirteen-character value.
        // Its layout is a hard contract of the legacy system and is enforced
        // again by the domain value object.
        RuleFor(request => request.LegacyOrderIdentifier)
            .Length(13)
            .When(request => request.LegacyOrderIdentifier is not null)
            .WithMessage("The legacy order identifier must be exactly 13 characters long.");

        RuleFor(request => request.ReferringPartyCode)
            .MaximumLength(16)
            .When(request => request.ReferringPartyCode is not null)
            .WithMessage("The referring party code cannot be longer than 16 characters.");

        RuleFor(request => request.ReferringPhysicianCode)
            .MaximumLength(16)
            .When(request => request.ReferringPhysicianCode is not null)
            .WithMessage("The physician code cannot be longer than 16 characters.");
    }
}

/// <summary>
/// Validates the patient search request.
/// </summary>
public sealed class SearchPatientsValidator : AbstractValidator<SearchPatientsQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public SearchPatientsValidator()
    {
        RuleFor(request => request.Page.PageNumber)
            .GreaterThan(0)
            .WithMessage("The page number must be greater than zero.");

        RuleFor(request => request.Page.PageSize)
            .InclusiveBetween(1, PageRequest.MaximumPageSize)
            .WithMessage($"The page size must be between 1 and {PageRequest.MaximumPageSize}.");
    }
}

/// <summary>
/// Validates the patient order request.
/// </summary>
public sealed class GetPatientOrderValidator : AbstractValidator<GetPatientOrderQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetPatientOrderValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");
    }
}

/// <summary>
/// Validates the patient history request.
/// </summary>
public sealed class GetPatientHistoryValidator : AbstractValidator<GetPatientHistoryQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetPatientHistoryValidator()
    {
        RuleFor(request => request.PatientId)
            .NotEmpty()
            .WithMessage("The patient identifier is required.");
    }
}

/// <summary>
/// Validates the attachment of the composite order identifier to an order.
/// </summary>
public sealed class AttachOrderIdentifierValidator : AbstractValidator<AttachOrderIdentifierCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public AttachOrderIdentifierValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");

        RuleFor(request => request.PersonKey)
            .NotEmpty()
            .WithMessage("The person key is required.");

        RuleFor(request => request.PersonKey)
            .Length(2)
            .WithMessage("The person key must be exactly 2 characters long.");

        RuleFor(request => request.LeadingSegment)
            .NotEmpty()
            .WithMessage("The leading segment is required.");

        RuleFor(request => request.LeadingSegment)
            .Length(2)
            .WithMessage("The leading segment must be exactly 2 characters long.");

        RuleFor(request => request.MiddleSegment)
            .NotEmpty()
            .WithMessage("The middle segment is required.");

        RuleFor(request => request.MiddleSegment)
            .Length(5)
            .WithMessage("The middle segment must be exactly 5 characters long.");

        RuleFor(request => request.DisplayCode)
            .NotEmpty()
            .WithMessage("The display code is required.");

        RuleFor(request => request.DisplayCode)
            .Length(3)
            .WithMessage("The display code must be exactly 3 characters long.");
    }
}
