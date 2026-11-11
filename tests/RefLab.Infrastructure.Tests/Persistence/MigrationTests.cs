using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RefLab.Infrastructure.Persistence;

namespace RefLab.Infrastructure.Tests.Persistence;

/// <summary>
/// The generated migration must stay in step with the model.
/// </summary>
/// <remarks>
/// <para>
/// The migration is the artefact that carries the schema to a laboratory
/// installation, so it is checked here rather than only at build time: a model
/// change that was not migrated would leave an installation on the old schema.
/// </para>
/// <para>
/// The scripts are generated from a SQL Server context, because the migration
/// targets SQL Server. The schema is never applied here: the build agent has
/// no database server, and applying the schema to a laboratory installation is
/// an owner-gated operation that happens after the generated schema has been
/// reviewed and approved.
/// </para>
/// </remarks>
public class MigrationTests
{
    private const string SqlServerConnectionString =
        "Server=localhost;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True";

    private static RefLabDbContext CreateSqlServerContext()
    {
        var options = new DbContextOptionsBuilder<RefLabDbContext>()
            .UseSqlServer(SqlServerConnectionString)
            .Options;

        return new RefLabDbContext(options);
    }

    private static string[] ExpectedTables(RefLabDbContext context) =>
        context.Model.GetEntityTypes()
            .Where(entityType => !entityType.IsOwned())
            .Select(entityType => entityType.GetTableName())
            .Where(tableName => tableName is not null)
            .Distinct()
            .Select(tableName => tableName!)
            .OrderBy(tableName => tableName, StringComparer.Ordinal)
            .ToArray();

    [Fact]
    public void The_baseline_migration_and_the_additive_migrations_exist()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var migrations = context.Database.GetMigrations().ToArray();

        // The baseline migration creates the schema; every migration after it
        // is additive — it adds a column or an index and never rewrites or
        // drops what the baseline created. The assertion is deliberately
        // explicit about that shape rather than merely counting migrations.
        Assert.NotEmpty(migrations);
        Assert.EndsWith("_InitialCreate", migrations[0], StringComparison.Ordinal);

        var additive = migrations.Skip(1).ToArray();

