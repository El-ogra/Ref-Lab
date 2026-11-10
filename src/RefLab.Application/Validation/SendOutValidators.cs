using FluentValidation;
using RefLab.Application.Features.SendOut;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the send-out enrolment request.
/// </summary>
public sealed class CreateSendOutRequestValidator : AbstractValidator<CreateSendOutRequestCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public CreateSendOutRequestValidator()
    {
        RuleFor(request => request.OrderTestId)
            .NotEmpty()
            .WithMessage("The requested test identifier is required.");

        RuleFor(request => request.DestinationLabName)
            .NotEmpty()
            .WithMessage("The destination laboratory is required.");

        RuleFor(request => request.OutPrice)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.OutPrice.HasValue)
            .WithMessage("A send-out price cannot be negative.");
    }
}

/// <summary>
/// Validates the send-out queue query.
/// </summary>
public sealed class GetSendOutQueueValidator : AbstractValidator<GetSendOutQueueQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetSendOutQueueValidator()
    {
        RuleFor(request => request.OrderTestId)
            .NotEmpty()
            .When(request => request.OrderTestId.HasValue)
            .WithMessage("The requested test identifier is required.");
    }
}

/// <summary>
/// Validates the sample-disposition request.
/// </summary>
public sealed class SplitOrWithdrawSampleValidator : AbstractValidator<SplitOrWithdrawSampleCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public SplitOrWithdrawSampleValidator()
    {
        RuleFor(request => request.OrderTestId)
            .NotEmpty()
            .WithMessage("The requested test identifier is required.");

        RuleFor(request => request.Action)
            .IsInEnum()
            .WithMessage("The disposition action is not known.");
    }
}

/// <summary>
/// Validates the send-out dispatch request.
/// </summary>
public sealed class DispatchSendOutSampleValidator : AbstractValidator<DispatchSendOutSampleCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public DispatchSendOutSampleValidator()
    {
        RuleFor(request => request.SendOutRequestId)
            .NotEmpty()
            .WithMessage("The send-out request identifier is required.");
    }
}

/// <summary>
/// Validates the send-out settlement request.
/// </summary>
public sealed class SettleSendOutSampleValidator : AbstractValidator<SettleSendOutSampleCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public SettleSendOutSampleValidator()
    {
        RuleFor(request => request.SendOutRequestId)
            .NotEmpty()
            .WithMessage("The send-out request identifier is required.");
    }
}
