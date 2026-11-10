using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Abstractions;
using RefLab.Infrastructure.Persistence;

namespace RefLab.Infrastructure.Tests.Persistence;

/// <summary>
/// The persistence adapter is registered with the container.
/// </summary>
public class PersistenceRegistrationTests
{
    private const string ConnectionString =
        "Server=localhost;Database=RefLab;User Id=sa;Password=not-a-real-password;TrustServerCertificate=True";

    [Fact]
    public void The_persistence_layer_registers_both_abstractions()
    {
        var services = new ServiceCollection();

        services.AddPersistenceLayer(ConnectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IAppDbContext>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<RefLabDbContext>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<DatabaseInitializer>());
    }

    [Fact]
    public void The_read_side_and_the_write_side_share_one_unit_of_work()
    {
        var services = new ServiceCollection();

        services.AddPersistenceLayer(ConnectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var readSide = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var writeSide = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var context = scope.ServiceProvider.GetRequiredService<RefLabDbContext>();

        Assert.Same(context, readSide);
        Assert.Same(context, writeSide);
    }

    [Fact]
    public void An_empty_connection_string_is_rejected()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentException>(() => services.AddPersistenceLayer("  "));
    }

    [Fact]
    public void Every_application_abstraction_has_an_implementation()
    {
        var services = new ServiceCollection();

        services.AddInfrastructureLayer();
        services.AddPersistenceLayer(ConnectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IClock>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IPasswordHasher>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICurrentUser>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IReportRenderer>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IPdfExporter>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IPrintSequenceGenerator>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IAppDbContext>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
    }

    [Fact]
    public void The_context_is_registered_against_sql_server()
    {
        var services = new ServiceCollection();

        services.AddPersistenceLayer(ConnectionString);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<RefLabDbContext>();

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
    }
}
