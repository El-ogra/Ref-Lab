using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Abstractions;
using RefLab.Infrastructure.Persistence;
using RefLab.Infrastructure.Reports;
using RefLab.Infrastructure.Security;
using RefLab.Infrastructure.Session;
using RefLab.Infrastructure.Time;

namespace RefLab.Infrastructure;

/// <summary>
/// Registers the Infrastructure layer with the dependency-injection container.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers the infrastructure adapters that implement the Application
    /// layer abstractions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This registers six of the eight Application abstractions:
    /// <c>IClock</c>, <c>IPasswordHasher</c>, <c>ICurrentUser</c>,
    /// <c>IReportRenderer</c>, <c>IPdfExporter</c> and
    /// <c>IPrintSequenceGenerator</c>.
    /// </para>
    /// <para>
    /// The two remaining abstractions — <c>IAppDbContext</c> and
    /// <c>IUnitOfWork</c> — are registered by
    /// <see cref="AddPersistenceLayer(IServiceCollection, string)"/>, which must
    /// be called as well before the use case handlers can be resolved.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<SessionCurrentUser>();
        services.AddSingleton<ICurrentUser>(provider => provider.GetRequiredService<SessionCurrentUser>());
        services.AddSingleton<IReportRenderer, QuestPdfReportRenderer>();
        services.AddSingleton<IPdfExporter, QuestPdfExporter>();
        services.AddSingleton<IPrintSequenceGenerator, PrintSequenceGenerator>();

        return services;
    }

    /// <summary>
    /// Registers the Entity Framework Core persistence adapter against a
    /// SQL Server database.
    /// </summary>
    /// <param name="services">The container to register with.</param>
    /// <param name="connectionString">The SQL Server connection string.</param>
    /// <remarks>
    /// <para>
    /// The context is registered once, as a scoped service, and is then exposed
    /// through both <see cref="IAppDbContext"/> and <see cref="IUnitOfWork"/>.
    /// The two interfaces therefore share one unit of work, so a handler that
    /// reads through the query side and writes through the unit of work sees a
    /// consistent snapshot.
    /// </para>
    /// <para>
    /// The laboratory is a single-site installation: the connection string
    /// points at the one laboratory database, and the schema carries no branch
    /// table and no per-branch schema.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddPersistenceLayer(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("A connection string is required.", nameof(connectionString));
        }

        return services.AddPersistenceLayer(options => options.UseSqlServer(
            connectionString,
            sql => sql.MigrationsAssembly(typeof(RefLabDbContext).Assembly.FullName)));
    }

    /// <summary>
    /// Registers the Entity Framework Core persistence adapter against a
    /// provider chosen by the caller.
    /// </summary>
    /// <param name="services">The container to register with.</param>
    /// <param name="configure">The callback that configures the provider.</param>
    /// <remarks>
    /// The integration tests use this overload to point the context at an
    /// in-memory SQLite database; the laboratory installation uses the SQL
    /// Server overload.
    /// </remarks>
    public static IServiceCollection AddPersistenceLayer(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddDbContext<RefLabDbContext>(configure);

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<RefLabDbContext>());
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<RefLabDbContext>());
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}
