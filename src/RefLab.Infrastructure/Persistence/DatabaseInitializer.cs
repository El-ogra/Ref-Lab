using Microsoft.EntityFrameworkCore;

namespace RefLab.Infrastructure.Persistence;

/// <summary>
/// Brings the database up to date.
/// </summary>
/// <remarks>
/// <para>
/// The laboratory is a single-site installation, so there is one schema and
/// one migration history. The initialiser applies the generated migrations;
/// it never creates a schema by hand.
/// </para>
/// <para>
/// Applying a migration to a laboratory database is a gated operation: the
/// generated schema is reviewed and approved by the laboratory owner before it
/// is applied to an installation that already holds data. The initialiser is
/// therefore an operator tool, not something the shell runs at start-up.
/// </para>
/// </remarks>
public sealed class DatabaseInitializer
{
    private readonly RefLabDbContext _context;

    /// <summary>
    /// Initialises a new initialiser.
    /// </summary>
    /// <param name="context">The context to bring up to date.</param>
    public DatabaseInitializer(RefLabDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    /// <summary>
    /// Applies every pending migration.
    /// </summary>
    /// <returns>The number of migrations that were applied.</returns>
    public async Task<int> MigrateAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _context.Database
            .GetPendingMigrationsAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!pending.Any())
        {
            return 0;
        }

        await _context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

        return pending.Count();
    }
}
