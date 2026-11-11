namespace RefLab.Integration.Tests.Integration;

/// <summary>
/// The environment the integration tests run in.
/// </summary>
/// <remarks>
/// <para>
/// The only switch the test suite exposes is the
/// <c>REFLAB_TEST_CONNECTION_STRING</c> environment variable. When it is set,
/// the tests run against SQL Server; when it is not set, they run against an
/// in-memory SQLite database.
/// </para>
/// <para>
/// The variable is deliberately <em>not</em> the application's own
/// <c>REFLAB_CONNECTION_STRING</c>. The application's variable points at the
/// laboratory database, and a test harness must never be able to reach it by
/// accident.
/// </para>
/// </remarks>
public static class IntegrationTestEnvironment
{
    /// <summary>The name of the environment variable that selects SQL Server.</summary>
    public const string ConnectionStringVariable = "REFLAB_TEST_CONNECTION_STRING";

    /// <summary>
    /// The SQL Server connection string the tests must use, or
    /// <see langword="null"/> when the tests must use SQLite.
    /// </summary>
    public static string? SqlServerConnectionString
    {
        get
        {
            var value = Environment.GetEnvironmentVariable(ConnectionStringVariable);

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }

    /// <summary>Whether the tests are running against SQL Server.</summary>
    public static bool UsesSqlServer => SqlServerConnectionString is not null;
}