        // Every migration after the baseline is additive: it adds the columns
        // the laboratory settings and the order rows gained, and never
        // rewrites or removes what the baseline created.
        Assert.All(additive, migration =>
            Assert.True(
                migration.EndsWith("_AddReportFooterAndLayoutText", StringComparison.Ordinal) ||
                migration.EndsWith("_AddReportBrandingAndSampleOrigin", StringComparison.Ordinal),
                $"{migration} is not a known additive migration."));
    }

    [Fact]
    public void The_additive_migration_only_adds_columns()
    {
        using var context = CreateSqlServerContext();

        var migrator = context.GetService<IMigrator>();

        var migrations = context.Database.GetMigrations().ToArray();

        Assert.True(migrations.Length >= 2, "The additive migration is missing.");

        var script = migrator.GenerateScript(
            fromMigration: migrations[0],
            toMigration: migrations[^1]);

        Assert.False(string.IsNullOrWhiteSpace(script));

        // The additive migration must not rewrite or remove anything the
        // baseline created: an existing laboratory database keeps every row
        // it holds.
        Assert.DoesNotContain("DROP COLUMN", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DROP TABLE", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("RENAME COLUMN", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ALTER COLUMN", script, StringComparison.OrdinalIgnoreCase);

        // It adds exactly the columns the model gained.
        Assert.Contains("ADD [ReportFooterText]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [LayoutText]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [DoctorSignature]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [DoctorName]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [ThanksText]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [LabLogoPath]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [ReportLogoPath]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [LogoWidthPx]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [LogoHeightPx]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [HeadingMode]", script, StringComparison.Ordinal);
        Assert.Contains("ADD [SampleTakenOutsideLab]", script, StringComparison.Ordinal);
    }

    [Fact]
    public void The_model_and_the_migration_agree()
    {
        // The snapshot was produced against SQL Server, so the comparison has
        // to be made against the same provider: a column type is a
        // provider-specific part of the model.
        using var context = CreateSqlServerContext();

        Assert.False(
            context.Database.HasPendingModelChanges(),
            "The model has changed since the migration was generated. "
            + "Run 'dotnet ef migrations add' to capture the change.");
    }

    [Fact]
    public void The_migration_script_creates_every_table_of_the_laboratory()
    {
        using var context = CreateSqlServerContext();

        var migrator = context.GetService<IMigrator>();
        var script = migrator.GenerateScript();

        Assert.False(string.IsNullOrWhiteSpace(script));

        foreach (var table in ExpectedTables(context))
        {
            Assert.Contains($"CREATE TABLE [{table}]", script, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_migration_script_creates_no_branch_table()
    {
        using var context = CreateSqlServerContext();

        var migrator = context.GetService<IMigrator>();
        var script = migrator.GenerateScript();

        Assert.DoesNotContain("Branches", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("BranchId", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("BranchCode", script, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void The_migration_targets_sql_server()
    {
        using var context = CreateSqlServerContext();

        var migrator = context.GetService<IMigrator>();
        var script = migrator.GenerateScript();

        Assert.Contains("uniqueidentifier", script, StringComparison.Ordinal);
        Assert.Contains("datetimeoffset", script, StringComparison.Ordinal);
        Assert.Contains("nvarchar", script, StringComparison.Ordinal);
    }

    [Fact]
    public void The_migration_creates_the_foreign_keys_of_the_order_pipeline()
    {
        using var context = CreateSqlServerContext();

        var migrator = context.GetService<IMigrator>();
        var script = migrator.GenerateScript();

        Assert.Contains("FOREIGN KEY ([PatientId]) REFERENCES [Patients]", script, StringComparison.Ordinal);
        Assert.Contains("FOREIGN KEY ([OrderId]) REFERENCES [PatientOrders]", script, StringComparison.Ordinal);
        Assert.Contains("FOREIGN KEY ([OrderTestId]) REFERENCES [OrderTests]", script, StringComparison.Ordinal);
    }

    [Fact]
    public void The_rollback_script_drops_every_table()
    {
        using var context = CreateSqlServerContext();

        var migrator = context.GetService<IMigrator>();
        var script = migrator.GenerateScript(fromMigration: "InitialCreate", toMigration: "0");

        Assert.False(string.IsNullOrWhiteSpace(script));

        foreach (var table in ExpectedTables(context))
        {
            Assert.Contains($"DROP TABLE [{table}]", script, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_rollback_script_carries_no_branch_table()
    {
        using var context = CreateSqlServerContext();

        var migrator = context.GetService<IMigrator>();
        var script = migrator.GenerateScript(fromMigration: "InitialCreate", toMigration: "0");

        Assert.DoesNotContain("Branches", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("BranchId", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("BranchCode", script, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void The_model_script_creates_the_same_tables_as_the_migration()
    {
        using var context = CreateSqlServerContext();

        var modelScript = context.Database.GenerateCreateScript();

        var migrator = context.GetService<IMigrator>();
        var migrationScript = migrator.GenerateScript();

        foreach (var table in ExpectedTables(context))
        {
            Assert.Contains(table, modelScript, StringComparison.Ordinal);
            Assert.Contains(table, migrationScript, StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task The_database_initialiser_reports_nothing_to_do_when_the_migration_is_applied()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateUninitializedContext();

        var migrationIds = context.Database.GetMigrations().ToArray();

        // Stand in for an installation where the migration has already been
        // applied, so that the "nothing to do" path is exercised against a
        // real database rather than only being asserted in the abstract.
        await context.Database.ExecuteSqlRawAsync(
            "CREATE TABLE \"__EFMigrationsHistory\" (\"MigrationId\" TEXT NOT NULL "
            + "CONSTRAINT \"PK__EFMigrationsHistory\" PRIMARY KEY, \"ProductVersion\" TEXT NOT NULL);");

        foreach (var migrationId in migrationIds)
        {
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ({0}, {1});",
                migrationId,
                "8.0.33");
        }

        var initializer = new DatabaseInitializer(context);

        var applied = await initializer.MigrateAsync();

        Assert.Equal(0, applied);
    }

    [Fact]
    public async Task The_initial_migration_is_pending_on_an_empty_database()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateUninitializedContext();

        var pending = (await context.Database.GetPendingMigrationsAsync()).ToArray();

        // The baseline migration creates the schema; the additive migration
        // that follows it adds the two report columns. Both are pending on an
        // empty database.
        Assert.True(pending.Length >= 3, "An additive migration is missing.");
        Assert.EndsWith("_InitialCreate", pending[0], StringComparison.Ordinal);
        Assert.EndsWith("_AddReportFooterAndLayoutText", pending[^2], StringComparison.Ordinal);
        Assert.EndsWith("_AddReportBrandingAndSampleOrigin", pending[^1], StringComparison.Ordinal);
    }
}
