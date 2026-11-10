using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefLab.Domain.ValueObjects;

namespace RefLab.Infrastructure.Persistence.Configurations;

/// <summary>
/// The column widths and precisions used across the schema.
/// </summary>
/// <remarks>
/// The widths are not derived from the legacy Access database, whose column
/// declarations were free-text and inconsistent. They are sized from the
/// content the laboratory actually stores, and they are declared once here so
/// that the schema stays uniform and the migration stays readable.
/// </remarks>
internal static class ColumnWidths
{
    /// <summary>The width of a short code, such as a currency or a unit code.</summary>
    public const int Short = 16;

    /// <summary>The width of a name or a caption.</summary>
    public const int Name = 128;

    /// <summary>The width of a free-text description.</summary>
    public const int Description = 512;

    /// <summary>The width of a long free-text note.</summary>
    public const int Note = 2048;

    /// <summary>The width of the composite order identifier.</summary>
    public const int OrderIdentifier = 13;

    /// <summary>The precision of a monetary amount.</summary>
    public const int MoneyPrecision = 18;

    /// <summary>The scale of a monetary amount.</summary>
    public const int MoneyScale = 4;

    /// <summary>The precision of a percentage.</summary>
    public const int PercentagePrecision = 9;

    /// <summary>The scale of a percentage.</summary>
    public const int PercentageScale = 4;

    /// <summary>The precision of a conversion factor.</summary>
    public const int FactorPrecision = 18;

    /// <summary>The scale of a conversion factor.</summary>
    public const int FactorScale = 8;
}

/// <summary>
/// The shared mapping of the domain value objects onto owned columns.
/// </summary>
/// <remarks>
/// <para>
/// A value object is mapped as an owned type, so its columns live in the table
/// of the entity that owns it. The mapping is declared once here and reused,
/// so that the same value object is stored the same way everywhere it
/// appears.
/// </para>
/// <para>
/// Every method takes the column-name prefix of the owning property, because
/// the same value object is owned by more than one entity — a price is a
/// `Money` on both the order test and the order, and the two must not collide
/// on one column name.
/// </para>
/// </remarks>
internal static class OwnedValueObjectConfigurations
{
    /// <summary>Maps a monetary amount.</summary>
    /// <param name="builder">The owned navigation to configure.</param>
    /// <param name="prefix">The column-name prefix of the owning property.</param>
    public static void ConfigureMoney<T>(
        this OwnedNavigationBuilder<T, Money> builder,
        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefix);

