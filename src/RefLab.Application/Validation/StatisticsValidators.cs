using FluentValidation;
using RefLab.Application.Features.Statistics;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the laboratory-statistics request.
/// </summary>
public sealed class GetLaboratoryStatisticsValidator : AbstractValidator<GetLaboratoryStatisticsQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetLaboratoryStatisticsValidator()
    {
        RuleFor(request => request)
            .Must(request => request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");
    }
}

/// <summary>
/// Validates the performance-statistics request.
/// </summary>
public sealed class GetPerformanceStatisticsValidator : AbstractValidator<GetPerformanceStatisticsQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetPerformanceStatisticsValidator()
    {
        RuleFor(request => request)
            .Must(request => request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");
    }
}
