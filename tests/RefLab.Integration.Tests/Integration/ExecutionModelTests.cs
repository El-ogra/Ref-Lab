using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Features.Identity;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Enums;
using RefLab.Infrastructure.Persistence;
using RefLab.Integration.Tests.Integration;

namespace RefLab.Integration.Tests.Integration;

/// <summary>
/// The execution model of the application.
/// </summary>
/// <remarks>
/// <para>
/// MediatR resolves its handlers from the root service provider, and the
/// persistence layer registers the context as a scoped service. The two facts
/// together mean that the whole application shares one long-lived
/// <see cref="RefLabDbContext"/> for as long as the process runs.
/// </para>
/// <para>
/// That is not an accident of the test harness; it is how the desktop
/// application is wired, and the test fixture reproduces it deliberately. The
/// tests below pin the behaviour down, so that a future change to the
/// registration cannot silently alter the execution model of a laboratory
/// installation.
/// </para>
/// </remarks>
public sealed class ExecutionModelTests : IClassFixture<IntegrationTestHostFixture>
{
    private readonly IntegrationTestHostFixture _fixture;

    /// <summary>Initialises a new test class.</summary>
    /// <param name="fixture">The fixture that stands up the application.</param>
    public ExecutionModelTests(IntegrationTestHostFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>The host under test.</summary>
    private IntegrationTestHost Host => _fixture.Host;

    [Fact]
    public void The_container_has_no_service_scope_factory_for_the_mediator()
    {
        // MediatR 12 resolves the handlers through the provider it was built
        // with. There is no scope factory in the registration, so the handler
        // is taken from the root provider and the scoped context is created
        // in the root scope.
        var factory = Host.Services.GetService<IServiceScopeFactory>();

        Assert.NotNull(factory);

        var rootContext = Host.Services.GetRequiredService<RefLabDbContext>();

        using var scope = Host.Services.CreateScope();

        var scopedContext = scope.ServiceProvider.GetRequiredService<RefLabDbContext>();

        Assert.NotSame(rootContext, scopedContext);
    }

    [Fact]
    public async Task A_request_is_served_by_the_context_of_the_root_provider()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var order = await Host.Mediator.Send(new RegisterPatientCommand(
            SyntheticData.PatientName(suffix),
            Sex.Male,
            30,
            AgeUnit.Years,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "EGP",
            CreateOrder: true));

        var rootContext = Host.Services.GetRequiredService<RefLabDbContext>();

        // The handler wrote through the context of the root provider, so the
        // change tracker of that very instance knows about the new rows.
        Assert.Contains(
            rootContext.ChangeTracker.Entries<Domain.Patients.Patient>(),
            entry => entry.Entity.Name == SyntheticData.PatientName(suffix));

        Assert.Contains(
            rootContext.ChangeTracker.Entries<Domain.Patients.PatientOrder>(),
            entry => entry.Entity.Id == order.Id);
    }

    [Fact]
    public async Task The_context_survives_from_one_request_to_the_next()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();
        var patientName = SyntheticData.PatientName(suffix);

        await Host.Mediator.Send(new RegisterPatientCommand(
            patientName,
            Sex.Male,
            30,
            AgeUnit.Years,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "EGP",
            CreateOrder: true));

        var context = Host.Services.GetRequiredService<RefLabDbContext>();

        // A second request does not reset the context.
        await Host.Mediator.Send(new RegisterPatientCommand(
            SyntheticData.PatientName(suffix + "b"),
            Sex.Female,
            26,
            AgeUnit.Years,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "EGP",
            CreateOrder: true));

        // The row of the first request is still tracked.
        Assert.Contains(
            context.ChangeTracker.Entries<Domain.Patients.Patient>(),
            entry => entry.Entity.Name == patientName);
    }
}
