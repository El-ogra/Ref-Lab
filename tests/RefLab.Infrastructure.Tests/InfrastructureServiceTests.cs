using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Abstractions;
using RefLab.Domain.Identity;
using RefLab.Infrastructure.Reports;
using RefLab.Infrastructure.Session;

namespace RefLab.Infrastructure.Tests;

public class PrintSequenceGeneratorTests
{
    [Fact]
    public void The_sequence_starts_at_the_legacy_origin()
    {
        var generator = new PrintSequenceGenerator();

        Assert.Equal(1984, generator.Next());
        Assert.Equal(1985, generator.Next());
    }
}

public class SessionCurrentUserTests
{
    [Fact]
    public void The_session_holds_the_signed_in_operator()
    {
        var session = new SessionCurrentUser();
        var @operator = Operator.Create(1, "lab.tech", "hash", "Technician");
        @operator.Grant(Permission.EnterResults);

        session.SignIn(@operator);

        Assert.True(session.IsAuthenticated);
        Assert.Equal(1, session.OperatorId);
        Assert.Equal("lab.tech", session.OperatorName);
        Assert.True(session.HasPermission(Permission.EnterResults));
        Assert.False(session.HasPermission(Permission.PrintReports));
    }

    [Fact]
    public void Signing_out_clears_the_session()
    {
        var session = new SessionCurrentUser();
        session.SignIn(Operator.Create(1, "lab.tech", "hash"));

        session.SignOut();

        Assert.False(session.IsAuthenticated);
        Assert.Null(session.OperatorId);
        Assert.Null(session.OperatorName);
    }
}

public class InfrastructureRegistrationTests
{
    [Fact]
    public void Every_application_abstraction_has_an_infrastructure_implementation()
    {
        var services = new ServiceCollection();

        services.AddInfrastructureLayer();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IClock>());
        Assert.NotNull(provider.GetRequiredService<IPasswordHasher>());
        Assert.NotNull(provider.GetRequiredService<ICurrentUser>());
        Assert.NotNull(provider.GetRequiredService<IReportRenderer>());
        Assert.NotNull(provider.GetRequiredService<IPdfExporter>());
        Assert.NotNull(provider.GetRequiredService<IPrintSequenceGenerator>());
    }
}
