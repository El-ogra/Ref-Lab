using FluentValidation;
using RefLab.Application.Features.Configuration;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the report-layout customisation request.
/// </summary>
public sealed class CustomiseReportLayoutValidator : AbstractValidator<CustomiseReportLayoutCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public CustomiseReportLayoutValidator()
    {
        RuleFor(request => request.LeftMargin)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.LeftMargin.HasValue)
            .WithMessage("A report margin cannot be negative.");

        RuleFor(request => request.RightMargin)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.RightMargin.HasValue)
            .WithMessage("A report margin cannot be negative.");

        RuleFor(request => request.TopMargin)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.TopMargin.HasValue)
            .WithMessage("A report margin cannot be negative.");

        RuleFor(request => request.BottomMargin)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.BottomMargin.HasValue)
            .WithMessage("A report margin cannot be negative.");
    }
}

/// <summary>
/// Validates the connection-receipt request.
/// </summary>
public sealed class ConfigureConnectionReceiptValidator : AbstractValidator<ConfigureConnectionReceiptCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ConfigureConnectionReceiptValidator()
    {
        RuleFor(request => request)
            .Must(request => request.Header is not null || request.Footer is not null)
            .WithMessage("A header or a footer must be supplied.");
    }
}

/// <summary>
/// Validates the laboratory-settings request.
/// </summary>
public sealed class ConfigureLaboratorySettingsValidator : AbstractValidator<ConfigureLaboratorySettingsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ConfigureLaboratorySettingsValidator()
    {
        RuleFor(request => request.PaperSize)
            .IsInEnum()
            .When(request => request.PaperSize.HasValue)
            .WithMessage("The paper size is not known.");

        RuleFor(request => request.ReportSequenceStart)
            .GreaterThan(0)
            .When(request => request.ReportSequenceStart.HasValue)
            .WithMessage("The report sequence origin must be a positive integer.");

        RuleFor(request => request.HistoryDisplayMode)
            .GreaterThanOrEqualTo(0)
            .When(request => request.HistoryDisplayMode.HasValue)
            .WithMessage("The history display mode cannot be negative.");
    }
}
