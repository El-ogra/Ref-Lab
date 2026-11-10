using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RefLab.Infrastructure.Persistence;

/// <summary>
/// Builds a context for the Entity Framework Core design-time tools.
/// </summary>
/// <remarks>
/// <para>
/// The factory is what <c>dotnet ef migrations add</c> and
/// <c>dotnet ef database update</c> call. It never touches a live database:
/// the tools only need the <em>model</em>, and the model is built from the
/// fluent configuration, not from the connection string.
/// </para>
/// <para>
/// The connection string is read from the <c>REFLAB_CONNECTION_STRING</c>
/// environment variable when it is set, so the tools can be pointed at a
/// throwaway database without editing any file. The fallback is a local
/// Express edition string and is only ever used to shape the model.
/// </para>
/// </remarks>
public sealed class RefLabDbContextFactory : IDesignTimeDbContextFactory<RefLabDbContext>
{
    /// <summary>The environment variable that overrides the connection string.</summary>
    public const string ConnectionStringVariable = "REFLAB_CONNECTION_STRING";

    private const string FallbackConnectionString =
        "Server=(localdb)\\mssqllocaldb;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True";

    /// <inheritdoc />
    public RefLabDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(ConnectionStringVariable) ?? FallbackConnectionString;

        var options = new DbContextOptionsBuilder<RefLabDbContext>()
            .UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(RefLabDbContext).Assembly.FullName))
            .Options;

        return new RefLabDbContext(options);
    }
}
