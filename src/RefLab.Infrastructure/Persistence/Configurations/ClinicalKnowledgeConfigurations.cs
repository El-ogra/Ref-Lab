using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Infrastructure.Persistence.Converters;

namespace RefLab.Infrastructure.Persistence.Configurations;

/// <summary>
/// The mapping of the clinical knowledge tables.
/// </summary>
/// <remarks>
/// <para>
/// The clinical knowledge is the reference data of the laboratory: the test
/// catalogue, the reference ranges, the clinical constants, the units, the
/// conversion factors and the abbreviations. It is read far more often than it
/// is written, and it is the data that must be reproduced exactly when the
/// legacy installation is migrated.
/// </para>
/// <para>
/// The legacy Access tables had no reliable keys — several of them were keyed
/// by a free-text name — so the keys below are the natural keys of the domain
/// rather than a guess at the legacy index.
/// </para>
/// </remarks>
internal static class ClinicalKnowledgeConfigurations
{
    /// <summary>Maps the test catalogue.</summary>
    public static void ConfigureTestDefinition(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<TestDefinition>(builder =>
        {
            builder.ToTable("TestDefinitions");
            builder.HasKey(test => test.Code);
            builder.Property(test => test.Code).ValueGeneratedNever();

            builder.Property(test => test.BillingCode).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.SimpleName).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(test => test.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(test => test.ArabicName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.Abbreviation).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.WorkSheetName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.WorkSheetName2).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.HistoryName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.BarcodeSymbol).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.Barcode1).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.Barcode2).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.Barcode3).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.SubGroupName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.LogGroup).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.OtherLabName).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.Question).HasMaxLength(ColumnWidths.Note);
            builder.Property(test => test.Description).HasMaxLength(ColumnWidths.Note);
            builder.Property(test => test.Effect).HasMaxLength(ColumnWidths.Note);
            builder.Property(test => test.Usage).HasMaxLength(ColumnWidths.Note);
            builder.Property(test => test.SampleType).HasMaxLength(ColumnWidths.Name);
            builder.Property(test => test.SeeReport).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.NormalType).HasMaxLength(ColumnWidths.Short);
            builder.Property(test => test.ReferenceRangeType).HasMaxLength(ColumnWidths.Short);

            builder.Property(test => test.OtherLabPrice)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.Property(test => test.PatientPrice)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.Property(test => test.LabPrice)
                .HasPrecision(ColumnWidths.MoneyPrecision, ColumnWidths.MoneyScale);

            builder.HasIndex(test => test.SimpleName);
            builder.HasIndex(test => test.SubGroupCode);

            builder.OwnsOne(test => test.CriticalInterval, critical =>
                critical.ConfigureCriticalInterval("CriticalInterval"));
        });
    }

    /// <summary>Maps the reference ranges.</summary>
    public static void ConfigureReferenceRange(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ReferenceRange>(builder =>
        {
            builder.ToTable("ReferenceRanges");
            builder.HasKey(range => range.Id);
            builder.Property(range => range.Id).ValueGeneratedNever();

            builder.Property(range => range.TestSimpleName).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(range => range.ReferenceRangeType).HasMaxLength(ColumnWidths.Short);

            builder.Property(range => range.PregnancySmokingFactor)
                .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);

            builder.HasIndex(range => new { range.TestCode, range.SubGroupCode, range.SexScope });

            builder.OwnsOne(range => range.AgeBand, ageBand =>
                ageBand.ConfigureAgeBand("AgeBand"));

            builder.OwnsOne(range => range.Interval, interval =>
                interval.ConfigureReferenceInterval("Interval"));
        });
    }

    /// <summary>Maps the clinical constants.</summary>
    public static void ConfigureClinicalConstant(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ClinicalConstant>(builder =>
        {
            builder.ToTable("ClinicalConstants");
            builder.HasKey(constant => constant.GroupName);
            builder.Property(constant => constant.GroupName)
                .HasMaxLength(ColumnWidths.Name)
                .ValueGeneratedNever();

            var values = builder.Property(constant => constant.Values);

            values.HasConversion(new NullableStringListConverter())
                .HasColumnName("Values")
                .IsRequired();

            values.Metadata.SetValueComparer(PersistenceComparers.NullableStringList);
        });
    }

    /// <summary>Maps the clinical sub-groups.</summary>
    public static void ConfigureClinicalSubGroup(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ClinicalSubGroup>(builder =>
        {
            builder.ToTable("ClinicalSubGroups");
            builder.HasKey(subGroup => subGroup.Code);
            builder.Property(subGroup => subGroup.Code).ValueGeneratedNever();
            builder.Property(subGroup => subGroup.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(subGroup => subGroup.ArabicName).HasMaxLength(ColumnWidths.Name);
        });
    }

    /// <summary>Maps the measurement units.</summary>
    public static void ConfigureTestUnit(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<TestUnit>(builder =>
        {
            builder.ToTable("TestUnits");
            builder.HasKey(unit => unit.Id);
            builder.Property(unit => unit.Id).ValueGeneratedNever();
            builder.Property(unit => unit.Name).HasMaxLength(ColumnWidths.Name).IsRequired();
        });
    }

    /// <summary>Maps the unit conversion factors.</summary>
    public static void ConfigureConversionFactor(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ConversionFactor>(builder =>
        {
            builder.ToTable("ConversionFactors");
            builder.HasKey(factor => new { factor.TestSimpleName, factor.FromUnit, factor.ToUnit });

            builder.Property(factor => factor.TestSimpleName).HasMaxLength(ColumnWidths.Name);
            builder.Property(factor => factor.FromUnit).HasMaxLength(ColumnWidths.Short);
            builder.Property(factor => factor.ToUnit).HasMaxLength(ColumnWidths.Short);

            builder.Property(factor => factor.Factor)
                .HasPrecision(ColumnWidths.FactorPrecision, ColumnWidths.FactorScale);
        });
    }

    /// <summary>Maps the abbreviations.</summary>
    public static void ConfigureAbbreviation(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Abbreviation>(builder =>
        {
            builder.ToTable("Abbreviations");
            builder.HasKey(abbreviation => abbreviation.Id);
            builder.Property(abbreviation => abbreviation.Id).ValueGeneratedNever();
            builder.Property(abbreviation => abbreviation.Text).HasMaxLength(ColumnWidths.Name).IsRequired();
            builder.Property(abbreviation => abbreviation.Meaning).HasMaxLength(ColumnWidths.Description).IsRequired();
        });
    }

    /// <summary>Maps the prothrombin control calibration table.</summary>
    public static void ConfigureProthrombinControlCalibration(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ProthrombinControlCalibration>(builder =>
        {
            builder.ToTable("ProthrombinControlCalibrations");
            builder.HasKey(calibration => calibration.Id);
            builder.Property(calibration => calibration.Id).ValueGeneratedNever();

            builder.Property(calibration => calibration.ControlTime)
                .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);

            builder.Property(calibration => calibration.Concentration)
                .HasPrecision(ColumnWidths.PercentagePrecision, ColumnWidths.PercentageScale);
        });
    }
}
