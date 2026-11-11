using RefLab.Application.Features.Identity;
using RefLab.Domain.Identity;
using RefLab.Presentation.Navigation;
using RefLab.Presentation.Tests.Fakes;

namespace RefLab.Presentation.Tests.ViewModels;

/// <summary>
/// The sign-in screen.
/// </summary>
/// <remarks>
/// The sign-in screen is the first thing the laboratory operator sees and the
/// only screen that runs before the session exists. Its behaviour is pinned
/// down here: an empty name is refused without a request, a successful
/// sign-in establishes the session and moves to the patient work screen, a
/// refusal shows a message and stays put, and the password is cleared once
/// it has been accepted.
/// </remarks>
public sealed class SignInViewModelTests
{
    [Fact]
    public async Task An_empty_operator_name_is_refused_without_a_request()
    {
        var mediator = StubMediator.Immediate(_ => null);

        using var host = new PresentationTestHost(mediator);

        var viewModel = host.SignIn;

        viewModel.OperatorName = "   ";
        viewModel.Password = "irrelevant";

        await viewModel.SignInCommand.ExecuteAsync(null);

        Assert.NotNull(viewModel.ErrorMessage);
        Assert.Contains("operator name is required", viewModel.ErrorMessage!, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(mediator.Sent);
        Assert.Empty(host.Navigation.Visited);
    }

    [Fact]
    public async Task A_successful_sign_in_establishes_the_session_and_opens_the_patient_work_screen()
    {
        var mediator = PresentationTestHost.SignInMediator(
            operatorId: 42,
            name: "p6.operator",
            nameof(Permission.RegisterPatient),
            nameof(Permission.OrderTests),
            nameof(Permission.EnterResults),
            nameof(Permission.ReviewResults),
            nameof(Permission.PrintReports));

        using var host = new PresentationTestHost(mediator);

        var viewModel = host.SignIn;

        viewModel.OperatorName = "p6.operator";
        viewModel.Password = "p6-secret";

        await viewModel.SignInCommand.ExecuteAsync(null);

        Assert.Null(viewModel.ErrorMessage);
        Assert.Equal("Signed in.", viewModel.StatusMessage);

        // The session carries the operator and the permissions.
        Assert.True(host.Session.IsAuthenticated);
        Assert.Equal(42, host.Session.OperatorId);
        Assert.Equal("p6.operator", host.Session.OperatorName);
        Assert.True(host.Session.HasPermission(Permission.OrderTests));
        Assert.True(host.Session.HasPermission(Permission.PrintReports));

        // The shell moved to the patient work screen.
        Assert.Equal(new[] { ScreenKeys.PatientWork }, host.Navigation.Visited);
        Assert.Equal(ScreenKeys.PatientWork, host.Navigation.CurrentScreenKey);
    }

    [Fact]
    public async Task A_refused_sign_in_shows_a_message_and_stays_put()
    {
        var mediator = StubMediator.Immediate(_ =>
            throw new Application.Common.ForbiddenException("The operator name or the password is incorrect."));

        using var host = new PresentationTestHost(mediator);

        var viewModel = host.SignIn;

        viewModel.OperatorName = "p6.operator";
        viewModel.Password = "wrong";

        await viewModel.SignInCommand.ExecuteAsync(null);

        Assert.NotNull(viewModel.ErrorMessage);
        Assert.Empty(host.Navigation.Visited);
        Assert.False(host.Session.IsAuthenticated);
    }

    [Fact]
    public async Task The_password_is_cleared_after_a_successful_sign_in()
    {
        var mediator = PresentationTestHost.SignInMediator();

        using var host = new PresentationTestHost(mediator);

        var viewModel = host.SignIn;

        viewModel.OperatorName = "p6.operator";
        viewModel.Password = "p6-secret";

        await viewModel.SignInCommand.ExecuteAsync(null);

        Assert.Equal(string.Empty, viewModel.Password);
    }

    [Fact]
    public async Task The_sign_in_command_is_not_executed_while_the_screen_is_busy()
    {
        var operatorDto = new OperatorDto(
            42,
            "p6.operator",
            "Phase 6 operator",
            IsAdministrator: false,
            IsActive: true,
            new[] { nameof(Permission.RegisterPatient) });

        // The response is held open, so the first request is genuinely still
        // in flight when the second one is attempted.
        var gate = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        var mediator = new StubMediator(_ => gate.Task);

        using var host = new PresentationTestHost(mediator);

        var viewModel = host.SignIn;

        viewModel.OperatorName = "p6.operator";
        viewModel.Password = "p6-secret";

        viewModel.SignInCommand.Execute(null);

        // A command that is already running refuses a second execution, which
        // is what keeps a double click on the sign-in button from sending the
        // request twice.
        await viewModel.SignInCommand.ExecuteAsync(null);

        Assert.Single(mediator.Sent);

        // The first request is released with a valid response, so that it
        // completes without faulting.
        gate.SetResult(new SignInResultDto(operatorDto, DateTimeOffset.UtcNow));

        await viewModel.SignInCommand.ExecutionTask!;
    }
}
