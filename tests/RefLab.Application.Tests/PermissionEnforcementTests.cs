using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Domain.Identity;

namespace RefLab.Application.Tests;

/// <summary>
/// Proves that every permission-gated use case is actually enforced.
/// </summary>
/// <remarks>
/// The authorisation behaviour is invoked directly, so the test does not depend
/// on the request's own validation rules — the question here is only whether
/// the operator without the permission is refused.
/// </remarks>
public class PermissionEnforcementTests
{
    private static readonly Assembly ApplicationAssembly = typeof(RefLab.Application.ApplicationServiceCollectionExtensions).Assembly;

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        var db = new Fakes.FakeAppDbContext();
        var currentUser = new Fakes.FakeCurrentUser { GrantAllPermissions = false };

        services.AddSingleton<IAppDbContext>(db);
        services.AddSingleton<IUnitOfWork>(new Fakes.FakeUnitOfWork(db));
        services.AddSingleton(currentUser);
        services.AddSingleton<ICurrentUser>(currentUser);
        services.AddSingleton<IClock>(new Fakes.FakeClock());
        services.AddSingleton<IPasswordHasher>(new Fakes.FakePasswordHasher());
        services.AddSingleton<IReportRenderer>(new Fakes.FakeReportRenderer());
        services.AddSingleton<IPdfExporter>(new Fakes.FakePdfExporter());
        services.AddSingleton<IPrintSequenceGenerator>(new Fakes.FakePrintSequenceGenerator());

        services.AddApplicationLayer();

        return services.BuildServiceProvider();
    }

    private static object CreateInstance(Type type)
    {
        var constructor = type.GetConstructors().OrderBy(c => c.GetParameters().Length).First();
        var parameters = constructor.GetParameters();
        var arguments = new object?[parameters.Length];

        for (var index = 0; index < parameters.Length; index++)
        {
            var parameterType = parameters[index].ParameterType;

            arguments[index] = parameterType.IsValueType
                ? Activator.CreateInstance(parameterType)
                : null;
        }

        return constructor.Invoke(arguments);
    }

    private static async Task AssertRejected<TRequest, TResponse>(
        IServiceProvider provider,
        TRequest request)
        where TRequest : notnull
    {
        // GetRequiredService would return only the last registered behaviour,
        // so the authorisation behaviour is selected explicitly.
        var behaviour = provider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .OfType<AuthorizationBehavior<TRequest, TResponse>>()
            .Single();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            behaviour.Handle(request, _ => Task.FromResult(default(TResponse)!), CancellationToken.None));
    }

    [Fact]
    public async Task Every_permission_gated_use_case_is_refused_without_the_permission()
    {
        using var provider = BuildProvider();

        var gated = ApplicationAssembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.GetInterfaces().Contains(typeof(IRequirePermission)))
            .ToArray();

        Assert.True(gated.Length >= 40, $"Only {gated.Length} permission-gated use cases were found.");

        var method = typeof(PermissionEnforcementTests).GetMethod(
            nameof(AssertRejected),
            BindingFlags.NonPublic | BindingFlags.Static)!;

        var refusals = new List<string>();

        foreach (var requestType in gated)
        {
            var requestInterface = requestType.GetInterfaces().First(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IRequest<>));

            var responseType = requestInterface.GetGenericArguments()[0];

            var generic = method.MakeGenericMethod(requestType, responseType);

            try
            {
                var task = (Task)generic.Invoke(null, new[] { provider, CreateInstance(requestType) })!;

                await task;
            }
            catch (ForbiddenException)
            {
                // the expected outcome
            }
            catch (Exception exception)
            {
                refusals.Add($"{requestType.Name}: {exception.GetType().Name} {exception.Message}");
            }
        }

        Assert.True(
            refusals.Count == 0,
            "These permission-gated use cases were not refused: " + string.Join(" | ", refusals));
    }

    [Fact]
    public void Every_gated_use_case_names_a_permission_that_exists()
    {
        var gated = ApplicationAssembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.GetInterfaces().Contains(typeof(IRequirePermission)))
            .ToArray();

        var unknown = new List<string>();

        foreach (var requestType in gated)
        {
            var property = requestType.GetProperty("Permission");

            if (property is null)
            {
                unknown.Add($"{requestType.Name}: no Permission property");
                continue;
            }

            var value = property.GetValue(CreateInstance(requestType));

            if (value is not Permission permission || !Enum.IsDefined(permission))
            {
                unknown.Add($"{requestType.Name}: '{value}'");
            }
        }

        Assert.True(
            unknown.Count == 0,
            "These use cases name an unknown permission: " + string.Join(", ", unknown));
    }
}
