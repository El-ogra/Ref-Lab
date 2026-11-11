using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RefLab.Infrastructure.Persistence;

namespace RefLab.Integration.Tests.Integration;

/// <summary>
/// The database the integration tests run against.
/// </summary>
/// <remarks>
/// <para>
/// Two providers are supported, and the choice is made by the environment
/// rather than by the code:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// <b>SQLite in memory</b> — the default, and the provider the Linux build
/// agent uses. The build agent has no database server, so a real server is
/// not available there.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>SQL Server</b> — selected when the <c>REFLAB_TEST_CONNECTION_STRING</c>
/// environment variable is set. This is the path the owner's Windows machine
/// takes, and it is the path that exercises the SQL Server schema and the
/// SQL Server migrations. The harness creates and drops its own database; the
/// database named in the connection string is used only to reach the server.
/// </description>
/// </item>
/// </list>
/// <para>
/// The production schema is produced by <c>EnsureCreated</c> on SQLite and by
/// the generated migration on SQL Server. The fluent configuration is
/// provider-agnostic — it declares lengths and precisions but never a
/// provider-specific column type — so the same model builds both schemas.
/// </para>
/// </remarks>
public sealed class IntegrationTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection? _sqliteConnection;
    private readonly string? _sqlServerDatabaseName;
    private readonly string? _sqlServerMasterConnectionString;
    private bool _schemaCreated;
    private bool _disposed;

    private IntegrationTestDatabase(
        string providerName,
        SqliteConnection? sqliteConnection,
        string? sqlServerDatabaseName,
        string? sqlServerMasterConnectionString)
    {
        ProviderName = providerName;
        _sqliteConnection = sqliteConnection;
        _sqlServerDatabaseName = sqlServerDatabaseName;
        _sqlServerMasterConnectionString = sqlServerMasterConnectionString;
    }

    /// <summary>The name of the provider the tests are running against.</summary>
    public string ProviderName { get; }

    /// <summary>
    /// The name of the database the tests write to, or <see langword="null"/>
    /// for the in-memory SQLite database, which has no name on the server.
    /// </summary>
    public string? DatabaseName => _sqlServerDatabaseName;

    /// <summary>
    /// Creates the test database.
    /// </summary>
    /// <returns>The database the tests will run against.</returns>
    public static async Task<IntegrationTestDatabase> CreateAsync()
    {
        var connectionString = IntegrationTestEnvironment.SqlServerConnectionString;

        if (connectionString is null)
        {
            var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync();

            return new IntegrationTestDatabase("SQLite (in memory)", connection, null, null);
        }

        var builder = ParseServerConnectionString(connectionString);

        var databaseName = BuildManagedDatabaseName();

        await CreateDatabaseAsync(builder, databaseName);

        return new IntegrationTestDatabase(
            $"SQL Server ({databaseName})",
            null,
            databaseName,
            connectionString);
    }

    /// <summary>
    /// Configures the context against this database.
    /// </summary>
    /// <param name="options">The builder to configure.</param>
    public void Configure(DbContextOptionsBuilder options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (_sqliteConnection is not null)
        {
            options.UseSqlite(_sqliteConnection);
            return;
        }

        var connectionString = new SqlConnectionStringBuilder(_sqlServerMasterConnectionString!)
        {
            InitialCatalog = _sqlServerDatabaseName,
        }.ConnectionString;

        options.UseSqlServer(
            connectionString,
            sql => sql.MigrationsAssembly(typeof(RefLabDbContext).Assembly.FullName));
    }

    /// <summary>
    /// Creates the schema when it does not exist yet.
    /// </summary>
    /// <param name="context">The context to initialise.</param>
    public async Task EnsureSchemaAsync(RefLabDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (_schemaCreated)
        {
            return;
        }

        if (_sqliteConnection is not null)
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            // The SQL Server path applies the generated migration, which is the
            // schema the laboratory installation actually uses.
            await context.Database.MigrateAsync();
        }

        _schemaCreated = true;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_sqliteConnection is not null)
        {
            await _sqliteConnection.DisposeAsync();
            return;
        }

        await DropDatabaseAsync();
    }

    private static SqlConnectionStringBuilder ParseServerConnectionString(string connectionString)
    {
        try
        {
            return new SqlConnectionStringBuilder(connectionString);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(
                $"The value of {IntegrationTestEnvironment.ConnectionStringVariable} is not a valid SQL Server " +
                "connection string. The expected form names the server and the authentication, for example " +
                "'Server=.\\SQLEXPRESS;Database=master;Integrated Security=True;TrustServerCertificate=True'. " +
                "The harness creates and drops its own database; the database named in the connection string is " +
                "used only to reach the server.",
                exception);
        }
    }

    private static string BuildManagedDatabaseName() =>
        $"RefLab_Phase6_{Guid.NewGuid():N}"[..32];

    private static async Task CreateDatabaseAsync(SqlConnectionStringBuilder builder, string databaseName)
    {
        var masterBuilder = new SqlConnectionStringBuilder(builder.ConnectionString)
        {
            InitialCatalog = "master",
        };

        await using var connection = new SqlConnection(masterBuilder.ConnectionString);

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = $"CREATE DATABASE [{databaseName}]";

        await command.ExecuteNonQueryAsync();
    }

    private async Task DropDatabaseAsync()
    {
        var masterBuilder = new SqlConnectionStringBuilder(_sqlServerMasterConnectionString!)
        {
            InitialCatalog = "master",
        };

        await using var connection = new SqlConnection(masterBuilder.ConnectionString);

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText =
            $"IF DB_ID('{_sqlServerDatabaseName}') IS NOT NULL " +
            $"BEGIN ALTER DATABASE [{_sqlServerDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
            $"DROP DATABASE [{_sqlServerDatabaseName}]; END";

        await command.ExecuteNonQueryAsync();
    }
}
