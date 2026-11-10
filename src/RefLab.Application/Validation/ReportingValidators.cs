using FluentValidation;
using RefLab.Application.Features.Reporting;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the consolidated report request.
/// </summary>
public sealed class ProduceConsolidatedReportValidator : AbstractValidator<ProduceConsolidatedReportCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ProduceConsolidatedReportValidator()
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
/// Validates the empty-report request.
/// </summary>
public sealed class ProduceEmptyReportValidator : AbstractValidator<ProduceEmptyReportCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ProduceEmptyReportValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("The report title is required.");

        RuleFor(request => request.ColumnNames)
            .NotEmpty()
            .WithMessage("At least one column heading is required.");

        RuleFor(request => request.DestinationDirectory)
            .NotEmpty()
            .WithMessage("The destination directory is required.");
    }
}

/// <summary>
/// Validates the invoice printing request.
/// </summary>
public sealed class PrintPatientInvoiceValidator : AbstractValidator<PrintPatientInvoiceCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public PrintPatientInvoiceValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");

        RuleFor(request => request.DestinationDirectory)
            .NotEmpty()
            .WithMessage("The destination directory is required.");
    }
}

/// <summary>
/// Validates the identity-card printing request.
/// </summary>
public sealed class PrintPatientIdCardValidator : AbstractValidator<PrintPatientIdCardCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public PrintPatientIdCardValidator()
    {
        RuleFor(request => request.PatientId)
            .NotEmpty()
            .WithMessage("The patient identifier is required.");

        RuleFor(request => request.DestinationDirectory)
            .NotEmpty()
            .WithMessage("The destination directory is required.");
    }
}

/// <summary>
/// Validates the printer binding request.
/// </summary>
public sealed class ConfigurePrintersValidator : AbstractValidator<ConfigurePrintersCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ConfigurePrintersValidator()
    {
        RuleFor(request => request.Role)
            .IsInEnum()
            .WithMessage("The printer role is not known.");

        RuleFor(request => request.PrinterName)
            .NotEmpty()
            .When(request => request.PrinterName is not null)
            .WithMessage("A printer name cannot be empty.");
    }
}

/// <summary>
/// Validates the report header and footer request.
/// </summary>
public sealed class ConfigureReportHeaderFooterValidator : AbstractValidator<ConfigureReportHeaderFooterCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ConfigureReportHeaderFooterValidator()
    {
        RuleFor(request => request.PaperSize)
            .IsInEnum()
            .When(request => request.PaperSize.HasValue)
            .WithMessage("The paper size is not known.");
    }
}
