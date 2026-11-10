using Microsoft.EntityFrameworkCore;
using RefLab.Application.Abstractions;
using RefLab.Infrastructure.Persistence;

namespace RefLab.Infrastructure.Tests.Persistence;

/// <summary>
/// The shape of the schema itself.
/// </summary>
/// <remarks>
/// The laboratory is a single-site installation. These tests are the guard
/// rail that keeps it that way: no branch table, no branch column and no
/// branch identifier may reappear in the model.
/// </remarks>
public class RefLabDbContextTests
{
    [Fact]
    public void The_context_implements_both_application_abstractions()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        Assert.IsAssignableFrom<IAppDbContext>(context);
        Assert.IsAssignableFrom<IUnitOfWork>(context);
    }

    [Fact]
    public void The_query_side_exposes_only_a_queryable()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var readSide = (IAppDbContext)context;

        Assert.IsAssignableFrom<IQueryable<Domain.Identity.Operator>>(readSide.Set<Domain.Identity.Operator>());
    }

    [Fact]
    public void Every_mapped_entity_has_a_key()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var entityTypes = context.Model.GetEntityTypes().ToList();

        Assert.NotEmpty(entityTypes);

        foreach (var entityType in entityTypes)
        {
            Assert.True(
                entityType.FindPrimaryKey() is not null,
                $"'{entityType.ClrType.Name}' has no primary key.");
        }
    }

    [Fact]
    public void The_schema_covers_every_aggregate_of_the_laboratory()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var tables = context.Model.GetEntityTypes()
            .Where(entityType => !entityType.IsOwned())
            .Select(entityType => entityType.GetTableName())
            .Where(tableName => tableName is not null)
            .Distinct()
            .ToList();

        Assert.Equal(27, tables.Count);

        foreach (var expected in new[]
                 {
                     "Operators",
                     "LaboratorySettings",
                     "TestDefinitions",
                     "ReferenceRanges",
                     "ClinicalConstants",
                     "ClinicalSubGroups",
                     "TestUnits",
                     "ConversionFactors",
                     "Abbreviations",
                     "ProthrombinControlCalibrations",
                     "Patients",
                     "PatientOrders",
                     "OrderTests",
                     "PatientHistories",
                     "PatientPictures",
                     "Payments",
                     "PriceLists",
                     "ReferringParties",
                     "TreatingPhysicians",
                     "Antibiotics",
                     "CultureResults",
                     "SendOutRequests",
                     "SampleDispositions",
                     "PrintAudits",
                     "AttendanceEntries",
                     "Farms",
                     "FarmTransactions",
                 })
        {
            Assert.Contains(expected, tables);
        }
    }

    [Fact]
    public void The_schema_carries_no_branch_table()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var tables = context.Model.GetEntityTypes()
            .Where(entityType => !entityType.IsOwned())
            .Select(entityType => entityType.GetTableName() ?? string.Empty)
            .Distinct()
            .ToList();

        Assert.DoesNotContain("Branches", tables);
        Assert.DoesNotContain(tables, table => table.Contains("Branch", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void The_schema_carries_no_branch_column()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var columns = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .Select(property => property.Name)
            .ToList();

        Assert.DoesNotContain("BranchId", columns);
        Assert.DoesNotContain("BranchCode", columns);
        Assert.DoesNotContain("Branch", columns);
        Assert.DoesNotContain(columns, column => column.Contains("Branch", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void The_identifier_of_an_order_is_thirteen_characters_long()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var owned = context.Model.GetEntityTypes()
            .Single(entityType => entityType.IsOwned() &&
                                 entityType.ClrType == typeof(Domain.ValueObjects.OrderIdentifier));

        var identifier = owned.FindProperty(nameof(Domain.ValueObjects.OrderIdentifier.Value))!;

        Assert.NotNull(identifier);
        Assert.Equal(13, identifier.GetMaxLength());
    }
}
