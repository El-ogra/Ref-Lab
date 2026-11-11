using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Identity;
using RefLab.Infrastructure.Session;
using RefLab.Presentation.Common;
using RefLab.Presentation.Tests.Fakes;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Tests.Fakes;

/// <summary>
/// The presentation layer, wired the way the composition root wires it, with
/// the mediator replaced by a stub.
/// </summary>
/// <remarks>
/// The composition root builds a generic host and registers the four layers.
/// This host does the same for the presentation layer alone, so that the view
/// models under test are the real ones and their collaborators are the real
/// ones, with only the MediatR pipeline replaced.
/// </remarks>
public sealed class PresentationTestHost : IDisposable
{
    private readonly ServiceProvider _services;

    /// <summary>Initialises a new host.</summary>
    /// <param name="mediator">The mediator that answers the requests.</param>
    public PresentationTestHost(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        var services = new ServiceCollection();

        services.AddSingleton(mediator);
        services.AddSingleton<RequestExecutor>();
        services.AddSingleton<SessionCurrentUser>();
        services.AddSingleton<ICurrentUser>(provider => provider.GetRequiredService<SessionCurrentUser>());
        services.AddSingleton<OperatorSession>();

        _services = services.BuildServiceProvider();
    }

    /// <summary>The root service provider of the host.</summary>
    public IServiceProvider Services => _services;

    /// <summary>The navigation service the view models use.</summary>
    public RecordingNavigationService Navigation { get; } = new();

    /// <summary>The session of the signed-in operator.</summary>
    public SessionCurrentUser Session => _services.GetRequiredService<SessionCurrentUser>();

    /// <summary>The shell view model.</summary>
    public ShellViewModel Shell =>
        new(
            _services.GetRequiredService<RequestExecutor>(),
            Navigation,
            Session,
            _services.GetRequiredService<ICurrentUser>());

    /// <summary>The sign-in view model.</summary>
    public SignInViewModel SignIn =>
        new(
            _services.GetRequiredService<RequestExecutor>(),
            _services.GetRequiredService<OperatorSession>(),
            Navigation,
            Shell);

    /// <summary>Builds a mediator that answers a sign-in with the given operator.</summary>
    /// <param name="operatorId">The identifier of the operator.</param>
    /// <param name="name">The name of the operator.</param>
    /// <param name="permissions">The permissions the operator holds.</param>
    /// <returns>The mediator.</returns>
    public static StubMediator SignInMediator(
        int operatorId = 7,
        string name = "p6.operator",
        params string[] permissions)
    {
        var granted = permissions.Length == 0
            ? new[] { nameof(Domain.Identity.Permission.RegisterPatient) }
            : permissions;

        var dto = new OperatorDto(
            operatorId,
            name,
            "Phase 6 operator",
            IsAdministrator: false,
            IsActive: true,
            granted);

        return StubMediator.Immediate(_ => new SignInResultDto(dto, DateTimeOffset.UtcNow));
    }

    /// <inheritdoc />
    public void Dispose() => _services.Dispose();
}
