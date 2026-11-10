using FluentValidation;
using MediatR;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Identity;

namespace RefLab.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    private sealed record SampleRequest(string Name) : IRequest<string>;

    private sealed class RejectingValidator : AbstractValidator<SampleRequest>
    {
        public RejectingValidator()
        {
            RuleFor(request => request.Name).NotEmpty().WithMessage("The name is required.");
        }
    }

    private sealed class AcceptingValidator : AbstractValidator<SampleRequest>
    {
        public AcceptingValidator()
        {
            RuleFor(request => request.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task A_request_that_fails_validation_is_rejected()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>(new[] { new RejectingValidator() });

        var exception = await Assert.ThrowsAsync<RequestValidationException>(() =>
            behavior.Handle(new SampleRequest(string.Empty), (_ => Task.FromResult("ok")), CancellationToken.None));

        Assert.Contains("The name is required.", exception.Errors);
    }

    [Fact]
    public async Task A_request_that_passes_validation_reaches_the_handler()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>(new[] { new AcceptingValidator() });

        var response = await behavior.Handle(
            new SampleRequest("value"),
            (_ => Task.FromResult("reached")),
            CancellationToken.None);

        Assert.Equal("reached", response);
    }

    [Fact]
    public async Task A_request_without_validators_reaches_the_handler()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>(Array.Empty<IValidator<SampleRequest>>());

        var response = await behavior.Handle(
            new SampleRequest(string.Empty),
            (_ => Task.FromResult("reached")),
            CancellationToken.None);

        Assert.Equal("reached", response);
    }
}

public class AuthorizationBehaviorTests
{
    private sealed record ProtectedRequest : IRequest<string>, IRequirePermission
    {
        public Permission Permission => Permission.EnterResults;
    }

    private sealed record UnprotectedRequest : IRequest<string>;

    [Fact]
    public async Task A_request_without_the_permission_is_rejected()
    {
        var currentUser = new Tests.Fakes.FakeCurrentUser();
        var behavior = new AuthorizationBehavior<ProtectedRequest, string>(currentUser);

        var exception = await Assert.ThrowsAsync<ForbiddenException>(() =>
            behavior.Handle(new ProtectedRequest(), (_ => Task.FromResult("ok")), CancellationToken.None));

        Assert.Contains("EnterResults", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_request_with_the_permission_reaches_the_handler()
    {
        var currentUser = new Tests.Fakes.FakeCurrentUser();
        currentUser.Permissions.Add(Permission.EnterResults);

        var behavior = new AuthorizationBehavior<ProtectedRequest, string>(currentUser);

        var response = await behavior.Handle(
            new ProtectedRequest(),
            (_ => Task.FromResult("reached")),
            CancellationToken.None);

        Assert.Equal("reached", response);
    }

    [Fact]
    public async Task A_request_that_requires_no_permission_reaches_the_handler()
    {
        var behavior = new AuthorizationBehavior<UnprotectedRequest, string>(new Tests.Fakes.FakeCurrentUser());

        var response = await behavior.Handle(
            new UnprotectedRequest(),
            (_ => Task.FromResult("reached")),
            CancellationToken.None);

        Assert.Equal("reached", response);
    }
}

public class DomainExceptionBehaviorTests
{
    private sealed record FailingRequest : IRequest<string>;

    [Fact]
    public async Task A_domain_exception_is_translated_into_a_conflict()
    {
        var behavior = new DomainExceptionBehavior<FailingRequest, string>();

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            behavior.Handle(
                new FailingRequest(),
                (_ => throw new InvalidPatientOrderException("The order is not valid.")),
                CancellationToken.None));

        Assert.Equal("The order is not valid.", exception.Message);
    }

    [Fact]
    public async Task A_successful_request_passes_through_unchanged()
    {
        var behavior = new DomainExceptionBehavior<FailingRequest, string>();

        var response = await behavior.Handle(
            new FailingRequest(),
            (_ => Task.FromResult("reached")),
            CancellationToken.None);

        Assert.Equal("reached", response);
    }
}
