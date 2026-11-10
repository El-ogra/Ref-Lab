using FluentValidation;
using RefLab.Application.Features.Microbiology;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the culture-result request.
/// </summary>
public sealed class RecordCultureResultValidator : AbstractValidator<RecordCultureResultCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public RecordCultureResultValidator()
    {
        RuleFor(request => request.PatientId)
            .NotEmpty()
            .WithMessage("The patient identifier is required.");

        RuleFor(request => request.CultureId)
            .NotEmpty()
            .WithMessage("The culture identifier is required.");

        RuleFor(request => request.Organism)
            .NotEmpty()
            .WithMessage("The isolated organism is required.");

        RuleForEach(request => request.Susceptibilities)
            .Must(susceptibility => !string.IsNullOrWhiteSpace(susceptibility.AntibioticSymbol))
            .WithMessage("A susceptibility must name the antibiotic.");

        RuleForEach(request => request.Susceptibilities)
            .Must(susceptibility => !string.IsNullOrWhiteSpace(susceptibility.Reading))
            .WithMessage("A susceptibility must carry a reading.");
    }
}

/// <summary>
/// Validates the culture-results query.
/// </summary>
public sealed class GetCultureResultsValidator : AbstractValidator<GetCultureResultsQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetCultureResultsValidator()
    {
        RuleFor(request => request.PatientId)
            .NotEmpty()
            .When(request => request.PatientId.HasValue)
            .WithMessage("The patient identifier is required.");
    }
}

/// <summary>
/// Validates the antibiotic-catalogue request.
/// </summary>
public sealed class ManageAntibioticCatalogueValidator : AbstractValidator<ManageAntibioticCatalogueCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ManageAntibioticCatalogueValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("The antibiotic identifier must be a positive integer.");

        RuleFor(request => request.Symbol)
            .NotEmpty()
            .WithMessage("The antibiotic symbol is required.");

        RuleFor(request => request.ScientificName)
            .NotEmpty()
            .WithMessage("The scientific name is required.");
    }
}

/// <summary>
/// Validates the farm-result request.
/// </summary>
public sealed class ManageFarmResultsValidator : AbstractValidator<ManageFarmResultsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ManageFarmResultsValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("The transaction identifier must be a positive integer.");

        RuleFor(request => request.FarmId)
            .GreaterThan(0)
            .WithMessage("The farm identifier must be a positive integer.");

        RuleFor(request => request.FarmName)
            .NotEmpty()
            .WithMessage("The farm name is required.");
    }
}
