using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RefLab.Infrastructure.Persistence;

namespace RefLab.Infrastructure.Tests.Persistence;

/// <summary>
/// A throw-away SQLite database for the integration tests.
/// </summary>
/// <remarks>
/// <para>
/// The integration tests run against an in-memory SQLite database rather than
/// against SQL Server, because the build agent has no database server. The
/// fluent configuration is provider-agnostic — it declares lengths and
/// precisions but never a provider-specific column type — so the same model
/// builds the SQL Server schema that the migration targets.
/// </para>
/// <para>
/// The connection is held open for the whole lifetime of the fixture. An
/// in-memory SQLite database exists only as long as its connection is open,
/// so letting the context close the connection would drop the schema.
/// </para>
/// </remarks>
public sealed class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    private bool _schemaCreated;

    /// <summary>
    /// Initialises a new database.
    /// </summary>
    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");

        _connection.Open();
    }

    /// <summary>
    /// Creates a context bound to this database.
    /// </summary>
    /// <returns>A context whose schema is already in place.</returns>
    public RefLabDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RefLabDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new RefLabDbContext(options);

        if (!_schemaCreated)
        {
            context.Database.EnsureCreated();

            _schemaCreated = true;
        }

        return context;
    }

    /// <summary>
    /// Creates a context whose schema has not been created yet.
    /// </summary>
    /// <returns>A context against an empty database.</returns>
    public RefLabDbContext CreateUninitializedContext()
    {
        var options = new DbContextOptionsBuilder<RefLabDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new RefLabDbContext(options);
    }

    /// <inheritdoc />
    public void Dispose() => _connection.Dispose();
}
