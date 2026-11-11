using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RefLab.Application;
using RefLab.Application.Abstractions;
using RefLab.Domain.Identity;
using RefLab.Infrastructure;
using RefLab.Infrastructure.Persistence;
using RefLab.Infrastructure.Session;

namespace RefLab.Integration.Tests.Integration;

/// <summary>
/// The application under test, wired exactly as the composition root wires it.
/// </summary>
/// <remarks>
/// <para>
/// The host registers the Application layer, the Infrastructure layer and the
/// persistence layer in exactly the order the production composition root
/// does, and it builds a <em>root</em> service provider. It deliberately does
/// not create a scope per request, because that is what the desktop
/// application does: MediatR resolves its handlers from the root provider, and
/// the scoped <see cref="RefLabDbContext"/> is therefore created once and
/// lives for the lifetime of the process.
/// </para>
/// <para>
/// Reproducing that lifetime is not a convenience — it is the point. A test
/// that resolved every handler in a fresh scope would exercise a different
/// execution model from the one the laboratory runs, and it would hide the
/// behaviour defects that depend on the shared context. The fixture therefore
/// mirrors the production composition root, and the execution model is pinned
/// down by an explicit test.
/// </para>
/// <para>
/// The generic host is used for the same reason: it contributes the logging
/// services that the <c>LoggingBehavior</c> pipeline behaviour requires, and
/// it contributes them exactly as the production composition root does.
/// </para>
/// </remarks>
public sealed class IntegrationTestHost : IAsyncDisposable
{
    private readonly IHost _host;
    private readonly IntegrationTestDatabase _database;
    private bool _disposed;

    private IntegrationTestHost(IHost host, IntegrationTestDatabase database)
    {
        _host = host;
        _database = database;
    }

    /// <summary>The root service provider, exactly as the application builds it.</summary>
    public IServiceProvider Services => _host.Services;

    /// <summary>The mediator, resolved from the root provider.</summary>
    public IMediator Mediator => Services.GetRequiredService<IMediator>();

    /// <summary>The context, resolved from the root provider.</summary>
    public RefLabDbContext Context => Services.GetRequiredService<RefLabDbContext>();

    /// <summary>The session of the signed-in operator.</summary>
    public SessionCurrentUser Session => Services.GetRequiredService<SessionCurrentUser>();

    /// <summary>The name of the provider the host is running against.</summary>
    public string ProviderName => _database.ProviderName;

    /// <summary>
    /// The name of the database the host writes to, or
    /// <see langword="null"/> for the in-memory SQLite database.
    /// </summary>
    public string? DatabaseName => _database.DatabaseName;

    /// <summary>
    /// Builds a host against a fresh test database.
    /// </summary>
    /// <returns>The host.</returns>
    public static async Task<IntegrationTestHost> CreateAsync()
    {
        var database = await IntegrationTestDatabase.CreateAsync();

        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddApplicationLayer();
        builder.Services.AddInfrastructureLayer();
        builder.Services.AddPersistenceLayer(database.Configure);

        var host = builder.Build();

        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RefLabDbContext>();

            await database.EnsureSchemaAsync(context);
        }

        return new IntegrationTestHost(host, database);
    }

    /// <summary>
    /// Establishes the session of the signed-in operator.
    /// </summary>
    /// <remarks>
    /// This is the equivalent of the presentation layer's
    /// <c>OperatorSession.Establish</c>: the Application layer returns an
    /// operator DTO, and the session singleton is populated from it. The test
    /// host signs the operator entity in, because the entity carries the
    /// permissions that the DTO carries.
    /// </remarks>
    /// <param name="operatorId">The identifier of the operator to sign in.</param>
    public void EstablishSession(int operatorId)
    {
        var @operator = Context.Operators.Single(candidate => candidate.Id == operatorId);

        Session.SignIn(@operator);
    }

    /// <summary>
    /// Reads a single entity back through the query side.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="predicate">The predicate that selects the row.</param>
    /// <param name="shape">
    /// An optional shaping of the query, used to eager-load a navigation.
    /// </param>
    /// <returns>The entity, or <see langword="null"/> when there is none.</returns>
    public TEntity? ReadOne<TEntity>(
        Func<TEntity, bool> predicate,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? shape = null)
        where TEntity : class
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();

        if (shape is not null)
        {
            query = shape(query);
        }

        return query.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Reads every entity of a type back through the query side.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The rows.</returns>
    public IReadOnlyList<TEntity> ReadAll<TEntity>()
        where TEntity : class =>
        Context.Set<TEntity>().ToArray();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        await _host.StopAsync();

        _host.Dispose();

        await _database.DisposeAsync();
    }
}
