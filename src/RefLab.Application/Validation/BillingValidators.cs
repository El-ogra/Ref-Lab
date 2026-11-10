using FluentValidation;
using RefLab.Application.Features.Billing;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the referring-party listing request.
/// </summary>
public sealed class GetReferringPartiesValidator : AbstractValidator<GetReferringPartiesQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetReferringPartiesValidator()
    {
        RuleFor(request => request.Name)
            .MaximumLength(40)
            .When(request => request.Name is not null)
            .WithMessage("The referring-party name filter cannot be longer than 40 characters.");
    }
}

/// <summary>
/// Validates the payment query.
/// </summary>
public sealed class GetPaymentsValidator : AbstractValidator<GetPaymentsQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetPaymentsValidator()
    {
        RuleFor(request => request)
            .Must(request => !request.From.HasValue || !request.To.HasValue || request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");
    }
}

/// <summary>
/// Validates the period stock-take request.
/// </summary>
public sealed class RunPeriodStockTakeValidator : AbstractValidator<RunPeriodStockTakeQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public RunPeriodStockTakeValidator()
    {
        RuleFor(request => request)
            .Must(request => request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .WithMessage("The currency code is required.");
    }
}

/// <summary>
/// Validates the stock-take by physician or referring party.
/// </summary>
public sealed class RunStockTakeByPhysicianOrPartyValidator
    : AbstractValidator<RunStockTakeByPhysicianOrPartyQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public RunStockTakeByPhysicianOrPartyValidator()
    {
        RuleFor(request => request)
            .Must(request => request.From <= request.To)
            .WithMessage("The start of the period cannot be after its end.");

        RuleFor(request => request)
            .Must(request => request.ReferringPartyCode is not null || request.TreatingPhysicianCode is not null)
            .WithMessage("A referring party or a treating physician must be supplied.");

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .WithMessage("The currency code is required.");
    }
}

/// <summary>
/// Validates the default account-type request.
/// </summary>
public sealed class SetDefaultAccountTypeValidator : AbstractValidator<SetDefaultAccountTypeCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public SetDefaultAccountTypeValidator()
    {
        RuleFor(request => request.AccountType)
            .NotEmpty()
            .WithMessage("The account type is required.");
    }
}

/// <summary>
/// Validates the payment-recording request.
/// </summary>
public sealed class RecordPaymentValidator : AbstractValidator<RecordPaymentCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public RecordPaymentValidator()
    {
        RuleFor(request => request.OrderId)
            .NotEmpty()
            .WithMessage("The order identifier is required.");

        RuleFor(request => request.Amount)
            .GreaterThan(0m)
            .WithMessage("The settled amount must be greater than zero.");
    }
}

/// <summary>
/// Validates the commission-computation request.
/// </summary>
public sealed class ComputeCommissionValidator : AbstractValidator<ComputeCommissionQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public ComputeCommissionValidator()
    {
        RuleFor(request => request.ReferringPartyId)
            .GreaterThan(0)
            .WithMessage("The referring party identifier must be positive.");

        RuleFor(request => request.OrderIds)
            .NotEmpty()
            .WithMessage("At least one order must be supplied.");
    }
}
