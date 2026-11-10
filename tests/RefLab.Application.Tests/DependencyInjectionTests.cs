using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Application;
using RefLab.Application.Abstractions;
using RefLab.Application.Features.Identity;
using RefLab.Application.Features.Patients;
using RefLab.Application.Tests.Fakes;

namespace RefLab.Application.Tests;

/// <summary>
/// Verifies that the Application layer registers itself correctly.
/// </summary>
/// <remarks>
/// Every handler and every validator in the assembly is enumerated from the
/// compiled types and checked against the container, so a use case that is
/// implemented but never registered cannot pass unnoticed.
/// </remarks>
public class DependencyInjectionTests
{
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationServiceCollectionExtensions).Assembly;

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        var db = new FakeAppDbContext();
        var currentUser = new FakeCurrentUser();

        services.AddSingleton<IAppDbContext>(db);
        services.AddSingleton<IUnitOfWork>(new FakeUnitOfWork(db));
        services.AddSingleton(currentUser);
        services.AddSingleton<ICurrentUser>(currentUser);
        services.AddSingleton<IClock>(new FakeClock());
        services.AddSingleton<IPasswordHasher>(new FakePasswordHasher());
        services.AddSingleton<IReportRenderer>(new FakeReportRenderer());
        services.AddSingleton<IPdfExporter>(new FakePdfExporter());
        services.AddSingleton<IPrintSequenceGenerator>(new FakePrintSequenceGenerator());

        services.AddApplicationLayer();

        return services.BuildServiceProvider();
    }

    private static IEnumerable<Type> HandlerTypes() =>
        ApplicationAssembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .Where(type => type.GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

    private static IEnumerable<Type> ValidatorTypes() =>
        ApplicationAssembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .Where(type => type.GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>)));

    [Fact]
    public void The_mediator_is_resolvable_and_carries_the_four_behaviours()
    {
        using var provider = BuildProvider();

        var mediator = provider.GetRequiredService<ISender>();

        Assert.NotNull(mediator);

        var behaviours = provider.GetServices<IPipelineBehavior<RegisterPatientCommand, PatientOrderDto>>().ToArray();

        Assert.Equal(4, behaviours.Length);
    }

    [Fact]
    public void Every_use_case_handler_is_registered()
    {
        using var provider = BuildProvider();

        var handlers = HandlerTypes().ToArray();

        Assert.True(handlers.Length >= 70, $"Only {handlers.Length} handlers were found.");

        var unregistered = new List<string>();

        foreach (var handler in handlers)
        {
            var contract = handler.GetInterfaces().First(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            var resolved = provider.GetServices(contract).Any(service => service is not null && service.GetType() == handler);

            if (!resolved)
            {
                unregistered.Add(handler.FullName ?? handler.Name);
            }
        }

        Assert.True(
            unregistered.Count == 0,
            "These handlers are implemented but not registered: " + string.Join(", ", unregistered));
    }

    [Fact]
    public void Every_validator_is_registered()
    {
        using var provider = BuildProvider();

        var validators = ValidatorTypes().ToArray();

        Assert.True(validators.Length >= 70, $"Only {validators.Length} validators were found.");

        var unregistered = new List<string>();

        foreach (var validator in validators)
        {
            var contract = validator.GetInterfaces().First(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>));

            var resolved = provider.GetServices(contract).Any(service => service is not null && service.GetType() == validator);

            if (!resolved)
            {
                unregistered.Add(validator.FullName ?? validator.Name);
            }
        }

        Assert.True(
            unregistered.Count == 0,
            "These validators are implemented but not registered: " + string.Join(", ", unregistered));
    }

    [Fact]
    public void The_sign_in_and_the_registration_handlers_resolve()
    {
        using var provider = BuildProvider();

        Assert.NotNull(provider.GetRequiredService<IRequestHandler<SignInCommand, SignInResultDto>>());
        Assert.NotNull(provider.GetRequiredService<IRequestHandler<RegisterPatientCommand, PatientOrderDto>>());
    }
}