        builder.Property(money => money.Amount)
            .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale)
            .HasColumnName($"{prefix}_Amount")
            .IsRequired();

        builder.Property(money => money.CurrencyCode)
            .HasMaxLength(ColumnWidths.Short)
            .HasColumnName($"{prefix}_CurrencyCode")
            .IsRequired();
    }

    /// <summary>Maps a percentage.</summary>
    /// <param name="builder">The owned navigation to configure.</param>
    /// <param name="prefix">The column-name prefix of the owning property.</param>
    public static void ConfigurePercentage<T>(
        this OwnedNavigationBuilder<T, Percentage> builder,
        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefix);

        builder.Property(percentage => percentage.Value)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_Value")
            .IsRequired();
    }

    /// <summary>Maps the composite order identifier.</summary>
    /// <param name="builder">The owned navigation to configure.</param>
    /// <param name="prefix">The column-name prefix of the owning property.</param>
    public static void ConfigureOrderIdentifier<T>(
        this OwnedNavigationBuilder<T, OrderIdentifier> builder,
        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefix);

        builder.Property(identifier => identifier.Value)
            .HasMaxLength(ColumnWidths.OrderIdentifier)
            .HasColumnName(prefix)
            .IsRequired();
    }

    /// <summary>Maps the age band of a reference range.</summary>
    /// <param name="builder">The owned navigation to configure.</param>
    /// <param name="prefix">The column-name prefix of the owning property.</param>
    public static void ConfigureAgeBand<T>(
        this OwnedNavigationBuilder<T, AgeBand> builder,
        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefix);

        builder.Property(ageBand => ageBand.From)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_From");

        builder.Property(ageBand => ageBand.To)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_To");

        builder.Property(ageBand => ageBand.Unit)
            .HasColumnName($"{prefix}_Unit")
            .IsRequired();
    }

    /// <summary>Maps the critical interval of a reference range.</summary>
    /// <param name="builder">The owned navigation to configure.</param>
    /// <param name="prefix">The column-name prefix of the owning property.</param>
    public static void ConfigureCriticalInterval<T>(
        this OwnedNavigationBuilder<T, CriticalInterval> builder,
        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefix);

        builder.Property(critical => critical.LowCriticalLimit)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_LowCriticalLimit");

        builder.Property(critical => critical.HighCriticalLimit)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_HighCriticalLimit");

        builder.Property(critical => critical.CriticalRangeText)
            .HasMaxLength(ColumnWidths.Description)
            .HasColumnName($"{prefix}_CriticalRangeText");

        builder.Property(critical => critical.CriticalExceptionText)
            .HasMaxLength(ColumnWidths.Description)
            .HasColumnName($"{prefix}_CriticalExceptionText");

        builder.Property(critical => critical.CriticalComment)
            .HasMaxLength(ColumnWidths.Note)
            .HasColumnName($"{prefix}_CriticalComment");
    }

    /// <summary>Maps the reference interval of a reference range.</summary>
    /// <param name="builder">The owned navigation to configure.</param>
    /// <param name="prefix">The column-name prefix of the owning property.</param>
    public static void ConfigureReferenceInterval<T>(
        this OwnedNavigationBuilder<T, ReferenceInterval> builder,
        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefix);

        builder.Property(interval => interval.LowLimit)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_LowLimit");

        builder.Property(interval => interval.HighLimit)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_HighLimit");

        builder.Property(interval => interval.LowExceptionText)
            .HasMaxLength(ColumnWidths.Description)
            .HasColumnName($"{prefix}_LowExceptionText");

        builder.Property(interval => interval.HighExceptionText)
            .HasMaxLength(ColumnWidths.Description)
            .HasColumnName($"{prefix}_HighExceptionText");

        builder.Property(interval => interval.LowComment)
            .HasMaxLength(ColumnWidths.Note)
            .HasColumnName($"{prefix}_LowComment");

        builder.Property(interval => interval.HighComment)
            .HasMaxLength(ColumnWidths.Note)
            .HasColumnName($"{prefix}_HighComment");

        builder.Property(interval => interval.RangeText)
            .HasMaxLength(ColumnWidths.Description)
            .HasColumnName($"{prefix}_RangeText");

        builder.Property(interval => interval.Unit)
            .HasMaxLength(ColumnWidths.Short)
            .HasColumnName($"{prefix}_Unit");

        builder.OwnsOne(interval => interval.CriticalInterval, critical =>
            critical.ConfigureCriticalInterval($"{prefix}_CriticalInterval"));
    }

    /// <summary>Maps the result of a test.</summary>
    /// <param name="builder">The owned navigation to configure.</param>
    /// <param name="prefix">The column-name prefix of the owning property.</param>
    public static void ConfigureResultValue<T>(
        this OwnedNavigationBuilder<T, ResultValue> builder,
        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(prefix);

        builder.Property(result => result.Text)
            .HasMaxLength(ColumnWidths.Description)
            .HasColumnName($"{prefix}_Text")
            .IsRequired();

        builder.Property(result => result.NumericValue)
            .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale)
            .HasColumnName($"{prefix}_NumericValue");

        builder.Property(result => result.Unit)
            .HasMaxLength(ColumnWidths.Short)
            .HasColumnName($"{prefix}_Unit");
    }
}
