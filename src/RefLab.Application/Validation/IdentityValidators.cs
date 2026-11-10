using FluentValidation;
using RefLab.Application.Features.Identity;
using RefLab.Domain.Identity;

namespace RefLab.Application.Validation;

/// <summary>
/// Validates the operator-listing request.
/// </summary>
public sealed class GetOperatorsValidator : AbstractValidator<GetOperatorsQuery>
{
    /// <summary>Initialises a new validator.</summary>
    public GetOperatorsValidator()
    {
        RuleFor(request => request.Name)
            .MaximumLength(40)
            .When(request => request.Name is not null)
            .WithMessage("The operator name filter cannot be longer than 40 characters.");
    }
}

/// <summary>
/// Validates the amendment of an operator account.
/// </summary>
public sealed class UpdateOperatorValidator : AbstractValidator<UpdateOperatorCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public UpdateOperatorValidator()
    {
        RuleFor(request => request.OperatorId)
            .GreaterThan(0)
            .WithMessage("The operator identifier must be a positive integer.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("The operator name is required.");

    }
}

/// <summary>
/// Validates the replacement of the permissions granted to an operator.
/// </summary>
public sealed class AssignOperatorPermissionsValidator : AbstractValidator<AssignOperatorPermissionsCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public AssignOperatorPermissionsValidator()
    {
        RuleFor(request => request.OperatorId)
            .GreaterThan(0)
            .WithMessage("The operator identifier must be a positive integer.");

        RuleForEach(request => request.Permissions)
            .Must(permission => Enum.TryParse<Permission>(permission, ignoreCase: true, out _))
            .WithMessage("'{PropertyValue}' is not a known permission.");
    }
}

/// <summary>
/// Validates the operator sign-in request.
/// </summary>
public sealed class SignInValidator : AbstractValidator<SignInCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public SignInValidator()
    {
        RuleFor(request => request.OperatorName)
            .NotEmpty()
            .WithMessage("The operator name is required.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("The password is required.");
    }
}

/// <summary>
/// Validates the operator password change request.
/// </summary>
public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public ChangePasswordValidator()
    {
        RuleFor(request => request.OperatorId)
            .GreaterThan(0)
            .WithMessage("The operator identifier must be positive.");

        RuleFor(request => request.CurrentPassword)
            .NotEmpty()
            .WithMessage("The current password is required.");

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .WithMessage("The new password is required.");

        RuleFor(request => request.NewPassword)
            .Must(PasswordPolicy.IsSatisfiedBy)
            .WithMessage("The new password does not satisfy the password policy.");
    }
}

/// <summary>
/// Validates the creation of an operator account.
/// </summary>
public sealed class CreateOperatorValidator : AbstractValidator<CreateOperatorCommand>
{
    /// <summary>Initialises a new validator.</summary>
    public CreateOperatorValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("The operator name is required.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("The password is required.");

        RuleFor(request => request.Password)
            .Must(PasswordPolicy.IsSatisfiedBy)
            .WithMessage("The password does not satisfy the password policy.");

        RuleForEach(request => request.Permissions)
            .Must(permission => Enum.TryParse<Permission>(permission, ignoreCase: true, out _))
            .WithMessage("'{PropertyValue}' is not a known permission.");
    }
}
