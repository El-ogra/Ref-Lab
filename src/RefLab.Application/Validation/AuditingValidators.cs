using FluentValidation;
using RefLab.Application.Common;
using RefLab.Application.Features.Auditing;
using RefLab.Domain.Enums;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the attendance-recording request.
/// </summary>
public sealed class RecordAttendanceValidator : AbstractValidator<RecordAttendanceCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public RecordAttendanceValidator()
    {
        RuleFor(request => request.EventType)
            .IsInEnum()
            .WithMessage("The attendance event type is not known.");
    }
}

/// <summary>
/// Validates the print-audit-trail request.
/// </summary>
public sealed class GetPrintAuditTrailValidator : AbstractValidator<GetPrintAuditTrailQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetPrintAuditTrailValidator()
    {
        RuleFor(request => request.Page.PageNumber)
            .GreaterThan(0)
            .WithMessage("The page number must be greater than zero.");

        RuleFor(request => request.Page.PageSize)
            .InclusiveBetween(1, PageRequest.MaximumPageSize)
            .WithMessage($"The page size must be between 1 and {PageRequest.MaximumPageSize}.");
    }
}
